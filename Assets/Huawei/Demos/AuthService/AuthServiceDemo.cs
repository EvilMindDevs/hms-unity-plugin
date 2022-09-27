using HmsPlugin;

using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AuthServiceDemo : MonoBehaviour
{
    private HMSAuthServiceManager authServiceManager = null;
    private AGConnectUser user = null;
    private Text loggedInUser;

    private GameObject verifyCodePhone;
    private GameObject verifyCodeEmail;

    private InputField PhoneCountryCode, PhoneNumber, PhoneVerifyCode, PhonePassword;
    private InputField EmailAddress, EmailVerifyCode, EmailPassword;

    private Button Btn_AnonymousLogin;
    private Button Btn_AccountLogin;
    private Button Btn_RegisterWithMobileNumber;  // Register w/ Mobile Number
    private Button Btn_RegisterWithEmail;  // Register w/ E-Mail
    private Button Btn_SignOut;
    private Button Btn_DeleteUser;

    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGGED_IN_ANONYMOUSLY = "Anonymously Logged In";
    private const string LOGIN_ERROR = "Error or cancelled login";

    private void Awake()
    {
        Btn_AnonymousLogin = GameObject.Find("Anonymous Login").GetComponent<Button>();
        Btn_AccountLogin = GameObject.Find("Huawei Account Login").GetComponent<Button>();
        Btn_RegisterWithMobileNumber = GameObject.Find("Register w/ Mobile Number").GetComponent<Button>();
        Btn_RegisterWithEmail = GameObject.Find("Register w/ E-Mail").GetComponent<Button>();
        Btn_DeleteUser = GameObject.Find("DeleteUser").GetComponent<Button>();
        Btn_SignOut = GameObject.Find("SignOut").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_AnonymousLogin.onClick.AddListener(SignInAnonymously);
        Btn_AccountLogin.onClick.AddListener(SignInWithHuaweiAccount);
        Btn_RegisterWithMobileNumber.onClick.AddListener(RegisterWithPhoneNumber);
        Btn_RegisterWithEmail.onClick.AddListener(RegisterWithEmail);
        Btn_DeleteUser.onClick.AddListener(DeleteUser);
        Btn_SignOut.onClick.AddListener(SignOut);
    }

    private void OnDisable()
    {
        Btn_AnonymousLogin.onClick.RemoveListener(SignInAnonymously);
        Btn_AccountLogin.onClick.RemoveListener(SignInWithHuaweiAccount);
        Btn_RegisterWithMobileNumber.onClick.RemoveListener(RegisterWithPhoneNumber);
        Btn_RegisterWithEmail.onClick.RemoveListener(RegisterWithEmail);
        Btn_DeleteUser.onClick.RemoveListener(DeleteUser);
        Btn_SignOut.onClick.RemoveListener(SignOut);
    }

    public void Start()
    {
        verifyCodePhone = GameObject.Find("verifyCodePhone");
        verifyCodeEmail = GameObject.Find("verifyCodeEmail");

        loggedInUser = GameObject.Find("LoggedUserText").GetComponent<Text>();
        loggedInUser.text = NOT_LOGGED_IN;

        PhoneCountryCode = GameObject.Find("PhoneCountryCode").GetComponent<InputField>();
        PhoneNumber = GameObject.Find("PhoneNumber").GetComponent<InputField>();
        EmailAddress = GameObject.Find("EmailAddress").GetComponent<InputField>();

        PhoneVerifyCode = verifyCodePhone.transform.Find("PhoneVerifyCode").GetComponent<InputField>();
        PhonePassword = verifyCodePhone.transform.Find("PhonePassword").GetComponent<InputField>();

        EmailVerifyCode = verifyCodeEmail.transform.Find("EmailVerifyCode").GetComponent<InputField>();
        EmailPassword = verifyCodeEmail.transform.Find("EmailPassword").GetComponent<InputField>();

        authServiceManager = HMSAuthServiceManager.Instance;
        authServiceManager.OnSignInSuccess = OnAuthSericeSignInSuccess;
        authServiceManager.OnSignInFailed = OnAuthSericeSignInFailed;
        authServiceManager.OnCreateUserSuccess = OnAuthSericeCreateUserSuccess;
        authServiceManager.OnCreateUserFailed = OnAuthSericeCreateUserFailed;

        user = authServiceManager.GetCurrentUser();

        if (user != null)
        {
            loggedInUser.text = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
        }
    }

    private void OnAuthSericeSignInFailed(HMSException error)
    {
        loggedInUser.text = LOGIN_ERROR;
    }

    private void OnAuthSericeSignInSuccess(SignInResult signInResult)
    {
        user = signInResult.GetUser();
        loggedInUser.text = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
    }

    private void OnAuthSericeCreateUserSuccess(SignInResult signInResult)
    {
        user = signInResult.GetUser();
        loggedInUser.text = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
        verifyCodePhone.SetActive(false);
        verifyCodeEmail.SetActive(false);
    }

    private void OnAuthSericeCreateUserFailed(HMSException error)
    {
        loggedInUser.text = error.WrappedExceptionMessage;
        verifyCodePhone.SetActive(false);
        verifyCodeEmail.SetActive(false);
    }

    private void OnAccountKitLoginSuccess(AuthAccount authHuaweiId)
    {
        AGConnectAuthCredential credential = HwIdAuthProvider.CredentialWithToken(authHuaweiId.AccessToken);
        authServiceManager.SignIn(credential);
    }

    public void SignInWithHuaweiAccount()
    {
        HMSAccountKitManager.Instance.OnSignInSuccess = OnAccountKitLoginSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnAuthSericeSignInFailed;
        HMSAccountKitManager.Instance.SignIn();
    }

    public void SignInAnonymously() => authServiceManager.SignInAnonymously();

    public void RequestVerifyCodeWithPhoneNumber()
    {
        VerifyCodeSettings verifyCodeSettings = new VerifyCodeSettings.Builder()
            .Action(VerifyCodeSettings.ACTION_REGISTER_LOGIN)
            .Locale(Locale.GetDefault())
            .SendInterval(30).Build();

        AGConnectAuth.GetInstance().RequestVerifyCode(PhoneCountryCode.text, PhoneNumber.text, verifyCodeSettings)
            .AddOnSuccessListener(verifyCodeResult =>
            {
                verifyCodePhone.SetActive(true);
            })
            .AddOnFailureListener(exception =>
            {
                loggedInUser.text = exception.WrappedExceptionMessage;
            });
    }

    public void RegisterWithPhoneNumber()
    {
        PhoneUser phoneUser = new PhoneUser.Builder()
            .SetCountryCode(PhoneCountryCode.text)
            .SetPhoneNumber(PhoneNumber.text)
            .SetVerifyCode(PhoneVerifyCode.text)
            .SetPassword(PhonePassword.text).Build();
        authServiceManager.CreateUser(phoneUser);
    }

    public void RequestVerifyCodeWithEmail()
    {
        VerifyCodeSettings verifyCodeSettings = new VerifyCodeSettings.Builder()
            .Action(VerifyCodeSettings.ACTION_REGISTER_LOGIN)
            .Locale(Locale.GetDefault())
            .SendInterval(30).Build();

        AGConnectAuth.GetInstance().RequestVerifyCode(EmailAddress.text, verifyCodeSettings)
            .AddOnSuccessListener(result =>
            {
                verifyCodeEmail.SetActive(true);
            })
            .AddOnFailureListener(error =>
            {
                loggedInUser.text = error.WrappedExceptionMessage;
            });
    }

    public void RegisterWithEmail()
    {
        EmailUser emailUser = new EmailUser.Builder().SetEmail(EmailAddress.text).SetVerifyCode(EmailVerifyCode.text).SetPassword(EmailPassword.text).Build();
        authServiceManager.CreateUser(emailUser);
    }

    public void CloseVerifyCodePanel() { verifyCodePhone.SetActive(false); verifyCodeEmail.SetActive(false); }

    public void DeleteUser()
    {
        authServiceManager.DeleteUser();
        loggedInUser.text = NOT_LOGGED_IN;
    }

    public void SignOut()
    {
        authServiceManager.SignOut();
        loggedInUser.text = NOT_LOGGED_IN;
    }

}
