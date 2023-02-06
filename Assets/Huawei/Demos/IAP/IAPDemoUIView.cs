
using HmsPlugin;

using UnityEngine;
using UnityEngine.UI;

public class IAPDemoUIView : MonoBehaviour
{

    private Button Btn_ItemCoins100;
    private Button Btn_ItemCoins1000;
    private Button Btn_ItemRemoveAds;
    private Button Btn_ItemPremium;
    private Button Btn_Init;
    private Button Btn_ManageSubscriptions;
    private Button Btn_EditSubscriptions;

    private Text Txt_Log;

    #region Monobehaviour

    private void Awake()
    {
        Btn_ItemCoins100 = GameObject.Find("ItemBuyButtonC100").GetComponent<Button>();
        Btn_ItemCoins1000 = GameObject.Find("ItemBuyButtonC1000").GetComponent<Button>();
        Btn_ItemRemoveAds = GameObject.Find("ItemBuyButtonRemoveAds").GetComponent<Button>();
        Btn_ItemPremium = GameObject.Find("ItemBuyButtonPremium").GetComponent<Button>();
        Btn_Init = GameObject.Find("InitButton").GetComponent<Button>();

        Btn_ManageSubscriptions = GameObject.Find("ManageSubscription").GetComponent<Button>();
        Btn_EditSubscriptions = GameObject.Find("EditSubscription").GetComponent<Button>();

        Txt_Log = GameObject.Find("StatusText").GetComponent<Text>();
    }

    private void OnEnable()
    {
        Btn_ItemCoins100.onClick.AddListener(ButtonClick_BuyItemCoins100);
        Btn_ItemCoins1000.onClick.AddListener(ButtonClick_BuyItemCoins1000);
        Btn_ItemRemoveAds.onClick.AddListener(ButtonClick_BuyItemRemoveAds);
        Btn_ItemPremium.onClick.AddListener(ButtonClick_BuyItemPremium);
        Btn_Init.onClick.AddListener(ButtonClick_InitializeIAP);

        Btn_ManageSubscriptions.onClick.AddListener(OpenSubscriptionManagementScreen);
        Btn_EditSubscriptions.onClick.AddListener(OpenSubscriptionEditingScreen);

        IapDemoManager.IAPLog += OnIAPLog;
    }

    private void OnDisable()
    {
        Btn_ItemCoins100.onClick.RemoveListener(ButtonClick_BuyItemCoins100);
        Btn_ItemCoins1000.onClick.RemoveListener(ButtonClick_BuyItemCoins1000);
        Btn_ItemRemoveAds.onClick.RemoveListener(ButtonClick_BuyItemRemoveAds);
        Btn_ItemPremium.onClick.RemoveListener(ButtonClick_BuyItemPremium);
        Btn_Init.onClick.RemoveListener(ButtonClick_InitializeIAP);

        Btn_ManageSubscriptions.onClick.RemoveListener(OpenSubscriptionManagementScreen);
        Btn_EditSubscriptions.onClick.RemoveListener(OpenSubscriptionEditingScreen);

        IapDemoManager.IAPLog -= OnIAPLog;
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
        IapDemoManager.Instance.PurchaseProduct("coins100");
    }

    private void ButtonClick_BuyItemCoins1000()
    {
        IapDemoManager.Instance.PurchaseProduct("coins1000");
    }

    private void ButtonClick_BuyItemRemoveAds()
    {
        IapDemoManager.Instance.PurchaseProduct("removeAds");
    }

    private void ButtonClick_BuyItemPremium()
    {
        IapDemoManager.Instance.PurchaseProduct("premium");
    }

    private void ButtonClick_InitializeIAP()
    {
        IapDemoManager.Instance.InitializeIAP();
    }

    private void OpenSubscriptionEditingScreen()
    {
        HMSIAPManager.Instance.RedirectingtoSubscriptionEditingScreen("premium");
    }

    private void OpenSubscriptionManagementScreen()
    {
        HMSIAPManager.Instance.RedirectingtoSubscriptionManagementScreen();
    }

    #endregion

}
