using System.Collections.Generic;
using UnityEngine;
using HmsPlugin;
using HuaweiMobileServices.Ads;
using System;
using HuaweiMobileServices.Utils;
using HuaweiMobileServices.Id;
using static HmsPlugin.HMSAdsKitManager;

public class AdsDemoManager : MonoBehaviour
{
    //private Toggle testAdStatusToggle;
    private AdLoadMethod RewardedAdLoadMethod = AdLoadMethod.Default;
    private AdLoadMethod InterstitialAdLoadMethod = AdLoadMethod.Default;
    private AdLoadMethod BannerAdLoadMethod = AdLoadMethod.Default;
    private AdLoadMethod SplashAdLoadMethod = AdLoadMethod.Default;
    private RewardVerifyConfig rewardVerifyConfig;

    private const string TAG = "[HMS] AdsDemoManager: ";

    #region Singleton

    public static AdsDemoManager Instance { get; private set; }
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        BeforeBuildManager();
    }

    private void Init()
    {
        HMSAdsKitManager.Instance = new Builder()
                                    .SetHasPurchasedNoAds(false)
                                    .SetRewardedAdLoadMethod(RewardedAdLoadMethod, rewardVerifyConfig)
                                    .SetBannerAdLoadMethod(BannerAdLoadMethod)
                                    .SetInterstitialAdLoadMethod(InterstitialAdLoadMethod)
                                    .SetSplashAdLoadMethod(SplashAdLoadMethod)
                                    .Build();

        HMSAdsKitManager.Instance.OnRewardedAdLoaded = OnRewardedAdLoaded;
        HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
        HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;
        HMSAdsKitManager.Instance.ConsentOnSuccess = OnConsentSuccess;
        HMSAdsKitManager.Instance.ConsentOnFail = OnConsentFail;

        HMSAdsKitManager.Instance.RequestConsentUpdate();

        //testAdStatusToggle = GameObject.FindGameObjectWithTag("Toggle").GetComponent<Toggle>();
        //testAdStatusToggle.isOn = HMSAdsKitSettings.Instance.Settings.GetBool(HMSAdsKitSettings.UseTestAds);

        #region SetNonPersonalizedAd , SetRequestLocation

        var builder = HwAds.RequestOptions.ToBuilder();

        builder
            .SetConsent("tcfString")
            .SetNonPersonalizedAd((int)NonPersonalizedAd.ALLOW_ALL)
            .Build();

        bool requestLocation = true;
        var requestOptions = builder.SetConsent("testConsent").SetRequestLocation(requestLocation).Build();

        Debug.Log($"{TAG}RequestOptions NonPersonalizedAds:  {requestOptions.NonPersonalizedAd}");
        Debug.Log($"{TAG}Consent: {requestOptions.Consent}");

        #endregion
    }

    private void OnConsentSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
    {
        Debug.Log($"{TAG}OnConsentSuccess consentStatus:{consentStatus} isNeedConsent:{isNeedConsent}");
        foreach (var AdProvider in adProviders)
        {
            Debug.Log($"{TAG}OnConsentSuccess adproviders: Id:{AdProvider.Id} Name:{AdProvider.Name} PrivacyPolicyUrl:{AdProvider.PrivacyPolicyUrl} ServiceArea:{AdProvider.ServiceArea}");
        }
    }

    private void OnConsentFail(string desc)
    {
        Debug.LogError($"{TAG}OnConsentFail:{desc}");
    }

    public void ShowBannerAd()
    {
        Debug.Log($"{TAG}ShowBannerAd");

        HMSAdsKitManager.Instance.ShowBannerAd();
    }

    public void HideBannerAd()
    {
        Debug.Log($"{TAG}HideBannerAd");

        HMSAdsKitManager.Instance.HideBannerAd();
    }

    public void SetRewardVerifyConfig(RewardVerifyConfig config)
    {
        Debug.Log($"{TAG}SetRewardVerifyConfig");
        HMSAdsKitManager.Instance.SetRewardVerifyConfig(config);
    }

    public void ShowRewardedAd()
    {
        Debug.Log($"{TAG}ShowRewardedAd");
        HMSAdsKitManager.Instance.ShowRewardedAd();
    }

    public void ShowInterstitialAd()
    {
        Debug.Log($"{TAG}ShowInterstitialAd");
        HMSAdsKitManager.Instance.ShowInterstitialAd();
    }

    public void ShowSplashImage()
    {
        Debug.Log($"{TAG}ShowSplashImage!");

        HMSAdsKitManager.Instance.LoadSplashAd("testq6zq98hecj", SplashAd.SplashAdOrientation.PORTRAIT);
    }

    public void ShowSplashVideo()
    {
        Debug.Log($"{TAG}ShowSplashVideo!");

        HMSAdsKitManager.Instance.LoadSplashAd("testd7c5cewoj6", SplashAd.SplashAdOrientation.PORTRAIT);
    }

    private void OnRewardedAdLoaded()
    {
        Debug.Log($"{TAG}OnRewardedAdLoaded");
        #region RewardVerifyConfig
        /*RewardVerifyConfig verifyConfig = new RewardVerifyConfig.Builder().SetData("CUSTOM_DATA").SetUserId("123456").Build();
        SetRewardVerifyConfig(verifyConfig);
        Debug.Log($"{TAG}OnRewardedAdLoaded:{verifyConfig.UserId} - {verifyConfig.Data}");*/
        #endregion
    }

    public void OnRewarded(Reward reward)
    {
        Debug.Log($"{TAG}rewarded!");
    }

    public void OnInterstitialAdClosed()
    {
        Debug.Log($"{TAG}interstitial ad closed");
    }

    public void SetTestAdStatus()
    {
        // HMSAdsKitManager.Instance.SetTestAdStatus(testAdStatusToggle.isOn);
        HMSAdsKitManager.Instance.SetTestAdStatus(HMSAdsKitSettings.Instance.Settings.GetBool(HMSAdsKitSettings.UseTestAds));
        HMSAdsKitManager.Instance.DestroyBannerAd();
        HMSAdsKitManager.Instance.LoadAllAds();
    }

    private void BeforeBuildManager()
    {
        if (RewardedAdLoadMethod == AdLoadMethod.WithConfig)
        {
            AssignLoadRewardAdWithConfig();
            return;
        }
        Init();
    }

    private void AssignLoadRewardAdWithConfig()
    {
        var user = HMSAccountKitManager.Instance.HuaweiId;
        Debug.Log($"{TAG} GetCurrentUser is null : {user == null}");
        if (user != null)
        {
            SetRewardAdConfig(user.OpenId);
            return;
        }

        HMSAccountKitManager.Instance.OnSignInSuccess += OnSignInAccountSilentSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed += OnSignInAccountSilentFailed;

        HMSAccountKitManager.Instance.SilentSignIn();

    }
    private void OnSignInAccountSilentSuccess(AuthAccount authHuaweiId)
    {
        Debug.Log($"{TAG} SignIn success. AuthHuaweiId: {authHuaweiId?.OpenId}");
        if (authHuaweiId != null)
        {
            SetRewardAdConfig(authHuaweiId.OpenId);
            return;
        }
        else
        {
            Debug.LogError($"{TAG} SignIn failed. AuthHuaweiId is null.");
        }
    }
    private void OnSignInAccountSilentFailed(HMSException exception)
    {
        Debug.LogError($"{TAG} SignIn failed with exception: {exception.WrappedExceptionMessage}");
        if (exception.WrappedExceptionMessage.Contains("2002"))
        {
            Debug.Log($"{TAG} SilentSignIn failed. User is not signed in. Trying SignIn.");
            HMSAccountKitManager.Instance.OnSignInSuccess -= OnSignInAccountSilentSuccess;
            HMSAccountKitManager.Instance.OnSignInFailed -= OnSignInAccountSilentFailed;

            HMSAccountKitManager.Instance.OnSignInSuccess += OnSignInAccountSuccess;
            HMSAccountKitManager.Instance.OnSignInFailed += OnSignInAccountFailed;

            HMSAccountKitManager.Instance.SignIn();
        }
    }
    private void OnSignInAccountSuccess(AuthAccount authHuaweiId)
    {
        Debug.Log($"{TAG} SignIn success. AuthHuaweiId: {authHuaweiId?.OpenId}");
        if (authHuaweiId != null)
        {
            SetRewardAdConfig(authHuaweiId.OpenId);
            return;
        }
        else
        {
            Debug.LogError($"{TAG} SignIn failed. AuthHuaweiId is null.");
        }
    }
    private void OnSignInAccountFailed(HMSException exception)
    {
        Debug.LogError($"{TAG} SignIn failed with exception: {exception.WrappedExceptionMessage}");
    }
    private void SetRewardAdConfig(string userId)
    {
        var exampleData = $"{{\"userId\":\"{userId}\", \"date\":\"{DateTime.Now}\"}}";
        rewardVerifyConfig = new RewardVerifyConfig.Builder()
            .SetUserId(userId)
            .SetData(exampleData)
            .Build();
        Init();

    }

}
