using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSRemoteDefaultValueSettings : HMSEditorSingleton<HMSRemoteDefaultValueSettings>
    {
        private const string SettingsFilename = "HMSRemoteDefaultValueSettings";
        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSRemoteDefaultValueSettings()
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

        public Dictionary<string, object> GetDefaultValues()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < _settings.Keys.Count(); i++)
            {
                string name = _settings.Keys.ElementAt(i);
                object value = _settings.Values.ElementAt(i);

                result.Add(name, value);
            }

            return result;
        }
    }
}
