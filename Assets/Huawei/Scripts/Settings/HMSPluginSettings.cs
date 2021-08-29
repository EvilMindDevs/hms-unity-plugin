using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSPluginSettings : HMSEditorSingleton<HMSPluginSettings>
    {
        private const string SettingsFilename = "HMSPluginSettings";

        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSPluginSettings()
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
