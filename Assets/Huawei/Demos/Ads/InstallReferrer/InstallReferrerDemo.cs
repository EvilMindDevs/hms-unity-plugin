using HmsPlugin;

using HuaweiMobileServices.Ads.InstallReferrer;

using UnityEngine;

public class InstallReferrerDemo : MonoBehaviour
{

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
        Debug.Log("InstallReferrerDisconnect");
    }

    private void OnInstallReferrerFail()
    {
        Debug.Log("InstallReferrerFail");
    }

    private void OnInstallReferrerSuccess(ReferrerDetails referrerDetails)
    {
        Debug.Log($"Install Referrer Text : {referrerDetails.getInstallReferrer()}");
        Debug.Log($"getInstallBeginTimestampMillisecond : {referrerDetails.getInstallBeginTimestampMillisecond()}");
        Debug.Log($"getReferrerClickTimestampMillisecond : {referrerDetails.getReferrerClickTimestampMillisecond()}");
    }



}







