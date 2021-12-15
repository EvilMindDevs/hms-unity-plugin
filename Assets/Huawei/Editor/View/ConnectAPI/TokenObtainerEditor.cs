using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Networking;

namespace HmsPlugin.ConnectAPI
{
    public class TokenObtainerEditor : VerticalSequenceDrawer
    {
        TextField.TextFieldBase clientIdTextField;
        TextField.TextFieldBase clientSecretTextField;

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

        private async void OnButtonClick()
        {
            var token = await HMSWebUtils.GetAccessTokenAsync(clientIdTextField.GetCurrentText(), clientSecretTextField.GetCurrentText());
            Debug.Log("Token async: " + token);
        }
    }
}