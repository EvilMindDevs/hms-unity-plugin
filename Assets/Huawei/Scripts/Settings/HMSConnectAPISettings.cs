using System;

namespace HmsPlugin
{
    public class HMSConnectAPISettings : HMSEditorSingleton<HMSConnectAPISettings>
    {
        private const string SettingsFilename = "HMSConnectAPISettings";

        public const string ClientID = "ClientID";
        public const string ClientSecret = "ClientSecret";
        public const string AccessToken = "AccessToken";
        public const string ExpiresInTicks = "ExpiresInTicks";
        public const string UploadAfterBuild = "UploadAfterBuild";

        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSConnectAPISettings()
        {
            loadedSettings = ScriptableHelper.Load<SettingsScriptableObject>(SettingsFilename, "Assets/Huawei/Settings/Resources");

            if (loadedSettings == null)
            {
                throw new InvalidOperationException($"Failed to load the {SettingsFilename}. Please restart Unity Editor");
            }
            _settings = loadedSettings.settings;

            _settings.OnDictionaryChanged += _settings_OnDictionaryChanged;
        }

        private void _settings_OnDictionaryChanged()
        {
            loadedSettings.Save();
        }

        public void Reset()
        {
            _settings.Dispose();
            _instance = null;
        }
    }
}
