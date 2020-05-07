//# define DEBUG

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using HuaweiConstants;
//using HuaweiMobileServices.Base;

//using HuaweiMobileServices.IAP;
//using System;
//using UnityEngine.Events;
//using HuaweiMobileServices.Id;

//public class IapDemoManager : MonoBehaviour
//{

//    public string key;

//    public ProductInfo[] products;

//    [HideInInspector]
//    public int numberOfProductsRetrieved;

//    List<string> productIdConsumablesList = new List<string>();
//    List<string> productIdNonConsumablesList = new List<string>();
//    List<ProductInfo> productInfoList = new List<ProductInfo>();

//    private IapManager iapManager;

//    UnityEvent loadedEvent;

//    void Awake()
//    {
//        Debug.Log("[HMSPlugin]: IAPP manager Init");
//        loadedEvent = new UnityEvent();
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        var accountManager = AccountManager.Instance;
//        accountManager.OnSignInFailed = (error) =>
//        {
//            Debug.Log($"[HMSPlugin]: SignIn failed. {error.Message}");
//        };
//        accountManager.OnSignInSuccess = SignedIn;
//        accountManager.SignIn();
//    }

//    private void SignedIn(AuthHuaweiId authHuaweiId)
//    {
//        iapManager = IapManager.Instance;
//        iapManager.OnCheckIapAvailabilitySuccess = LoadStore;
//        iapManager.OnCheckIapAvailabilityFailure = (error) =>
//        {
//            Debug.Log($"[HMSPlugin]: IAP check failed. {error.Message}");
//        };
//        iapManager.CheckIapAvailability();
//    }

//    private void LoadStore()
//    {

//        iapManager.OnObtainOwnedPurchasesFailure = (error) =>
//        {
//            Debug.Log($"[HMSPlugin]: IAP check failed. {error.Message}");
//        };
//        iapManager.OnObtainOwnedPurchasesSuccess = (ownedPurchrases) =>
//        {

//        };
//        iapManager.ObtainOwnedPurchases();

//        foreach (ProductInfo product in products)
//        {
//            if (product.isConsumable)
//            {
//                productIdConsumablesList.Add(product.productID);
//            }

//        }

//        ProductInfoReq productInfoReq = new ProductInfoReq();

//        // CONSUMABLE REQUEST
//        productInfoReq.PriceType = 0;

//        productInfoReq.ProductIds = productIdConsumablesList;

//        ITask<ProductInfoResult> task = iapClient.ObtainProductInfo(productInfoReq);

//        Debug.Log(task.Result);

//        task.AddOnSuccessListener((result) =>
//        {

//            if (result != null)
//            {

//                if (result.ProductInfoList.Count != 0)
//                {

//                    Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
//                    Debug.Log("[HMSPlugin]: Found " + result.ProductInfoList.Count + "consumable products");
//                    foreach (ProductInfo productInfo in result.ProductInfoList)
//                    {
//                        Debug.Log("[HMSPlugin]: " + productInfo.ProductName + "," + productInfo.Price);
//                        productInfoList.Add(productInfo);
//                    }
//                }


//            }
//            loadedEvent.Invoke();

//        }).AddOnFailureListener((exception) =>
//        {
//            Debug.Log("[HMSPlugin]: ERROR Houston!!" + exception.Message);
//        });



//        RestorePurchases();
//        ObtainOwnedPurchases();

//    }


//    private void RestorePurchases()
//    {
//        foreach (Product product in products)
//        {
//            if (!product.isConsumable)
//            {
//                productIdConsumablesList.Add(product.productID);
//            }

//        }

//        ProductInfoReq productInfoReq = new ProductInfoReq();


//        // NON CONSUMABLE REQUEST
//        productInfoReq.PriceType = 1;
//        productInfoReq.ProductIds = productIdConsumablesList;

//        ITask<ProductInfoResult> task = iapClient.ObtainProductInfo(productInfoReq);

//        task.AddOnSuccessListener((result) =>
//        {
//            Debug.Log("HMSPlugin: NON consumables result success");
//            if (result != null && result.ProductInfoList.Count != 0)
//            {
//                Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
//                Debug.Log("[HMSPlugin]: Found " + result.ProductInfoList.Count + "non consumable products");
//                foreach (ProductInfo productInfo in result.ProductInfoList)
//                {
//                    Debug.Log("[HMSPlugin]: " + productInfo.ProductName + "," + productInfo.Price);
//                    productInfoList.Add(productInfo);
//                }

//            }

//        }).AddOnFailureListener((exception) =>
//        {
//            Debug.Log("[HMSPlugin]: ERROR Houston!!" + exception.Message);
//        });
//    }

//    public ProductInfo GetProductInfo(string productID)
//    {
//        return productInfoList.Find(productInfo => productInfo.ProductId == productID);
//    }

//    private void recoverPurchases()
//    {
//        OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq();

//        ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

//        task.AddOnSuccessListener((result) =>
//        {
//            Debug.Log("HMSP: recoverPurchases");
//            foreach (string inAppPurchaseData in result.InAppPurchaseDataList)
//            {

//                InAppPurchaseData inAppPurchaseDataBean = new InAppPurchaseData(inAppPurchaseData);
//                string purchaseToken = inAppPurchaseDataBean.PurchaseToken;

//                ConsumePurchase(purchaseToken);

//                Debug.Log("HMSP: recoverPurchases result> " + result.ReturnCode);
//            }


//        }).AddOnFailureListener((exception) =>
//        {
//            Debug.Log("HMSP: Error on recoverPurchases" + exception.StackTrace);
//        });
//    }


//    private void ConsumePurchase(string token)
//    {

//        ConsumeOwnedPurchaseReq consumeOwnedPurchaseReq = new ConsumeOwnedPurchaseReq();
//        consumeOwnedPurchaseReq.PurchaseToken = token;

//        ITask<ConsumeOwnedPurchaseResult> task = iapClient.ConsumeOwnedPurchase(consumeOwnedPurchaseReq);

//        task.AddOnSuccessListener((result) =>
//        {
//            Debug.Log("HMSP: consumePurchase");

//        }).AddOnFailureListener((exception) =>
//        {
//            Debug.Log("HMSP: Error on consumePurchase");
//        });
//    }

//    public void BuyProduct(string productID)
//    {

//        PurchaseIntentReq purchaseIntentReq = new PurchaseIntentReq();

//        ProductInfo productInfo = productInfoList.Find(product => product.ProductId == productID);
//        purchaseIntentReq.PriceType = productInfo.PriceType;
//        purchaseIntentReq.ProductId = productID;
//        // ToDo : developer payload???
//        purchaseIntentReq.DeveloperPayload = "test";

//        ITask<PurchaseIntentResult> task = iapClient.CreatePurchaseIntent(purchaseIntentReq);

//        InAppPurchaseData inAppPurchaseDataBean;

//        task.AddOnSuccessListener((result) =>
//        {

//            if (result != null)
//            {
//                Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
//                Debug.Log("[HMSPlugin]: Bought " + productID);
//                Status status = result.Status;
//                status.StartResolutionForResult((androidIntent) =>
//                {
//                    PurchaseResultInfo purchaseResultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);

//                    Debug.Log("HMSPluginResult: " + purchaseResultInfo.ReturnCode);
//                    Debug.Log("HMErrorMssg: " + purchaseResultInfo.ErrMsg);
//                    Debug.Log("HMS: HMSInAppPurchaseData" + purchaseResultInfo.InAppPurchaseData);
//                    Debug.Log("HMS: HMSInAppDataSignature" + purchaseResultInfo.InAppDataSignature);

//                    switch (purchaseResultInfo.ReturnCode)
//                    {
//                        case OrderStatusCode.ORDER_STATE_CANCEL:
//                            // User cancel payment.
//                            Debug.Log("[HMS]: User cancel payment");
//                            break;
//                        case OrderStatusCode.ORDER_STATE_FAILED:
//                            Debug.Log("[HMS]: order paymentf failed");
//                            break;

//                        case OrderStatusCode.ORDER_PRODUCT_OWNED:
//                            Debug.Log("[HMS]: Product owned");
//                            inAppPurchaseDataBean = new InAppPurchaseData(purchaseResultInfo.InAppPurchaseData);
//                            ConsumePurchase(inAppPurchaseDataBean.PurchaseToken);

//                            // to check if there exists undelivered products.
//                            break;
//                        case OrderStatusCode.ORDER_STATE_SUCCESS:
//                            // pay success.
//                            Debug.Log("[HMSPlugin]: PURCHASE SUCCESS");

//                            String inAppPurchaseData = purchaseResultInfo.InAppPurchaseData;
//                            String inAppPurchaseDataSignature = purchaseResultInfo.InAppDataSignature;
//                            Debug.Log("HMS" + inAppPurchaseData);
//                            Debug.Log("HMS:" + inAppPurchaseDataSignature);

//                            //ToDO: use the public key of your app to verify the signature.
//                            // If ok, you can deliver your products.
//                            // If the user purchased a consumable product, call the consumeOwnedPurchase API to consume it after successfully delivering the product.

//                            // Consume purchase
//                            inAppPurchaseDataBean = new InAppPurchaseData(purchaseResultInfo.InAppPurchaseData);
//                            ConsumePurchase(inAppPurchaseDataBean.PurchaseToken);
//                            break;
//                        default:
//                            break;
//                    }


//                }, (exception) =>
//                 {
//                     Debug.Log("[HMSPlugin]:startIntent ERROR");
//                 }
//                );

//            }

//        }).AddOnFailureListener((exception) =>
//        {
//            Debug.Log("[HMSPlugin]: ERROR BuyProduct!!" + exception.Message);
//        });

//    }

//    public void ObtainOwnedPurchases()
//    {
//        Debug.Log("HMSP: ObtainOwnedPurchaseRequest");
//        OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq();

//        ownedPurchasesReq.PriceType = 1;

//        ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

//        task.AddOnSuccessListener((result) =>
//        {
//            Debug.Log("HMSP: ObtainOwnedPurchases");

//            if (result != null && result.InAppPurchaseDataList != null)
//            {
//                Debug.Log("HMSP: Restored " + result.InAppPurchaseDataList.Count + "non consumable products");

//                foreach (string inAppPurchaseData in result.InAppPurchaseDataList)
//                {
//                    InAppPurchaseData inAppPurchaseDataBean = new InAppPurchaseData(inAppPurchaseData);

//                    Debug.Log("HMSP: " + inAppPurchaseDataBean.ProductId);
//                }
//            }

//        }).AddOnFailureListener((exception) =>
//        {
//            Debug.Log("HMSP: Error on ObtainOwnedPurchases");
//        });
//    }


//    public void addListener(UnityAction action)
//    {
//        if (loadedEvent != null)
//        {
//            loadedEvent.AddListener(action);
//        }

//    }

//}
