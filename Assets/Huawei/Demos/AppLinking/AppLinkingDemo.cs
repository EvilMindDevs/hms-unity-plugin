using HuaweiMobileServices.AppLinking;
using HuaweiMobileServices.InAppComment;
using UnityEngine;
using UnityEngine.UI;
using static HuaweiMobileServices.AppLinking.AGConnectAppLinking;

public class AppLinkingDemo : MonoBehaviour
{
    private string TAG = "AppLinking Manager";
    private static string shortLink;
    private static string longLink;

    [Header("Builder")]
    [SerializeField] private string DeepLink = "https://developer.huawei.com/consumer/cn";
    [SerializeField] private string UriPrefix = "https://hmsunityplugin.dre.agconnect.link";

    [Header("SocialCardInfo")]
    [SerializeField] private string Title = "Title";
    [SerializeField] private string Description = "Description";
    [SerializeField] private string ImageUrl = "https://appimg.dbankcdn.com/application/icon144/65/5ed8340ee28c4735915c1aa08e209fe5.png";

    [Header("CampaignInfo")]
    [SerializeField] private string Name = "Name";
    [SerializeField] private string AGC = "AGC";
    [SerializeField] private string App = "App";

    [SerializeField] private Text ShortLinkText;
    [SerializeField] private Text LongLinkText;

    void Start()
    {
        GetInstance();
    }

    public void CreateAppLinking()
    {
        InAppComment.ShowInAppComment();
        AppLinking.Builder builder = new AppLinking.Builder();

        builder.SetUriPrefix(UriPrefix).SetDeepLink(DeepLink)
       .SetAndroidLinkInfo(AndroidLinkInfo.NewBuilder()
       .SetAndroidDeepLink(DeepLink).SetOpenType(AndroidLinkInfo.AndroidOpenType.AppGallery)
       .Build())
       .SetSocialCardInfo(SocialCardInfo.NewBuilder()
       .SetTitle(Title)
       .SetImageUrl(ImageUrl)
       .SetDescription(Description)
       .Build())
       .SetCampaignInfo(CampaignInfo.NewBuilder()
       .SetName(Name)
       .SetSource(AGC)
       .SetMedium(App)
       .Build())
       .SetPreviewType(AppLinking.LinkingPreviewType.AppInfo);

        BuildShortAppLink(builder);

        BuildLongAppLink(builder);
    }

    public void BuildLongAppLink(AppLinking.Builder builder)
    {
        longLink = builder.BuildAppLinking().GetUri();
        LongLinkText.text = longLink;
        Debug.Log("Long Link = " + longLink);
    }

    public void BuildShortAppLink(AppLinking.Builder builder)
    {
        var task = builder.BuildShortAppLinking();
        task.AddOnSuccessListener(it =>
        {
            shortLink = it.GetShortUrl();
            Debug.Log("Short Link = " + shortLink);

            ShortLinkText.text = shortLink;

        }).AddOnFailureListener(exception =>
        {
            Debug.LogError(TAG + " Failure on BuildShortAppLinking error " + exception.WrappedExceptionMessage + " cause : " + exception.WrappedCauseMessage);
        });
    }

    public void ShareShortLink()
    {
        AGConnectAppLinking.ShareLink(shortLink);
    }

    public void ShareLongLink()
    {
        AGConnectAppLinking.ShareLink(longLink);
    }

    private void GetLink()
    {
        AGConnectAppLinking.GetInstance().GetAppLinking().AddOnSuccessListener(verifyCodeResult =>
        {
            Debug.Log("GetAppLinking Success" + verifyCodeResult);
        })
          .AddOnFailureListener(exception =>
          {
              Debug.LogError(TAG + " Failure on GetAppLinking error " + exception.WrappedExceptionMessage + " cause : " + exception.WrappedCauseMessage);
          });
    }

    public void GetInstance()
    {
        Debug.Log("Get instance called for app linking");
        if (agc == null) agc = AGConnectAppLinking.GetInstance();
        Debug.Log($"[{TAG}]: GetInstance() {agc}");
    }
}