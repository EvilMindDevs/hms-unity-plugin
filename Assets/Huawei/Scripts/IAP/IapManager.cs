# define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiConstants;
using HuaweiMobileServices.Base;

using HuaweiMobileServices.IAP;
using System;
using UnityEngine.Events;

public class IapManager : MonoBehaviour
{

    public string key;


    public Product[] products;

    [HideInInspector]
    public int numberOfProductsRetrieved;

    List<string> productIdConsumablesList = new List<string>();
    List<string> productIdNonConsumablesList = new List<string>();
    List<ProductInfo> productInfoList = new List<ProductInfo>();

    IIapClient iapClient;

    UnityEvent loadedEvent;


    void Awake()
    {
#if DEBUG
        Debug.Log("[HMSPlugin]: IAPP manager Init");
#endif
        Debug.Log("[HMSPlugin]: IAP manager Init");
        loadedEvent = new UnityEvent();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        InitIAP();
        // RestorePurchases();
        recoverPurchases();

        


    }

    



    private void InitIAP()
    {
        foreach (Product product in products)
        {
            if (product.isConsumable)
            {
                productIdConsumablesList.Add(product.productID);
            }

        }

        ProductInfoReq productInfoReq = new ProductInfoReq();
       
        iapClient = Iap.GetIapClient();
     

        // CONSUMABLE REQUEST
        productInfoReq.PriceType = 0;
    
        productInfoReq.ProductIds = productIdConsumablesList;
        
        ITask<ProductInfoResult> task = iapClient.ObtainProductInfo(productInfoReq);
        
        Debug.Log(task.Result);

        task.AddOnSuccessListener((result) =>
        {

            if (result != null)
            {
                
                if (result.ProductInfoList.Count != 0)
                {
                    
                    Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
                    Debug.Log("[HMSPlugin]: Found " + result.ProductInfoList.Count + "consumable products");
                    foreach (ProductInfo productInfo in result.ProductInfoList)
                    {
                        Debug.Log("[HMSPlugin]: " + productInfo.ProductName + "," + productInfo.Price);
                        productInfoList.Add(productInfo);
                    }
                }


            }
            loadedEvent.Invoke();

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMSPlugin]: ERROR Houston!!" + exception.Message);
        });

        

        RestorePurchases();
    }


    private void RestorePurchases()
    {
        foreach (Product product in products)
        {
            if (!product.isConsumable)
            {
                productIdConsumablesList.Add(product.productID);
            }

        }

        ProductInfoReq productInfoReq = new ProductInfoReq();
        

        // NON CONSUMABLE REQUEST
        productInfoReq.PriceType = 1;
        productInfoReq.ProductIds = productIdConsumablesList;

        ITask<ProductInfoResult> task = iapClient.ObtainProductInfo(productInfoReq);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("HMSPlugin: NON consumables result success");
            if (result != null && result.ProductInfoList.Count != 0)
            {
                Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
                Debug.Log("[HMSPlugin]: Found " + result.ProductInfoList.Count + "non consumable products");
                foreach (ProductInfo productInfo in result.ProductInfoList)
                {
                    Debug.Log("[HMSPlugin]: " + productInfo.ProductName + "," + productInfo.Price);
                    productInfoList.Add(productInfo);
                }

            }

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMSPlugin]: ERROR Houston!!" + exception.Message);
        });
    }

    public ProductInfo GetProductInfo(string productID)
    {
        return productInfoList.Find( productInfo => productInfo.ProductId == productID);
    }

    private void recoverPurchases()
    {
        OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq();

        ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("HMSP: recoverPurchases");
            foreach(string inAppPurchaseData in result.InAppPurchaseDataList) {

                InAppPurchaseData inAppPurchaseDataBean = new InAppPurchaseData(inAppPurchaseData);
                string purchaseToken = inAppPurchaseDataBean.PurchaseToken;

                ConsumePurchase(purchaseToken);

                Debug.Log("HMSP: recoverPurchases result> " + result.ReturnCode);
            }
            
            
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("HMSP: Error on recoverPurchases"+ exception.StackTrace);
        });
    }


    private void ConsumePurchase(string token) {

        ConsumeOwnedPurchaseReq consumeOwnedPurchaseReq = new ConsumeOwnedPurchaseReq();
        consumeOwnedPurchaseReq.PurchaseToken = token;

        ITask<ConsumeOwnedPurchaseResult> task = iapClient.ConsumeOwnedPurchase(consumeOwnedPurchaseReq);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("HMSP: consumePurchase");

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("HMSP: Error on consumePurchase");
        });
    }

    public void BuyProduct(string productID)
    {

        PurchaseIntentReq purchaseIntentReq = new PurchaseIntentReq();
        purchaseIntentReq.PriceType = 0;
        purchaseIntentReq.ProductId = productID;
        // ToDo : developer payload???
        purchaseIntentReq.DeveloperPayload = "test";

        ITask<PurchaseIntentResult> task = iapClient.CreatePurchaseIntent(purchaseIntentReq);

        task.AddOnSuccessListener((result) =>
        {

            if (result != null)
            {
                Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
                Debug.Log("[HMSPlugin]: Bought " + productID);
                Status status = result.Status;
                status.StartResolutionForResult((androidIntent) =>
                {
                    PurchaseResultInfo purchaseResultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);

                    Debug.Log("HMSPluginResult: " + purchaseResultInfo.ReturnCode);
                    Debug.Log("HMSPluginPurchaseData: " + purchaseResultInfo.InAppPurchaseData);

                    switch (purchaseResultInfo.ReturnCode)
                    {
                        case OrderStatusCode.ORDER_STATE_CANCEL:
                            // User cancel payment.
                            break;
                        case OrderStatusCode.ORDER_STATE_FAILED:

                        case OrderStatusCode.ORDER_PRODUCT_OWNED:
                            // to check if there exists undelivered products.
                            break;
                        case OrderStatusCode.ORDER_STATE_SUCCESS:
                            // pay success.
                            String inAppPurchaseData = purchaseResultInfo.InAppPurchaseData;
                            String inAppPurchaseDataSignature = purchaseResultInfo.InAppDataSignature;
                            Debug.Log("HMS:" + inAppPurchaseData);
                            Debug.Log("HMS:" + inAppPurchaseDataSignature);
                            // use the public key of your app to verify the signature.
                            // If ok, you can deliver your products.
                            // If the user purchased a consumable product, call the consumeOwnedPurchase API to consume it after successfully delivering the product.
                            break;
                        default:
                            break;
                    }

                    onPurchaseSuccess();
                },(exception) =>
                {
                    onPurchaseError();
                }
                );


            }

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMSPlugin]: ERROR BuyProduct!!" + exception.Message);
        });



    }

    public void onPurchaseSuccess()
    {
        Debug.Log("[HMSPlugin]: PURCHASE SUCCESS");
    }

    public void onPurchaseError()
    {
        Debug.Log("[HMSPlugin]: PURCHASE ERROR");
    }


    public void addListener(UnityAction action){
        if (loadedEvent!= null)
        {
            loadedEvent.AddListener(action);  
        } 
        
    }




}




[System.Serializable]
public class Product
{
    [SerializeField]
    public string productID;
    [SerializeField]
    public bool isConsumable;
}
