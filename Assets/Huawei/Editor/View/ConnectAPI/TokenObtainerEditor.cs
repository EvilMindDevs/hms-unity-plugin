using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Networking;

namespace HmsPlugin.ConnectAPI
{
    public class TokenObtainerEditor : VerticalSequenceDrawer
    {
        TextField.TextField clientIdTextField;
        TextField.TextField clientSecretTextField;

        public TokenObtainerEditor()
        {
            clientIdTextField = new TextField.TextField("Client ID:", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID)).SetLabelWidth(85).SetFieldWidth(300);
            clientSecretTextField = new TextField.TextField("Client Secret:", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientSecret)).SetLabelWidth(85).SetFieldWidth(300);

            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), clientIdTextField, new Spacer()));
            AddDrawer(new HmsPlugin.Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), clientSecretTextField, new Spacer()));
            AddDrawer(new HmsPlugin.Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Obtain Token", OnButtonClick).SetWidth(200), new Spacer()));
        }

        private void OnButtonClick()
        {
            var tokenRequest = new TokenRequest("client_credentials", clientIdTextField.GetCurrentText(), clientSecretTextField.GetCurrentText());
            HMSWebRequestHelper.PostRequest("https://connect-api.cloud.huawei.com/api/oauth2/v1/token", JsonUtility.ToJson(tokenRequest), OnTokenResponse);
        }

        private void OnTokenResponse(UnityWebRequest obj)
        {
            HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.ClientID, clientIdTextField.GetCurrentText());
            HMSConnectAPISettings.Instance.Settings.Set(HMSConnectAPISettings.ClientSecret, clientSecretTextField.GetCurrentText());

            var response = JsonUtility.FromJson<TokenResponse>(obj.downloadHandler.text);
            Debug.Log("Access token: " + response.access_token);
            Debug.Log("Expires in: " + response.expires_in);
            Debug.Log("Retcode: " + response.ret.code);
            Debug.Log("message: " + response.ret.msg);
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
}