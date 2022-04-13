﻿using HuaweiConstants;
using HuaweiMobileServices.Ads;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static HuaweiConstants.UnityBannerAdPositionCode;
using static HuaweiMobileServices.Ads.SplashAd;

namespace HmsPlugin
{

    public class HMSAdsKitManager : HMSManagerSingleton<HMSAdsKitManager>
    {

        private const string TestBannerAdId = "testw6vs28auh3";
        private const string TestInterstitialAdId = "testb4znbuh3n2";
        private const string TestRewardedAdId = "testx9dtjwj8hp";
        private const string TestSplashImageAdId = "testq6zq98hecj";
        private const string TestSplashVideoAdId = "testd7c5cewoj6";

        private BannerAd bannerView;
        private InterstitialAd interstitialView;
        private RewardAd rewardedView;
        private SplashAd splashView;

        private HMSSettings adsKitSettings;

        private bool isInitialized;

        public HMSAdsKitManager()
        {
            Debug.Log("[HMS] HMSAdsKitManager Constructor");
            adsKitSettings = HMSAdsKitSettings.Instance.Settings;
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
            HMSDispatcher.InvokeAsync(OnStart);
        }

        private void OnAwake()
        {
            Debug.Log("[HMS] HMSAdsKitManager OnAwake");
            Init();
            if (adsKitSettings.GetBool(HMSAdsKitSettings.EnableSplashAd))
                LoadSplashAd();
        }

        private void OnStart()
        {
            Debug.Log("[HMS] HMSAdsKitManager OnStart");
            LoadingAds();
        }

        private void Init()
        {
            Debug.Log("[HMS] HMSAdsKitManager Init");
            HwAds.Init();
            isInitialized = true;
            adsKitSettings = HMSAdsKitSettings.Instance.Settings;
        }

        private async void LoadingAds()
        {
            while (Application.internetReachability == NetworkReachability.NotReachable)
                await Task.Delay(500);

            Debug.Log("[HMS] HMSAdsKitManager Loading Ads");
            LoadAllAds();
        }

        public void LoadAllAds(bool hasPurchasedNoAds = false)
        {
            if (!hasPurchasedNoAds)
            {
                LoadBannerAd(UnityBannerAdPositionCodeType.POSITION_BOTTOM);
                LoadInterstitialAd();
            }
            LoadRewardedAd();
        }

        public void SetTestAdStatus(bool value)
        {
            adsKitSettings.SetBool(HMSAdsKitSettings.UseTestAds, value);
            Debug.Log("[HMS] HMSAdsKitManager SetTestAdStatus set to " + value.ToString());
        }

        #region BANNER

        #region PUBLIC METHODS

        public void LoadBannerAd(UnityBannerAdPositionCodeType position, string bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableBannerAd)) return;

            Debug.Log("[HMS] HMSAdsKitManager Loading Banner Ad.");
            var bannerAdStatusListener = new AdStatusListener();
            bannerAdStatusListener.mOnAdLoaded += BannerAdStatusListener_mOnAdLoaded;
            bannerAdStatusListener.mOnAdClosed += BannerAdStatusListener_mOnAdClosed;
            bannerAdStatusListener.mOnAdImpression += BannerAdStatusListener_mOnAdImpression;
            bannerAdStatusListener.mOnAdClicked += BannerAdStatusListener_mOnAdClicked;
            bannerAdStatusListener.mOnAdOpened += BannerAdStatusListener_mOnAdOpened;
            bannerAdStatusListener.mOnAdFailed += BannerAdStatusListener_mOnAdFailed;

            bannerView = new BannerAd(bannerAdStatusListener);
            bannerView.AdId = adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestBannerAdId : adsKitSettings.Get(HMSAdsKitSettings.BannerAdID);
            bannerView.PositionType = (int)position;
            bannerView.SizeType = bannerSize;
            bannerView.AdStatusListener = bannerAdStatusListener;
            if (!string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.BannerRefreshInterval)))
                bannerView.BannerRefresh = long.Parse(adsKitSettings.Get(HMSAdsKitSettings.BannerRefreshInterval));
            _isBannerAdLoaded = false;
            bannerView.LoadBanner(new AdParam.Builder().Build());
            if (adsKitSettings.GetBool(HMSAdsKitSettings.ShowBannerOnLoad))
                bannerView.ShowBanner();
            else
                bannerView.HideBanner();
        }

        public void LoadBannerAd(string adId, UnityBannerAdPositionCodeType position, string bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableBannerAd)) return;

            Debug.Log("[HMS] HMSAdsKitManager Loading Banner Ad.");
            var bannerAdStatusListener = new AdStatusListener();
            bannerAdStatusListener.mOnAdLoaded += BannerAdStatusListener_mOnAdLoaded;
            bannerAdStatusListener.mOnAdClosed += BannerAdStatusListener_mOnAdClosed;
            bannerAdStatusListener.mOnAdImpression += BannerAdStatusListener_mOnAdImpression;
            bannerAdStatusListener.mOnAdClicked += BannerAdStatusListener_mOnAdClicked;
            bannerAdStatusListener.mOnAdOpened += BannerAdStatusListener_mOnAdOpened;
            bannerAdStatusListener.mOnAdFailed += BannerAdStatusListener_mOnAdFailed;

            bannerView = new BannerAd(bannerAdStatusListener);
            bannerView.AdId = adId;
            bannerView.PositionType = (int)position;
            bannerView.SizeType = bannerSize;
            bannerView.AdStatusListener = bannerAdStatusListener;
            if (!string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.BannerRefreshInterval)))
                bannerView.BannerRefresh = long.Parse(adsKitSettings.Get(HMSAdsKitSettings.BannerRefreshInterval));
            _isBannerAdLoaded = false;
            bannerView.LoadBanner(new AdParam.Builder().Build());
            if (adsKitSettings.GetBool(HMSAdsKitSettings.ShowBannerOnLoad))
                bannerView.ShowBanner();
            else
                bannerView.HideBanner();
        }

        public void ShowBannerAd()
        {
            if (bannerView == null)
            {
                Debug.LogError("[HMS] HMSAdsKitManager Banner Ad is Null.");
                return;
            }
            bannerView.ShowBanner();
        }

        public void HideBannerAd()
        {
            if (bannerView == null)
            {
                Debug.LogError("[HMS] HMSAdsKitManager Banner Ad is Null.");
                return;
            }
            bannerView.HideBanner();
        }

        public void DestroyBannerAd()
        {
            if (bannerView == null)
            {
                Debug.LogError("[HMS] HMSAdsKitManager Banner Ad is Null.");
                return;
            }
            bannerView.DestroyBanner();
        }

        public void SetBannerRefresh(long seconds)
        {
            if (bannerView != null)
            {
                bannerView.SetBannerRefresh(seconds);
            }
            else
            {
                Debug.LogError("[HMS] HMSAdsKitManager BannerView not initialized yet.");
            }
        }

        private bool _isBannerAdLoaded;
        public bool IsBannerAdLoaded { get => _isBannerAdLoaded; set => _isBannerAdLoaded = value; }

        #endregion

        #region LISTENERS

        public event Action OnBannerLoadEvent;
        public event Action OnBannerFailedToLoadEvent;

        private void BannerAdStatusListener_mOnAdFailed(object sender, AdLoadErrorCodeEventArgs e)
        {
            Debug.LogError("[HMS] HMSAdsKitManager BannerAdLoadFailed. Error Code: " + e.ErrorCode);
            OnBannerFailedToLoadEvent?.Invoke();
        }

        private void BannerAdStatusListener_mOnAdOpened(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdOpened : ");
        }

        private void BannerAdStatusListener_mOnAdClicked(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdClicked : ");
        }

        private void BannerAdStatusListener_mOnAdImpression(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdImpression : ");
        }

        private void BannerAdStatusListener_mOnAdClosed(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdClosed : ");
        }

        private void BannerAdStatusListener_mOnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdLoadSuccess : ");
            _isBannerAdLoaded = true;
            OnBannerLoadEvent?.Invoke();
        }

        #endregion

        #endregion

        #region INTERSTITIAL

        #region PUBLIC METHODS

        public void LoadInterstitialAd()
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableInterstitialAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager Loading Interstitial Ad.");
            interstitialView = new InterstitialAd
            {
                AdId = adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestInterstitialAdId : adsKitSettings.Get(HMSAdsKitSettings.InterstitialAdID),
                AdListener = new InterstitialAdListener(this)
            };
            interstitialView.LoadAd(new AdParam.Builder().Build());
        }

        public void LoadInterstitialAd(string adId)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableInterstitialAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager Loading Interstitial Ad.");
            interstitialView = new InterstitialAd
            {
                AdId = adId,
                AdListener = new InterstitialAdListener(this)
            };
            interstitialView.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowInterstitialAd()
        {
            Debug.Log("[HMS] HMSAdsKitManager ShowInterstitialAd called");
            if (interstitialView?.Loaded == true)
            {
                Debug.Log("[HMS] HMSAdsKitManager Showing Interstitial Ad");
                interstitialView.Show();
            }
            else
                Debug.LogError("[HMS] HMSAdsKitManager Interstitial Ad Still Not Loaded Yet!");
        }

        public bool IsInterstitialAdLoaded
        {
            get
            {
                if (interstitialView == null)
                    return false;
                return interstitialView.Loaded;
            }
        }

        #endregion

        #region LISTENERS
        private class InterstitialAdListener : IAdListener
        {
            private readonly HMSAdsKitManager mAdsManager;
            public InterstitialAdListener(HMSAdsKitManager adsManager)
            {
                mAdsManager = adsManager;
            }

            public void OnAdClicked()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdClicked");
                mAdsManager.OnInterstitialAdClicked?.Invoke();
            }

            public void OnAdClosed()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdClosed");
                mAdsManager.OnInterstitialAdClosed?.Invoke();
                mAdsManager.LoadInterstitialAd();
            }

            public void OnAdFailed(int reason)
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdFailed");
                mAdsManager.OnInterstitialAdFailed?.Invoke(reason);
            }

            public void OnAdImpression()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdImpression");
                mAdsManager.OnInterstitialAdImpression?.Invoke();
            }

            public void OnAdLeave()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdLeave");
                mAdsManager.OnInterstitialAdLeave?.Invoke();
            }

            public void OnAdLoaded()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdLoaded");
                mAdsManager.OnInterstitialAdLoaded?.Invoke();
            }

            public void OnAdOpened()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdOpened");
                mAdsManager.OnInterstitialAdOpened?.Invoke();
            }
        }

        public Action OnInterstitialAdClicked { get; set; }
        public Action OnInterstitialAdClosed { get; set; }
        public Action<int> OnInterstitialAdFailed { get; set; }
        public Action OnInterstitialAdImpression { get; set; }
        public Action OnInterstitialAdLeave { get; set; }
        public Action OnInterstitialAdLoaded { get; set; }
        public Action OnInterstitialAdOpened { get; set; }

        #endregion

        #endregion

        #region REWARDED

        #region PUBLIC METHODS

        public void LoadRewardedAd()
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableRewardedAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager LoadRewardedAd");
            rewardedView = new RewardAd(adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestRewardedAdId : adsKitSettings.Get(HMSAdsKitSettings.RewardedAdID));
            rewardedView.RewardAdListener = new RewardAdListener(this);
            rewardedView.LoadAd(new AdParam.Builder().Build());
        }

        public void LoadRewardedAd(string adId)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableRewardedAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager LoadRewardedAd");
            rewardedView = new RewardAd(adId);
            rewardedView.RewardAdListener = new RewardAdListener(this);
            rewardedView.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowRewardedAd()
        {
            Debug.Log("[HMS] HMSAdsKitManager ShowRewardedAd called");
            if (rewardedView?.Loaded == true)
            {
                Debug.Log("[HMS] HMSAdsKitManager Showing Rewarded Ad");
                rewardedView.Show();
            }
            else
            {
                Debug.LogError("[HMS] HMSAdsKitManager Rewarded Ad still not loaded");
            }
        }

        public bool IsRewardedAdLoaded
        {
            get
            {
                if (rewardedView == null)
                    return false;
                return rewardedView.Loaded;
            }
        }

        #endregion

        #region LISTENERS

        private class RewardAdListener : IRewardAdListener
        {
            private readonly HMSAdsKitManager mAdsManager;

            public RewardAdListener(HMSAdsKitManager adsManager)
            {
                mAdsManager = adsManager;
            }
            public void OnRewardAdClosed()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdClosed");
                mAdsManager.OnRewardAdClosed?.Invoke();
                mAdsManager.LoadRewardedAd();
            }

            public void OnRewardAdCompleted()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdCompleted!");
                mAdsManager.OnRewardAdCompleted?.Invoke();
            }

            public void OnRewardAdFailedToLoad(int errorCode)
            {
                Debug.Log($"[HMS] HMSAdsKitManager Rewarded ad loading failed with error ${errorCode}");
                mAdsManager.OnRewardedAdFailedToLoad?.Invoke(errorCode);
            }

            public void OnRewardAdLeftApp()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdLeftApp!");
                mAdsManager.OnRewardAdLeftApp?.Invoke();
            }

            public void OnRewardAdLoaded()
            {
                Debug.Log("[HMS] HMSAdsKitManager Rewarded ad loaded!");
                mAdsManager.OnRewardedAdLoaded?.Invoke();
            }

            public void OnRewardAdOpened()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdOpened");
                mAdsManager.OnRewardAdOpened?.Invoke();
            }

            public void OnRewardAdStarted()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdStarted!");
                mAdsManager.OnRewardAdStarted?.Invoke();
            }

            public void OnRewarded(Reward reward)
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewarded " + reward);
                mAdsManager.OnRewarded?.Invoke(reward);
            }
        }

        public Action OnRewardAdClosed { get; set; }
        public Action OnRewardAdLeftApp { get; set; }
        public Action OnRewardAdStarted { get; set; }
        public Action OnRewardAdOpened { get; set; }
        public Action OnRewardAdCompleted { get; set; }
        public Action<Reward> OnRewarded { get; set; }
        public Action OnRewardedAdLoaded { get; set; }
        public Action<int> OnRewardedAdFailedToLoad { get; set; }

        #endregion

        #endregion

        #region SPLASH

        #region PUBLIC METHODS

        public void LoadSplashAd()
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableSplashAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager Loading Splash Ad.");
            splashView = new SplashAd();
            splashView.AdId = adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestSplashImageAdId : adsKitSettings.Get(HMSAdsKitSettings.SplashAdID);
            splashView.Orientation = (SplashAdOrientation)Enum.Parse(typeof(SplashAdOrientation), adsKitSettings.Get(HMSAdsKitSettings.SplashOrientation, "PORTRAIT"));
            splashView.Title = string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashTitle)) ? "Splash Title" : adsKitSettings.Get(HMSAdsKitSettings.SplashTitle);
            splashView.SubText = string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashSubText)) ? "Splash SubText" : adsKitSettings.Get(HMSAdsKitSettings.SplashSubText);
            if (!string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashImageBytes)))
            {
                Texture2D texture = new Texture2D(28, 28);
                texture.LoadImage(Convert.FromBase64String(adsKitSettings.Get(HMSAdsKitSettings.SplashImageBytes)));
                splashView.Icon = texture;
            }
            splashView.SetSplashAdDisplayListener(new SplashAdDisplayListener(SplashAdStatusListener_OnAdShowed, SplashAdStatusListener_OnAdClicked));
            splashView.SetSplashAdLoadListener(new SplashAdLoadListener(SplashAdStatusListener_OnAdDismissed, SplashAdStatusListener_OnAdFailedToLoad, SplashAdStatusListener_OnAdLoaded));
            splashView.LoadAd(new AdParam.Builder().Build());
        }

        public void LoadSplashAd(string adId, SplashAdOrientation orientation)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableSplashAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager Loading Splash Ad.");
            splashView = new SplashAd();
            splashView.AdId = adId;
            splashView.Orientation = orientation;
            splashView.Title = string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashTitle)) ? "Splash Title" : adsKitSettings.Get(HMSAdsKitSettings.SplashTitle);
            splashView.SubText = string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashSubText)) ? "Splash SubText" : adsKitSettings.Get(HMSAdsKitSettings.SplashSubText);
            if (!string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashImageBytes)))
            {
                Texture2D texture = new Texture2D(28, 28);
                texture.LoadImage(Convert.FromBase64String(adsKitSettings.Get(HMSAdsKitSettings.SplashImageBytes)));
                splashView.Icon = texture;
            }
            splashView.SetSplashAdDisplayListener(new SplashAdDisplayListener(SplashAdStatusListener_OnAdShowed, SplashAdStatusListener_OnAdClicked));
            splashView.SetSplashAdLoadListener(new SplashAdLoadListener(SplashAdStatusListener_OnAdDismissed, SplashAdStatusListener_OnAdFailedToLoad, SplashAdStatusListener_OnAdLoaded));
            splashView.LoadAd(new AdParam.Builder().Build());
        }

        #endregion

        #region LISTENERS

        public event Action OnSplashAdDismissed;
        public event Action<int> OnSplashAdFailedToLoad;
        public event Action OnSplashAdLoaded;
        public event Action OnSplashAdClicked;
        public event Action OnSplashAdShowed;

        private void SplashAdStatusListener_OnAdDismissed()
        {
            Debug.Log("[HMS] HMSAdsKitManager SplashAdDismissed.");
            OnSplashAdDismissed?.Invoke();
        }

        private void SplashAdStatusListener_OnAdFailedToLoad(int errorCode)
        {
            Debug.LogError("[HMS] HMSAdsKitManager SplashAdLoadFailed. Error Code: " + errorCode);
            OnSplashAdFailedToLoad?.Invoke(errorCode);
        }

        private void SplashAdStatusListener_OnAdLoaded()
        {
            Debug.Log("[HMS] HMSAdsKitManager SplashAdLoaded.");
            OnSplashAdLoaded?.Invoke();
        }

        private void SplashAdStatusListener_OnAdClicked()
        {
            Debug.Log("[HMS] HMSAdsKitManager SplashAdClicked.");
            OnSplashAdClicked?.Invoke();
        }

        private void SplashAdStatusListener_OnAdShowed()
        {
            Debug.Log("[HMS] HMSAdsKitManager SplashAdShowed.");
            OnSplashAdShowed?.Invoke();
        }

        #endregion

        #endregion

        #region CONSENT

        #region PUBLIC METHODS

        public void AddTestDeviceId(String testDeviceId)
        {
            Consent consent = Consent.GetInstance();
            consent.AddTestDeviceId(testDeviceId);
        }

        public String GetTestDeviceId()
        {
            Consent consent = Consent.GetInstance();
            return consent.TestDeviceId;
        }

        public void RequestConsentUpdate()
        {
            Consent consent = Consent.GetInstance();
            consent.RequestConsentUpdate(new ConsentUpdateListener(this));
        }

        public void SetConsentStatus(ConsentStatus consentStatus)
        {
            Consent consent = Consent.GetInstance();
            consent.SetConsentStatus(ConsentStatusWrapper.ForValue((int)consentStatus));
        }

        public void SetDebugNeedConsent(DebugNeedConsent debugNeedConsent)
        {
            Consent consent = Consent.GetInstance();
            consent.SetDebugNeedConsent(DebugNeedConsentWrapper.ForValue((int)debugNeedConsent));
        }

        public void SetUnderAgeOfPromise(bool underAgeOfPromise)
        {
            Consent consent = Consent.GetInstance();
            consent.SetUnderAgeOfPromise(underAgeOfPromise);
        }

        #endregion

        #region LISTENERS

        private class ConsentUpdateListener : IConsentUpdateListener
        {
            private readonly HMSAdsKitManager mAdsManager;

            public ConsentUpdateListener(HMSAdsKitManager adsManager)
            {
                mAdsManager = adsManager;
            }
            void IConsentUpdateListener.OnFail(string desc)
            {
                Debug.Log("[HMS] HMSAdsKitManager CONSENT OnFail " + desc);
                mAdsManager.ConsentOnFail?.Invoke(desc);
            }

            void IConsentUpdateListener.OnSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
            {
                Debug.Log($"[HMS] HMSAdsKitManager CONSENT OnSuccess consentStatus:{consentStatus} isNeedConsent:{isNeedConsent} adProviders listSize:{adProviders.Count}");
                mAdsManager.ConsentOnSuccess?.Invoke(consentStatus, isNeedConsent, adProviders);
            }
        }
        public Action<string> ConsentOnFail { get; set; }
        public Action<ConsentStatus, bool, IList<AdProvider>> ConsentOnSuccess { get; set; }

        #endregion

        #endregion

    }

}