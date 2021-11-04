﻿using HmsPlugin.Dropdown;
using HmsPlugin.Image;
using HmsPlugin.TextField;
using HmsPlugin.Toggle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static HuaweiMobileServices.Ads.SplashAd;

namespace HmsPlugin
{
    internal class HMSAdsSettingsDrawer : VerticalSequenceDrawer
    {

        private Toggle.Toggle _enableBannerAdsToggle;
        private TextField.TextFieldWithAccept _bannerAdsTextField;
        private TextField.TextFieldWithAccept _bannerAdsRefreshField;
        private Toggle.Toggle _enableBannerAdLoadToggle;
        private DisabledDrawer _bannerAdsDisabledDrawer;

        private Toggle.Toggle _enableInterstitialAdsToggle;
        private TextField.TextFieldWithAccept _interstitialAdsTextField;
        private DisabledDrawer _interstitialAdsDisabledDrawer;

        private Toggle.Toggle _enableRewardedAdsToggle;
        private TextField.TextFieldWithAccept _rewardedAdsTextField;
        private DisabledDrawer _rewardedAdsDisabledDrawer;

        private Toggle.Toggle _enableSplashAdsToggle;
        private TextField.TextFieldWithAccept _splashAdsIdTextField;
        private EnumDropdown _splashAdOrientation;
        private TextField.TextFieldWithAccept _splashAdsTitleTextField;
        private TextField.TextFieldWithAccept _splashAdsSubTextField;
        private SpriteImage _splashSpriteImage;
        private DisabledDrawer _splashAdsDisabledDrawer;

        private Toggle.Toggle _testAdstoggle;

        private HMSSettings _settings;


        public HMSAdsSettingsDrawer()
        {
            _settings = HMSAdsKitSettings.Instance.Settings;
            _enableBannerAdsToggle = new Toggle.Toggle("Enable Banner Ads", _settings.GetBool(HMSAdsKitSettings.EnableBannerAd), OnBannerAdsToggleChanged, false);
            _bannerAdsTextField = new TextFieldWithAccept("Banner Ad ID", _settings.Get(HMSAdsKitSettings.BannerAdID), "Save", OnBannerAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _bannerAdsRefreshField = new TextFieldWithAccept("Banner Refresh Interval*", _settings.Get(HMSAdsKitSettings.BannerRefreshInterval), "Save", OnBannerRefreshIntervalSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100).AddTooltip("Default is 60 seconds. Value can be between 30 and 120 seconds");
            _enableBannerAdLoadToggle = new Toggle.Toggle("Show Banner on Load*", _settings.GetBool(HMSAdsKitSettings.ShowBannerOnLoad), OnShowBannerOnLoadChanged, false).SetTooltip("Enabling this will make the banner to be shown right after it finishes loading.");
            _bannerAdsDisabledDrawer = new DisabledDrawer(new VerticalSequenceDrawer(_bannerAdsTextField, _bannerAdsRefreshField, _enableBannerAdLoadToggle)).SetEnabled(!_enableBannerAdsToggle.IsChecked());

            _enableInterstitialAdsToggle = new Toggle.Toggle("Enable Interstitial Ads", _settings.GetBool(HMSAdsKitSettings.EnableInterstitialAd), OnInterstitialAdsToggleChanged, false);
            _interstitialAdsTextField = new TextFieldWithAccept("Interstitial Ad ID", _settings.Get(HMSAdsKitSettings.InterstitialAdID), "Save", OnInterstitialAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _interstitialAdsDisabledDrawer = new DisabledDrawer(_interstitialAdsTextField).SetEnabled(!_enableInterstitialAdsToggle.IsChecked());

            _enableRewardedAdsToggle = new Toggle.Toggle("Enable Rewarded Ads", _settings.GetBool(HMSAdsKitSettings.EnableRewardedAd), OnRewardedAdsToggleChanged, false);
            _rewardedAdsTextField = new TextFieldWithAccept("Rewarded Ad ID", _settings.Get(HMSAdsKitSettings.RewardedAdID), "Save", OnRewardedAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _rewardedAdsDisabledDrawer = new DisabledDrawer(_rewardedAdsTextField).SetEnabled(!_enableRewardedAdsToggle.IsChecked());

            _enableSplashAdsToggle = new Toggle.Toggle("Enable Splash Ads", _settings.GetBool(HMSAdsKitSettings.EnableSplashAd), OnSplashAdToggleChanged, false);
            _splashAdsIdTextField = new TextFieldWithAccept("Splash Ad ID", _settings.Get(HMSAdsKitSettings.SplashAdID), "Save", OnSplashAdIDSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _splashAdOrientation = new EnumDropdown((SplashAdOrientation)Enum.Parse(typeof(SplashAdOrientation), _settings.Get(HMSAdsKitSettings.SplashOrientation, "PORTRAIT")), "Orientation");
            _splashAdsTitleTextField = new TextFieldWithAccept("Splash Title", _settings.Get(HMSAdsKitSettings.SplashTitle), "Save", OnSplashTitleSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _splashAdsSubTextField = new TextFieldWithAccept("Splash Sub Text", _settings.Get(HMSAdsKitSettings.SplashSubText), "Save", OnSplashSubTextSaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);
            _splashSpriteImage = new SpriteImage(AssetDatabase.LoadAssetAtPath<Sprite>(_settings.Get(HMSAdsKitSettings.SplashImagePath, "")), "Image", OnSpriteImageChanged);
            _splashAdsDisabledDrawer = new DisabledDrawer(new VerticalSequenceDrawer(_splashAdsIdTextField, _splashAdOrientation, _splashAdsTitleTextField, _splashAdsSubTextField, _splashSpriteImage)).SetEnabled(!_enableSplashAdsToggle.IsChecked());

            _splashAdOrientation.OnChangedSelection += _splashAdOrientation_OnChangedSelection;
            _testAdstoggle = new Toggle.Toggle("Use Test Ads*", _settings.GetBool(HMSAdsKitSettings.UseTestAds), OnTestAdsToggleChanged);
            _testAdstoggle.SetTooltip("This will overwrite all ads with test ads.");
            SetupSequence();
        }

        private void _splashAdOrientation_OnChangedSelection()
        {
            _settings.Set(HMSAdsKitSettings.SplashOrientation, ((SplashAdOrientation)_splashAdOrientation.GetCurrentValue()).ToString());
        }

        private void OnSplashSubTextSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.SplashSubText, _splashAdsSubTextField.GetCurrentText());
        }

        private void OnSplashTitleSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.SplashTitle, _splashAdsTitleTextField.GetCurrentText());
        }

        private void OnSplashAdIDSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.SplashAdID, _splashAdsIdTextField.GetCurrentText());
        }

        private void OnBannerRefreshIntervalSaveButtonClick()
        {
            _settings.Set(HMSAdsKitSettings.BannerRefreshInterval, _bannerAdsRefreshField.GetCurrentText());
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

        private void OnSplashAdToggleChanged(bool value)
        {
            _splashAdsDisabledDrawer.SetEnabled(!value);
            _settings.SetBool(HMSAdsKitSettings.EnableSplashAd, value);
        }

        private void OnSpriteImageChanged(Sprite image)
        {
            _settings.Set(HMSAdsKitSettings.SplashImagePath, image != null ? AssetDatabase.GetAssetPath(image.GetInstanceID()) : "");
            _settings.Set(HMSAdsKitSettings.SplashImageBytes, image != null ? Convert.ToBase64String(image.texture.EncodeToPNG(), 0, image.texture.EncodeToPNG().Length) : "");
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Banner").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableBannerAdsToggle);
            AddDrawer(_bannerAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(25));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Interstitial").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableInterstitialAdsToggle);
            AddDrawer(_interstitialAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(25));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Rewarded").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableRewardedAdsToggle);
            AddDrawer(_rewardedAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(25));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Splash (Beta)").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_enableSplashAdsToggle);
            AddDrawer(_splashAdsDisabledDrawer);
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(25));

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Testing").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_testAdstoggle);
            AddDrawer(new HorizontalLine());
        }
    }
}
