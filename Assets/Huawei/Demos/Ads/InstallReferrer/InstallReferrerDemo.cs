using HmsPlugin;

using HuaweiMobileServices.Ads.InstallReferrer;

using UnityEngine;

public class InstallReferrerDemo : MonoBehaviour
{
    private readonly string TAG = "[HMS] InstallReferrerDemo ";

    private void OnEnable()
    {
        HMSAdsKitManager.Instance.InstallReferrerSuccess += OnInstallReferrerSuccess;
        HMSAdsKitManager.Instance.InstallReferrerFail += OnInstallReferrerFail;
        HMSAdsKitManager.Instance.InstallReferrerDisconnect += OnInstallReferrerDisconnect;
    }

    private void OnDisable()
    {
        HMSAdsKitManager.Instance.InstallReferrerSuccess -= OnInstallReferrerSuccess;
        HMSAdsKitManager.Instance.InstallReferrerFail -= OnInstallReferrerFail;
        HMSAdsKitManager.Instance.InstallReferrerDisconnect -= OnInstallReferrerDisconnect;
    }

    void Start()
    {
        bool isTest = false;
        HMSAdsKitManager.Instance.Init_InstallReferrer(isTest);
    }

    private void OnInstallReferrerDisconnect()
    {
        Debug.Log(TAG+"InstallReferrerDisconnect");
    }

    private void OnInstallReferrerFail(InstallReferrerResponse response)
    {
        Debug.LogError(TAG+ "InstallReferrerFail response:"+ response);
    }

    private void OnInstallReferrerSuccess(ReferrerDetails referrerDetails)
    {
        Debug.Log($"{TAG}Install Referrer Text : {referrerDetails.GetInstallReferrer()}");
        Debug.Log($"{TAG}getInstallBeginTimestampMillisecond : {referrerDetails.GetInstallBeginTimestampMillisecond()}");
        Debug.Log($"{TAG}getReferrerClickTimestampMillisecond : {referrerDetails.GetReferrerClickTimestampMillisecond()}");
    }

}







