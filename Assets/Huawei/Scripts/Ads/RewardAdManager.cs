using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;

public class RewardAdManager : MonoBehaviour
{

    private class RewardAdListener : IRewardAdStatusListener
    {
        private readonly RewardAdManager mAdsManager;

        public RewardAdListener(RewardAdManager adsManager)
        {
            mAdsManager = adsManager;
        }

        public void OnRewardAdClosed()
        {
            Debug.Log("[HMS] AdsManager OnRewardAdClosed");
            mAdsManager.OnRewardAdClosed?.Invoke();
            mAdsManager.LoadNextRewardedAd();
        }

        public void OnRewardAdFailedToShow(int errorCode)
        {
            Debug.Log("[HMS] AdsManager OnRewardAdClosed " + errorCode);
            mAdsManager.OnRewardAdFailedToShow?.Invoke(errorCode);
            mAdsManager.LoadNextRewardedAd();
        }

        public void OnRewardAdOpened()
        {
            Debug.Log("[HMS] AdsManager OnRewardAdOpened");
            mAdsManager.OnRewardAdOpened?.Invoke();
        }

        public void OnRewarded(Reward reward)
        {
            Debug.Log("[HMS] AdsManager OnRewarded " + reward);
            mAdsManager.OnRewarded?.Invoke(reward);
        }
    }

    private RewardAd rewardAd = null;

    public string AdId { get; set; }
    public Action OnRewardAdClosed { get; set; }
    public Action<int> OnRewardAdFailedToShow { get; set; }
    public Action OnRewardAdOpened { get; set; }
    public Action<Reward> OnRewarded { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[HMS] AdsManager Start");
        LoadNextRewardedAd();
    }

    private void LoadNextRewardedAd()
    {
        Debug.Log("[HMS] AdsManager LoadNextRewardedAd");
        //rewardAd = new RewardAd("testx9dtjwj8hp");
        rewardAd = new RewardAd(AdId);
        rewardAd.LoadAd(new AdParam.Builder().Build(),
            () => { Debug.Log("[HMS] Ad loaded!"); },
            (errorCode) =>
            {
                Debug.Log("[HMS] Ad loading failed");
                LoadNextRewardedAd();
            }
        );
    }

    public void ShowRewardedAd()
    {
        Debug.Log("[HMS] AdsManager ShowRewardedAd");
        if (rewardAd?.Loaded == true)
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
