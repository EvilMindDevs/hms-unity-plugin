

using HuaweiConstants;

using HuaweiMobileServices.Base;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace HmsPlugin
{
    public class HMSIAPManager : HMSManagerSingleton<HMSIAPManager>
    {
        private const string Tag = "HMSIAPManager";

        private readonly HMSException IAP_NOT_AVAILABLE = new HMSException("[HMSIAPManager] IAP not available", "IAP not available", "IAP not available") { };

        #region Delegates

        public Action OnInitializeIAPSuccess { get; set; }
        public Action<HMSException> OnInitializeIAPFailure { get; set; }

        public Action<IList<ProductInfoResult>> OnObtainProductInfoSuccess { get; set; }
        public Action<HMSException> OnObtainProductInfoFailure { get; set; }

        public Action<ConsumeOwnedPurchaseResult> OnConsumePurchaseSuccess { get; set; }
        public Action<HMSException> OnConsumePurchaseFailure { get; set; }

        public Action<PurchaseResultInfo> OnBuyProductSuccess { get; set; }
        public Action<int> OnBuyProductFailure { get; set; }

        public Action<OwnedPurchasesResult> OnObtainOwnedPurchasesSuccess { get; set; }
        public Action<HMSException> OnObtainOwnedPurchasesFailure { get; set; }

        public Action<OwnedPurchasesResult> OnObtainOwnedPurchaseRecordSuccess { get; set; }
        public Action<HMSException> OnObtainOwnedPurchaseRecordFailure { get; set; }

        #endregion



        #region Variable: IapClient (interface)

        public IIapClient IapClient { get => iapClient; set => iapClient = value; }

        private IIapClient iapClient;

        #endregion

        #region Variable: iapAvailable

        private bool? iapAvailable = null;

        #endregion

        #region Variable: productInfoList

        private List<ProductInfo> productInfoList = new List<ProductInfo>();

        #endregion

        #region Variable: IsSandboxUser

        private IsSandboxActivatedResult sandboxState;
        public IsSandboxActivatedResult SandboxState { get => sandboxState; set => sandboxState = value; }

        #endregion

        #region Constructor (InitializeIAP)

        public HMSIAPManager()
        {

            if (!HMSDispatcher.InstanceExists)
            {
                HMSDispatcher.CreateDispatcher();
            }

            HMSDispatcher.InvokeAsync(InitControlOfIAP);
        }

        private void InitControlOfIAP()
        {
            Debug.Log($"[{Tag}]: Init Control Of IAP");

            if (HMSIAPKitSettings.Instance.Settings.GetBool(HMSIAPKitSettings.InitializeOnStart))
            {
                InitializeIAP();
            }
        }

        public void InitializeIAP()
        {
            iapClient = Iap.GetIapClient();
            ITask<EnvReadyResult> task = iapClient.EnvReady;

            task.AddOnSuccessListener((result) =>
            {
                Debug.Log($"[{Tag}]: Initialize IAP SUCCESS");

                iapAvailable = true;
                OnInitializeIAPSuccess?.Invoke();

                Prepare_IAP_Products();

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[{Tag}]: Error on Env Ready: " + exception.WrappedCauseMessage + " " + exception.WrappedExceptionMessage);

                IapApiException iapEx = exception.AsIapApiException();

                if (iapEx.Status != null || HMSResponses.UnresolvableStatusCodes.Contains(iapEx.Status.StatusCode))
                {
                    Debug.LogError($"[{Tag}]: Cannot Start Resolution for Status Code: " + iapEx.Status.StatusCode);

                    OnInitializeIAPFailure?.Invoke(exception);
                    return;
                }

                iapEx.Status.StartResolutionForResult
                (
                    (intent) =>
                    {
                        var returnEnum = (HMSResponses.IapStatusCodes)intent.GetIntExtra("returnCode");
                        if (returnEnum == HMSResponses.IapStatusCodes.ORDER_STATE_SUCCESS)
                        {
                            Debug.Log($"[{Tag}]: Success on iapEx Resolution");
                            Prepare_IAP_Products();
                        }
                        else
                        {
                            iapAvailable = false;
                            iapClient = null;
                            Debug.LogError($"[{Tag}]: ERROR on StartResolutionForResult. Return Code: " + (int)returnEnum + ". Reason: " + returnEnum);
                            OnInitializeIAPFailure?.Invoke(IAP_NOT_AVAILABLE);
                        }
                    },
                    (ex) =>
                    {
                        iapClient = null;
                        iapAvailable = false;

                        Debug.LogError($"[{Tag}]: ERROR on StartResolutionForResult: " + ex.WrappedCauseMessage + " " + ex.WrappedExceptionMessage);
                        OnInitializeIAPFailure?.Invoke(exception);
                    }
                );
            });
        }

        private void Prepare_IAP_Products()
        {

            void ConsumeControl()
            {
                RestoreOwnedPurchases((ownedPurchaseResult) =>
                {
                    if (ownedPurchaseResult != null)
                    {
                        foreach (var obj in ownedPurchaseResult.InAppPurchaseDataList)
                        {
                            if (sandboxState.SandboxUser)
                            {
                                if ((IAPProductType)obj.Kind == IAPProductType.Consumable || (IAPProductType)obj.Kind == IAPProductType.NonConsumable)
                                {
                                    ConsumePurchaseWithToken(obj.PurchaseToken);
                                }
                            }
                            else if ((IAPProductType)obj.Kind == IAPProductType.Consumable)
                            {
                                ConsumePurchaseWithToken(obj.PurchaseToken);
                            }
                        }
                    }
                });
            }

            void NextPhase()
            {
                var consumables = HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.Consumable);
                var nonConsumables = HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.NonConsumable);
                var subscriptions = HMSIAPProductListSettings.Instance.GetProductIdentifiersByType(HMSIAPProductType.Subscription);

                GetProductInfo(consumables, nonConsumables, subscriptions);

                ConsumeControl();
            }

            GetSandboxState(NextPhase);

        }

        #endregion

        #region Product Info (Obtains product details configured in AppGallery Connect)

        public void GetProductInfo(List<string> productIdConsumablesList, List<string> productIdNonConsumablesList, List<string> productIdSubscriptionList)
        {

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                Debug.Log($"[{Tag}]: ObtainProductInfo IAP not available");
                return;
            }

            if (!IsNullOrEmpty(productIdConsumablesList))
            {
                Debug.Log($"[{Tag}]: Consumable Products Available");
                GetProduct(new List<string>(productIdConsumablesList), PriceType.IN_APP_CONSUMABLE);
            }
            else
            {
                Debug.Log($"[{Tag}]: Consumable Products Not-Available");
            }

            if (!IsNullOrEmpty(productIdNonConsumablesList))
            {
                Debug.Log($"[{Tag}]: Non-Consumable Products Available");
                GetProduct(new List<string>(productIdNonConsumablesList), PriceType.IN_APP_NONCONSUMABLE);
            }
            else
            {
                Debug.Log($"[{Tag}]: Non-Consumable Products Not-Available");
            }

            if (!IsNullOrEmpty(productIdSubscriptionList))
            {
                Debug.Log($"[{Tag}]: Subscription Products Available");
                GetProduct(new List<string>(productIdSubscriptionList), PriceType.IN_APP_SUBSCRIPTION);
            }
            else
            {
                Debug.Log($"[{Tag}]: Subscription Products Not-Available");
            }
        }

        private void GetProduct(IList<string> productIdList, PriceType priceType)
        {

            if (iapAvailable != true)
            {
                Debug.Log($"[{Tag}]: IAP not available");

                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            ProductInfoReq productInfoReq = new ProductInfoReq
            {
                PriceType = priceType,
                ProductIds = productIdList
            };

            iapClient.ObtainProductInfo(productInfoReq).AddOnSuccessListener((type) =>
            {

                foreach (var productInfo in type.ProductInfoList)
                {
                    if (!productInfoList.Exists(c => c.ProductId == productInfo.ProductId))
                    {
                        productInfoList.Add(productInfo);
                        Debug.Log($"[{Tag}]: Available Product Info - Product Type: {(IAPProductType)priceType.Value} - ProductId: " + productInfo.ProductId + ", ProductName: " + productInfo.ProductName + ", Price: " + productInfo.Price);
                    }
                }

                OnObtainProductInfoSuccess?.Invoke(new List<ProductInfoResult> { type });

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[{Tag}]: Fail Message - Product Type: {(IAPProductType)priceType.Value} failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnObtainProductInfoFailure?.Invoke(exception);
            });
        }

        #endregion

        #region Owned Products

        public void GetAllOwnedPurchases()
        {
            ObtainOwnedPurchases(PriceType.IN_APP_CONSUMABLE);
            ObtainOwnedPurchases(PriceType.IN_APP_NONCONSUMABLE);
            ObtainOwnedPurchases(PriceType.IN_APP_SUBSCRIPTION);
        }

        public void ObtainOwnedPurchases(PriceType priceType)
        {
            if (iapAvailable != true)
            {
                OnObtainOwnedPurchasesFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            ObtainOwnedPurchasesRequest(priceType);
        }

        private void ObtainOwnedPurchasesRequest(PriceType priceType)
        {
            var ownedPurchasesReq = new OwnedPurchasesReq() { PriceType = priceType };

            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

            /*      Queries purchase data of the products that a user has bought.
            For consumables, this method returns purchase data of those already bought but not consumed.
            For non-consumables, this method returns purchase data of all the products that have been bought.
            For subscriptions, this method returns only the currently active subscription relationships.      */

            task.AddOnSuccessListener((result) =>
            {
                OnObtainOwnedPurchasesSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[{Tag}]: Obtain Owned Purchases Request failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnObtainOwnedPurchasesFailure?.Invoke(exception);
            });
        }

        #endregion

        #region PurchaseRecords (Obtains the purchase information of all consumed products or the receipts of all subscriptions.)
        public void GetPurchaseRecords()
        {
            if (iapAvailable != true)
            {
                OnObtainOwnedPurchaseRecordFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            ObtainOwnedPurchaseRecord(PriceType.IN_APP_CONSUMABLE);
            ObtainOwnedPurchaseRecord(PriceType.IN_APP_NONCONSUMABLE);
            ObtainOwnedPurchaseRecord(PriceType.IN_APP_SUBSCRIPTION);
        }

        private void ObtainOwnedPurchaseRecord(PriceType priceType)
        {

            if (iapAvailable != true)
            {
                OnObtainOwnedPurchaseRecordFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            var ownedPurchaseRequest = new OwnedPurchasesReq() { PriceType = priceType };

            ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchaseRecord(ownedPurchaseRequest);

            task.AddOnSuccessListener((result) =>
            {
                OnObtainOwnedPurchaseRecordSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[{Tag}]: ObtainOwnedPurchaseRecord - Fail");

                OnObtainOwnedPurchaseRecordFailure?.Invoke(exception);
            });
        }

        #endregion

        #region Consume

        public void ConsumePurchase(InAppPurchaseData inAppPurchaseData)
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
                Debug.Log($"[{Tag}]: ConsumePurchaseWithToken - Product Id: " + result.ConsumePurchaseData.ProductId);
                OnConsumePurchaseSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[{Tag}]: ConsumePurchaseWithToken - Fail - CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage + ", Message: " + exception.Message + "exception.ErrorCode " + exception.ErrorCode);
                OnConsumePurchaseFailure?.Invoke(exception);
            });
        }

        #endregion

        #region Purchase

        public void PurchaseProduct(string productId)
        {
            Debug.Log($"[{Tag}]: PurchaseProduct");

            var productInfo = GetProductInfo(productId);

            if (productInfo != null)
            {
                PurchaseProductMethod(productInfo);
            }
            else
            {
                Debug.LogError($"[{Tag}]: {productId} could not be found in retrieved product list!");
            }
        }

        private void PurchaseProductMethod(ProductInfo productInfo)
        {
            Debug.Log($"[{Tag}]: PurchaseProductMethod");

            if (iapAvailable != true)
            {
                OnObtainProductInfoFailure?.Invoke(IAP_NOT_AVAILABLE);
                return;
            }

            PurchaseIntentReq purchaseIntentReq = new PurchaseIntentReq
            {
                PriceType = productInfo.PriceType,
                ProductId = productInfo.ProductId,
                DeveloperPayload = string.Empty
            };

            bool isSubscription = (IAPProductType)productInfo.PriceType.Value == IAPProductType.Subscription;
            bool isConsumable = (IAPProductType)productInfo.PriceType.Value == IAPProductType.Consumable;
            bool isNonConsumable = (IAPProductType)productInfo.PriceType.Value == IAPProductType.NonConsumable;

            ITask<PurchaseIntentResult> task = iapClient.CreatePurchaseIntent(purchaseIntentReq);

            task.AddOnSuccessListener((result) =>
            {
                if (result == null)
                    return;

                Debug.Log($"[{Tag}]: PurchaseProduct - Success - ProductID: {purchaseIntentReq.ProductId} - Result: {result.ErrMsg + result.ReturnCode}");

                Status status = result.Status;

                status.StartResolutionForResult((androidIntent) =>
                {
                    PurchaseResultInfo purchaseResultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);

                    if (purchaseResultInfo.ReturnCode == OrderStatusCode.ORDER_STATE_SUCCESS)
                    {
                        Debug.Log($"[{Tag}]: Purchase Result Info - ProductId: {purchaseResultInfo.InAppPurchaseData.ProductId} - InAppDataSignature: {purchaseResultInfo.InAppDataSignature}");

                        OnBuyProductSuccess.Invoke(purchaseResultInfo);

                        if (sandboxState.SandboxUser)
                        {
                            if (isConsumable || isNonConsumable)
                            {
                                ConsumePurchase(purchaseResultInfo.InAppPurchaseData);
                            }
                        }
                        else if (isConsumable)
                        {
                            ConsumePurchase(purchaseResultInfo.InAppPurchaseData);
                        }

                    }
                    else
                    {
                        switch (purchaseResultInfo.ReturnCode)
                        {
                            case OrderStatusCode.ORDER_STATE_CANCEL:
                                Debug.LogError($"[{Tag}]: User Cancel Payment");
                                break;

                            case OrderStatusCode.ORDER_STATE_FAILED:
                                Debug.LogError($"[{Tag}]: Order Payment Failed");
                                break;

                            case OrderStatusCode.ORDER_PRODUCT_OWNED:
                                {
                                    if (isSubscription)
                                    {
                                        Debug.Log($"[{Tag}]: Subscription - PurchaseResultInfo.ReturnCode: {purchaseResultInfo.ReturnCode}");
                                        RedirectingtoSubscriptionEditingScreen(productInfo.ProductId);
                                        return;
                                    }

                                    Debug.LogError($"[{Tag}]: Product Owned");
                                }
                                break;

                            case OrderStatusCode.ORDER_STATE_PARAM_ERROR:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_PARAM_ERROR");
                                break;

                            case OrderStatusCode.ORDER_STATE_IAP_NOT_ACTIVATED:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_IAP_NOT_ACTIVATED");
                                break;

                            case OrderStatusCode.ORDER_STATE_PRODUCT_INVALID:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_PRODUCT_INVALID");
                                break;

                            case OrderStatusCode.ORDER_STATE_CALLS_FREQUENT:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_CALLS_FREQUENT");
                                break;

                            case OrderStatusCode.ORDER_STATE_NET_ERROR:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_NET_ERROR");
                                break;

                            case OrderStatusCode.ORDER_STATE_PMS_TYPE_NOT_MATCH:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_PMS_TYPE_NOT_MATCH");
                                break;

                            case OrderStatusCode.ORDER_STATE_PRODUCT_COUNTRY_NOT_SUPPORTED:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_STATE_PRODUCT_COUNTRY_NOT_SUPPORTED");
                                break;

                            case OrderStatusCode.ORDER_VR_UNINSTALL_ERROR:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_VR_UNINSTALL_ERROR");
                                break;

                            case OrderStatusCode.ORDER_HWID_NOT_LOGIN:
                                Debug.LogError($"[{Tag}] - ReturnCode:   {purchaseResultInfo.ReturnCode}  : ORDER_HWID_NOT_LOGIN");
                                break;

                            case OrderStatusCode.ORDER_PRODUCT_NOT_OWNED:
                                Debug.LogError($"[{Tag}] - ReturnCode:  {purchaseResultInfo.ReturnCode} : ORDER_PRODUCT_NOT_OWNED");
                                break;

                            case OrderStatusCode.ORDER_PRODUCT_CONSUMED:
                                Debug.LogError($"[{Tag}] - ReturnCode:  {purchaseResultInfo.ReturnCode} : ORDER_PRODUCT_CONSUMED");
                                break;

                            case OrderStatusCode.ORDER_ACCOUNT_AREA_NOT_SUPPORTED:
                                Debug.LogError($"[{Tag}] - ReturnCode:  {purchaseResultInfo.ReturnCode} : ORDER_ACCOUNT_AREA_NOT_SUPPORTED");
                                break;

                            case OrderStatusCode.ORDER_NOT_ACCEPT_AGREEMENT:
                                Debug.LogError($"[{Tag}] - ReturnCode:  {purchaseResultInfo.ReturnCode} : ORDER_NOT_ACCEPT_AGREEMENT");
                                break;

                            case OrderStatusCode.ORDER_HIGH_RISK_OPERATIONS:
                                Debug.LogError($"[{Tag}] - ReturnCode: {purchaseResultInfo.ReturnCode}: ORDER_HIGH_RISK_OPERATIONS");
                                break;

                            case OrderStatusCode.ORDER_STATE_PENDING:
                                Debug.LogError($"[{Tag}] - ReturnCode: {purchaseResultInfo.ReturnCode}: ORDER_STATE_PENDING");
                                break;

                            default:
                                Debug.LogError($"[{Tag}]: BuyProduct failed. ReturnCode: " + purchaseResultInfo.ReturnCode + ", ErrorMsg: " + purchaseResultInfo.ErrMsg);
                                break;
                        }
                        OnBuyProductFailure?.Invoke(purchaseResultInfo.ReturnCode);
                    }

                }, (exception) =>
                {
                    Debug.LogError($"[{Tag}]: startIntent ERROR");
                });

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"[{Tag}]: BuyProduct failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
            });
        }

        #endregion

        #region Restore

        public void RestoreOwnedPurchases(Action<OwnedPurchasesResult> action)
        {
            OnObtainOwnedPurchasesSuccess = (ownedPurchaseResult) =>
            {
                action?.Invoke(ownedPurchaseResult);
            };

            OnObtainOwnedPurchasesFailure = (error) =>
            {
                Debug.LogError($"[{Tag}]: RestorePurchasesError failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
            };

            GetAllOwnedPurchases();
        }

        public void RestorePurchaseRecords(Action<OwnedPurchasesResult> action)
        {
            Debug.Log($"[{Tag}]: RestorePurchases");

            OnObtainOwnedPurchaseRecordSuccess = (ownedPurchaseResult) =>
            {
                action?.Invoke(ownedPurchaseResult);
            };

            OnObtainOwnedPurchaseRecordFailure = (error) =>
            {
                Debug.LogError($"[{Tag}]: RestorePurchasesError failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
            };

            GetPurchaseRecords();
        }

        #endregion


        #region Utils

        public bool IsNullOrEmpty(List<string> array)
        {
            return (array == null || array.Count == 0);
        }

        public ProductInfo GetProductInfo(string productID)
        {
            return productInfoList.Find(productInfo => productInfo.ProductId == productID);
        }

        #endregion

        #region Sandbox

        public void GetSandboxState(Action nextPhase)
        {
            if (iapClient != null)
            {
                var task = iapClient.SandboxActivated;

                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log($"[{Tag}]: Sandbox Is Active = {result.SandboxUser}");

                    sandboxState = result;

                    nextPhase?.Invoke();

                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError($"[{Tag}]: IsSandboxActivated Failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                });
            }
            else
            {
                Debug.LogError($"[{Tag}]: Sandbox Activated Failed. IAP is not initialized.");
            }
        }

        #endregion

        #region SubscriptionScreens

        public void RedirectingtoSubscriptionEditingScreen(string SubscribeProductId)
        {

            StartIapActivityReq req = new StartIapActivityReq
            {
                SubscribeProductId = SubscribeProductId,
                Type = StartIapActivityReq.TYPE_SUBSCRIBE_EDIT_ACTIVITY
            };
            var task = iapClient.StartIapActivity(req);
            task.AddOnFailureListener(
                exception =>
                {
                    Debug.LogError($"[{Tag}]: RedirectingtoSubscriptionEditingScreen error" + exception);
                }
                );
            task.AddOnSuccessListener(activity =>
            {
                activity.StartActivity();
            });
        }

        public void RedirectingtoSubscriptionManagementScreen()
        {

            StartIapActivityReq req = new StartIapActivityReq
            {
                Type = StartIapActivityReq.TYPE_SUBSCRIBE_MANAGER_ACTIVITY
            };

            var task = iapClient.StartIapActivity(req);

            task.AddOnSuccessListener(activity =>
            {
                activity.StartActivity();
            });

            task.AddOnFailureListener(
                exception =>
            {
                Debug.LogError($"[{Tag}]: RedirectingtoSubscriptionManagementScreen error" + exception);
            });

        }

        #endregion

    }
}

public enum IAPProductType
{
    Consumable = 0,
    NonConsumable = 1,
    Subscription = 2
}