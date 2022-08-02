using HuaweiMobileServices.AppLinking;

using System;

using UnityEngine;

using static HuaweiMobileServices.AppLinking.AGConnectAppLinking;

public class AppLinkingDemo : MonoBehaviour
{
    private string TAG = "[HMS] AppLinking Demo ";
    private static string shortLink;
    private static string longLink;

    [Header("Builder")]
    private readonly string deepLink = "https://developer.huawei.com/consumer/cn";
    private readonly string uriPrefix = "https://hmsunityplugin.dre.agconnect.link";

    [Header("SocialCardInfo")]
    private readonly string title = "Title";
    private readonly string description = "Description";
    private readonly string imageUrl = "https://appimg.dbankcdn.com/application/icon144/65/5ed8340ee28c4735915c1aa08e209fe5.png";

    [Header("CampaignInfo")]
    private string name = "Name";
    private string AGC = "AGC";
    private string app = "App";

    public static Action<string> shortLinkText;
    public static Action<string> longLinkText;
    public static Action<string> deepLinkText;

    #region Singleton

    public static AppLinkingDemo Instance { get; private set; }
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

    void Start()
    {
        GetInstance();
    }

    public void CreateAppLinking()
    {
        Debug.Log(TAG+"CreateAppLinking");

        AppLinking.Builder builder = new AppLinking.Builder();

        builder.SetUriPrefix(uriPrefix).SetDeepLink(deepLink)
       .SetAndroidLinkInfo(AndroidLinkInfo.NewBuilder()
       .SetAndroidDeepLink(deepLink).SetOpenType(AndroidLinkInfo.AndroidOpenType.AppGallery)
       .Build())
       .SetSocialCardInfo(SocialCardInfo.NewBuilder()
       .SetTitle(title)
       .SetImageUrl(imageUrl)
       .SetDescription(description)
       .Build())
       .SetCampaignInfo(CampaignInfo.NewBuilder()
       .SetName(name)
       .SetSource(AGC)
       .SetMedium(app)
       .Build())
       .SetPreviewType(AppLinking.LinkingPreviewType.AppInfo);

        BuildShortAppLink(builder);

        BuildLongAppLink(builder);
    }

    public void BuildLongAppLink(AppLinking.Builder builder)
    {
        longLink = builder.BuildAppLinking().GetUri();
        longLinkText?.Invoke(longLink);
        Debug.Log($"[{TAG}]:Long Link = {longLink}");
    }

    public void BuildShortAppLink(AppLinking.Builder builder)
    {
        var task = builder.BuildShortAppLinking();
        task.AddOnSuccessListener(it =>
        {
            shortLink = it.GetShortUrl();
            Debug.Log($"[{TAG}]:Short Link = {shortLink}");

            shortLinkText?.Invoke(shortLink);

        }).AddOnFailureListener(exception =>
        {
            Debug.LogError($"[{TAG}]: Failure on BuildShortAppLinking error { exception.WrappedExceptionMessage} cause : {exception.WrappedCauseMessage}");
        });
    }

    public void ShareShortLink()
    {
        Debug.Log(TAG + "ShareShortLink");

        AGConnectAppLinking.ShareLink(shortLink);
    }

    public void ShareLongLink()
    {
        Debug.Log(TAG + "ShareLongLink");

        AGConnectAppLinking.ShareLink(longLink);
    }

    public void GetLink()
    {
        Debug.Log(TAG + "GetLink");

        AGConnectAppLinking.GetInstance().GetAppLinking().AddOnSuccessListener(verifyCodeResult =>
                {
                    deepLinkText?.Invoke(verifyCodeResult.GetDeepLink());
                })
            .AddOnFailureListener(exception =>
          {
              Debug.LogError($"[{TAG}]: Failure on GetAppLinking error " + exception.WrappedExceptionMessage + " cause : " + exception.WrappedCauseMessage);
          });
    }

    public void GetInstance()
    {
        Debug.Log($"[{TAG}]:Get instance called for app linking");
        if (agc == null) agc = AGConnectAppLinking.GetInstance();
        Debug.Log($"[{TAG}]: GetInstance() {agc}");
    }
}