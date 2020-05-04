//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ShopManager : MonoBehaviour
//{
//    private GameObject shopItemsGO;

//    private IapManager iapManager;

//    //UI
//    Text coinsText;
//    Text userText;
//    Button loginButton;

//    //// Consumables
//    int coins;

//    //Non consumables
//    bool ads;
//    bool premium;

//    bool logged;

//    string user;

//    ShopItem[] shopItems;


//    void Start()
//    {
//        iapManager = GameObject.Find("IapManager").GetComponent<IapManager>();

//        if (iapManager != null)
//        {
//            Debug.Log("[HMSPlugin]: adding listener ");
//            iapManager.addListener(activateProducts);
//        }
//        else
//        {
//            Debug.Log("[HMSPlugin]: NO IAP MANAGER IN SHOP");
//        }

//        coinsText = GameObject.Find("CoinsText").GetComponent<Text>();
//        userText = GameObject.Find("UserText").GetComponent<Text>();
//        loginButton = GameObject.Find("LoginButton").GetComponent<Button>();
//        shopItemsGO = GameObject.Find("ShopItems");


//        if (!logged)
//        {
//            userText.text = "not logged";
//        }



//    }


//    void activateProducts()
//    {
//        Debug.Log("[HMSPlugin]: Loading products ");
//        shopItems = shopItemsGO.GetComponentsInChildren<ShopItem>();
//        foreach (ShopItem item in shopItems)
//        {
//            item.Load();
//        }
//    }

//    public void addCoins(int amount)
//    {
//        coins += amount;
//        coinsText.text = coins.ToString();
//    }

//    //public void setNoAds(bool purchased)
//    //{
//    //    ads = purchased;

//    //    NonConsumableButton[] buttons = transform.GetComponentsInChildren<NonConsumableButton>();
//    //    foreach (NonConsumableButton button in buttons)
//    //    {
//    //        button.alreadyBought("no_ads");
//    //    }
//    //}

//    //public void setPremium(bool purchased)
//    //{
//    //    premium = purchased;

//    //    NonConsumableButton[] buttons = transform.GetComponentsInChildren<NonConsumableButton>();
//    //    foreach (NonConsumableButton button in buttons)
//    //    {
//    //        button.alreadyBought("premium");
//    //    }
//    //}



//    public void setUser(string name)
//    {
//        userText.text = name;
//        logged = true;
//        loginButton.interactable = false;
//    }
//}
