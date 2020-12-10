using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;
using HmsPlugin;

public class AdsDemoManager : MonoBehaviour
{

    private const string REWARD_AD_ID = "testx9dtjwj8hp";
    private const string INTERSTITIAL_AD_ID = "testb4znbuh3n2";
    private const string BANNER_AD_ID = "testw6vs28auh3";

    private RewardAdManager rewardAdManager;
    private InterstitialAdManager interstitialAdManager;
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
    {
        rewardAdManager = RewardAdManager.GetInstance();
        rewardAdManager.AdId = REWARD_AD_ID;
        rewardAdManager.OnRewarded = OnRewarded;
    }

    private void InitInterstitialAds()
    {
        interstitialAdManager = InterstitialAdManager.GetInstance();
        interstitialAdManager.AdId = INTERSTITIAL_AD_ID;
        interstitialAdManager.OnAdClosed = OnInterstitialAdClosed;
    }
    public void ShowBannerAd()
    {
        bannerAdsManager.ShowBannerAd();
    }
    public void HideBannerAd()
    {
        bannerAdsManager.HideBannerAd();
    }
    public void ShowRewardedAd()
    {
        Debug.Log("[HMS] AdsDemoManager ShowRewardedAd");
        rewardAdManager.ShowRewardedAd();
    }

    public void ShowInterstitialAd()
    {
        Debug.Log("[HMS] AdsDemoManager ShowInterstitialAd");
        interstitialAdManager.ShowInterstitialAd();
    }

    public void OnRewarded(Reward reward)
    {
        Debug.Log("[HMS] AdsDemoManager rewarded!");
    }

    public void OnInterstitialAdClosed()
    {
        Debug.Log("[HMS] AdsDemoManager interstitial ad closed");
    }
}
