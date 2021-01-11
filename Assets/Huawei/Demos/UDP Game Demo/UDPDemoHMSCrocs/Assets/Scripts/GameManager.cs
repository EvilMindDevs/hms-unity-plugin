using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UDP;

public class GameManager : MonoBehaviour
{

    InitListener initListener;
    private const string AdFreeIAPID = "ad_free";


    public void PlayGame ()
    {
        SceneManager.LoadScene("Game");
    }

    public void RemoveAds ()
    {
        initListener = new InitListener();
        StoreService.Initialize(initListener);
    }

    private class InitListener : IInitListener
    {

        PurchaseListener purchaseListener;

        public void OnInitialized(UserInfo userInfo)
        {
            Debug.Log("OnInitialized");
            purchaseListener = new PurchaseListener();
            StoreService.QueryInventory(purchaseListener);
            // EDIT StoreService.Purchase()
            // Purchase(<IAP product ID>, <Payload message>, <PurchaseListener instance>)
            StoreService.Purchase(AdFreeIAPID, "Information from buying user", purchaseListener);
        }

        public void OnInitializeFailed(string message)
        {
            throw new System.NotImplementedException();
        }
    }

    private class PurchaseListener : IPurchaseListener
    {
        public void OnPurchase(PurchaseInfo purchaseInfo)
        {
            Debug.Log("Purchased! prodId: " + purchaseInfo.ProductId);
        }

        public void OnPurchaseConsume(PurchaseInfo purchaseInfo)
        {
            Debug.Log("OnPurchaseConsume prodId: " + purchaseInfo.ProductId);
        }

        public void OnPurchaseConsumeFailed(string message, PurchaseInfo purchaseInfo)
        {
            Debug.Log("OnPurchaseConsumeFailed message: " + message + ", prodId: " + purchaseInfo.ProductId);
        }

        public void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo)
        {
            Debug.Log("OnPurchaseFailed  message: " + message + ", prodId: " + purchaseInfo.ProductId);
        }

        public void OnPurchaseRepeated(string productId)
        {
            Debug.Log("OnPurchaseRepeated prodId: " + productId);
        }

        public void OnQueryInventory(Inventory inventory)
        {
            Debug.Log("OnQueryInventory isInventoryEmpty: " + inventory.Equals(null));
        }

        public void OnQueryInventoryFailed(string message)
        {
            Debug.Log("OnQueryInventoryFailed message: " + message);
        }
    }
}
