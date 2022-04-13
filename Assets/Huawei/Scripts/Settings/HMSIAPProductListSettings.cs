using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSIAPProductListSettings : HMSEditorSingleton<HMSIAPProductListSettings>
    {
        private const string SettingsFilename = "HMSIAPProductListSettings";
        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSIAPProductListSettings()
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

        public List<HMSIAPProductEntry> GetAllIAPProducts()
        {
            var returnList = new List<HMSIAPProductEntry>();

            for (int i = 0; i < _settings.Keys.Count(); i++)
            {
                returnList.Add(new HMSIAPProductEntry(_settings.Keys.ElementAt(i), (HMSIAPProductType)Enum.Parse(typeof(HMSIAPProductType), _settings.Values.ElementAt(i))));
            }

            return returnList;
        }

        public List<string> GetProductIdentifiersByType(HMSIAPProductType type)
        {
            var returnList = new List<string>();

            for (int i = 0; i < _settings.Keys.Count(); i++)
            {
                if (_settings.Values.ElementAt(i) == type.ToString())
                {
                    returnList.Add(_settings.Keys.ElementAt(i));
                }
            }

            return returnList;
        }
    }
}
