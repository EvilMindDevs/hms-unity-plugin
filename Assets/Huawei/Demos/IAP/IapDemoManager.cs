# define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiConstants;
using HuaweiMobileServices.Base;

using HuaweiMobileServices.IAP;
using System;
using UnityEngine.Events;
using HuaweiMobileServices.Id;

public class IapDemoManager : MonoBehaviour
{

    public string key;

    public HuaweiProduct[] products;

    [HideInInspector]
    public int numberOfProductsRetrieved;


    List<ProductInfo> productInfoList = new List<ProductInfo>();
    List<string> productPurchasedList = new List<string>();

    private IapManager iapManager;
    private AccountManager accountManager;

    UnityEvent loadedEvent;

    void Awake()
    {
        Debug.Log("[HMSPlugin]: IAPP manager Init");
        loadedEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        accountManager = GetComponent<AccountManager>();
        accountManager.OnSignInFailed = (error) =>
        {
            Debug.Log($"[HMSPlugin]: SignIn failed. {error.Message}");
        };
        accountManager.OnSignInSuccess = SignedIn;
        accountManager.SignIn();
    }

    private void SignedIn(AuthHuaweiId authHuaweiId)
    {
        iapManager = GetComponent<IapManager>();
        iapManager.OnCheckIapAvailabilitySuccess = LoadStore;
        iapManager.OnCheckIapAvailabilityFailure = (error) =>
        {
            Debug.Log($"[HMSPlugin]: IAP check failed. {error.Message}");
        };
        iapManager.CheckIapAvailability();
    }

    private void LoadStore()
    {

        // Set Callback for ObtainInfoSuccess
        iapManager.OnObtainProductInfoSuccess = (productInfoResultList) =>
        {

            if (productInfoResultList != null)
            {
                foreach (ProductInfoResult productInfoResult in productInfoResultList)
                {
                    foreach (ProductInfo productInfo in productInfoResult.ProductInfoList)
                    {
                        productInfoList.Add(productInfo);
                    }

                }
            }
            loadedEvent.Invoke();

        };
        // Set Callback for ObtainInfoFailure
        iapManager.OnObtainProductInfoFailure = (error) =>
        {
            Debug.Log($"[HMSPlugin]: IAP ObtainProductInfo failed. {error.Message}");
        };

        // Call ObtainProductInfo 
        iapManager.ObtainProductInfo(new List<HuaweiProduct>(products));

    }



    private void RestorePurchases()
    {
        iapManager.OnObtainOwnedPurchasesSuccess = (ownedPurchaseResult) =>
        {
            productPurchasedList = (List<string>)ownedPurchaseResult.InAppPurchaseDataList;
        };

        iapManager.OnObtainOwnedPurchasesFailure = (error) =>
        {
            Debug.Log("[HMS:] RestorePurchasesError" + error.Message);
        };

        iapManager.ObtainOwnedPurchases();
    }

    public ProductInfo GetProductInfo(string productID)
    {
        return productInfoList.Find(productInfo => productInfo.ProductId == productID);
    }

    

    public void BuyProduct(string productID)
    {
        iapManager.OnBuyProductSuccess = (purchaseResultInfo) =>
        {
            // Verify signature with purchaseResultInfo.InAppDataSignature

            // If signature ok, deliver product

            // Consume product purchaseResultInfo.InAppDataSignature
            iapManager.ConsumePurchase(purchaseResultInfo);

        };

        iapManager.OnBuyProductFailure = (errorCode) =>
        {
           
            switch (errorCode)
            {
                case OrderStatusCode.ORDER_STATE_CANCEL:
                    // User cancel payment.
                    Debug.Log("[HMS]: User cancel payment");
                    break;
                case OrderStatusCode.ORDER_STATE_FAILED:
                    Debug.Log("[HMS]: order paymentf failed");
                    break;

                case OrderStatusCode.ORDER_PRODUCT_OWNED:
                    Debug.Log("[HMS]: Product owned");
                    break;
                default:
                    Debug.Log("[HMS:] BuyProduct ERROR" + errorCode);
                    break;
            }
        };

        var productInfo = productInfoList.Find(info => info.ProductId == productID);
        var payload = "test";

        iapManager.BuyProduct(productInfo, payload);

    }


    public void addListener(UnityAction action)
    {
        if (loadedEvent != null)
        {
            loadedEvent.AddListener(action);
        }

    }

}

[System.Serializable]
public class HuaweiProduct
{
    [SerializeField]
    public string productID;
    [SerializeField]
    public bool isConsumable;
}



