

#if UNITY_PURCHASING
using System.Collections.ObjectModel;
using System.Collections.Generic;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Linq;
using System.Text;


namespace HmsPlugin
{
    public partial class HuaweiStore : IStore
    {
        static HuaweiStore currentInstance;
        public static HuaweiStore GetInstance()
        {
            if (currentInstance != null) return currentInstance;
            currentInstance = new HuaweiStore();
            return currentInstance;
        }

        IStoreCallback storeEvents;
        void IStore.Initialize(IStoreCallback callback)
        {
            this.storeEvents   = callback;

            this.BaseInit();
            this.CreateClient();
        }

        object locker;
        List<ProductInfo>               productsList;
        Dictionary<string, ProductInfo> productsByID;
        Dictionary<string, InAppPurchaseData> purchasedData;
        void BaseInit()
        {
            this.locker        = new object();
            this.productsList  = new List<ProductInfo>(100);
            this.productsByID  = new Dictionary<string, ProductInfo>(100);
            this.purchasedData = new Dictionary<string, InAppPurchaseData>(50);
        }

        private IIapClient iapClient;
        void CreateClient()
        {
            this.iapClient     = Iap.GetIapClient();
            var moduleInitTask = iapClient.EnvReady;

            moduleInitTask.AddOnSuccessListener(ClientinitSuccess).AddOnFailureListener(ClientInitFailed);
        }


        bool clientInited = false;
        void ClientinitSuccess(EnvReadyResult result)
        {   
            lock (locker)
            {
                this.clientInited = true;
                if (initProductDefinitions != null) LoadComsumableProducts();
            }
        }

        void ClientInitFailed(HMSException exception)
        {   
            this.storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);
        }


        ReadOnlyCollection<ProductDefinition> initProductDefinitions;
        void IStore.RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
            lock(locker)
            {
                initProductDefinitions = products;
                if(clientInited)LoadComsumableProducts();
            }
        }


        void LoadComsumableProducts()
        {
            var consumablesIDs = (from defenition in initProductDefinitions where defenition.type == ProductType.Consumable select defenition.storeSpecificId) .ToList();
            CreateProductRequest(consumablesIDs, HuaweiConstants.IAP.IapType.CONSUMABLE, LoadNonComsumableProducts);
        }

        void LoadNonComsumableProducts()
        {
            var nonConsumablesIDs = (from defenition in initProductDefinitions where defenition.type == ProductType.NonConsumable select defenition.storeSpecificId) .ToList();
            CreateProductRequest(nonConsumablesIDs, HuaweiConstants.IAP.IapType.NON_CONSUMABLE, LoadSubscribeProducts);
        }

        void LoadSubscribeProducts()
        {
            var nonConsumablesIDs = (from defenition in initProductDefinitions where defenition.type == ProductType.Subscription select defenition.storeSpecificId) .ToList();
            CreateProductRequest(nonConsumablesIDs, HuaweiConstants.IAP.IapType.SUBSCRIPTION, LoadOwnedConsumables);
        }

        private void CreateProductRequest(List<string> consumablesIDs, HuaweiConstants.IAP.IapType type, System.Action onSuccess)
        {
            var productsDataRequest        = new ProductInfoReq();
            productsDataRequest.PriceType  = (int)type;
            productsDataRequest.ProductIds = consumablesIDs;

            var task = iapClient.ObtainProductInfo(productsDataRequest);
            task.AddOnFailureListener(GetProductsFailure);
            task.AddOnSuccessListener((result) => { ParseProducts(result, type.ToString()); onSuccess(); });
        }

        void GetProductsFailure(HMSException exception)
        {   
            this.storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);
        }

        void ParseProducts(ProductInfoResult result, string type)
        {
            if (result == null) return;
            if (result.ProductInfoList.Count == 0) return;

            foreach (ProductInfo productInfo in result.ProductInfoList)
            {   
                this.productsList.Add(productInfo);
                this.productsByID.Add(productInfo.ProductId, productInfo);
            }
        }

        void LoadOwnedConsumables()
        {   
            CreateOwnedPerchaseRequest(HuaweiConstants.IAP.IapType.CONSUMABLE, LoadOwnedNonConsumables);
        }

        void LoadOwnedNonConsumables()
        {   
            CreateOwnedPerchaseRequest(HuaweiConstants.IAP.IapType.NON_CONSUMABLE, LoadOwnedSubscribes);
        }

        void LoadOwnedSubscribes()
        {   
            CreateOwnedPerchaseRequest(HuaweiConstants.IAP.IapType.SUBSCRIPTION, ProductsLoaded);
        }

        void CreateOwnedPerchaseRequest(HuaweiConstants.IAP.IapType type, System.Action onSuccess)
        {
            var ownedPurchasesReq        = new OwnedPurchasesReq();
            ownedPurchasesReq.PriceType  = (int)type;

            var task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

            task.AddOnSuccessListener((result) => { ParseOwned(result); onSuccess(); });
        }

        void ParseOwned(OwnedPurchasesResult result)
        {
            if (result == null || result.InAppPurchaseDataList == null) return;

            foreach (var inAppPurchaseData in result.InAppPurchaseDataList)
            {
                this.purchasedData[inAppPurchaseData.ProductId] = inAppPurchaseData;
            }
        }

        void ProductsLoaded()
        {
            var descList = new List<ProductDescription>(this.productsList.Count);

            foreach(var product in this.productsList)
            {
                string priceString;
                float price     = product.MicrosPrice * 0.000001f;

                if (price < 100) priceString = price.ToString("0.00");
                else             priceString = ((int)(price + 0.5f)).ToString();

                var prodMeta = new ProductMetadata(priceString, product.ProductName, product.ProductDesc, product.Currency, (decimal)price);
                ProductDescription prodDesc;

                if(this.purchasedData.TryGetValue(product.ProductId, out var purchaseData))
                {                    
                    prodDesc  = new ProductDescription(product.ProductId, prodMeta, CreateReceipt(purchaseData),purchaseData.OrderID);
                }
                else prodDesc = new ProductDescription(product.ProductId, prodMeta);

                descList.Add(prodDesc);
            }

            this.storeEvents.OnProductsRetrieved(descList);
        }

        string CreateReceipt(InAppPurchaseData purchaseData)
        {
            var sb = new StringBuilder(1024);

            sb.Append('{').Append("\"Store\":\"AppGallery\",\"TransactionID\":\"").Append(purchaseData.OrderID).Append("\", \"Payload\":{ ");
            sb.Append("\"product\":\"").Append(purchaseData.ProductId).Append("\"");
            sb.Append('}');
            sb.Append('}');
            return sb.ToString();

        }


        void IStore.Purchase(ProductDefinition product, string developerPayload)
        {
            if (!productsByID.ContainsKey(product.storeSpecificId))
            {
                storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.id, PurchaseFailureReason.ProductUnavailable, "UnknownProduct"));
                return;
            }

            var productInfo = productsByID[product.storeSpecificId];
            PurchaseIntentReq purchaseIntentReq = new PurchaseIntentReq
            {
                PriceType        = productInfo.PriceType,
                ProductId        = productInfo.ProductId,
                DeveloperPayload = developerPayload
            };

            var task = iapClient.CreatePurchaseIntent(purchaseIntentReq)
                .AddOnSuccessListener((intentResult)=>
                {
                    PurchaseInitentCreated(intentResult, product);
                })
                .AddOnFailureListener((exception) =>
                {
                    storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.id, PurchaseFailureReason.Unknown, exception.Message));
                });
        }

        void PurchaseInitentCreated(PurchaseIntentResult intentResult, ProductDefinition product)
        {
            if(intentResult == null)
            {   
                storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.id, PurchaseFailureReason.Unknown, "IntentIsNull"));
                return;
            }

            var status = intentResult.Status;
            status.StartResolutionForResult((androidIntent) =>
            {   
                PurchaseResultInfo purchaseResultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);


                switch (purchaseResultInfo.ReturnCode)
                {
                    case OrderStatusCode.ORDER_STATE_SUCCESS:
                        this.purchasedData[product.storeSpecificId] = purchaseResultInfo.InAppPurchaseData;
                        storeEvents.OnPurchaseSucceeded(product.storeSpecificId, purchaseResultInfo.InAppDataSignature, purchaseResultInfo.InAppPurchaseData.OrderID );
                        break;

                    case OrderStatusCode.ORDER_PRODUCT_OWNED:
                        storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.DuplicateTransaction, purchaseResultInfo.ErrMsg ));                        
                        break;

                    case OrderStatusCode.ORDER_STATE_CANCEL:
                        storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.UserCancelled, purchaseResultInfo.ErrMsg ));
                        break;

                    default:
                        storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.Unknown, purchaseResultInfo.ErrMsg ));
                        break;
                }
            }, (exception) =>
            {
                storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.id, PurchaseFailureReason.Unknown, exception.Message));
            });

        }


        void IStore.FinishTransaction(ProductDefinition product, string transactionId)
        {
            if(this.purchasedData.TryGetValue(product.storeSpecificId, out var data))
            {
                var token = data.PurchaseToken;
                var request           = new ConsumeOwnedPurchaseReq();
                request.PurchaseToken = token;

                var task = iapClient.ConsumeOwnedPurchase(request);
                task.AddOnSuccessListener((result) =>
                {
                    this.purchasedData.Remove(product.storeSpecificId);
                });

                task.AddOnFailureListener((exception) =>
                {
                    UnityEngine.Debug.Log("Comsume failed " + exception.Message + " " + exception.StackTrace);
                });
            }
        }
        
    }
}

#endif
