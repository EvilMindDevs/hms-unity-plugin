using UnityEngine;

namespace HmsPlugin
{
    public class HMSMainEditorSettings : HMSEditorSingleton<HMSMainEditorSettings>
    {
        private const string SettingsFilename = "HMSMainEditorSettings";

        private SettingsScriptableObject loadedSettings;

        private Settings _settings;
        public Settings Settings => _settings;

        public HMSMainEditorSettings()
        {
            loadedSettings = ScriptableHelper.Load<SettingsScriptableObject>(SettingsFilename, "Assets/Huawei/Settings/Resources");

            Debug.Assert(loadedSettings != null, "Failed to load the " + SettingsFilename);
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
