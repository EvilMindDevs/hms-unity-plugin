

using HmsPlugin;

using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

#if UNITY_PURCHASING

using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

#endif


public class UnityIapDemoManager : MonoBehaviour
#if UNITY_PURCHASING
    ,  IStoreCallback
#endif
{
#if UNITY_PURCHASING

    public static Action<string> IAPLog;
    public ProductCollection products => throw new NotImplementedException();
    public bool useTransactionLog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    IStore iStore;

    #region Singleton

    public static UnityIapDemoManager Instance { get; private set; }

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

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    void Start()
    {
        SignIn();

        var huaweiStore = HuaweiStore.GetInstance();
        iStore = huaweiStore;
        iStore.Initialize(this);

        List<ProductDefinition> list = new List<ProductDefinition>();

        var coins100 = new ProductDefinition("coins100", ProductType.Consumable);
        var removeAds = new ProductDefinition("removeAds", ProductType.NonConsumable);
        var premium = new ProductDefinition("premium", ProductType.Subscription);

        list.Add(coins100);
        list.Add(removeAds);
        list.Add(premium);

        ReadOnlyCollection<ProductDefinition> products = new ReadOnlyCollection<ProductDefinition>(list);
        iStore.RetrieveProducts(products);

        huaweiStore.LoadOwnedConsumables();
        huaweiStore.LoadOwnedNonConsumables();
        huaweiStore.LoadOwnedSubscribes();

        //huaweiStore.LoadComsumableProducts(list);
        //huaweiStore.LoadNonComsumableProducts();
        //huaweiStore.LoadSubscribeProducts();

    }

    public void BuyProduct(string productID, ProductType productType)
    {
        var product = new ProductDefinition(productID, productType);

        iStore.Purchase(product, $"payload {productID}");
    }


    public void OnSetupFailed(InitializationFailureReason reason)
    {
        Debug.Log($"OnSetupFailed , reason : {reason}");
    }

    public void OnProductsRetrieved(List<ProductDescription> products)
    {

        Debug.Log($"OnProductsRetrieved {  products.Count  }");

        foreach (var item in products)
        {
            Debug.Log($"transactionId: {item.transactionId} , storeSpecificId: {item.storeSpecificId}, metadata: {item.metadata} , type: {item.type}");

            var productDefinition = new ProductDefinition(item.storeSpecificId, item.storeSpecificId, item.type);

            iStore.FinishTransaction(productDefinition, item.transactionId);

        }

    }

    public void OnPurchaseSucceeded(string storeSpecificId, string receipt, string transactionIdentifier)
    {
        Debug.Log($"storeSpecificId {storeSpecificId} ,receipt {receipt} , transactionIdentifier {transactionIdentifier}");

        var productDefinition = new ProductDefinition(storeSpecificId, storeSpecificId, HuaweiStore.currentProduct.type);

        iStore.FinishTransaction(productDefinition, receipt);
    }

    public void OnAllPurchasesRetrieved(List<Product> purchasedProducts)
    {
        foreach (var item in purchasedProducts)
        {
            Debug.Log($"transactionID: {item.transactionID} , definition: {item.definition}, hasReceipt: {item.hasReceipt} , metadata: {item.metadata}");
        }
    }

    public void OnPurchaseFailed(PurchaseFailureDescription desc)
    {
        Debug.Log($"message: {desc.message} ,productId: {desc.productId} ,reason: {desc.reason} ");
    }

    public void SignIn()
    {
        Debug.Log("SignIn");

        HMSAccountKitManager.Instance.SignIn();

    }

#endif

}

