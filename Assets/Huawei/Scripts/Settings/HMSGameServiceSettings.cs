using System;

namespace HmsPlugin
{
    public class HMSGameServiceSettings : HMSEditorSingleton<HMSGameServiceSettings>
    {
        private const string SettingsFilename = "HMSGameServiceSettings";

        public const string InitializeOnStart = "InitializeOnStart";

        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSGameServiceSettings()
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
