using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;

public class AdsManager : MonoBehaviour
{

    private RewardAd rewardAd = null;

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
            mAdsManager.LoadNextAd();
        }

        public void OnRewardAdFailedToShow(int errorCode)
        {
            Debug.Log("[HMS] AdsManager OnRewardAdClosed " + errorCode);
            mAdsManager.LoadNextAd();
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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[HMS] AdsManager Start");
        LoadNextAd();
    }

    public void ShowAd()
    {
        Debug.Log("[HMS] AdsManager ShowAd");
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

    private void LoadNextAd()
    {
        rewardAd = new RewardAd("testx9dtjwj8hp");
        rewardAd.LoadAd(new AdParam.Builder().Build(),
            () => { Debug.Log("[HMS] Ad loaded!"); },
            (errorCode) =>
            {
                Debug.Log("[HMS] Ad loading failed");
                LoadNextAd();
            }
        );
    }
}
