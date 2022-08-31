using HmsPlugin;

using UnityEngine;
using UnityEngine.UI;

public class AdsUIView : MonoBehaviour
{

    private Button Btn_ShowInterstitialAd;
    private Button Btn_ShowRewardedAd;
    private Button Btn_ShowBannerAd;
    private Button Btn_HideBannerAd;
    private Button Btn_ShowSplashImage;
    private Button Btn_ShowSplashVideo;

    #region Monobehaviour

    private void Awake()
    {
        Btn_ShowInterstitialAd = GameObject.Find("Button - InterstitialAd").GetComponent<Button>();
        Btn_ShowRewardedAd = GameObject.Find("Button  - RewardedAd").GetComponent<Button>();
        Btn_ShowBannerAd = GameObject.Find("Button - ShowBannerAd").GetComponent<Button>();

        Btn_HideBannerAd = GameObject.Find("Button - HideBannerAd").GetComponent<Button>();
        Btn_ShowSplashImage = GameObject.Find("Button - ShowSplashImage").GetComponent<Button>();
        Btn_ShowSplashVideo = GameObject.Find("Button - ShowSplashVideo").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_ShowInterstitialAd.onClick.AddListener(ButtonClick_ShowInterstitialAd);
        Btn_ShowRewardedAd.onClick.AddListener(ButtonClick_ShowRewardedAd);
        Btn_ShowBannerAd.onClick.AddListener(ButtonClick_ShowBannerAd);

        Btn_HideBannerAd.onClick.AddListener(ButtonClick_HideBannerAd);
        Btn_ShowSplashImage.onClick.AddListener(ButtonClick_ShowSplashImage);
        Btn_ShowSplashVideo.onClick.AddListener(ButtonClick_ShowSplashVideo);
    }

    private void OnDisable()
    {
        Btn_ShowInterstitialAd.onClick.RemoveListener(ButtonClick_ShowInterstitialAd);
        Btn_ShowRewardedAd.onClick.RemoveListener(ButtonClick_ShowRewardedAd);
        Btn_ShowBannerAd.onClick.RemoveListener(ButtonClick_ShowBannerAd);

        Btn_HideBannerAd.onClick.RemoveListener(ButtonClick_HideBannerAd);
        Btn_ShowSplashImage.onClick.RemoveListener(ButtonClick_ShowSplashImage);
        Btn_ShowSplashVideo.onClick.RemoveListener(ButtonClick_ShowSplashVideo);
    }

    #endregion
   
    #region Button Events

    private void ButtonClick_ShowInterstitialAd()
    {
        AdsDemoManager.Instance.ShowInterstitialAd();
    }

    private void ButtonClick_ShowRewardedAd()
    {
        AdsDemoManager.Instance.ShowRewardedAd();
    }

    private void ButtonClick_ShowBannerAd()
    {
        AdsDemoManager.Instance.ShowBannerAd();
    }

    private void ButtonClick_HideBannerAd()
    {
        AdsDemoManager.Instance.HideBannerAd();
    }

    private void ButtonClick_ShowSplashImage()
    {
        AdsDemoManager.Instance.ShowSplashImage();
    }

    private void ButtonClick_ShowSplashVideo()
    {
        AdsDemoManager.Instance.ShowSplashVideo();
    }

    #endregion

}


