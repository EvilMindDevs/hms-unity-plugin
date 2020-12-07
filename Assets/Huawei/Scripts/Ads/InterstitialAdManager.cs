using HuaweiMobileServices.Ads;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class InterstitialAdManager : MonoBehaviour
    {
        private class InterstitialAdListener : IAdListener
        {
            private readonly InterstitialAdManager mAdsManager;

            public InterstitialAdListener(InterstitialAdManager adsManager)
            {
                mAdsManager = adsManager;
            }

            public void OnAdClicked()
            {
                Debug.Log("[HMS] AdsManager OnAdClicked");
                mAdsManager.OnAdClicked?.Invoke();
            }

            public void OnAdClosed()
            {
                Debug.Log("[HMS] AdsManager OnAdClosed");
                mAdsManager.OnAdClosed?.Invoke();
                mAdsManager.LoadNextInterstitialAd();
            }

            public void OnAdFailed(int reason)
            {
                Debug.Log("[HMS] AdsManager OnAdFailed");
                mAdsManager.OnAdFailed?.Invoke(reason);
            }

            public void OnAdImpression()
            {
                Debug.Log("[HMS] AdsManager OnAdImpression");
                mAdsManager.OnAdImpression?.Invoke();
            }

            public void OnAdLeave()
            {
                Debug.Log("[HMS] AdsManager OnAdLeave");
                mAdsManager.OnAdLeave?.Invoke();
            }

            public void OnAdLoaded()
            {
                Debug.Log("[HMS] AdsManager OnAdLoaded");
                mAdsManager.OnAdLoaded?.Invoke();
            }

            public void OnAdOpened()
            {
                Debug.Log("[HMS] AdsManager OnAdOpened");
                mAdsManager.OnAdOpened?.Invoke();
            }
        }

        public static InterstitialAdManager GetInstance(string name = "AdsManager") => GameObject.Find(name).GetComponent<InterstitialAdManager>();

        private InterstitialAd interstitialAd = null;

        private string mAdId;

        public string AdId
        {
            get => mAdId;
            set
            {
                Debug.Log($"[HMS] InterstitialAdManager: Set interstitial ad ID: {value}");
                mAdId = value;
                LoadNextInterstitialAd();
            }
        }

        public Action OnAdClicked { get; set; }
        public Action OnAdClosed { get; set; }
        public Action<int> OnAdFailed { get; set; }
        public Action OnAdImpression { get; set; }
        public Action OnAdLeave { get; set; }
        public Action OnAdLoaded { get; set; }
        public Action OnAdOpened { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("[HMS] InterstitalAdManager Start");
            HwAds.Init();
        }

        public void LoadNextInterstitialAd()
        {
            Debug.Log("[HMS] InterstitalAdManager LoadNextInterstitialAd");
            interstitialAd = new InterstitialAd
            {
                AdId = AdId,
                AdListener = new InterstitialAdListener(this)
            };
            interstitialAd.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowInterstitialAd()
        {
            Debug.Log("[HMS] InterstitialAdManager ShowInterstitialAd");
            if (interstitialAd?.Loaded == true)
            {
                Debug.Log("[HMS] InterstitalAdManager interstitialAd.Show");
                interstitialAd.Show();
            }
            else
            {
                Debug.Log("[HMS] Interstitial ad clicked but still not loaded");
            }
        }
    }
}