using HmsPlugin;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*  You can find detailed explanation with medium article: https://medium.com/huawei-developers/guide-to-using-and-understanding-huawei-inapppurchases-demo-scene-in-unity-e41626d5b47a
*   This demo scene has been created to show how to use IAP and Account kit in multiple scenes.
*   Here are the steps to use this demo.
*       1. Complete the quick start steps. https://evilminddevs.gitbook.io/hms-unity-plugin/getting-started/quick-start
*       2. Enable Account kit - IAP from kit settings. https://evilminddevs.gitbook.io/hms-unity-plugin/getting-started/quick-start/connect-your-game-with-the-hms-kit-managers
*       3. Make sure you enabled IAP and Account kit services in the AppGallery console. https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/config-agc-0000001050033072#section382112213818
*       4. Add These products to your project. (with same productID) https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/config-product-0000001050033076
*           a. Product ID: coin100   - Type: consumable
*           b. Product ID: coin1000  - Type: consumable
*           c. Product ID: removeAds - Type: non-consumable
*           d. Product ID: premium   - Type: subscription (1 week)
*       5. Add these products to IAP tab. Unity > Huawei > Kit Settings > IAP. Then click create constant classes
*
*       Have a question? You can ask by creating an issue in our project on Github or via our discord channel.
*/


public class MainMenuManager : MonoBehaviour
{
    string TAG = "[MainMenuManager]:";

    Button playBtn, storeBtn, signinBtn;
    Text userTxt, coinTxt;
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        playBtn = GameObject.Find("Play").GetComponent<Button>();
        storeBtn = GameObject.Find("Store").GetComponent<Button>();
        signinBtn = GameObject.Find("Login").GetComponent<Button>();
        userTxt = GameObject.Find("userText").GetComponent<Text>();
        coinTxt = GameObject.Find("coinText").GetComponent<Text>();

        coinTxt.text = "Coin: " + PlayerPrefs.GetInt("Coin", 0);

        //Note: Enable Store button when InitializeIAPSuccess
        storeBtn.enabled = false;

        playBtn.onClick.AddListener(delegate { ClickListener(CLICKENUM.ELSE); });
        signinBtn.onClick.AddListener(delegate { ClickListener(CLICKENUM.SIGNIN); });
        storeBtn.onClick.AddListener(delegate { ClickListener(CLICKENUM.STORE); });

        HMSAccountKitManager.Instance.OnSignInSuccess = SignInSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = SignInFailed;
        //If the user is already logged in to the AppGallery store, it is possible to log in without disturbing the user with silent login.
        //But if not, the 2002 error can be return.
        //Also check for IsSignedIn. This will prevent your user from logging in again and again when they return to the main menu.
        if (HMSAccountKitManager.Instance.IsSignedIn)
            SignInSuccess(HMSAccountKitManager.Instance.HuaweiId);
        else
            HMSAccountKitManager.Instance.SilentSignIn();

        //When user back to mainMenu, this can work for subscriptions & non_consumable products.
        if (HMSIAPManager.Instance.isIapAvailable())
        {
            if (HMSIAPManager.Instance.isUserOwnThisProduct("premium"))
            {
                //unlock premium features
                AndroidToast.MakeText("You are premium now.").Show();
                Debug.Log($"{TAG}OnObtainOwnedPurchasesSuccess. You have premium.");
            }
        }
    }

    private void SignInFailed(HMSException exception)
    {
        Debug.LogError($"{TAG}User SignInFailed. HMSException:{exception}");
        if (exception.ErrorCode == 2002)
            HMSAccountKitManager.Instance.SignIn();
    }

    private void SignInSuccess(AuthAccount authAccount)
    {
        Debug.Log($"{TAG}User SignInSuccess DisplayName{authAccount.DisplayName}");
        signinBtn.onClick.RemoveAllListeners();
        signinBtn.onClick.AddListener(delegate { ClickListener(CLICKENUM.SIGNOUT); });
        signinBtn.gameObject.GetComponentInChildren<Text>().text = "Logout";
        userTxt.text = "Welcome back\n" + authAccount.DisplayName;

        //Init IAP after OnSignInSuccess
        InitIAP();
    }
    private void InitIAP()
    {
        HMSIAPManager.Instance.OnInitializeIAPSuccess = OnInitializeIAPSuccess;
        HMSIAPManager.Instance.OnObtainOwnedPurchasesSuccess = OnObtainOwnedPurchasesSuccess;

        //I don't need productInfos such as price, name, etc. in this scene. If you need productsInfos, you can remove comment from following code.
        //HMSIAPManager.Instance.OnObtainProductInfoSuccess = OnObtainProductInfoSuccess;

        /*
        * Important: To use this demo please uncheck Init on start box in IAP tab. (Unity>Huawei>Kit Settings>IAP) The line below does the same.
        */
        HMSIAPManager.Instance.InitializeIAP();
    }

    private void OnObtainProductInfoSuccess(IList<ProductInfoResult> list)
    {
        Debug.Log($"{TAG}OnObtainProductInfoSuccess:{list.Count}");
    }

    private void OnInitializeIAPSuccess()
    {
        //Enable Store button after Init success
        storeBtn.enabled = true;
        Debug.Log($"{TAG}OnInitializeIAPSuccess");
    }

    private void OnObtainOwnedPurchasesSuccess(OwnedPurchasesResult result)
    {
        Debug.Log($"{TAG}OnObtainOwnedPurchasesSuccess:{result.ItemList.Count}");
        foreach (InAppPurchaseData product in result.InAppPurchaseDataList)
        {
            //ConsumptionState:
            //    0: not consumed
            //    1: consumed
            //Kind:
            //    0: consumable
            //    1: non - consumable
            //    2: subscription
            // https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/inapppurchasedata-0000001050137635#section1031573764815
            if (product.ConsumptionState == 0 && product.Kind == 0)
            {
                // not consumed means that there was a problem in the presentation of the product after the purchase process and the product could not be consumed.
                // This part is mandatory for consumable products.
                ConsumeProduct(product);
            }
            if (product.ProductId == "removeAds")
            {
                //removeAds
                AndroidToast.MakeText("You have removeAds.").Show();
                Debug.Log($"{TAG}OnObtainOwnedPurchasesSuccess. You have removeAds.");
            }
            else if (product.ProductId == "premium")
            {
                //unlock premium features
                AndroidToast.MakeText("You are premium now.").Show();
                Debug.Log($"{TAG}OnObtainOwnedPurchasesSuccess. You have premium.");
            }
        }
    }

    private void ConsumeProduct(InAppPurchaseData product)
    {
        Debug.Log($"{TAG} ConsumeProduct product:{product.ProductId}");
        //Product not consumed so it did not given to your user. Make sure they receive the product.
        if (product.ProductId == "coin100")
        {
            int coin = PlayerPrefs.GetInt("Coin", 0) + 100;
            coinTxt.text = "Coin: " + coin;
            PlayerPrefs.SetInt("Coin", coin);
        }
        //Then consume non consumed product.
        HMSIAPManager.Instance.ConsumePurchase(product);
    }

    private void ClickListener(CLICKENUM enumValue)
    {
        if (enumValue == CLICKENUM.SIGNIN)
        {
            HMSAccountKitManager.Instance.SignIn();
        }
        else if (enumValue == CLICKENUM.SIGNOUT)
        {
            HMSAccountKitManager.Instance.SignOut();
            userTxt.text = "User not Logged in";
            signinBtn.gameObject.GetComponentInChildren<Text>().text = "Login";
            signinBtn.onClick.RemoveAllListeners();
            signinBtn.onClick.AddListener(delegate { ClickListener(CLICKENUM.SIGNIN); });

            //Disable store function after signout.
            storeBtn.enabled = false;
        }
        else if (enumValue == CLICKENUM.STORE)
        {
            SceneManager.LoadScene("Scene1_Store");
        }
        else
        {
            AndroidToast.MakeText("This button has no function as this is a demo scene.").Show();
        }
    }

    enum CLICKENUM
    {
        SIGNIN,
        SIGNOUT,
        ELSE,
        STORE
    }
}
