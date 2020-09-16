using HuaweiConstants;
using HuaweiMobileServices.Ads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.Video;
using static HuaweiConstants.UnityBannerAdPositionCode;

namespace HmsPlugin
{
    public class BannerAdsManager : MonoBehaviour
    {
        private GameObject mLoadButton;
        private GameObject mHideButton;
        public static BannerAdsManager GetInstance(string name = "AdsManager") => GameObject.Find(name).GetComponent<BannerAdsManager>();
        private BannerAd bannerAdView = null;

        private string mAdId;
        public string AdId
        {
            get => mAdId;
            set
            {
                Debug.Log($"[HMS] BannerAdManager: Set banner ads ID: {value}");
                mAdId = value; 
            }
        }
        private void LoadBannerAds()
        {
            AdStatusListener mAdStatusListener = new AdStatusListener();
            
            Debug.Log("[HMS] BannerAdManager Start : "  + mAdId);
            String bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50;
            bannerAdView = new BannerAd(mAdStatusListener);
             
             bannerAdView.AdId = mAdId;
             bannerAdView.positionType = (int) UnityBannerAdPositionCodeType.POSITION_TOP;
 
            bannerAdView.sizeType = bannerSize;
 
            bannerAdView.mAdStatusListener = mAdStatusListener;
 

        }
        // Start is called before the first frame update
        void Start()
        {  
            Debug.Log("[HMS] BannerAdManager Start");
            HwAds.Init();
            
            mLoadButton = GameObject.Find("BannerAdButton");
            mLoadButton.GetComponent<Button>().onClick.AddListener(ShowBannerAd);
            mHideButton = GameObject.Find("HideAdButton");
            mHideButton.GetComponent<Button>().onClick.AddListener(HideBannerAd);
        } 
        public void ShowBannerAd()
        {
            LoadBannerAds();
            bannerAdView.showBanner(new AdParam.Builder().Build());
        }
        public void HideBannerAd()
        {
            bannerAdView.hideBanner();
        }
    }

}