using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSRemoteConfigSettings : HMSEditorSingleton<HMSRemoteConfigSettings>
    {
        private const string SettingsFilename = "HMSRemoteConfigSettings";

        public const string DeveloperMode = "DeveloperMode";
        public const string ApplyDefaultValues = "ApplyDefaultValues";

        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSRemoteConfigSettings()
        {
            loadedSettings = ScriptableHelper.Load<SettingsScriptableObject>(SettingsFilename, "Assets/Huawei/Settings/Resources");

            if (loadedSettings == null)
            {
                throw new NullReferenceException("Failed to load the " + SettingsFilename + ". Please restart Unity Editor");
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
