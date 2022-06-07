using HuaweiMobileServices.AppLinking;
using UnityEngine;
using UnityEngine.UI;
using static HuaweiMobileServices.AppLinking.AGConnectAppLinking;

public class AppLinkingDemo : MonoBehaviour
{
    private string TAG = "HMS AppLinking Demo";
    private static string shortLink;
    private static string longLink;

    [Header("Builder")]
    [SerializeField] private string deepLink = "https://developer.huawei.com/consumer/cn";
    [SerializeField] private string uriPrefix = "https://hmsunityplugin.dre.agconnect.link";

    [Header("SocialCardInfo")]
    [SerializeField] private string title = "Title";
    [SerializeField] private string description = "Description";
    [SerializeField] private string imageUrl = "https://appimg.dbankcdn.com/application/icon144/65/5ed8340ee28c4735915c1aa08e209fe5.png";

    [Header("CampaignInfo")]
    [SerializeField] private string name = "Name";
    [SerializeField] private string AGC = "AGC";
    [SerializeField] private string app = "App";

    [SerializeField] private Text shortLinkText;
    [SerializeField] private Text longLinkText;
    [SerializeField] private Text deepLinkText;

    void Start()
    {
        GetInstance();
    }

    public void CreateAppLinking()
    {
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
        longLinkText.text = longLink;
        Debug.Log($"[{TAG}]:Long Link = {longLink}");
    }

    public void BuildShortAppLink(AppLinking.Builder builder)
    {
        var task = builder.BuildShortAppLinking();
        task.AddOnSuccessListener(it =>
        {
            shortLink = it.GetShortUrl();
            Debug.Log($"[{TAG}]:Short Link = {shortLink}");

            shortLinkText.text = shortLink;

        }).AddOnFailureListener(exception =>
        {
            Debug.LogError($"[{TAG}]: Failure on BuildShortAppLinking error { exception.WrappedExceptionMessage} cause : {exception.WrappedCauseMessage}");
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

    public void GetLink()
    {
        AGConnectAppLinking.GetInstance().GetAppLinking().AddOnSuccessListener(verifyCodeResult =>
                {
                    deepLinkText.text = verifyCodeResult.GetDeepLink();
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