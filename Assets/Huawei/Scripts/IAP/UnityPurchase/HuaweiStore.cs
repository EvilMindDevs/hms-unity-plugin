#if UNITY_PURCHASING

using System.Collections.ObjectModel;
using System.Collections.Generic;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HmsPlugin
{
    public class HuaweiStore : IStore
    {
        static HuaweiStore currentInstance;

        public static string PurchaseTokenOfLastPurchase = null;

        public static HuaweiStore GetInstance()
        {
            if (currentInstance != null) return currentInstance;
            currentInstance = new HuaweiStore();
            return currentInstance;
        }

        IStoreCallback storeEvents;
        void IStore.Initialize(IStoreCallback callback)
        {
            this.storeEvents = callback;

            this.BaseInit();
            this.CreateClient();
        }

        object locker;
        List<ProductInfo> productsList;
        Dictionary<string, ProductInfo> productsByID;
        Dictionary<string, InAppPurchaseData> purchasedData;
        public static ProductDefinition currentProduct;

        public void BaseInit()
        {
            this.locker = new object();
            this.productsList = new List<ProductInfo>(100);
            this.productsByID = new Dictionary<string, ProductInfo>(100);
            this.purchasedData = new Dictionary<string, InAppPurchaseData>(50);
        }

        private IIapClient iapClient;
        public void CreateClient()
        {
            this.iapClient = Iap.GetIapClient();
            Debug.Log("[HuaweiStore] IAP Client Created");
            var moduleInitTask = iapClient.EnvReady;

            moduleInitTask.AddOnSuccessListener(ClientinitSuccess).AddOnFailureListener(ClientInitFailed);
        }


        bool clientInited = false;
        public void ClientinitSuccess(EnvReadyResult result)
        {
            Debug.Log("[HuaweiStore] IAP Client Success");
            lock (locker)
            {
                this.clientInited = true;
                if (initProductDefinitions != null) LoadComsumableProducts(productList);
            }
        }

        public void ClientInitFailed(HMSException exception)
        {
            Debug.LogError("[HuaweiStore]: ERROR on ClientInitFailed: " + exception.WrappedCauseMessage + " " + exception.WrappedExceptionMessage);
            this.storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);
        }


        ReadOnlyCollection<ProductDefinition> initProductDefinitions;
        void IStore.RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
            lock (locker)
            {
                Debug.Log("[HuaweiStore] IAP RetrieveProducts");
                foreach (var item in products)
                {
                    Debug.Log($"[HuaweiStore] Product Id: {item.id} ");
                    productList.Add(item);
                }
                initProductDefinitions = products;

                if (clientInited) LoadComsumableProducts(productList);
            }
        }

        public List<ProductDefinition> productList = new List<ProductDefinition>();

        public void LoadComsumableProducts(List<ProductDefinition> list)
        {
            productList = list;
            var consumablesIDs = list.Where(c => c.type == ProductType.Consumable).Select(c => c.storeSpecificId).ToList();
            CreateProductRequest(consumablesIDs, PriceType.IN_APP_CONSUMABLE, LoadNonComsumableProducts);

        }

        public void LoadNonComsumableProducts()
        {
            var nonConsumablesIDs = productList.Where(c => c.type == ProductType.NonConsumable).Select(c => c.storeSpecificId).ToList();
            if (nonConsumablesIDs.Count > 0)
                CreateProductRequest(nonConsumablesIDs, PriceType.IN_APP_NONCONSUMABLE, LoadSubscribeProducts);
            else
                LoadSubscribeProducts();
        }

        public void LoadSubscribeProducts()
        {
            var subscribeIDs = productList.Where(c => c.type == ProductType.Subscription).Select(c => c.storeSpecificId).ToList();
            if (subscribeIDs.Count > 0)
                CreateProductRequest(subscribeIDs, PriceType.IN_APP_SUBSCRIPTION, ProductsLoaded);
            else
                ProductsLoaded();
        }

        public void CreateProductRequest(List<string> consumablesIDs, PriceType type, System.Action onSuccess)
        {
            var productsDataRequest = new ProductInfoReq();
            productsDataRequest.PriceType = type;
            productsDataRequest.ProductIds = consumablesIDs;

            var task = iapClient.ObtainProductInfo(productsDataRequest);
            task.AddOnFailureListener(GetProductsFailure);
            task.AddOnSuccessListener((result) => { ParseProducts(result, type); onSuccess(); });
        }

        public void GetProductsFailure(HMSException exception)
        {
            Debug.LogError("[HuaweiStore]: ERROR on GetProductsFailure: " + exception.WrappedCauseMessage + " " + exception.WrappedExceptionMessage);
            this.storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);
        }

        public void ParseProducts(ProductInfoResult result, PriceType type)
        {
            if (result == null) return;
            if (result.ProductInfoList.Count == 0) return;

            foreach (var item in result.ProductInfoList)
            {
                Debug.Log($"[HuaweiStore]  Huawei Product Id: {item.ProductId}");
            }

            foreach (ProductInfo productInfo in result.ProductInfoList)
            {
                this.productsList.Add(productInfo);
                this.productsByID.Add(productInfo.ProductId, productInfo);
            }
        }

        public void LoadOwnedConsumables()
        {
            CreateOwnedPurchaseRequest(PriceType.IN_APP_CONSUMABLE, LoadOwnedNonConsumables);
        }

        public void LoadOwnedNonConsumables()
        {
            CreateOwnedPurchaseRequest(PriceType.IN_APP_NONCONSUMABLE, LoadOwnedSubscribes);
        }

        public void LoadOwnedSubscribes()
        {
            CreateOwnedPurchaseRequest(PriceType.IN_APP_SUBSCRIPTION, ProductsLoaded);
        }

        public void CreateOwnedPurchaseRequest(PriceType type, System.Action onSuccess)
        {
            var ownedPurchasesReq = new OwnedPurchasesReq();
            ownedPurchasesReq.PriceType = type;

            var task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

            task.AddOnSuccessListener((result) => { ParseOwned(result); onSuccess(); });
        }

        public void ParseOwned(OwnedPurchasesResult result)
        {
            if (result == null || result.InAppPurchaseDataList == null) return;

            foreach (var inAppPurchaseData in result.InAppPurchaseDataList)
            {
                this.purchasedData[inAppPurchaseData.ProductId] = inAppPurchaseData;

                Debug.Log($"ProductId: " + inAppPurchaseData.ProductId + " , ProductName: " + inAppPurchaseData.ProductName);
            }
        }

        public void ProductsLoaded()
        {
            Debug.Log("ProductsLoaded");

            var descList = new List<ProductDescription>(this.productsList.Count);

            foreach (var product in this.productsList)
            {
                string priceString;
                float price = product.MicrosPrice * 0.000001f;

                if (price < 100) priceString = price.ToString("0.00");
                else priceString = ((int)(price + 0.5f)).ToString();

                priceString = product.Currency + " " + priceString;
                var prodMeta = new ProductMetadata(priceString, product.ProductName, product.ProductDesc, product.Currency, (decimal)price);
                ProductDescription prodDesc;

                if (this.purchasedData.TryGetValue(product.ProductId, out var purchaseData))
                {
                    prodDesc = new ProductDescription(product.ProductId, prodMeta, CreateReceipt(purchaseData), purchaseData.OrderID);
                }
                else prodDesc = new ProductDescription(product.ProductId, prodMeta);

                descList.Add(prodDesc);
            }

            this.storeEvents.OnProductsRetrieved(descList);
        }

        public string CreateReceipt(InAppPurchaseData purchaseData)
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
                PriceType = productInfo.PriceType,
                ProductId = productInfo.ProductId,
                DeveloperPayload = developerPayload
            };

            var task = iapClient.CreatePurchaseIntent(purchaseIntentReq)
                .AddOnSuccessListener((intentResult) =>
                {
                    PurchaseIntentCreated(intentResult, product);
                })
                .AddOnFailureListener((exception) =>
                {
                    storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.id, PurchaseFailureReason.Unknown, exception.Message));
                });
        }


        public void PurchaseIntentCreated(PurchaseIntentResult intentResult, ProductDefinition product)
        {
            if (intentResult == null)
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

                        Debug.Log($"token {purchaseResultInfo.InAppPurchaseData.PurchaseToken}");
                        PurchaseTokenOfLastPurchase = purchaseResultInfo.InAppPurchaseData.PurchaseToken;

                        currentProduct = product;
                        storeEvents.OnPurchaseSucceeded(product.storeSpecificId, purchaseResultInfo.InAppDataSignature, purchaseResultInfo.InAppPurchaseData.OrderID);
                        break;

                    case OrderStatusCode.ORDER_PRODUCT_OWNED:
                        storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.DuplicateTransaction, purchaseResultInfo.ErrMsg));
                        break;

                    case OrderStatusCode.ORDER_STATE_CANCEL:
                        storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.UserCancelled, purchaseResultInfo.ErrMsg));
                        break;

                    default:
                        storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.Unknown, purchaseResultInfo.ErrMsg));
                        break;
                }
            }, (exception) =>
            {
                storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.id, PurchaseFailureReason.Unknown, exception.Message));
            });

        }


        void IStore.FinishTransaction(ProductDefinition product, string transactionId)
        {
            if (this.purchasedData.TryGetValue(product.storeSpecificId, out var data))
            {
                var token = data.PurchaseToken;
                var request = new ConsumeOwnedPurchaseReq();
                request.PurchaseToken = token;

                var task = iapClient.ConsumeOwnedPurchase(request);
                task.AddOnSuccessListener((result) =>
                {
                    this.purchasedData.Remove(product.storeSpecificId);
                });

                task.AddOnFailureListener((exception) =>
                {
                    Debug.Log("Consume failed " + exception.Message + " " + exception.StackTrace);
                });
            }
        }

    }
}

#endif
