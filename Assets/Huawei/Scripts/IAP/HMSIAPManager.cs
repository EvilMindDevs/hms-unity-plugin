

using HuaweiMobileServices.Base;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using HuaweiConstants;

namespace HmsPlugin
{
    public class HMSIAPManager : HMSManagerSingleton<HMSIAPManager>
    {
        private readonly HMSException IAP_NOT_AVAILABLE = new HMSException("[HMSIAPManager] IAP not available", "IAP not available", "IAP not available") { };

        public Action OnCheckIapAvailabilitySuccess { get; set; }
        public Action<HMSException> OnCheckIapAvailabilityFailure { get; set; }

        public Action<IList<ProductInfoResult>> OnObtainProductInfoSuccess { get; set; }
        public Action<HMSException> OnObtainProductInfoFailure { get; set; }

        public Action OnRecoverPurchasesSuccess { get; set; }
        public Action<HMSException> OnRecoverPurchasesFailure { get; set; }

        public Action<ConsumeOwnedPurchaseResult> OnConsumePurchaseSuccess { get; set; }
        public Action<HMSException> OnConsumePurchaseFailure { get; set; }

        public Action<PurchaseResultInfo> OnBuyProductSuccess { get; set; }
        public Action<int> OnBuyProductFailure { get; set; }

        public Action<OwnedPurchasesResult> OnObtainOwnedPurchasesSuccess { get; set; }
        public Action<HMSException> OnObtainOwnedPurchasesFailure { get; set; }

        public Action<OwnedPurchasesResult> OnObtainOwnedPurchaseRecordSuccess { get; set; }
        public Action<HMSException> OnObtainOwnedPurchaseRecordFailure { get; set; }

        public Action<IsSandboxActivatedResult> OnIsSandboxActivatedSuccess { get; set; }
        public Action<HMSException> OnIsSandboxActivatedFailure { get; set; }

        private IIapClient iapClient;
        private bool? iapAvailable = null;
        private List<ProductInfo> productInfoList = new List<ProductInfo>();

        public HMSIAPManager()
        {
            Debug.Log($"[HMS] : HMSIAPManager Constructor");
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
        }

        private void OnAwake()
        {
            if (HMSIAPKitSettings.Instance.Settings.GetBool(HMSIAPKitSettings.InitializeOnStart))
                CheckIapAvailability();
        }

        public void CheckIapAvailability()
        {
            iapClient = Iap.GetIapClient();
            ITask<EnvReadyResult> task = iapClient.EnvReady;
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSIAPManager] checkIapAvailabity SUCCESS");
                iapAvailable = true;
                OnCheckIapAvailabilitySuccess?.Invoke();
                ObtainProductInfo(HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.Consumable),
                    HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.NonConsumable),
                    HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.Subscription));


            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSIAPManager]: Error on EnvReady: " + exception.WrappedCauseMessage + " " + exception.WrappedExceptionMessage);
                IapApiException iapEx = exception.AsIapApiException();
                if (iapEx.Status != null || HMSResponses.UnresolvableStatusCodes.Contains(iapEx.Status.StatusCode))
                {
                    Debug.LogError("[HMSIAPManager]: Cannot Start Resolution for Status Code: " + iapEx.Status.StatusCode);
                    OnCheckIapAvailabilityFailure?.Invoke(exception);
                    return;
                }

                iapEx.Status.StartResolutionForResult
                (
                    (intent) =>
                    {
                        var returnEnum = (HMSResponses.IapStatusCodes)intent.GetIntExtra("returnCode");
                        if (returnEnum == HMSResponses.IapStatusCodes.ORDER_STATE_SUCCESS)
                        {
                            Debug.Log("[HMSIAPManager]: Success on iapEx Resolution");
                            iapAvailable = true;
                            OnCheckIapAvailabilitySuccess?.Invoke();
                            ObtainProductInfo(HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.Consumable),
                                HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.NonConsumable),
                                HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.Subscription));
                        }
                        else
                        {
                            iapAvailable = false;
                            iapClient = null;
                            Debug.LogError("[HMSIAPManager]: ERROR on StartResolutionForResult. Return Code: " + (int)returnEnum + ". Reason: " + returnEnum);
                            OnCheckIapAvailabilityFailure?.Invoke(IAP_NOT_AVAILABLE);
                        }
                    },
                    (ex) =>
                    {
                        iapClient = null;
                        iapAvailable = false;

                        Debug.LogError("[HMSIAPManager]: ERROR on StartResolutionForResult: " + ex.WrappedCauseMessage + " " + ex.WrappedExceptionMessage);
                        OnCheckIapAvailabilityFailure?.Invoke(exception);
                    }
                );
            });
        }

        public void ObtainProductInfo(List<string> productIdConsumablesList, List<string> productIdNonConsumablesList, List<string> productIdSubscriptionList)
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                Debug.Log("[HMSIAPManager]: ObtainProductInfo IAP not available");
                return;
            }

            if (!IsNullOrEmpty(productIdConsumablesList))
            {
                Debug.Log("[HMSIAPManager]: ObtainProductInfo obtaining IN_APP_CONSUMABLES");
                ObtainProductInfo(new List<string>(productIdConsumablesList), PriceType.IN_APP_CONSUMABLE);
            }
            if (!IsNullOrEmpty(productIdNonConsumablesList))
            {
                Debug.Log("[HMSIAPManager]: ObtainProductInfo obtaining IN_APP_NONCONSUMABLES");
                ObtainProductInfo(new List<string>(productIdNonConsumablesList), PriceType.IN_APP_NONCONSUMABLE);
            }
            if (!IsNullOrEmpty(productIdSubscriptionList))
            {
                Debug.Log("[HMSIAPManager]: ObtainProductInfo obtaining IN_APP_SUBSCRIPTIONS");
                ObtainProductInfo(new List<string>(productIdSubscriptionList), PriceType.IN_APP_SUBSCRIPTION);
            }
        }

        private void ObtainProductInfo(IList<string> productIdList, PriceType priceType)
        {
            
            if (iapAvailable != true)
            {
                Debug.Log($"[HMSIAPManager]: ObtainProductInfo for Product Type:{priceType.Value} IAP not available");
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            Debug.Log($"[HMSIAPManager]: ObtainProductInfo for Price Type:{priceType.Value}" + "  {0=Consumable}  {1=Non-Consumable}  {2=Subscription}");
            Debug.Log($"[HMSIAPManager]: ObtainProductInfo started obtaining for Product Type:{priceType.Value} for {productIdList.Count} amount of product in productIdList");

            ProductInfoReq productInfoReq = new ProductInfoReq
            {
                PriceType = priceType,
                ProductIds = productIdList
            };

            iapClient.ObtainProductInfo(productInfoReq).AddOnSuccessListener((type) =>
            {
                Debug.Log($"[HMSIAPManager]: ObtainProductInfo for Price Type:{priceType.Value} ReturnMessage: {type.ErrMsg}  ReturnCode: {type.ReturnCode}");
                Debug.Log($"[HMSIAPManager]: ObtainProductInfo for Price Type:{priceType.Value} Obtained product count:"  + type.ProductInfoList.Count);
                foreach (var productInfo in type.ProductInfoList)
                {
                    if (!productInfoList.Exists(c => c.ProductId == productInfo.ProductId)) 
                    {
                        productInfoList.Add(productInfo);
                        Debug.Log($"[HMSIAPManager]: ObtainProductInfo for Price Type:{priceType.Value} ProductId:" + productInfo.ProductId + ", ProductName:" + productInfo.ProductName + ", Price:" + productInfo.Price);
                    }
                }

                OnObtainProductInfoSuccess?.Invoke(new List<ProductInfoResult> { type });
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[HMSIAPManager]: ObtainProductInfo for Price Type:{priceType.Value} failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnObtainProductInfoFailure?.Invoke(exception);
            });
        }

        public void ConsumeOwnedPurchases()
        {

            if (iapAvailable != true)
            {
                OnRecoverPurchasesFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq();

            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSIAPManager] recoverPurchases");
                foreach (var inAppPurchaseData in result.InAppPurchaseDataList)
                {
                    ConsumePurchaseWithPurchaseData(inAppPurchaseData);
                    Debug.Log("[HMSIAPManager] recoverPurchases result> " + result.ReturnCode);
                }

                OnRecoverPurchasesSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSIAPManager] ConsumeOwnedPurchases failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnRecoverPurchasesFailure?.Invoke(exception);

            });
        }

        public void ConsumePurchase(PurchaseResultInfo purchaseResultInfo)
        {
            ConsumePurchaseWithPurchaseData(purchaseResultInfo.InAppPurchaseData);
        }

        public void ConsumePurchaseWithPurchaseData(InAppPurchaseData inAppPurchaseData)
        {
            string purchaseToken = inAppPurchaseData.PurchaseToken;
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
                Debug.Log("[HMSIAPManager] ConsumePurchaseWithToken. Product Id: " + result.ConsumePurchaseData.ProductId);
                OnConsumePurchaseSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSIAPManager] ConsumePurchaseWithToken failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnConsumePurchaseFailure?.Invoke(exception);

            });
        }

        public void BuyProduct(string productId, bool consumeAfter = true, string payload = "")
        {
            var productInfo = GetProductInfo(productId);
            if (productInfo != null)
            {
                InternalBuyProduct(productInfo, consumeAfter, payload);
            }
            else
            {
                Debug.LogError($"[HMSIAPManager] Specified: {productId} could not be found in retrieved product list!");
            }
        }

        public void InternalBuyProduct(ProductInfo productInfo, bool consumeAfter = true, string payload = "")
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
                    Debug.Log("[HMSIAPManager]:" + result.ErrMsg + result.ReturnCode.ToString());
                    Debug.Log("[HMSIAPManager]: Buying " + purchaseIntentReq.ProductId);
                    Status status = result.Status;
                    status.StartResolutionForResult((androidIntent) =>
                    {
                        PurchaseResultInfo purchaseResultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);

                        if (purchaseResultInfo.ReturnCode == OrderStatusCode.ORDER_STATE_SUCCESS)
                        {
                            Debug.Log("[HMSIAPManager] HMSInAppPurchaseData ProductId: " + purchaseResultInfo.InAppPurchaseData.ProductId);
                            Debug.Log("[HMSIAPManager] HMSInAppDataSignature: " + purchaseResultInfo.InAppDataSignature);
                            OnBuyProductSuccess.Invoke(purchaseResultInfo);
                            if (consumeAfter)
                                ConsumePurchase(purchaseResultInfo);
                        }
                        else
                        {
                            switch (purchaseResultInfo.ReturnCode)
                            {
                                case OrderStatusCode.ORDER_STATE_CANCEL:
                                    Debug.LogError("[HMSIAPManager] User cancel payment");
                                    break;

                                case OrderStatusCode.ORDER_STATE_FAILED:
                                    Debug.LogError("[HMSIAPManager] order payment failed");
                                    break;

                                case OrderStatusCode.ORDER_PRODUCT_OWNED:
                                    Debug.LogError("[HMSIAPManager] Product owned");
                                    break;

                                default:
                                    Debug.LogError("[HMSIAPManager] BuyProduct failed. ReturnCode: " + purchaseResultInfo.ReturnCode + ", ErrorMsg: " + purchaseResultInfo.ErrMsg);
                                    break;
                            }
                            OnBuyProductFailure?.Invoke(purchaseResultInfo.ReturnCode);
                        }

                    }, (exception) =>
                    {
                        Debug.LogError("[HMSIAPManager] startIntent ERROR");
                    });

                }

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSIAPManager]: BuyProduct failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
            });
        }

        public void ObtainOwnedPurchases(PriceType priceType)
        {
            if (iapAvailable != true)
            {
                OnObtainOwnedPurchasesFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            Debug.Log("[HMSIAPManager] ObtainOwnedPurchaseRequest");
            ObtainOwnedPurchases(new OwnedPurchasesReq() { PriceType = priceType });
        }

        public void ObtainAllOwnedPurchases()
        {
            if (iapAvailable != true)
            {
                OnObtainOwnedPurchasesFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }


            Debug.Log("[HMSIAPManager] ObtainAllOwnedPurchaseRequest");
            ObtainOwnedPurchases(new OwnedPurchasesReq() { PriceType = PriceType.IN_APP_CONSUMABLE });
            ObtainOwnedPurchases(new OwnedPurchasesReq() { PriceType = PriceType.IN_APP_NONCONSUMABLE });
            ObtainOwnedPurchases(new OwnedPurchasesReq() { PriceType = PriceType.IN_APP_SUBSCRIPTION });
        }

        private void ObtainOwnedPurchases(OwnedPurchasesReq ownedPurchasesReq)
        {
            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSIAPManager] ObtainOwnedPurchases");
                foreach (var item in result.InAppPurchaseDataList)
                {
                    Debug.Log("[HMSIAPManager] ProductId: " + item.ProductId + ", ProductName: " + item.ProductName + ", Price: " + item.Price);
                }
                OnObtainOwnedPurchasesSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSIAPManager]: ObtainOwnedPurchases failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnObtainOwnedPurchasesFailure?.Invoke(exception);
            });
        }

        public void ObtainOwnedPurchaseRecord(PriceType priceType)
        {
            if (iapAvailable != true)
            {
                OnObtainOwnedPurchaseRecordFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            Debug.Log("HMSP: ObtainOwnedPurchaseRecord");
            ObtainOwnedPurchaseRecord(new OwnedPurchasesReq() { PriceType = priceType });
        }

        public void ObtainAllOwnedPurchaseRecord()
        {
            if (iapAvailable != true)
            {
                OnObtainOwnedPurchaseRecordFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            Debug.Log("HMSP: ObtainOwnedPurchaseRecord");
            ObtainOwnedPurchaseRecord(new OwnedPurchasesReq() { PriceType = PriceType.IN_APP_CONSUMABLE });
            ObtainOwnedPurchaseRecord(new OwnedPurchasesReq() { PriceType = PriceType.IN_APP_NONCONSUMABLE });
            ObtainOwnedPurchaseRecord(new OwnedPurchasesReq() { PriceType = PriceType.IN_APP_SUBSCRIPTION });
        }

        private void ObtainOwnedPurchaseRecord(OwnedPurchasesReq req)
        {
            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchaseRecord(req);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("HMSP: ObtainOwnedPurchaseRecord");
                foreach (var item in result.InAppPurchaseDataList)
                {
                    Debug.Log("[HMSPlugin]: ProductId: " + item.ProductId + ", ProductName: " + item.ProductName + ", Price: " + item.Price);
                }
                OnObtainOwnedPurchaseRecordSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("HMSP: Error on ObtainOwnedPurchaseRecord");
                OnObtainOwnedPurchaseRecordFailure?.Invoke(exception);
            });
        }

        public void RestorePurchases(Action<OwnedPurchasesResult> action)
        {
            OnObtainOwnedPurchasesSuccess = (ownedPurchaseResult) =>
            {
                Debug.Log("Return Code: " + ownedPurchaseResult.ReturnCode);
                Debug.Log("InAppPurchaseDataList: " + ownedPurchaseResult.InAppPurchaseDataList.Count);
                Debug.Log("ItemList: " + ownedPurchaseResult.ItemList.Count);

                action.Invoke(ownedPurchaseResult);
            };

            OnObtainOwnedPurchasesFailure = (error) =>
            {
                Debug.LogError("[HMSIAPManager]: RestorePurchasesError failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
            };

            ObtainAllOwnedPurchases();
        }

        public ProductInfo GetProductInfo(string productID)
        {
            return productInfoList.Find(productInfo => productInfo.ProductId == productID);
        }

        public bool IsNullOrEmpty(List<string> array)
        {
            return (array == null || array.Count == 0);
        }

        public void IsSandboxActivated()
        {
            if (iapClient != null)
            {
                var task = iapClient.SandboxActivated;
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log("[HMSIAPManager]: IsSandboxActivated success!");
                    OnIsSandboxActivatedSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError("[HMSIAPManager]: IsSandboxActivated failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnIsSandboxActivatedFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError("[HMSIAPManager]: IsSandboxActivated failed. IAP is not initialized.");
            }
        }
    }
}