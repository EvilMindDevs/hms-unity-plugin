using System;

using UnityEngine;

namespace HmsPlugin
{
    public class HMSAdsKitSettings : HMSEditorSingleton<HMSAdsKitSettings>
    {
        private const string SettingsFilename = "HMSAdsKitSettings";
        public const string EnableBannerAd = "EnableBannerAd";
        public const string BannerAdID = "BannerAdID";
        public const string BannerAdPositionType = "BannerAdPositionType";
        public const string UnityBannerAdSizeType = "UnityBannerAdSizeType";
        public const string BannerRefreshInterval = "BannerRefreshInterval";
        public const string ShowBannerOnLoad = "ShowBannerOnLoad";
        public const string EnableInterstitialAd = "EnableInterstitialAd";
        public const string InterstitialAdID = "InterstitialAdID";
        public const string EnableRewardedAd = "EnableRewardedAd";
        public const string RewardedAdID = "RewardedAdID";
        public const string UseTestAds = "UseTestAds";
        public const string EnableSplashAd = "EnableSplashAd";
        public const string SplashAdID = "SplashAdID";
        public const string SplashTitle = "SplashTitle";
        public const string SplashSubText = "SplashSubText";
        public const string SplashOrientation = "SplashOrientation";
        public const string SplashImagePath = "SplashImagePath";
        public const string SplashImageBytes = "SplashImageBytes";

        private SettingsScriptableObject loadedSettings;

        private HMSSettings _settings;
        public HMSSettings Settings => _settings;

        public HMSAdsKitSettings()
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
