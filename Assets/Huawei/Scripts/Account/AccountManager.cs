using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

public class AccountManager : MonoBehaviour
{

    private const string NAME = "AccountManager";

    public static AccountManager Instance => GameObject.Find(NAME).GetComponent<AccountManager>();

    private static HuaweiIdAuthService DefaultAuthService
    {
        get
        {
            var authParams = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DEFAULT_AUTH_REQUEST_PARAM).SetIdToken().CreateParams();
            return HuaweiIdAuthManager.GetService(authParams);
        }
    }
    
    public AuthHuaweiId HuaweiId { get; private set; }
    public Action<AuthHuaweiId> LoginSuccess { get; set; }
    public Action<HMSException> LoginFailed { get; set; }

    private HuaweiIdAuthService authService;

    // Start is called before the first frame update
    void Start()
    {
        authService = DefaultAuthService;
    }

    public void SignIn()
    {
        authService.StartSignIn((authId) =>
        {
            HuaweiId = authId;
            LoginSuccess?.Invoke(authId);
        }, (error) =>
        {
            HuaweiId = null;
            LoginFailed?.Invoke(error);
        });
    }

    public void SignOut()
    {
        authService.SignOut();
        HuaweiId = null;
    }
}
