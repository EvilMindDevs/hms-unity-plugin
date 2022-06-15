using System.Collections.Generic;
using UnityEngine;
using HmsPlugin;
using UnityEngine.UI;
using HuaweiMobileServices.Ads;
using static HuaweiConstants.UnityBannerAdPositionCode;
using HuaweiConstants;

public class AdsDemoManager : MonoBehaviour
{
    [SerializeField]
    private Toggle testAdStatusToggle;
    int counter =  0;

    private void Start()
    {
        HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
        HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;
        testAdStatusToggle.isOn = HMSAdsKitSettings.Instance.Settings.GetBool(HMSAdsKitSettings.UseTestAds);

        HMSAdsKitManager.Instance.ConsentOnFail = OnConsentFail;
        HMSAdsKitManager.Instance.ConsentOnSuccess = OnConsentSuccess;
        HMSAdsKitManager.Instance.RequestConsentUpdate();
    }

    private void OnConsentSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
    {
        Debug.Log($"[HMS] AdsDemoManager OnConsentSuccess consentStatus:{consentStatus} isNeedConsent:{isNeedConsent}");
        foreach (var AdProvider in adProviders)
        {
            Debug.Log($"[HMS] AdsDemoManager OnConsentSuccess adproviders: Id:{AdProvider.Id} Name:{AdProvider.Name} PrivacyPolicyUrl:{AdProvider.PrivacyPolicyUrl} ServiceArea:{AdProvider.ServiceArea}");
        }
    }

    private void OnConsentFail(string desc)
    {
        Debug.Log($"[HMS] AdsDemoManager OnConsentFail:{desc}");
    }

    public void ShowBannerAd()
    {
        var bannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM;
        

        switch (counter) 
        {
            case 0:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM;
                    break;
            case 1:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM_LEFT;
                break;
            case 2:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM_RIGHT;
                break;
            case 3:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_CENTER;
                break;
            case 4:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_CUSTOM;
                break;
            case 5:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_TOP;
                break;
            case 6:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_TOP_LEFT;
                break;
            case 7:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_TOP_RIGHT;
                break;
            case 8:
                bannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM;
                counter = 0;
                break;
        }
        Debug.Log("<!!> COUNTER:" + counter++);

        if(HMSAdsKitManager.Instance.IsBannerAdLoaded) 
            HMSAdsKitManager.Instance.DestroyBannerAd(); 

        HMSAdsKitManager.Instance.LoadBannerAd(bannerPosition, UnityBannerAdSize.BANNER_SIZE_320_50);
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

    public void ShowSplashImage()
    {
        HMSAdsKitManager.Instance.LoadSplashAd("testq6zq98hecj", SplashAd.SplashAdOrientation.PORTRAIT);
    }

    public void ShowSplashVideo()
    {
        HMSAdsKitManager.Instance.LoadSplashAd("testd7c5cewoj6", SplashAd.SplashAdOrientation.PORTRAIT);
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
