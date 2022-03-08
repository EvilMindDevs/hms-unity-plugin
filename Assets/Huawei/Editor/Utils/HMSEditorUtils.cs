using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public static class HMSEditorUtils
    {
        public static void HandleAssemblyDefinitions(bool enable, bool refreshAssets = true)
        {
            string huaweiMobileServicesCorePath = Application.dataPath + "/Huawei/HuaweiMobileServices.Core.asmdef";
            var huaweiMobileServicesCore = JsonUtility.FromJson<AssemblyDefinitionInfo>(File.ReadAllText(huaweiMobileServicesCorePath));

            if (huaweiMobileServicesCore != null)
            {
                huaweiMobileServicesCore.includePlatforms = enable ? new List<string> { "Editor", "Android" } : new List<string> { "Editor" };
                File.WriteAllText(huaweiMobileServicesCorePath, JsonUtility.ToJson(huaweiMobileServicesCore, true));
            }
            if (refreshAssets)
                AssetDatabase.Refresh();
        }

        public static void SetHMSPlugin(bool status, bool enableToggle, bool refreshAssets = true)
        {
            HMSPluginSettings.Instance.Settings.SetBool(PluginToggleEditor.PluginEnabled, status);
            var enabledEditors = HMSMainKitsTabFactory.GetEnabledEditors();
            if (status)
            {
                if (enableToggle)
                {
                    if (enabledEditors != null && enabledEditors.Count > 0)
                    {
                        enabledEditors.ForEach(f => f.EnableToggle());
                    }
                }
                else
                {
                    if (enabledEditors != null && enabledEditors.Count > 0)
                        enabledEditors.ForEach(f => f.RemoveToggleTabView(false));
                }
            }
            else
            {
                if (enabledEditors != null && enabledEditors.Count > 0)
                {
                    enabledEditors.ForEach(f => f.RemoveToggleTabView(true));
                }
            }
            HMSMainKitsTabFactory.RefreshPluginStatus();
            if (refreshAssets)
                AssetDatabase.Refresh();
        }

        [Serializable]
        private class AssemblyDefinitionInfo
        {
            public string name;
            public List<string> references;
            public List<string> includePlatforms;
            public List<string> excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public List<string> precompiledReferences;
            public bool autoReferenced;
            public List<string> defineConstraints;
            public List<string> versionDefines;
            public bool noEngineReferences;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void CheckOldFiles()
        {
            EditorApplication.delayCall += CheckOldFilesNow;
        }

        private static void CheckOldFilesNow()
        {
            string[] fileNames = { "/Plugins/Android/app-debug.aar", "/Plugins/Android/BookInfo.java", "/Plugins/Android/ObjectTypeInfoHelper.java", "/Resources/coins100.png", "/Resources/coins1000.png", "/Resources/no_ads.png", "/Resources/premium.png", "/Resources/xml/remoteConfig.xml", "/Plugins/Android/hmsMainTemplate.gradle", "/Plugins/Android/hmsLauncherTemplate.gradle", "/Plugins/Android/hmsBaseProjectTemplate.gradle" };

            List<string> foundFiles = new List<string>();

            for (int i = 0; i < fileNames.Length; i++)
            {
                string path = fileNames[i];
                if (File.Exists(Application.dataPath + path))
                {
                    foundFiles.Add(path);
                }
            }
            if (foundFiles.Count > 0)
            {
                string allPaths = "";
                foundFiles.ForEach(c => allPaths += c + "\n");
                if (EditorUtility.DisplayDialog("HMS Unity Plugin", "We've found some files that needs to be moved. Do you want to move these files?\n" + allPaths, "Move", "Cancel"))
                {
                    foreach (var path in foundFiles)
                    {
                        if (File.Exists(Application.dataPath + path))
                        {
                            string destPath = Application.dataPath + "/Huawei" + path;
                            if (!File.Exists(destPath))
                                new FileInfo(destPath).Directory.Create();
                            File.Move(Application.dataPath + path, destPath);
                        }
                    }
                    AssetDatabase.Refresh();
                }
            }
        }

        public static Dictionary<string, string> SupportedLanguages()
        {
            return new Dictionary<string, string>
        {
            {"Amharic","am_ET"},
            {"Arabic","ar"},
            {"Assamese","as_IN"},
            {"Azerbaijani","az_AZ"},
            {"Belarusian","be"},
            {"Bulgarian","bg"},
            {"Bengali","bn_BD"},
            {"Tibetan","bo"},
            {"Bosnian","bs"},
            {"Catalan","ca"},
            {"Czech","cs_CZ"},
            {"Danish","da_DK"},
            {"German","de_DE"},
            {"Greek","el_GR"},
            {"English (UK)","en_GB"},
            {"English (US)","en_US"},
            {"Spanish (Latin America)","es_419"},
            {"Spanish (Europe)","es_ES"},
            {"Estonian","et"},
            {"Basque","eu_ES"},
            {"Persian","fa"},
            {"Finnish","fi_FI"},
            {"Tagalog","fil"},
            {"French","fr_FR"},
            {"Galician","gl_ES"},
            {"Gujarati","gu_IN"},
            {"Hebrew","he_IL"},
            {"Hindi","hi_IN"},
            {"Croatian","hr"},
            {"Hungarian","hu_HU"},
            {"Indonesian","id"},
            {"Italian","it_IT"},
            {"Japanese","ja_JP"},
            {"Javanese","jv"},
            {"Georgian","ka_GE"},
            {"Kazakh","kk"},
            {"Khmer","km_KH"},
            {"Kannada","kn_IN"},
            {"Korean","ko_KR"},
            {"Lao","lo_LA"},
            {"Lithuanian","lt"},
            {"Latvian","lv"},
            {"Maithili","mai_Deva_IN"},
            {"Maori","mi_NZ"},
            {"Macedonian","mk_MK"},
            {"Malayalam","ml_IN"},
            {"Mongolian (Cyrillic)","mn_MN"},
            {"Marathi","mr_IN"},
            {"Malay","ms"},
            {"Burmese","my_MM"},
            {"Nepali","ne_NP"},
            {"Dutch","nl_NL"},
            {"Norwegian","no_NO"},
            {"Odia","or_IN"},
            {"Punjabi, Panjabi","pa_Guru_IN"},
            {"Polish","pl_PL"},
            {"Portuguese (Brazil)","pt_BR"},
            {"Portuguese (Europe)","pt_PT"},
            {"Romanian, Moldavian, Moldovan","ro"},
            {"Russian","ru_RU"},
            {"Sinhala","si_LK"},
            {"Slovak","sk"},
            {"Slovenian","sl"},
            {"Serbian","sr"},
            {"Swedish","sv_SE"},
            {"Swahili","sw_TZ"},
            {"Tamil","ta_IN"},
            {"Telugu","te_IN"},
            {"Thai","th"},
            {"Turkish","tr_TR"},
            {"Uighur, Uyghur","ug_CN"},
            {"Ukrainian","uk"},
            {"Urdu","ur"},
            {"Uzbek","uz"},
            {"Vietnamese","vi"},
            {"Simplified Chinese","zh_CN"},
            {"Traditional Chinese (Hong Kong, China)","zh_HK"},
            {"Traditional Chinese (Taiwan, China)","zh_TW"}
        };
        }

        public static List<CountryInfo> SupportedCountries()
        {
            return new List<CountryInfo>()
            {
                new CountryInfo("CH","CHF","de_DE","Switzerland"),
                new CountryInfo("CN","CNY","zh_CN","China"),
                new CountryInfo("CZ","CZK","cs_CZ","Czech Republic"),
                new CountryInfo("DK","DKK","da_DK","Denmark"),
                new CountryInfo("IE","EUR","en_GB","Ireland"),
                new CountryInfo("EE","EUR","et","Estonia"),
                new CountryInfo("AT","EUR","de_DE","Austria"),
                new CountryInfo("BE","EUR","nl_NL","Belgium"),
                new CountryInfo("DE","EUR","de_DE","Germany"),
                new CountryInfo("FR","EUR","fr_FR","France"),
                new CountryInfo("FI","EUR","fi_FI","Finland"),
                new CountryInfo("NL","EUR","nl_NL","Netherlands"),
                new CountryInfo("LV","EUR","lv","Latvia"),
                new CountryInfo("LT","EUR","lt","Lithuania"),
                new CountryInfo("PT","EUR","pt_PT","Portugal"),
                new CountryInfo("SK","EUR","sk","Slovakia"),
                new CountryInfo("ES","EUR","es_ES","Spain"),
                new CountryInfo("GR","EUR","el_GR","Greece"),
                new CountryInfo("IT","EUR","it_IT","Italy"),
                new CountryInfo("GB","GBP","en_GB","United Kingdom"),
                new CountryInfo("HU","HUF","hu_HU","Hungary"),
                new CountryInfo("NO","NOK","no_NO","Norway"),
                new CountryInfo("PL","PLN","pl_PL","Poland"),
                new CountryInfo("RO","RON","ro","Romania"),
                new CountryInfo("SE","SEK","sv_SE","Sweden"),
                new CountryInfo("AE","AED","ar","United Arab Emirates"),
                new CountryInfo("SG","SGD","en_GB","Singapore"),
                new CountryInfo("RS","RSD","sr","Serbia"),
                new CountryInfo("TR","TRY","tr_TR","Turkey"),
                new CountryInfo("UA","UAH","uk","Ukraine"),
                new CountryInfo("SI","EUR","sl","Slovenia"),
                new CountryInfo("UZ","UZS","uz","Uzbekistan"),
                new CountryInfo("MY","MYR","ms","Malaysia"),
                new CountryInfo("PH","PHP","en_US","Philippines"),
                new CountryInfo("HK","HKD","zh_HK","Hong Kong (China)"),
                new CountryInfo("LK","LKR","si_LK","Sri Lanka"),
                new CountryInfo("NP","NPR","ne_NP","Nepal"),
                new CountryInfo("BD","BDT","bn_BD","Bangladesh"),
                new CountryInfo("TH","THB","th","Thailand"),
                new CountryInfo("JO","JOD","ar","Jordan"),
                new CountryInfo("BH","BHD","ar","Bahrain"),
                new CountryInfo("ZA","ZAR","en_GB","South Africa"),
                new CountryInfo("NG","USD","en_GB","Nigeria"),
                new CountryInfo("TZ","TZS","en_GB","Tanzania"),
                new CountryInfo("CO","COP","es_419","Colombia"),
                new CountryInfo("PE","PEN","es_419","Peru"),
                new CountryInfo("MX","MXN","es_419","Mexico"),
                new CountryInfo("AR","ARS","es_419","Argentina"),
                new CountryInfo("QA","QAR","ar","Qatar"),
                new CountryInfo("VN","VND","vi","Vietnam"),
                new CountryInfo("ID","IDR","id","Indonesia"),
                new CountryInfo("KZ","KZT","kk","Kazakhstan"),
                new CountryInfo("BY","USD","be","Belarus"),
                new CountryInfo("BG","BGN","bg","Bulgaria"),
                new CountryInfo("HR","HRK","hr","Croatia"),
                new CountryInfo("MK","MKD","mk_MK","North Macedonia"),
                new CountryInfo("BA","BAM","sr","Bosnia and Herzegovina"),
                new CountryInfo("GE","GEL","ka_GE","Georgia"),
                new CountryInfo("BO","BOB","bo","Bolivia"),
                new CountryInfo("EC","USD","es_419","Ecuador"),
                new CountryInfo("UY","UYU","es_419","Uruguay"),
                new CountryInfo("BW","BWP","en_US","Botswana"),
                new CountryInfo("ZM","ZMW","en_US","Zambia"),
                new CountryInfo("MU","MUR","en_US","Mauritius"),
                new CountryInfo("KH","KHR","km_KH","Cambodia"),
                new CountryInfo("PG","PGK","en_GB","Papua New Guinea"),
                new CountryInfo("LA","USD","lo_LA","Laos"),
                new CountryInfo("LB","USD","ar","Lebanon"),
                new CountryInfo("YE","YER","ar","Yemen"),
                new CountryInfo("CD","CDF","fr_FR","Congo-Kinshasa"),
                new CountryInfo("EG","EGP","ar","Egypt"),
                new CountryInfo("UG","USD","en_US","Uganda"),
                new CountryInfo("GH","GHS","en_GB","Ghana"),
                new CountryInfo("PY","PYG","es_419","Paraguay"),
                new CountryInfo("CR","CRC","es_419","Costa Rica"),
                new CountryInfo("DZ","DZD","ar","Algeria"),
                new CountryInfo("SA","SAR","ar","Saudi Arabia"),
                new CountryInfo("MT","EUR","en_US","Malta"),
                new CountryInfo("LI","CHF","de_DE","Liechtenstein"),
                new CountryInfo("NZ","NZD","en_GB","New Zealand"),
                new CountryInfo("JP","JPY","ja_JP","Japan"),
                new CountryInfo("MO","MOP","zh_HK","Macao (China)"),
                new CountryInfo("BN","BND","ms","Brunei"),
                new CountryInfo("FJ","FJD","en_GB","Fiji"),
                new CountryInfo("PF","XPF","fr_FR","French Polynesia"),
                new CountryInfo("ZW","USD","en_GB","Zimbabwe"),
                new CountryInfo("NA","NAD","en_GB","Namibia"),
                new CountryInfo("MZ","MZN","pt_PT","Mozambique"),
                new CountryInfo("MG","EUR","fr_FR","Madagascar"),
                new CountryInfo("JM","JMD","en_GB","Jamaica"),
                new CountryInfo("TT","TTD","en_US","Trinidad and Tobago"),
                new CountryInfo("RU","RUB","ru_RU","Russia"),
                new CountryInfo("PK","PKR","ur","Pakistan"),
                new CountryInfo("PS","USD","ar","Palestine"),
                new CountryInfo("CM","XAF","en_GB","Cameroon"),
                new CountryInfo("SN","XOF","fr_FR","Senegal"),
                new CountryInfo("CG","XAF","fr_FR","Republic of the Congo"),
                new CountryInfo("GN","XAF","fr_FR","Guinea"),
                new CountryInfo("GA","XAF","fr_FR","Gabon"),
                new CountryInfo("IL","ILS","iw_IL","Israel"),
                new CountryInfo("CY","EUR","el_GR","Cyprus"),
                new CountryInfo("AM","RUB","ru_RU","Armenia"),
                new CountryInfo("LU","EUR","de_DE","Luxembourg"),
                new CountryInfo("IS","ISK","en_GB","Iceland"),
                new CountryInfo("MW","MWK","en_GB","Malawi"),
                new CountryInfo("CL","CLP","es_ES","Chile"),
                new CountryInfo("GT","GTQ","es_ES","Guatemala"),
                new CountryInfo("HN","HNL","es_ES","Honduras"),
                new CountryInfo("NI","NIO","es_ES","Nicaragua"),
                new CountryInfo("DO","DOP","es_ES","Dominican Republic"),
                new CountryInfo("AW","AWG","nl_NL","Aruba"),
                new CountryInfo("MV","MVR","en_US","Maldave"),
                new CountryInfo("IQ","IQD","ar","Iraq"),
                new CountryInfo("KE","KES","en_GB","Kenya"),
                new CountryInfo("VG","USD","en_GB","British Virgin Islands"),
                new CountryInfo("LC","XCD","en_GB","Saint Lucia"),
                new CountryInfo("GF","GYD","fr_FR","French Guiana"),
                new CountryInfo("KY","KYD","en_GB","Cayman Islands"),
                new CountryInfo("MR","MRO","ar","Mauritania"),
                new CountryInfo("NE","XOF","fr_FR","Niger"),
                new CountryInfo("TD","XAF","ar","Chad"),
                new CountryInfo("GQ","XAF","es_ES","Equatorial Guinea"),
                new CountryInfo("MD","EUR","ro","Moldova, Republic of"),
                new CountryInfo("ME","EUR","en_GB","Montenegro"),
                new CountryInfo("AZ","AZN","az_AZ","Azerbaijan"),
                new CountryInfo("KG","RUB","ru_RU","Kyrgyzstan"),
                new CountryInfo("MN","MNT","mn_MN","Mongolia"),
                new CountryInfo("MC","EUR","fr_FR","Monaco"),
                new CountryInfo("AD","EUR","ca","Andorra"),
                new CountryInfo("SM","EUR","it_IT","San Marino"),
                new CountryInfo("VA","EUR","it_IT","Holy See (Vatican City State)"),
                new CountryInfo("TW","TWD","zh_TW","Taiwan (China)"),
                new CountryInfo("MM","MMK","my_MM","Myanmar"),
                new CountryInfo("CI","XOF","fr_FR","Cote d'Ivoire"),
                new CountryInfo("BF","XOF","fr_FR","Burkina Faso"),
                new CountryInfo("ML","XOF","fr_FR","Mali"),
                new CountryInfo("LY","LYD","ar","Libya"),
                new CountryInfo("AO","EUR","pt_PT","Angola"),
                new CountryInfo("RE","EUR","fr_FR","Reunion Island"),
                new CountryInfo("PA","PAB","es_ES","Panama"),
                new CountryInfo("VE","USD","es_ES","Venezuela"),
                new CountryInfo("SV","USD","es_ES","El Salvador"),
                new CountryInfo("BR","BRL","pt_PT","Brazil"),
                new CountryInfo("GP","EUR","fr_FR","Guadeloupe"),
                new CountryInfo("AU","AUD","en_GB","Australia"),
                new CountryInfo("TM","RUB","ru_RU","Turkmenistan"),
                new CountryInfo("TJ","USD","ru_RU","Tajikistan"),
                new CountryInfo("SB","USD","en_GB","Solomon Islands"),
                new CountryInfo("TO","USD","en_GB","Tonga"),
                new CountryInfo("BJ","XOF","fr_FR","Benin"),
                new CountryInfo("TG","XOF","fr_FR","Togo"),
                new CountryInfo("CV","USD","pt_PT","Cape Verde"),
                new CountryInfo("CF","XAF","fr_FR","Central African Republic"),
                new CountryInfo("GM","USD","en_GB","Gambia"),
                new CountryInfo("GW","XOF","pt_PT","Guinea-Bissau"),
                new CountryInfo("KM","USD","fr_FR","Comoros"),
                new CountryInfo("LR","USD","en_US","Liberia"),
                new CountryInfo("ST","USD","pt_PT","Sao Tome and Principe"),
                new CountryInfo("YT","EUR","fr_FR","Mayotte"),
                new CountryInfo("PR","USD","pt_PT","Puerto Rico"),
                new CountryInfo("GD","XCD","en_GB","Grenada"),
                new CountryInfo("KW","KWD","ar","Kuwait"),
                new CountryInfo("TN","TND","ar","Tunisia"),
                new CountryInfo("ET","ETB","am_ET","Ethiopia"),
                new CountryInfo("DJ","DJF","ar","Djibouti"),
                new CountryInfo("ER","ERN","en_US","Eritrea"),
                new CountryInfo("CK","NZD","en_GB","Cook Islands"),
                new CountryInfo("NR","AUD","en_GB","Nauru"),
                new CountryInfo("SZ","SZL","en_GB","Eswatini"),
                new CountryInfo("LS","LSL","en_GB","Lesotho"),
                new CountryInfo("SL","SLL","en_GB","Sierra Leone"),
                new CountryInfo("AI","XCD","en_GB","Anguilla"),
                new CountryInfo("GY","GYD","en_US","Guyana"),
                new CountryInfo("OM","OMR","ar","Oman"),
                new CountryInfo("MA","MAD","ar","Morocco"),
                new CountryInfo("CA","CAD","en_US","Canada"),
                new CountryInfo("BS","BSD","en_US","Bahamas"),
                new CountryInfo("IN","INR","hi_IN","India")
            };
        }

        public static HMSAGConnectConfig GetAGConnectConfig()
        {
            var obj = JsonUtility.FromJson<HMSAGConnectConfig>(File.ReadAllText(Application.streamingAssetsPath + "/agconnect-services.json"));
            return obj;
        }

        [Serializable]
        public class CountryInfo
        {
            public CountryInfo(string region, string currency, string locale, string country)
            {
                Region = region;
                Currency = currency;
                Locale = locale;
                Country = country;
            }

            public string Region;
            public string Currency;
            public string Locale;
            public string Country;
        }

        #region AGConnect Config Classes

        [Serializable]
        public class Agcgw
        {
            public string backurl;
            public string url;
        }

        [Serializable]
        public class Client
        {
            public string appType;
            public string cp_id;
            public string product_id;
            public string client_id;
            public string client_secret;
            public string project_id;
            public string app_id;
            public string api_key;
            public string package_name;
        }

        [Serializable]
        public class Analytics
        {
            public string collector_url;
            public string resource_id;
            public string channel_id;
        }

        [Serializable]
        public class Search
        {
            public string url;
        }

        [Serializable]
        public class Cloudstorage
        {
            public string storage_url;
        }

        [Serializable]
        public class Ml
        {
            public string mlservice_url;
        }

        [Serializable]
        public class Service
        {
            public Analytics analytics;
            public Search search;
            public Cloudstorage cloudstorage;
            public Ml ml;
        }

        [Serializable]
        public class HMSAGConnectConfig
        {
            public Agcgw agcgw;
            public Client client;
            public Service service;
            public string region;
            public string configuration_version;
        }

        #endregion
    }

}
