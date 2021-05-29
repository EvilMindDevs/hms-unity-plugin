using HmsPlugin.TextField;
using HmsPlugin.Toggle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    internal class HMSAdsSettingsDrawer : VerticalSequenceDrawer
    {

        private Toggle.Toggle _enableBannerAdsToggle;
        private TextField.TextFieldWithAccept _bannerAdsTextField;
        private Toggle.Toggle _enableBannerAdLoadToggle;
        private DisabledDrawer _bannerAdsDisabledDrawer;

        private Toggle.Toggle _enableInterstitialAdsToggle;
        private TextField.TextFieldWithAccept _interstitialAdsTextField;
        private DisabledDrawer _interstitialAdsDisabledDrawer;

        private Toggle.Toggle _enableRewardedAdsToggle;
        private TextField.TextFieldWithAccept _rewardedAdsTextField;
        private DisabledDrawer _rewardedAdsDisabledDrawer;

        private Toggle.Toggle _testAdstoggle;

        private Settings _settings;


        public HMSAdsSettingsDrawer()
        {
            _settings = HMSAdsKitSettings.Instance.Settings;
            _enableBannerAdsToggle = new Toggle.Toggle("Enable Banner Ads", _settings.GetBool(HMSAdsKitSettings.EnableBannerAd), OnBannerAdsToggleChanged, false);
            _bannerAdsTextField = new TextFieldWithAccept("Banner Ad ID", _settings.Get(HMSAdsKitSettings.BannerAdID), "Save", OnBannerAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _enableBannerAdLoadToggle = new Toggle.Toggle("Show Banner on Load*", _settings.GetBool(HMSAdsKitSettings.ShowBannerOnLoad), OnShowBannerOnLoadChanged, false).SetTooltip("Enabling this will make the banner to be shown right after it finishes loading.");
            _bannerAdsDisabledDrawer = new DisabledDrawer(new VerticalSequenceDrawer(_bannerAdsTextField, _enableBannerAdLoadToggle)).SetEnabled(!_enableBannerAdsToggle.IsChecked());

            _enableInterstitialAdsToggle = new Toggle.Toggle("Enable Interstitial Ads", _settings.GetBool(HMSAdsKitSettings.EnableInterstitialAd), OnInterstitialAdsToggleChanged, false);
            _interstitialAdsTextField = new TextFieldWithAccept("Interstitial Ad ID", _settings.Get(HMSAdsKitSettings.InterstitialAdID), "Save", OnInterstitialAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _interstitialAdsDisabledDrawer = new DisabledDrawer(_interstitialAdsTextField).SetEnabled(!_enableInterstitialAdsToggle.IsChecked());

            _enableRewardedAdsToggle = new Toggle.Toggle("Enable Rewarded Ads", _settings.GetBool(HMSAdsKitSettings.EnableRewardedAd), OnRewardedAdsToggleChanged, false);
            _rewardedAdsTextField = new TextFieldWithAccept("Rewarded Ad ID", _settings.Get(HMSAdsKitSettings.RewardedAdID), "Save", OnRewardedAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _rewardedAdsDisabledDrawer = new DisabledDrawer(_rewardedAdsTextField).SetEnabled(!_enableRewardedAdsToggle.IsChecked());

            _testAdstoggle = new Toggle.Toggle("Use Test Ads*", _settings.GetBool(HMSAdsKitSettings.UseTestAds), OnTestAdsToggleChanged);
            _testAdstoggle.SetTooltip("This will overwrite all ads with test ads.");
            SetupSequence();
        }

        private void OnRewardedAdsToggleChanged(bool value)
        {
            _rewardedAdsDisabledDrawer.SetEnabled(!value);
            _settings.SetBool(HMSAdsKitSettings.EnableRewardedAd, value);
        }

        private void OnInterstitialAdsToggleChanged(bool value)
        {
            _interstitialAdsDisabledDrawer.SetEnabled(!value);
            _settings.SetBool(HMSAdsKitSettings.EnableInterstitialAd, value);
        }

        private void OnBannerAdsToggleChanged(bool value)
        {
            _bannerAdsDisabledDrawer.SetEnabled(!value);
            _settings.SetBool(HMSAdsKitSettings.EnableBannerAd, value);
        }

        private void OnInterstitialAdIDSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.InterstitialAdID, _interstitialAdsTextField.GetCurrentText());
        }

        private void OnBannerAdIDSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.BannerAdID, _bannerAdsTextField.GetCurrentText());
        }

        private void OnRewardedAdIDSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.RewardedAdID, _rewardedAdsTextField.GetCurrentText());
        }

        private void OnTestAdsToggleChanged(bool value)
        {
            _settings.SetBool(HMSAdsKitSettings.UseTestAds, value);
        }

        private void OnShowBannerOnLoadChanged(bool value)
        {
            _settings.SetBool(HMSAdsKitSettings.ShowBannerOnLoad, value);
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Banner").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableBannerAdsToggle);
            AddDrawer(_bannerAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(5));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Interstitial").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableInterstitialAdsToggle);
            AddDrawer(_interstitialAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(5));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Rewarded").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableRewardedAdsToggle);
            AddDrawer(_rewardedAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(5));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Testing").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_testAdstoggle);
            AddDrawer(new HorizontalLine());
        }
    }
}
