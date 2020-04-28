using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HuaweiMobileServices.Id;

public class AccountManager : MonoBehaviour
{

    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGIN_ERROR = "Error or cancelled login";

    private static HuaweiIdAuthService DefaultAuthService()
    {
        var authParams = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DEFAULT_AUTH_REQUEST_PARAM).SetIdToken().CreateParams();
        return HuaweiIdAuthManager.GetService(authParams);
    }

    private Text loggedInUser;
    private HuaweiIdAuthService authService;

    // Start is called before the first frame update
    void Start()
    {
        loggedInUser = GameObject.Find("LoggedUserText").GetComponent<Text>();
        loggedInUser.text = NOT_LOGGED_IN;
        authService = DefaultAuthService();
    }

    public void LogIn()
    {
        authService.StartSignIn((authId) =>
        {
            loggedInUser.text = string.Format(LOGGED_IN, authId.DisplayName);
        }, (error) =>
        {
            loggedInUser.text = LOGIN_ERROR;
        });
    }

    public void LogOut()
    {
        authService.SignOut();
        loggedInUser.text = NOT_LOGGED_IN;
    }
}
