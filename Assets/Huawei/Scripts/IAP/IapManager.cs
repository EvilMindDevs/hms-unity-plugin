# define DEBUG

using HuaweiMobileServices.Base;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class IapManager : MonoBehaviour
    {

    public static IapManager GetInstance(string name = "IapManager") => GameObject.Find(name).GetComponent<IapManager>();

    private static readonly HMSException IAP_NOT_AVAILABLE = new HMSException("IAP not available");

        public Action OnCheckIapAvailabilitySuccess { get; set; }
        public Action<HMSException> OnCheckIapAvailabilityFailure { get; set; }

        public Action<IList<ProductInfoResult>> OnObtainProductInfoSuccess { get; set; }
        public Action<HMSException> OnObtainProductInfoFailure { get; set; }

        public Action OnRecoverPurchasesSuccess { get; set; }
        public Action<HMSException> OnRecoverPurchasesFailure { get; set; }

        public Action OnConsumePurchaseSuccess { get; set; }
        public Action<HMSException> OnConsumePurchaseFailure { get; set; }

        public Action<PurchaseResultInfo> OnBuyProductSuccess { get; set; }
        public Action<int> OnBuyProductFailure { get; set; }

        public Action<OwnedPurchasesResult> OnObtainOwnedPurchasesSuccess { get; set; }
        public Action<HMSException> OnObtainOwnedPurchasesFailure { get; set; }

        private IIapClient iapClient;
        private bool? iapAvailable = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void CheckIapAvailability()
        {
            iapClient = Iap.GetIapClient();
            ITask<EnvReadyResult> task = iapClient.EnvReady;
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("HMSP: checkIapAvailabity SUCCESS");
                iapAvailable = true;
                OnCheckIapAvailabilitySuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("HMSP: Error on ObtainOwnedPurchases");
                iapClient = null;
                iapAvailable = false;
                OnCheckIapAvailabilityFailure?.Invoke(exception);

            });
        }

        // TODO Obtain non-consumables too!
        public void ObtainProductInfo(IList<string> productIdConsumablesList, IList<string> productIdNonConsumablesList, IList<string> productIdSubscriptionList)
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            ProductInfoReq productInfoReq = new ProductInfoReq
            {
                PriceType = 0,
                ProductIds = productIdConsumablesList
            };

            iapClient.ObtainProductInfo(productInfoReq).AddOnSuccessListener((type0) =>
            {
                Debug.Log("[HMSPlugin]:" + type0.ErrMsg + type0.ReturnCode.ToString());
                Debug.Log("[HMSPlugin]: Found " + type0.ProductInfoList.Count + "consumable products");

                productInfoReq = new ProductInfoReq
                {
                    PriceType = 1,
                    ProductIds = productIdNonConsumablesList
                };

                iapClient.ObtainProductInfo(productInfoReq).AddOnSuccessListener((type1) =>
                {
                    Debug.Log("[HMSPlugin]:" + type1.ErrMsg + type1.ReturnCode.ToString());
                    Debug.Log("[HMSPlugin]: Found " + type1.ProductInfoList.Count + " non consumable products");

                    productInfoReq = new ProductInfoReq
                    {
                        PriceType = 2,
                        ProductIds = productIdSubscriptionList
                    };

                    iapClient.ObtainProductInfo(productInfoReq).AddOnSuccessListener((type2) =>
                    {
                        Debug.Log("[HMSPlugin]:" + type2.ErrMsg + type2.ReturnCode.ToString());
                        Debug.Log("[HMSPlugin]: Found " + type2.ProductInfoList.Count + " subscription products");


                        OnObtainProductInfoSuccess?.Invoke(new List<ProductInfoResult> { type0, type1, type2 });

                    }).AddOnFailureListener((exception) =>
                    {
                        Debug.Log("[HMSPlugin]: ERROR Subscriptions ObtainInfo " + exception.GetBaseException().Message);
                        OnObtainProductInfoFailure?.Invoke(exception);

                    });

                    

                }).AddOnFailureListener((exception) =>
                {
                    Debug.Log("[HMSPlugin]: ERROR Non Consumable ObtainInfo" + exception.Message);
                    OnObtainProductInfoFailure?.Invoke(exception);

                });

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMSPlugin]: ERROR Consumable ObtainInfo" + exception.Message);
                OnObtainProductInfoFailure?.Invoke(exception);

            });
        }

        public void ConsumeOwnedPurchases()
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq();

            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("HMSP: recoverPurchases");
                foreach (string inAppPurchaseData in result.InAppPurchaseDataList)
                {
                    ConsumePurchaseWithPurchaseData(inAppPurchaseData);
                    Debug.Log("HMSP: recoverPurchases result> " + result.ReturnCode);
                }

                OnRecoverPurchasesSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log($"HMSP: Error on recoverPurchases {exception.StackTrace}");
                OnRecoverPurchasesFailure?.Invoke(exception);

            });
        }

        public void ConsumePurchase(PurchaseResultInfo purchaseResultInfo)
        {
            ConsumePurchaseWithPurchaseData(purchaseResultInfo.InAppPurchaseData);
        }

        public void ConsumePurchaseWithPurchaseData(string inAppPurchaseData)
        {
            var inAppPurchaseDataBean = new InAppPurchaseData(inAppPurchaseData);
            string purchaseToken = inAppPurchaseDataBean.PurchaseToken;
            ConsumePurchaseWithToken(purchaseToken);
        }

        public void ConsumePurchaseWithToken(string token)
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            ConsumeOwnedPurchaseReq consumeOwnedPurchaseReq = new ConsumeOwnedPurchaseReq
            {
                PurchaseToken = token
            };

            ITask<ConsumeOwnedPurchaseResult> task = iapClient.ConsumeOwnedPurchase(consumeOwnedPurchaseReq);

            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("HMSP: consumePurchase");
                OnConsumePurchaseSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("HMSP: Error on consumePurchase");
                OnConsumePurchaseFailure?.Invoke(exception);

            });
        }

        public void BuyProduct(ProductInfo productInfo, string payload)
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            PurchaseIntentReq purchaseIntentReq = new PurchaseIntentReq
            {
                PriceType = productInfo.PriceType,
                ProductId = productInfo.ProductId,
                DeveloperPayload = payload
            };

            ITask<PurchaseIntentResult> task = iapClient.CreatePurchaseIntent(purchaseIntentReq);
            task.AddOnSuccessListener((result) =>
            {

                if (result != null)
                {
                    Debug.Log("[HMSPlugin]:" + result.ErrMsg + result.ReturnCode.ToString());
                    Debug.Log("[HMSPlugin]: Bought " + purchaseIntentReq.ProductId);
                    Status status = result.Status;
                    status.StartResolutionForResult((androidIntent) =>
                    {
                        PurchaseResultInfo purchaseResultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);

                        Debug.Log("HMSPluginResult: " + purchaseResultInfo.ReturnCode);
                        Debug.Log("HMErrorMssg: " + purchaseResultInfo.ErrMsg);
                        Debug.Log("HMS: HMSInAppPurchaseData" + purchaseResultInfo.InAppPurchaseData);
                        Debug.Log("HMS: HMSInAppDataSignature" + purchaseResultInfo.InAppDataSignature);

                        switch (purchaseResultInfo.ReturnCode)
                        {
                            case OrderStatusCode.ORDER_STATE_SUCCESS:
                                OnBuyProductSuccess.Invoke(purchaseResultInfo);
                                break;
                            default:
                                OnBuyProductFailure.Invoke(purchaseResultInfo.ReturnCode);
                                break;
                        }

                    }, (exception) =>
                    {
                        Debug.Log("[HMSPlugin]:startIntent ERROR");
                    });

                }

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMSPlugin]: ERROR BuyProduct!!" + exception.Message);
            });
        }

        public void ObtainOwnedPurchases()
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            Debug.Log("HMSP: ObtainOwnedPurchaseRequest");
            OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq
            {
                PriceType = 1
            };

            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("HMSP: ObtainOwnedPurchases");
                OnObtainOwnedPurchasesSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("HMSP: Error on ObtainOwnedPurchases");
                OnObtainProductInfoFailure?.Invoke(exception);
            });
        }

    }
}
