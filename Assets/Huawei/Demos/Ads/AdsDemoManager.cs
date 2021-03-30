using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;
using HmsPlugin;

public class AdsDemoManager : MonoBehaviour
{
    private void Start()
    {
        HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
        HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;
    }

    public void ShowBannerAd()
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
}
