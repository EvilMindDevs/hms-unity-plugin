using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin.ConnectAPI
{
    public class TokenObtainerEditor : VerticalSequenceDrawer
    {
        private TextField.TextFieldBase _clientIdTextField;
        private TextField.TextFieldBase _clientSecretTextField;

        public TokenObtainerEditor()
        {
            InitializeFields();
            SetupUI();
        }

        private void InitializeFields()
        {
            _clientIdTextField = new TextField.TextField("Client ID:", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID))
                .SetLabelWidth(85)
                .SetFieldWidth(300);

            _clientSecretTextField = new TextField.TextField("Client Secret:", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientSecret))
                .SetLabelWidth(85)
                .SetFieldWidth(300);
        }

        private void SetupUI()
        {
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), _clientIdTextField, new Spacer()));
            AddDrawer(new HmsPlugin.Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), _clientSecretTextField, new Spacer()));
            AddDrawer(new HmsPlugin.Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Obtain Token", async () => await ObtainTokenAsync()).SetWidth(200), new Spacer()));
        }

        private async Task ObtainTokenAsync()
        {
            var token = await HMSWebUtils.GetAccessTokenAsync(_clientIdTextField.GetCurrentText(), _clientSecretTextField.GetCurrentText());
            Debug.Log("Token async: " + token);
        }
    }
}
