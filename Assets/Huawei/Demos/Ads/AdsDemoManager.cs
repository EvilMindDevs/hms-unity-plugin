using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;

public class AdsDemoManager : MonoBehaviour
{

    private class RewardAdListener : IRewardAdStatusListener
    {
        private readonly AdsManager mAdsManager;

        public RewardAdListener(AdsManager adsManager)
        {
            mAdsManager = adsManager;
        }

        public void OnRewardAdClosed()
        {
            Debug.Log("[HMS] AdsManager OnRewardAdClosed");
            mAdsManager.LoadNextRewardedAd();
        }

        public void OnRewardAdFailedToShow(int errorCode)
        {
            Debug.Log("[HMS] AdsManager OnRewardAdClosed " + errorCode);
            mAdsManager.LoadNextRewardedAd();
        }

        public void OnRewardAdOpened()
        {
            Debug.Log("[HMS] AdsManager OnRewardAdOpened");
        }

        public void OnRewarded(Reward reward)
        {
            Debug.Log("[HMS] AdsManager OnRewarded " + reward);
        }
    }

    private class InterstitialAdListener : IAdListener
    {
        private readonly AdsManager mAdsManager;

        public InterstitialAdListener(AdsManager adsManager)
        {
            mAdsManager = adsManager;
        }

        public void OnAdClicked()
        {
            Debug.Log("[HMS] AdsManager OnAdClicked");
        }

        public void OnAdClosed()
        {
            Debug.Log("[HMS] AdsManager OnAdClosed");
            mAdsManager.LoadNextInterstitialAd();
        }

        public void OnAdFailed(int reason)
        {
            Debug.Log("[HMS] AdsManager OnAdFailed");
        }

        public void OnAdImpression()
        {
            Debug.Log("[HMS] AdsManager OnAdImpression");
        }

        public void OnAdLeave()
        {
            Debug.Log("[HMS] AdsManager OnAdLeave");
        }

        public void OnAdLoaded()
        {
            Debug.Log("[HMS] AdsManager OnAdLoaded");
        }

        public void OnAdOpened()
        {
            Debug.Log("[HMS] AdsManager OnAdOpened");
        }
    }

    private RewardAd rewardAd = null;
    private InterstitialAd interstitialAd = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[HMS] AdsManager Start");
        LoadNextRewardedAd();
        LoadNextInterstitialAd();
    }

    private void LoadNextRewardedAd()
    {
        Debug.Log("[HMS] AdsManager LoadNextRewardedAd");
        rewardAd = new RewardAd("testx9dtjwj8hp");
        rewardAd.LoadAd(new AdParam.Builder().Build(),
            () => { Debug.Log("[HMS] Ad loaded!"); },
            (errorCode) =>
            {
                Debug.Log("[HMS] Ad loading failed");
                LoadNextRewardedAd();
            }
        );
    }

    private void LoadNextInterstitialAd()
    {
        Debug.Log("[HMS] AdsManager LoadNextInterstitialAd");
        interstitialAd = new InterstitialAd
        {
            AdId = "testb4znbuh3n2",
            AdListener = new InterstitialAdListener(this)
        };
        interstitialAd.LoadAd(new AdParam.Builder().Build());
    }

    public void ShowRewardedAd()
    {
        Debug.Log("[HMS] AdsManager ShowRewardedAd");
        if (rewardAd != null)
        {
            if (rewardAd.Loaded)
            {
                Debug.Log("[HMS] AdsManager rewardAd.Show");
                rewardAd.Show(new RewardAdListener(this));
            }
            else
            {
                Debug.Log("[HMS] Reward ad clicked but still not loaded");
            }
        }
    }

    public void ShowInterstitialAd()
    {
        Debug.Log("[HMS] AdsManager ShowInterstitialAd");
        if (interstitialAd != null)
        {
            if (interstitialAd.Loaded)
            {
                Debug.Log("[HMS] AdsManager interstitialAd.Show");
                interstitialAd.Show();
            }
            else
            {
                Debug.Log("[HMS] Interstitial ad clicked but still not loaded");
            }
        }
    }
}
