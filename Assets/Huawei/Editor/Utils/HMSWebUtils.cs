using HmsPlugin;
using HmsPlugin.ConnectAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class HMSWebUtils
{
    public static async Task<string> GetAccessTokenAsync()
    {
        string accessToken = "";

        if (!string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken)))
        {
            var endDate = new DateTime(HMSConnectAPISettings.Instance.Settings.GetLong(HMSConnectAPISettings.ExpiresInTicks));
            if (endDate > DateTime.Now)
            {
                accessToken = HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken);
            }
            else
            {
                accessToken = await GetToken(accessToken);
            }
        }
        else
        {
            accessToken = await GetToken(accessToken);
        }

        return accessToken;
    }

    private static async Task<string> GetToken(string accessToken)
    {
        var tokenRequest = new TokenRequest("client_credentials", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID), HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientSecret));
        var request = await HMSWebRequestHelper.PostRequest("https://connect-api.cloud.huawei.com/api/oauth2/v1/token", JsonUtility.ToJson(tokenRequest));

        var response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);

        if (response.ret.code == 0)
        {
            accessToken = response.access_token;
            HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.AccessToken, response.access_token);
            HMSConnectAPISettings.Instance.Settings.SetLong(HMSConnectAPISettings.ExpiresInTicks, DateTime.Now.AddSeconds(response.expires_in).Ticks);
        }
        else
        {
            Debug.LogError($"[HMSWebUtils] GetToken Error on response. ErrorCode: {response.ret.code}, ErrorMessage: {response.ret.msg}.");
        }

        request.Dispose();
        return accessToken;
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