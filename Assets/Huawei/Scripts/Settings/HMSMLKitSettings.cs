using System;

namespace HmsPlugin
{
    public class HMSMLKitSettings : HMSEditorSingleton<HMSMLKitSettings>
    {
        private const string SettingsFilename = "HMSMLKitSettings";
        public const string EnableTranslateModule = "EnableTranslateModule";
        public const string EnableTextToSpeechModule = "EnableTextToSpeechModule";
        public const string EnableLanguageDetectionModule = "EnableLanguageDetectionModule";
        public const string EnableTextRecognitionModule = "EnableTextRecognitionModule";
        public const string MLKeyAPI = "MLKeyAPI";

        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSMLKitSettings()
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
