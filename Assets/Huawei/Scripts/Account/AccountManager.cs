using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

public class AccountManager : MonoBehaviour
{

    private static HuaweiIdAuthService DefaultAuthService
    {
        get
        {
            var authParams = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DEFAULT_AUTH_REQUEST_PARAM).SetIdToken().CreateParams();
            return HuaweiIdAuthManager.GetService(authParams);
        }
    }

    public Action<AuthHuaweiId> LoginSuccess { get; set; }
    public Action<HMSException> LoginFailed { get; set; }

    private HuaweiIdAuthService authService;

    // Start is called before the first frame update
    void Start()
    {
        authService = DefaultAuthService;
    }

    public void LogIn()
    {
        authService.StartSignIn((authId) =>
        {
            LoginSuccess?.Invoke(authId);
        }, (error) =>
        {
            LoginFailed?.Invoke(error);
        });
    }

    public void LogOut()
    {
        authService.SignOut();
    }
}
