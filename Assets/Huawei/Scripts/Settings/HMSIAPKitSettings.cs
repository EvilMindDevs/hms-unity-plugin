using System;

namespace HmsPlugin
{
    public class HMSIAPKitSettings : HMSEditorSingleton<HMSIAPKitSettings>
    {
        private const string SettingsFilename = "HMSIAPKitSettings";
        public const string InitializeOnStart = "InitializeOnStart";
        public const string ConsumptionOwnedItemsOnInitialize = "ConsumptionOwnedItemsOnInitialize";
        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSIAPKitSettings()
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
