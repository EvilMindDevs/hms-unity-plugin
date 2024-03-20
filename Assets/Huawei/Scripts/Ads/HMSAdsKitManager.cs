using HuaweiConstants;
using HuaweiMobileServices.Ads;
using HuaweiMobileServices.Ads.InstallReferrer;
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
        private readonly string TAG = "[HMS] HMSAdsKitManager";

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

        private InstallReferrerClient installReferrerClient;

        public Action<ReferrerDetails> InstallReferrerSuccess { get; set; }
        public Action<InstallReferrerResponse> InstallReferrerFail { get; set; }
        public Action InstallReferrerDisconnect { get; set; }

        public bool hasPurchasedNoAds = false;

        public HMSAdsKitManager()
        {
            adsKitSettings = HMSAdsKitSettings.Instance.Settings;
            HMSManagerStart.Start(OnAwake, OnStart, TAG);
        }

        public HMSAdsKitManager(bool hasPurchasedNoAds)
        {
            adsKitSettings = HMSAdsKitSettings.Instance.Settings;
            this.hasPurchasedNoAds = hasPurchasedNoAds;
            HMSManagerStart.Start(OnAwake, () => { OnStart(hasPurchasedNoAds); }, TAG);
        }

        private void OnAwake()
        {
            Debug.Log($"{TAG} OnAwake");
            Init();
        }

        private void OnStart()
        {
            Debug.Log($"{TAG} OnStart");
        }

        private void OnStart(bool hasPurchasedNoAds = false)
        {
            Debug.Log($"{TAG} OnStart");
            _ = LoadAdsWhenInternetIsAvailableAsync(hasPurchasedNoAds);
        }

        private void Init()
        {
            Debug.Log($"{TAG} Init");
            HwAds.Init();
            isInitialized = true;
            adsKitSettings = HMSAdsKitSettings.Instance.Settings;
        }

        private async Task LoadAdsWhenInternetIsAvailableAsync(bool hasPurchasedNoAds = false)
        {
            await WaitForInternetConnectionAsync();
            Debug.Log($"{TAG} Loading Ads");
            LoadAllAds(hasPurchasedNoAds);
        }

        private async Task WaitForInternetConnectionAsync()
        {
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        public void LoadAllAds(bool hasPurchasedNoAds = false)
        {
            if (!hasPurchasedNoAds)
            {
                if (adsKitSettings.GetBool(HMSAdsKitSettings.EnableSplashAd))
                    LoadSplashAd();
                UnityBannerAdPositionCodeType _adPositionCodeType = UnityBannerAdPositionCodeType.POSITION_BOTTOM;
                UnityBannerAdSizeType _adSizeType = UnityBannerAdSizeType.BANNER_SIZE_360_57;

                if (Enum.TryParse(adsKitSettings.Get(HMSAdsKitSettings.BannerAdPositionType), out UnityBannerAdPositionCodeType adPositionCodeType))
                    _adPositionCodeType = adPositionCodeType;
                if (Enum.TryParse(adsKitSettings.Get(HMSAdsKitSettings.UnityBannerAdSizeType), out UnityBannerAdSizeType adSizeType))
                    _adSizeType = adSizeType;

                LoadBannerAd(_adPositionCodeType, _adSizeType.ToString());
                LoadInterstitialAd();
            }
            LoadRewardedAd();
        }


        public void SetTestAdStatus(bool value)
        {
            adsKitSettings.SetBool(HMSAdsKitSettings.UseTestAds, value);
            Debug.Log($"{TAG} SetTestAdStatus set to " + value.ToString());
        }

        public void SetTestAd(bool status)
        {
            SetTestAdStatus(status);
            DestroyBannerAd();
            LoadAllAds();
        }

        #region Install-Referrer

        public void Init_InstallReferrer(bool isTest)
        {

            var installReferrerStateCallbackListener =
                new InstallReferrerStateCallbackListener(
                    OnInstallReferrerSetupFinished,
                OnInstallReferrerServiceDisconnected);

            InstallReferrerStateBridge.SetInstallReferrerCallbackListener(installReferrerStateCallbackListener);

            var listener = InstallReferrerStateBridge.GetInstallReferrerStateCallback();

            HMSDispatcher.Invoke(() =>
            {
                installReferrerClient = InstallReferrerClient.NewBuilder().SetTest(isTest).Build();
                installReferrerClient.StartConnection(listener);
            });

        }

        private void OnInstallReferrerSetupFinished(int responseCode)
        {
            Debug.Log($"{TAG} OnInstallReferrerSetupFinished");

            var response = (InstallReferrerResponse)(responseCode);

            if (response == InstallReferrerResponse.OK)
            {
                Debug.Log($"{TAG} Install Referrer Setup Finished");

                var referrerDetails = installReferrerClient.GetInstallReferrer();

                InstallReferrerSuccess?.Invoke(referrerDetails);

                return;
            }

            if (response == InstallReferrerResponse.SERVICE_UNAVAILABLE)
            {
                Debug.LogError($"{TAG} InstallReferrerResponse: SERVICE_UNAVAILABLE");

                InstallReferrerFail?.Invoke(response);

                return;
            }

            if (response == InstallReferrerResponse.FEATURE_NOT_SUPPORTED)
            {
                Debug.LogError($"{TAG} FEATURE_NOT_SUPPORTED");

                InstallReferrerFail?.Invoke(response);

                return;
            }

            if (response == InstallReferrerResponse.DEVELOPER_ERROR)
            {
                Debug.LogError($"{TAG} DEVELOPER_ERROR");

                InstallReferrerFail?.Invoke(response);

                return;
            }

        }

        private void OnInstallReferrerServiceDisconnected()
        {
            Debug.Log($"{TAG} OnInstallReferrerServiceDisconnected");

            InstallReferrerDisconnect?.Invoke();
        }

        #endregion

        #region BANNER

        #region PUBLIC METHODS

        public void LoadBannerAd(UnityBannerAdPositionCodeType position, string bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50)
        {
            LoadBannerAd(
                adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestBannerAdId : adsKitSettings.Get(HMSAdsKitSettings.BannerAdID),
                position,
                bannerSize
            );
        }

        public void LoadBannerAd(string adId, UnityBannerAdPositionCodeType position, string bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableBannerAd)) return;

            Debug.Log($"{TAG} Loading Banner Ad.");

            var bannerAdStatusListener = CreateBannerAdStatusListener();

            bannerView = new BannerAd(bannerAdStatusListener)
            {
                AdId = adId,
                PositionType = (int)position,
                SizeType = bannerSize,
                AdStatusListener = bannerAdStatusListener
            };

            string bannerRefreshInterval = adsKitSettings.Get(HMSAdsKitSettings.BannerRefreshInterval);
            if (long.TryParse(bannerRefreshInterval, out long bannerRefresh))
            {
                bannerView.BannerRefresh = bannerRefresh;
            }
            else if (!string.IsNullOrEmpty(bannerRefreshInterval))
            {
                Debug.Log($"{TAG} Failed to parse BannerRefreshInterval.");
            }

            _isBannerAdLoaded = false;
            bannerView.LoadBanner(new AdParam.Builder().Build());

            if (adsKitSettings.GetBool(HMSAdsKitSettings.ShowBannerOnLoad))
            {
                bannerView?.ShowBanner();
            }
            else
            {
                bannerView?.HideBanner();
            }
        }

        private AdStatusListener CreateBannerAdStatusListener()
        {
            var bannerAdStatusListener = new AdStatusListener();
            bannerAdStatusListener.mOnAdLoaded += BannerAdStatusListener_mOnAdLoaded;
            bannerAdStatusListener.mOnAdClosed += BannerAdStatusListener_mOnAdClosed;
            bannerAdStatusListener.mOnAdImpression += BannerAdStatusListener_mOnAdImpression;
            bannerAdStatusListener.mOnAdClicked += BannerAdStatusListener_mOnAdClicked;
            bannerAdStatusListener.mOnAdOpened += BannerAdStatusListener_mOnAdOpened;
            bannerAdStatusListener.mOnAdFailed += BannerAdStatusListener_mOnAdFailed;

            return bannerAdStatusListener;
        }

        public void ShowBannerAd()
        {
            if (bannerView == null)
            {
                Debug.LogError($"{TAG} Banner Ad is Null.");
                return;
            }
            bannerView.ShowBanner();
        }

        public void HideBannerAd()
        {
            if (bannerView == null)
            {
                Debug.LogError($"{TAG} Banner Ad is Null.");
                return;
            }
            bannerView.HideBanner();
        }

        public void DestroyBannerAd()
        {
            if (bannerView == null)
            {
                Debug.LogError($"{TAG} Banner Ad is Null.");
                return;
            }
            bannerView.DestroyBanner();
            _isBannerAdLoaded = false;
        }

        public void SetBannerRefresh(long seconds)
        {
            if (bannerView != null)
            {
                bannerView.SetBannerRefresh(seconds);
            }
            else
            {
                Debug.LogError($"{TAG} BannerView not initialized yet.");
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
            Debug.LogError($"{TAG} BannerAdLoadFailed. Error Code: " + e.ErrorCode);
            OnBannerFailedToLoadEvent?.Invoke();
        }

        private void BannerAdStatusListener_mOnAdOpened(object sender, EventArgs e)
        {
            Debug.Log($"{TAG} BannerAdOpened : ");
        }

        private void BannerAdStatusListener_mOnAdClicked(object sender, EventArgs e)
        {
            Debug.Log($"{TAG} BannerAdClicked : ");
        }

        private void BannerAdStatusListener_mOnAdImpression(object sender, EventArgs e)
        {
            Debug.Log($"{TAG} BannerAdImpression : ");
        }

        private void BannerAdStatusListener_mOnAdClosed(object sender, EventArgs e)
        {
            Debug.Log($"{TAG} BannerAdClosed : ");
        }

        private void BannerAdStatusListener_mOnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log($"{TAG} BannerAdLoadSuccess : ");
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
            Debug.Log($"{TAG} Loading Interstitial Ad.");
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
            Debug.Log($"{TAG} Loading Interstitial Ad.");
            interstitialView = new InterstitialAd
            {
                AdId = adId,
                AdListener = new InterstitialAdListener(this)
            };
            interstitialView.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowInterstitialAd()
        {
            Debug.Log($"{TAG} ShowInterstitialAd called");
            if (interstitialView?.Loaded == true)
            {
                Debug.Log($"{TAG} Showing Interstitial Ad");
                interstitialView.Show();
            }
            else
                Debug.LogError($"{TAG} Interstitial Ad Still Not Loaded Yet!");
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
            private const string TAG = "[HMS] HMSAdsKitManager";
            private readonly HMSAdsKitManager mAdsManager;
            public InterstitialAdListener(HMSAdsKitManager adsManager)
            {
                mAdsManager = adsManager;
            }

            public void OnAdClicked()
            {
                Debug.Log($"{TAG} OnInterstitialAdClicked");
                mAdsManager.OnInterstitialAdClicked?.Invoke();
            }

            public void OnAdClosed()
            {
                Debug.Log($"{TAG} OnInterstitialAdClosed");
                mAdsManager.OnInterstitialAdClosed?.Invoke();
                mAdsManager.LoadInterstitialAd();
            }

            public void OnAdFailed(int reason)
            {
                HMSExceptionHandler.Instance.HandleLogForListener($"{TAG} OnInterstitialAdFailed with error ${reason}", string.Empty, LogType.Error);
                mAdsManager.OnInterstitialAdFailed?.Invoke(reason);
            }

            public void OnAdImpression()
            {
                Debug.Log($"{TAG} OnInterstitialAdImpression");
                mAdsManager.OnInterstitialAdImpression?.Invoke();
            }

            public void OnAdLeave()
            {
                Debug.Log($"{TAG} OnInterstitialAdLeave");
                mAdsManager.OnInterstitialAdLeave?.Invoke();
            }

            public void OnAdLoaded()
            {
                Debug.Log($"{TAG} OnInterstitialAdLoaded");
                mAdsManager.OnInterstitialAdLoaded?.Invoke();
            }

            public void OnAdOpened()
            {
                Debug.Log($"{TAG} OnInterstitialAdOpened");
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
            Debug.Log($"{TAG} LoadRewardedAd");
            rewardedView = new RewardAd(adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestRewardedAdId : adsKitSettings.Get(HMSAdsKitSettings.RewardedAdID));
            rewardedView.RewardAdListener = new RewardAdListener(this);
            rewardedView.LoadAd(new AdParam.Builder().Build());
        }

        public void LoadRewardedAd(string adId)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableRewardedAd)) return;
            Debug.Log($"{TAG} LoadRewardedAd");
            rewardedView = new RewardAd(adId);
            rewardedView.RewardAdListener = new RewardAdListener(this);
            rewardedView.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowRewardedAd()
        {
            Debug.Log($"{TAG} ShowRewardedAd called");
            if (rewardedView?.Loaded == true)
            {
                Debug.Log($"{TAG} Showing Rewarded Ad");
                rewardedView.Show();
            }
            else
            {
                Debug.LogError($"{TAG} Rewarded Ad still not loaded");
            }
        }

        public void SetRewardVerifyConfig(RewardVerifyConfig config)
        {
            Debug.Log($"{TAG} SetRewardVerifyConfig called");
            if (rewardedView?.Loaded == true)
            {
                Debug.Log($"{TAG} SetRewardVerifyConfig ");
                rewardedView.SetRewardVerifyConfig(config);
            }
            else
            {
                Debug.LogError($"{TAG} Rewarded Ad still not loaded");
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
            private const string TAG = "[HMS] HMSAdsKitManager";
            private readonly HMSAdsKitManager mAdsManager;

            public RewardAdListener(HMSAdsKitManager adsManager)
            {
                mAdsManager = adsManager;
            }
            public void OnRewardAdClosed()
            {
                Debug.Log($"{TAG} OnRewardAdClosed");
                mAdsManager.OnRewardAdClosed?.Invoke();
                mAdsManager.LoadRewardedAd();
            }

            public void OnRewardAdCompleted()
            {
                Debug.Log($"{TAG} OnRewardAdCompleted!");
                mAdsManager.OnRewardAdCompleted?.Invoke();
            }

            public void OnRewardAdFailedToLoad(int errorCode)
            {
                HMSExceptionHandler.Instance.HandleLogForListener($"{TAG} OnRewardAdFailedToLoad with error ${errorCode}", string.Empty, LogType.Error);
                mAdsManager.OnRewardedAdFailedToLoad?.Invoke(errorCode);
            }

            public void OnRewardAdLeftApp()
            {
                Debug.Log($"{TAG} OnRewardAdLeftApp!");
                mAdsManager.OnRewardAdLeftApp?.Invoke();
            }

            public void OnRewardAdLoaded()
            {
                Debug.Log($"{TAG} Rewarded ad loaded!");
                mAdsManager.OnRewardedAdLoaded?.Invoke();
            }

            public void OnRewardAdOpened()
            {
                Debug.Log($"{TAG} OnRewardAdOpened");
                mAdsManager.OnRewardAdOpened?.Invoke();
            }

            public void OnRewardAdStarted()
            {
                Debug.Log($"{TAG} OnRewardAdStarted!");
                mAdsManager.OnRewardAdStarted?.Invoke();
            }

            public void OnRewarded(Reward reward)
            {
                Debug.Log($"{TAG} OnRewarded " + reward);
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
            string adId = adsKitSettings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestSplashImageAdId : adsKitSettings.Get(HMSAdsKitSettings.SplashAdID);
            if (!Enum.TryParse(adsKitSettings.Get(HMSAdsKitSettings.SplashOrientation, "PORTRAIT"), out SplashAdOrientation orientation))
            {
                orientation = SplashAdOrientation.PORTRAIT;
            }
            LoadSplashAd(adId, orientation);
        }

        public void LoadSplashAd(string adId, SplashAdOrientation orientation)
        {
            if (!isInitialized || !adsKitSettings.GetBool(HMSAdsKitSettings.EnableSplashAd)) return;
            Debug.Log($"{TAG} Loading Splash Ad.");
            splashView = new SplashAd
            {
                AdId = adId,
                Orientation = orientation,
                Title = string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashTitle)) ? "Splash Title" : adsKitSettings.Get(HMSAdsKitSettings.SplashTitle),
                SubText = string.IsNullOrEmpty(adsKitSettings.Get(HMSAdsKitSettings.SplashSubText)) ? "Splash SubText" : adsKitSettings.Get(HMSAdsKitSettings.SplashSubText)
            };

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
            Debug.Log($"{TAG} SplashAdDismissed.");
            OnSplashAdDismissed?.Invoke();
        }

        private void SplashAdStatusListener_OnAdFailedToLoad(int errorCode)
        {
            HMSExceptionHandler.Instance.HandleLogForListener($"{TAG} SplashAdLoadFailed. Error Code: " + errorCode, string.Empty, LogType.Error);
            OnSplashAdFailedToLoad?.Invoke(errorCode);
        }

        private void SplashAdStatusListener_OnAdLoaded()
        {
            Debug.Log($"{TAG} SplashAdLoaded.");
            OnSplashAdLoaded?.Invoke();
        }

        private void SplashAdStatusListener_OnAdClicked()
        {
            Debug.Log($"{TAG} SplashAdClicked.");
            OnSplashAdClicked?.Invoke();
        }

        private void SplashAdStatusListener_OnAdShowed()
        {
            Debug.Log($"{TAG} SplashAdShowed.");
            OnSplashAdShowed?.Invoke();
        }

        #endregion

        #endregion

        #region CONSENT

        #region PUBLIC METHODS

        public void AddTestDeviceId(string testDeviceId)
        {
            Consent consent = Consent.GetInstance();
            consent.AddTestDeviceId(testDeviceId);
        }

        public string GetTestDeviceId()
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
            private readonly string TAG = "[HMS] HMSAdsKitManager";

            private readonly HMSAdsKitManager mAdsManager;

            public ConsentUpdateListener(HMSAdsKitManager adsManager)
            {
                mAdsManager = adsManager;
            }
            void IConsentUpdateListener.OnFail(string desc)
            {
                HMSExceptionHandler.Instance.HandleLogForListener($"{TAG} AdsKitManager CONSENT OnFail " + desc, string.Empty, LogType.Error);

                if (mAdsManager != null)
                {
                    mAdsManager.ConsentOnFail?.Invoke(desc);
                }
                else
                {
                    Debug.LogWarning($"{TAG} Make sure to call RequestConsentUpdate First");
                }

            }

            void IConsentUpdateListener.OnSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
            {
                Debug.Log($"{TAG} HMSAdsKitManager CONSENT OnSuccess consentStatus:{consentStatus} isNeedConsent:{isNeedConsent} adProviders listSize:{adProviders.Count}");
                if (mAdsManager != null)
                {
                    mAdsManager.ConsentOnSuccess?.Invoke(consentStatus, isNeedConsent, adProviders);
                }
                else
                {
                    Debug.LogWarning($"{TAG} Make sure to call RequestConsentUpdate First");
                }

            }
        }
        public Action<string> ConsentOnFail { get; set; }
        public Action<ConsentStatus, bool, IList<AdProvider>> ConsentOnSuccess { get; set; }

        #endregion

        #endregion

    }

}
