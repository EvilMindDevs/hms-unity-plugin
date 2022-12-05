

using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif
using UnityEngine.UI;




public class UnityIAPDemoUIView : MonoBehaviour
{
#if UNITY_PURCHASING

    private Button Btn_ItemCoins100;
    private Button Btn_ItemCoins1000;
    private Button Btn_ItemRemoveAds;
    private Button Btn_ItemPremium;
    private Button Btn_SignIn;

    private Text Txt_Log;

    #region Monobehaviour


    private void Awake()
    {
        Btn_ItemCoins100 = GameObject.Find("ItemBuyButtonC100").GetComponent<Button>();
        Btn_ItemCoins1000 = GameObject.Find("ItemBuyButtonC1000").GetComponent<Button>();
        Btn_ItemRemoveAds = GameObject.Find("ItemBuyButtonRemoveAds").GetComponent<Button>();
        Btn_ItemPremium = GameObject.Find("ItemBuyButtonPremium").GetComponent<Button>();
        Btn_SignIn = GameObject.Find("LoginButton").GetComponent<Button>();

        Txt_Log = GameObject.Find("StatusText").GetComponent<Text>();
    }

    private void OnEnable()
    {
        Btn_ItemCoins100.onClick.AddListener(ButtonClick_BuyItemCoins100);
        Btn_ItemCoins1000.onClick.AddListener(ButtonClick_BuyItemCoins1000);
        Btn_ItemRemoveAds.onClick.AddListener(ButtonClick_BuyItemRemoveAds);
        Btn_ItemPremium.onClick.AddListener(ButtonClick_BuyItemPremium);
        Btn_SignIn.onClick.AddListener(ButtonClick_BuySignIn);

        UnityIapDemoManager.IAPLog += OnIAPLog;
    }

    private void OnDisable()
    {
        Btn_ItemCoins100.onClick.RemoveListener(ButtonClick_BuyItemCoins100);
        Btn_ItemCoins1000.onClick.RemoveListener(ButtonClick_BuyItemCoins1000);
        Btn_ItemRemoveAds.onClick.RemoveListener(ButtonClick_BuyItemRemoveAds);
        Btn_ItemPremium.onClick.RemoveListener(ButtonClick_BuyItemPremium);
        Btn_SignIn.onClick.RemoveListener(ButtonClick_BuySignIn);

        UnityIapDemoManager.IAPLog -= OnIAPLog;
    }

    #endregion

    #region Callbacks

    private void OnIAPLog(string log)
    {
        Txt_Log.text = log;
    }

    #endregion

    #region Button Events

    private void ButtonClick_BuyItemCoins100()
    {
        UnityIapDemoManager.Instance.BuyProduct("coins100", ProductType.Consumable);
    }

    private void ButtonClick_BuyItemCoins1000()
    {
        UnityIapDemoManager.Instance.BuyProduct("coins1000", ProductType.Consumable);
    }

    private void ButtonClick_BuyItemRemoveAds()
    {
        UnityIapDemoManager.Instance.BuyProduct("removeAds", ProductType.NonConsumable);
    }

    private void ButtonClick_BuyItemPremium()
    {
        UnityIapDemoManager.Instance.BuyProduct("premium", ProductType.Subscription);
    }

    private void ButtonClick_BuySignIn()
    {
        //UnityIapDemoManager.Instance.SignIn();
    }

    #endregion
#endif
}

