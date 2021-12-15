using HmsPlugin;
using HmsPlugin.ConnectAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class HMSWebUtils
{
    public static async Task<string> GetAccessTokenAsync(string clientId = "", string clientSecret = "")
    {
        if (string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID, "")))
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)) //Abort here because it cant be empty to get the token.
            {
                if (EditorUtility.DisplayDialog("Token Error", "Please enter clientId and clientSecret before obtaining the access token.", "Ok"))
                {
                    HMSConnectAPIWindow.ShowWindow();
                }
            }
            else
            {
                HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.ClientID, clientId);
                HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.ClientSecret, clientSecret);
                return await GetToken();
            }
        }
        else
        {
            bool getNewToken = false;
            if (!string.IsNullOrEmpty(clientId) && HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) != clientId)
            {
                getNewToken = true;
                HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.ClientID, clientId);
            }

            if (!string.IsNullOrEmpty(clientSecret) && HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientSecret) != clientSecret)
            {
                getNewToken = true;
                HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.ClientSecret, clientSecret);
            }

            if (!string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken)))
            {
                var endDate = new DateTime(HMSConnectAPISettings.Instance.Settings.GetLong(HMSConnectAPISettings.ExpiresInTicks));
                if (endDate > DateTime.Now && !getNewToken)
                {
                    return HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken);
                }
            }
            return await GetToken();
        }

        return string.Empty;
    }

    private static async Task<string> GetToken()
    {
        Debug.Log("[HMSWebUtils] Getting token from AGC.");
        var tokenRequest = new TokenRequest("client_credentials", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID), HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientSecret));
        var request = await HMSWebRequestHelper.Instance.PostRequest("https://connect-api.cloud.huawei.com/api/oauth2/v1/token", JsonUtility.ToJson(tokenRequest));

        var response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);

        if (response.ret.code == 0)
        {
            HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.AccessToken, response.access_token);
            HMSConnectAPISettings.Instance.Settings.SetLong(HMSConnectAPISettings.ExpiresInTicks, DateTime.Now.AddSeconds(response.expires_in).Ticks);
            return response.access_token;
        }
        else
        {
            Debug.LogError($"[HMSWebUtils] GetToken Error on response. ErrorCode: {response.ret.code}, ErrorMessage: {response.ret.msg}.");
            HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.AccessToken, string.Empty);
            HMSConnectAPISettings.Instance.Settings.SetLong(HMSConnectAPISettings.ExpiresInTicks, 0);
        }

        request.Dispose();
        return response.access_token;
    }

    [Serializable]
    private class TokenRequest
    {
        public string grant_type;
        public string client_id;
        public string client_secret;

        public TokenRequest(string grant_type, string client_id, string client_secret)
        {
            this.grant_type = grant_type;
            this.client_id = client_id;
            this.client_secret = client_secret;
        }
    }

    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public long expires_in;
        public Ret ret;
    }

    [Serializable]
    private class Ret
    {
        public long code;
        public string msg;
    }
}