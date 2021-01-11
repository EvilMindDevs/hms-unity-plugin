using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{

    private const string playStoreID = "3922667";
    private const string appStoreID = "3922666";

    private const string interstitialAd = "video";
    private const string rewardedVideoAd = "rewardedVideo";

    public bool isTargetPlayStore;
    public bool isTestAd;

    // Start is called before the first frame update
    void Start()
    {
        InitializeAdvertisement();
        PlayInterstitialAd();
    }

    private void InitializeAdvertisement()
    {
        if (isTargetPlayStore)
        {
            Advertisement.Initialize(playStoreID, isTestAd);
            return;
        }
        Advertisement.Initialize(appStoreID, isTestAd);
        return;
    }

    public void PlayInterstitialAd()
    {
        if (!Advertisement.IsReady(interstitialAd))
        {
            return;
        }

        Advertisement.Show(interstitialAd);
    }

}
