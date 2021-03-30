using HmsPlugin;
using HuaweiConstants;
using HuaweiMobileServices.Ads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HuaweiConstants.UnityBannerAdPositionCode;

public class HMSAdsKitManager : HMSSingleton<HMSAdsKitManager>
{

    private const string TestBannerAdId = "testw6vs28auh3";
    private const string TestInterstitialAdId = "testb4znbuh3n2";
    private const string TestRewardedAdId = "testx9dtjwj8hp";

    private BannerAd bannerView;
    private InterstitialAd interstitialView;
    private RewardAd rewardedView;

    private bool isInitialized;

    private void Start()
    {
        Init();
    }

    private Settings GetAdSettings()
    {
        return HMSAdsKitSettings.Instance.Settings;
    }

    private void Init()
    {
        Debug.Log("[HMS] HMSAdsKitManager Start");
        HwAds.Init();
        isInitialized = true;
        StartCoroutine(LoadingAds());
    }

    private IEnumerator LoadingAds()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
            yield return null;

        Debug.Log("[HMS] HMSAdsKitManager Loading Ads");
        LoadAllAds(GetAdSettings());
    }

    public void LoadAllAds(Settings settings, bool hasPurchasedNoAds = false)
    {
        if (!hasPurchasedNoAds)
        {
            LoadBannerAd(settings, UnityBannerAdPositionCodeType.POSITION_BOTTOM);
            LoadInterstitialAd(settings);
        }
        LoadRewardedAd(settings);
    }

    #region BANNER

    #region PUBLIC METHODS

    public void LoadBannerAd(Settings settings, UnityBannerAdPositionCodeType position, bool show = true, string bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50)
    {
        if (!isInitialized || !settings.GetBool(HMSAdsKitSettings.EnableBannerAd)) return;

        Debug.Log("[HMS] HMSAdsKitManager Loading Banner Ad.");

        var bannerAdStatusListener = new AdStatusListener();
        bannerAdStatusListener.mOnAdLoaded += BannerAdStatusListener_mOnAdLoaded;
        bannerAdStatusListener.mOnAdClosed += BannerAdStatusListener_mOnAdClosed;
        bannerAdStatusListener.mOnAdImpression += BannerAdStatusListener_mOnAdImpression;
        bannerAdStatusListener.mOnAdClicked += BannerAdStatusListener_mOnAdClicked;
        bannerAdStatusListener.mOnAdOpened += BannerAdStatusListener_mOnAdOpened;
        bannerAdStatusListener.mOnAdFailed += BannerAdStatusListener_mOnAdFailed;

        bannerView = new BannerAd(bannerAdStatusListener);
        bannerView.AdId = settings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestBannerAdId : settings.Get(HMSAdsKitSettings.BannerAdID);
        bannerView.PositionType = (int)position;
        bannerView.SizeType = bannerSize;
        bannerView.AdStatusListener = bannerAdStatusListener;
        bannerView.LoadBanner(new AdParam.Builder().Build());
        if (show)
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

    #endregion

    #region LISTENERS

    public event Action OnBannerLoadEvent;
    public event Action OnBannerFailedToLoadEvent;

    private void BannerAdStatusListener_mOnAdFailed(object sender, AdLoadErrorCodeEventArgs e)
    {
        Debug.Log("[HMS] HMSAdsKitManager BannerAdLoadFailed. Error Code: " + e.ErrorCode);
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
        OnBannerLoadEvent?.Invoke();
    }

    #endregion

    #endregion

    #region INTERSTITIAL

    #region PUBLIC METHODS

    public void LoadInterstitialAd(Settings settings)
    {
        if (!isInitialized || !settings.GetBool(HMSAdsKitSettings.EnableInterstitialAd)) return;
        Debug.Log("[HMS] HMSAdsKitManager Loading Interstitial Ad.");
        interstitialView = new InterstitialAd
        {
            AdId = settings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestInterstitialAdId : settings.Get(HMSAdsKitSettings.InterstitialAdID),
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
            Debug.LogError("[HMS] HMSAdsKitManager Interstitial Still Ad Not Loaded Yet!");
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
            mAdsManager.LoadInterstitialAd(mAdsManager.GetAdSettings());
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

    public void LoadRewardedAd(Settings settings)
    {
        if (!isInitialized || !settings.GetBool(HMSAdsKitSettings.EnableRewardedAd)) return;
        Debug.Log("[HMS] HMSAdsKitManager LoadRewardedAd");
        rewardedView = new RewardAd(settings.GetBool(HMSAdsKitSettings.UseTestAds) ? TestRewardedAdId : settings.Get(HMSAdsKitSettings.RewardedAdID));
        rewardedView.LoadAd(new AdParam.Builder().Build(), () => { Debug.Log("[HMS] HMSAdsKitManager Rewarded ad loaded!"); }, (errorCode) => { Debug.Log($"[HMS] HMSAdsKitManager Rewarded ad loading failed with error ${errorCode}"); });
    }

    public void ShowRewardedAd()
    {
        Debug.Log("[HMS] HMSAdsKitManager ShowRewardedAd called");
        if (rewardedView?.Loaded == true)
        {
            Debug.Log("[HMS] HMSAdsKitManager Showing Rewarded Ad");
            rewardedView.Show(new RewardAdListener(this));
        }
        else
        {
            Debug.LogError("[HMS] HMSAdsKitManager still not loaded");
        }
    }

    #endregion

    #region LISTENERS

    private class RewardAdListener : IRewardAdStatusListener
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
            mAdsManager.LoadRewardedAd(mAdsManager.GetAdSettings());
        }

        public void OnRewardAdFailedToShow(int errorCode)
        {
            Debug.Log("[HMS] HMSAdsKitManager OnRewardAdFailedToShow " + errorCode);
            mAdsManager.OnRewardAdFailedToShow?.Invoke(errorCode);
        }

        public void OnRewardAdOpened()
        {
            Debug.Log("[HMS] HMSAdsKitManager OnRewardAdOpened");
            mAdsManager.OnRewardAdOpened?.Invoke();
        }

        public void OnRewarded(Reward reward)
        {
            Debug.Log("[HMS] HMSAdsKitManager OnRewarded " + reward);
            mAdsManager.OnRewarded?.Invoke(reward);
        }
    }

    public Action OnRewardAdClosed { get; set; }
    public Action<int> OnRewardAdFailedToShow { get; set; }
    public Action OnRewardAdOpened { get; set; }
    public Action<Reward> OnRewarded { get; set; }

    #endregion

    #endregion
}
