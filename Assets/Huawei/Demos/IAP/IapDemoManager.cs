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
using HmsPlugin;

public class IapDemoManager : MonoBehaviour
{

    public string[] ConsumableProducts;
    public string[] NonConsumableProducts;
    public string[] SubscriptionProducts;

    [HideInInspector]
    public int numberOfProductsRetrieved;


    List<ProductInfo> productInfoList = new List<ProductInfo>();
    List<InAppPurchaseData> productPurchasedList = new List<InAppPurchaseData>();

    UnityEvent loadedEvent;

    void Awake()
    {
        Debug.Log("[HMSPlugin]: IAPP manager Init");
        loadedEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[HMS]: Started");
        HMSAccountManager.Instance.OnSignInFailed = (error) =>
        {
            Debug.Log($"[HMSPlugin]: SignIn failed. {error.Message}");
        };
        HMSAccountManager.Instance.OnSignInSuccess = SignedIn;
        HMSAccountManager.Instance.SignIn();
    }

    private void SignedIn(AuthAccount authHuaweiId)
    {
        Debug.Log("[HMS]: SignedIn");
        HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess = LoadStore;
        HMSIAPManager.Instance.OnCheckIapAvailabilityFailure = (error) =>
        {
            Debug.Log($"[HMSPlugin]: IAP check failed. {error.Message}");
        };
        HMSIAPManager.Instance.CheckIapAvailability();
    }

    private void LoadStore()
    {
        Debug.Log("[HMS]: LoadStore");
        // Set Callback for ObtainInfoSuccess
        HMSIAPManager.Instance.OnObtainProductInfoSuccess = (productInfoResultList) =>
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
        HMSIAPManager.Instance.OnObtainProductInfoFailure = (error) =>
        {
            Debug.Log($"[HMSPlugin]: IAP ObtainProductInfo failed. {error.Message}");
        };

        // Call ObtainProductInfo 
        HMSIAPManager.Instance.ObtainProductInfo(new List<string>(ConsumableProducts), new List<string>(NonConsumableProducts), new List<string>(SubscriptionProducts));

    }



    private void RestorePurchases()
    {
        HMSIAPManager.Instance.RestorePurchases((restoredProducts) =>
        {
            productPurchasedList = new List<InAppPurchaseData>(restoredProducts.InAppPurchaseDataList);
        });
    }

    public ProductInfo GetProductInfo(string productID)
    {
        return productInfoList.Find(productInfo => productInfo.ProductId == productID);
    }



    public void BuyProduct(string productID)
    {
        HMSIAPManager.Instance.BuyProduct(productID);
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



