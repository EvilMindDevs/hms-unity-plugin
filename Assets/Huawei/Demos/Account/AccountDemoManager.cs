using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;

public class AccountDemoManager : MonoBehaviour
{

    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGIN_ERROR = "Error or cancelled login";

    private Text loggedInUser;
    private AccountManager accountManager;

    // Start is called before the first frame update
    void Start()
    {
        loggedInUser = GameObject.Find("LoggedUserText").GetComponent<Text>();
        loggedInUser.text = NOT_LOGGED_IN;
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
        accountManager.LoginSuccess = (authId) => { OnLoginSuccess(authId); };
        accountManager.LoginFailed = (error) => { OnLoginFailure(error); };
    }

    public void LogIn()
    {
        accountManager.LogIn();
    }

    public void LogOut()
    {
        accountManager.LogOut();
        loggedInUser.text = NOT_LOGGED_IN;
    }

    public void OnLoginSuccess(AuthHuaweiId authHuaweiId)
    {
        loggedInUser.text = String.Format(LOGGED_IN, authHuaweiId.DisplayName);
    }

    public void OnLoginFailure(HMSException error)
    {
        loggedInUser.text = LOGIN_ERROR;
    }
}
