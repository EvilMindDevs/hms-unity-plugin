using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using HmsPlugin;
#endif
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HMSSplashAdPreview : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private CanvasScaler canvasScaler;
    [SerializeField]
    private Image splashImage;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text subText;

    void Start()
    {
        RefreshContent();
    }

    public void RefreshContent()
    {
        if (!string.IsNullOrEmpty(HMSAdsKitSettings.Instance.Settings.Get(HMSAdsKitSettings.SplashImagePath)))
            splashImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(HMSAdsKitSettings.Instance.Settings.Get(HMSAdsKitSettings.SplashImagePath));
        else
            splashImage.sprite = null;

        titleText.text = string.IsNullOrEmpty(HMSAdsKitSettings.Instance.Settings.Get(HMSAdsKitSettings.SplashTitle)) ? "Splash Title" : HMSAdsKitSettings.Instance.Settings.Get(HMSAdsKitSettings.SplashTitle);
        subText.text = string.IsNullOrEmpty(HMSAdsKitSettings.Instance.Settings.Get(HMSAdsKitSettings.SplashSubText)) ? "Splash SubText" : HMSAdsKitSettings.Instance.Settings.Get(HMSAdsKitSettings.SplashSubText);
    }
#endif
}