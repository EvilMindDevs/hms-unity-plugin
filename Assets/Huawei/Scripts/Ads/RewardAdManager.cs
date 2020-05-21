using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.Ads;

namespace HmsPlugin
{
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
                Debug.Log("[HMS] AdsManager OnRewardAdFailedToShow " + errorCode);
                mAdsManager.OnRewardAdFailedToShow?.Invoke(errorCode);
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

    public static RewardAdManager GetInstance(string name = "AdsManager") => GameObject.Find(name).GetComponent<RewardAdManager>();

    private RewardAd rewardAd = null;

        private string mAdId;

        public string AdId
        {
            get => mAdId;
            set
            {
                Debug.Log($"[HMS] RewardAdManager: Set reward ad ID: {value}");
                mAdId = value;
                LoadNextRewardedAd();
            }
        }

        public Action OnRewardAdClosed { get; set; }
        public Action<int> OnRewardAdFailedToShow { get; set; }
        public Action OnRewardAdOpened { get; set; }
        public Action<Reward> OnRewarded { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("[HMS] RewardAdManager Start");
            HwAds.Init();
        }

        private void LoadNextRewardedAd()
        {
            Debug.Log("[HMS] AdsManager LoadNextRewardedAd");
            rewardAd = new RewardAd(AdId);
            rewardAd.LoadAd(
                new AdParam.Builder().Build(),
                () => Debug.Log("[HMS] Rewarded ad loaded!"),
                (errorCode) => Debug.Log($"[HMS] Rewarded ad loading failed with error ${errorCode}")
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
}
