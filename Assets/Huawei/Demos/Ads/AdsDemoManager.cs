using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;
using HmsPlugin;
using UnityEngine.UI;

public class AdsDemoManager : MonoBehaviour
{
    [SerializeField]
    private Toggle testAdStatusToggle;

<<<<<<< HEAD
    private const string REWARD_AD_ID = "testx9dtjwj8hp";
    private const string INTERSTITIAL_AD_ID = "testb4znbuh3n2";
    private const string BANNER_AD_ID = "testw6vs28auh3";

    private RewardAdManager rewardAdManager;
    private InterstitalAdManager interstitialAdManager;
    private BannerAdsManager bannerAdsManager;

    // Start is called before the first frame update
    void Start()
    {
        InitBannerAds();
        InitRewardedAds();
        InitInterstitialAds();
    }
    private void InitBannerAds()
    {
        bannerAdsManager = BannerAdsManager.GetInstance();
        bannerAdsManager.AdId = BANNER_AD_ID;
    }
    private void InitRewardedAds()
=======
    private void Start()
    {
        HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
        HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;
        testAdStatusToggle.isOn = HMSAdsKitSettings.Instance.Settings.GetBool(HMSAdsKitSettings.UseTestAds);
    }

    public void ShowBannerAd()
>>>>>>> EvilMindDevs-master
    {
        HMSAdsKitManager.Instance.ShowBannerAd();
    }

    public void HideBannerAd()
    {
        HMSAdsKitManager.Instance.HideBannerAd();
    }

    public void ShowRewardedAd()
    {
        Debug.Log("[HMS] AdsDemoManager ShowRewardedAd");
        HMSAdsKitManager.Instance.ShowRewardedAd();
    }

    public void ShowInterstitialAd()
    {
        Debug.Log("[HMS] AdsDemoManager ShowInterstitialAd");
        HMSAdsKitManager.Instance.ShowInterstitialAd();
    }

    public void OnRewarded(Reward reward)
    {
        Debug.Log("[HMS] AdsDemoManager rewarded!");
    }

    public void OnInterstitialAdClosed()
    {
        Debug.Log("[HMS] AdsDemoManager interstitial ad closed");
    }

    public void SetTestAdStatus()
    {
        HMSAdsKitManager.Instance.SetTestAdStatus(testAdStatusToggle.isOn);
        HMSAdsKitManager.Instance.DestroyBannerAd();
        HMSAdsKitManager.Instance.LoadAllAds();
    }
}
