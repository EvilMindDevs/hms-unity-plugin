using UnityEngine;

namespace HmsPlugin
{
    public class HMSAdsKitSettings : HMSEditorSingleton<HMSAdsKitSettings>
    {
        private const string SettingsFilename = "HMSAdsKitSettings";
        public const string EnableBannerAd = "EnableBannerAd";
        public const string BannerAdID = "BannerAdID";
        public const string ShowBannerOnLoad = "ShowBannerOnLoad";
        public const string EnableInterstitialAd = "EnableInterstitialAd";
        public const string InterstitialAdID = "InterstitialAdID";
        public const string EnableRewardedAd = "EnableRewardedAd";
        public const string RewardedAdID = "RewardedAdID";
        public const string UseTestAds = "UseTestAds";

        private SettingsScriptableObject loadedSettings;

        private Settings _settings;
        public Settings Settings => _settings;

        public HMSAdsKitSettings()
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
