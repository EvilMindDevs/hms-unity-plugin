using HmsPlugin;

using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class IapDemoManager : MonoBehaviour
{
    //[SerializeField]
    //private Text statusText;
    private List<InAppPurchaseData> productPurchasedList;

    public static Action<string> IAPLog;

    // Please insert your products via custom editor. You can find it in Huawei > Kit Settings > IAP tab.

    #region Singleton

    public static IapDemoManager Instance { get; private set; }
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

    void Awake()
    {
        Singleton();

        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Start()
    {
        Debug.Log("[HMS]: IapDemoManager Started");
        HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;

        HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess += OnCheckIapAvailabilitySuccess;
        HMSIAPManager.Instance.OnCheckIapAvailabilityFailure += OnCheckIapAvailabilityFailure;

        HMSIAPManager.Instance.OnBuyProductFailure = OnBuyProductFailure;

        // This method should be run at every app start.
        //DeliveryControl(); 

        // Uncomment below if InitializeOnStart is not enabled in Huawei > Kit Settings > IAP tab.
        //HMSIAPManager.Instance.CheckIapAvailability();
    }

    private void DeliveryControl()
    {
        // https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/redelivering-consumables-0000001051356573
        // Check whether the purchase token is in the purchase token list of the delivered products.
    }

    // For sandbox testing
    private void OnBuyProductFailure(int code)
    {
        //https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/client-error-code-0000001050746111
        // this command is the solution of the error of product has already been purchased.(ORDER_PRODUCT_OWNED)

        if (code == OrderStatusCode.ORDER_PRODUCT_OWNED)
        {
            HMSIAPManager.Instance.OnObtainOwnedPurchasesSuccess = OnObtainOwnedPurchasesSuccess;
            HMSIAPManager.Instance.ObtainOwnedPurchases(PriceType.IN_APP_NONCONSUMABLE);
        }
    }

    private void OnObtainOwnedPurchasesSuccess(OwnedPurchasesResult result)
    {
        if (result != null)
        {
            foreach (var obj in result.InAppPurchaseDataList)
            {
                HMSIAPManager.Instance.ConsumePurchaseWithToken(obj.PurchaseToken);
            }
        }
    }



    private void OnCheckIapAvailabilityFailure(HMSException obj)
    {
        IAPLog?.Invoke("IAP is not ready.");
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        IAPLog?.Invoke("IAP is ready.");
    }

    public void SignIn()
    {
        Debug.Log("SignIn");

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
        Debug.Log("BuyProduct");

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