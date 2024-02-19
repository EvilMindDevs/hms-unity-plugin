using HmsPlugin;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public GameObject coin100, coin1000, removeAds, premium;
    public Text userTxt, coinTxt;
    public Button backBtn, editSubs, manageSubs;

    string TAG = "[StoreManager]:";

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //Text fields
        userTxt.text = "Hello, " + HMSAccountKitManager.Instance.HuaweiId.DisplayName;
        coinTxt.text = PlayerPrefs.GetInt("Coin", 0).ToString();
        //Button OnClicks
        backBtn.onClick.AddListener(delegate { SceneManager.LoadScene("Scene0_MainMenu"); });

        editSubs.onClick.AddListener(delegate
        {
            //You can redirect your user to subscription editing screen for your specific Product.
            HMSIAPManager.Instance.RedirectingtoSubscriptionEditingScreen("premium");
        });

        manageSubs.onClick.AddListener(delegate
        {
            //You can redirect your user to subscriptions management page.
            HMSIAPManager.Instance.RedirectingtoSubscriptionManagementScreen();
        });

        HMSIAPManager.Instance.OnBuyProductSuccess = OnBuyProductSuccess;
        HMSIAPManager.Instance.OnBuyProductFailure = OnBuyProductFailure;
        FillProducts();
    }

    private void OnBuyProductSuccess(PurchaseResultInfo purchaseResultInfo)
    {
        if (purchaseResultInfo.InAppPurchaseData.ProductId == "coin100")
        {
            AndroidToast.MakeText("You purchased 100 coin.").Show();
            if (int.TryParse(coinTxt.text, out int coin))
            {
                coin += 100;
                coinTxt.text = coin.ToString();
                PlayerPrefs.SetInt("Coin", coin);
            }
            else
            {
                Debug.LogError($"[{TAG}]: Failed to parse coinTxt.text");
            }
        }
        /*Other products
         *
         * ...
         *
         */
        //If you want to disable the buy button by typing you own this product after the purchase is successful, you can change the UI here. (Non-Consumable & Subscription)
        GameObject panel = (purchaseResultInfo.InAppPurchaseData.ProductId == "removeAds") ? removeAds :
            (purchaseResultInfo.InAppPurchaseData.ProductId == "premium") ? premium : null;

        if (panel != null)
        {
            panel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "You just bought";
            panel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }
    }

    private void OnBuyProductFailure(int errorCode)
    {
        Debug.LogError($"[{TAG}]: PurchaseProduct failed. errorCode: " + errorCode);
        switch (errorCode)
        {
            case OrderStatusCode.ORDER_STATE_CANCEL:
                Debug.LogError($"[{TAG}]: User Cancel Payment");
                AndroidToast.MakeText("Payment canceled.").Show();
                break;

            case OrderStatusCode.ORDER_STATE_FAILED:
                Debug.LogError($"[{TAG}]: Order Payment Failed");
                AndroidToast.MakeText("Payment failed.").Show();
                break;
            default:
                AndroidToast.MakeText("Purchase failed.").Show();
                break;
        }
    }

    private void FillProducts()
    {
        Debug.Log($"[{TAG}]: FillProducts list.count:" + HMSIAPManager.Instance.GetProductsList().Count);

        foreach (ProductInfo pinfo in HMSIAPManager.Instance.GetProductsList())
        {
            GameObject panel;
            //If you want to create constants, you can use IAP tab from Huawei>Kit Settings>IAP Then fill with your products and click "create constant classes". (Coin100 is an example of using constant class)
            if (pinfo.ProductId.Equals("coin100"))//or HMSIAPConstants.coin100))
                panel = coin100;

            else if (pinfo.ProductId == "coin1000")
                panel = coin1000;

            else if (pinfo.ProductId == "removeAds")
                panel = removeAds;

            else if (pinfo.ProductId == "premium")
                panel = premium;

            else
                panel = null;

            if (panel != null)
            {
                foreach (Text textComponent in panel.GetComponentsInChildren<Text>())
                {
                    if (textComponent.name == "ItemName")
                        textComponent.text = pinfo.ProductName;
                    else if (textComponent.name == "ItemDesc")
                        textComponent.text = pinfo.ProductDesc;
                    else if (textComponent.name == "ItemCost")
                        textComponent.text = pinfo.Price;
                }

                if (HMSIAPManager.Instance.isUserOwnThisProduct(pinfo.ProductId))
                {
                    panel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "You own this";
                }
                else
                {
                    panel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "BUY";
                    panel.GetComponentInChildren<Button>().onClick.AddListener(delegate { HMSIAPManager.Instance.PurchaseProduct(pinfo.ProductId); });
                }
            }
            else
            {
                Debug.LogWarning($"[{TAG}]: FillProducts panel is null");
            }
        }
    }
}
