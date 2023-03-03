using HuaweiMobileServices.Ads;
using HuaweiMobileServices.Ads.NativeAd;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LargeImageNative : MonoBehaviour
{
    [Header("Native Ad Unit ID")]
    public string adUnitID = "testy63txaom86";

    [Header("Components")]
    public Text ad_title;
    public Text button_title;
    public Text ad_source;
    public Button ad_call_to_action;
    public Button why_this_ad;
    public RawImage ad_media;

    NativeAd nativeAd;
    NativeView nativeView;

    //!!!! Must: The "Why this ad" icon (i) or "Dislike this ad" icon (x), and the ad flag must be displayed in the corner of each ad view.
    void Start()
    {
        Debug.Log("[HMS]LargeImageNative Start");
        LoadNativeAd();
    }

    //Call this method with click_catcher
    //You can create multiple click catchers and call this method with them. In this way, you can determine exactly where your user clicks on the ad.
    public void OnAdClicked(int value = 0) 
    {
        if (nativeAd != null) 
        {
            //if(value == 0) // You can create a customClick logic in here
            
            PerformClick();
        }
            
    }

    public void OnClosedButtonClicked() 
    {
        if (nativeAd != null)
            nativeAd.Destroy();

        this.gameObject.SetActive(false);
    }

    //Call this method with Why This Ad button
    private void GotoWhyThisAdPage() 
    {
        if(nativeAd != null)
            nativeAd.GotoWhyThisAdPage();
    }

    private void OnNativeAdLoaded(NativeAd nativeAd)
    {
        Debug.Log("[HMS] OnNativeAdLoaded");
        this.nativeAd = nativeAd;
        nativeView = new NativeView();
        nativeView.SetNativeAd(nativeAd);
        ad_title.text = nativeAd.Title;

        if (nativeAd.AdSource != null)
            ad_source.text = nativeAd.AdSource;
        else
            ad_source.gameObject.SetActive(false);
        
        button_title.text = (nativeAd.CallToAction!=null)? nativeAd.CallToAction : "Open";
        ad_call_to_action.onClick.AddListener(delegate { PerformClick(); });
        why_this_ad.onClick.AddListener(delegate { GotoWhyThisAdPage(); });

        foreach (var image in nativeAd.Images)
            StartCoroutine(DownloadImage(image.Uri.ToString()));
        Debug.Log("[HMS] OnNativeAdLoaded completed. Init success.");
    }

    private void PerformClick() 
    {
        if (nativeView != null)
            nativeView.PerformClick();
        else
            Debug.LogError("[HMS] Cannot Perform Click. nativeView is null.");
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            ad_media.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    public void LoadNativeAd()
    {
        NativeAdLoader.Builder builder = new NativeAdLoader.Builder(adUnitID);


        builder.SetNativeAdLoadedListener(new NativeAdLoadedListener(new LargeImageNativeAdLoadedListener(OnNativeAdLoaded)))
            .SetAdListener(new AdStatusListener());
        NativeAdLoader nativeAdLoader = builder.Build();
       
        nativeAdLoader.LoadAd(new AdParam.Builder().Build());
    }

    private class LargeImageNativeAdLoadedListener : INativeAdLoadedListener
    {
        Action<NativeAd> OnNativeAdLoaded;
        public LargeImageNativeAdLoadedListener(Action<NativeAd> OnNativeAdLoaded) 
        {
            this.OnNativeAdLoaded = OnNativeAdLoaded;
        }
        public void onNativeAdLoaded(NativeAd nativeAd)
        {
            Debug.Log("[HMS] LargeImageNativeAdLoadedListener onNativeAdLoaded");
            OnNativeAdLoaded.Invoke(nativeAd);
        }
    }

    private class AdStatusListener : IAdListener
    {
        public AdStatusListener(){}

        public void OnAdClicked()
        {
            Debug.Log("[HMS] OnNativeAdClicked");
        }

        public void OnAdClosed()
        {
            Debug.Log("[HMS] OnNativeAdClosed");
        }

        public void OnAdFailed(int reason)
        {
            Debug.LogError("[HMS] OnNativeAdFailed reason:" + reason);
        }

        public void OnAdImpression()
        {
            Debug.Log("[HMS] OnNativeAdImpression");
        }

        public void OnAdLeave()
        {
            Debug.Log("[HMS] OnNativeAdLeave");
        }

        public void OnAdLoaded()
        {
            Debug.Log("[HMS] OnNativeAdAdAdAdAdLoaded");
        }

        public void OnAdOpened()
        {
            Debug.Log("[HMS] OnNativeAdOpened");
        }
    }
}

