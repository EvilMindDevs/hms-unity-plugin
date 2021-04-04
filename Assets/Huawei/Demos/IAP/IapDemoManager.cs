using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiConstants;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.IAP;
using System;
using UnityEngine.Events;
using HuaweiMobileServices.Id;
using HmsPlugin;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;

public class IapDemoManager : MonoBehaviour
{
    [SerializeField]
    private Text statusText;
    private List<InAppPurchaseData> productPurchasedList;

    // Please insert your products via custom editor. You can find it in Huawei > Kit Settings > IAP tab.

    void Start()
    {
        Debug.Log("[HMS]: IapDemoManager Started");
        HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;
        HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess += OnCheckIapAvailabilitySuccess;
        HMSIAPManager.Instance.OnCheckIapAvailabilityFailure += OnCheckIapAvailabilityFailure;

        // Uncomment below if InitializeOnStart is not enabled in Huawei > Kit Settings > IAP tab.
        //HMSIAPManager.Instance.CheckIapAvailability();
    }

    private void OnCheckIapAvailabilityFailure(HMSException obj)
    {
        statusText.text = "IAP is not ready.";
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        statusText.text = "IAP is ready.";
    }

    public void SignIn()
    {
        HMSIAPManager.Instance.CheckIapAvailability();
    }

    private void RestorePurchases()
    {
        HMSIAPManager.Instance.RestorePurchases((restoredProducts) =>
        {
            productPurchasedList = new List<InAppPurchaseData>(restoredProducts.InAppPurchaseDataList);
        });
    }

    public void BuyProduct(string productID)
    {
        HMSIAPManager.Instance.BuyProduct(productID);
    }

    private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        if (obj.InAppPurchaseData.ProductId == "removeads")
        {
            // Hide banner Ad for example
            //HMSAdsKitManager.Instance.HideBannerAd();
        }
        else if (obj.InAppPurchaseData.ProductId == "coins100")
        {
            // Give your player coins here.
        }
        else if (obj.InAppPurchaseData.ProductId == "premium")
        {
            // Grant your player premium feature.
        }
    }
}