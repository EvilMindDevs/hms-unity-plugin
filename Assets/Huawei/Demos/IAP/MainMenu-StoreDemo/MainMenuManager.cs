using HmsPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    Button playBtn, storeBtn, signinBtn;
    Text userTxt, coinTxt;
    void Start()
    {
        playBtn = GameObject.Find("Play").GetComponent<Button>();
        storeBtn = GameObject.Find("Store").GetComponent<Button>();
        signinBtn = GameObject.Find("Login").GetComponent<Button>();
        userTxt = GameObject.Find("userText").GetComponent<Text>();
        coinTxt = GameObject.Find("coinText").GetComponent<Text>();

        if (HMSAccountKitManager.Instance.IsSignedIn) 
        {
            Debug.Log("UserSignIn" + HMSAccountKitManager.Instance.HuaweiId.DisplayName);
        }
        else 
        {
            Debug.Log("User Not SignIn");
        }

        signinBtn.onClick.AddListener(delegate { ClickListener(CLICKENUM.SIGNIN); });

        //HMSIAPManager.Instance.


    }

    private void ClickListener(CLICKENUM enumValue)
    {
        if(enumValue == CLICKENUM.SIGNIN) 
        {
            HMSAccountKitManager.Instance.SignIn();
        }
    }

    enum CLICKENUM 
    {
        SIGNIN
    }
}
