using HmsPlugin;
using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using HuaweiMobileServices.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Modeling3dDemoManager : MonoBehaviour
{
    private static readonly string[] REQUIRED_PERMISSIONS = new string[]{
        Permission.Camera,
        "android.permission.INTERNET",
        "android.permission.ACCESS_NETWORK_STATE",
        Permission.ExternalStorageWrite,
        Permission.ExternalStorageRead
    };

    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGGED_IN_ANONYMOUSLY = "Anonymously Logged In";
    private const string LOGIN_ERROR = "Error or cancelled login";

    private readonly string TAG = "[HMS] Modeling3dDemoManager ";
    private HMSAuthServiceManager authServiceManager;
    private AGConnectUser user;
    private HMSModeling3dKitManager modelling3dKitManager = null;





    /// TODO: Bu singleton generic bir class halinde MonoBehaviour extend edilecek þeyilde yazýlabilir. Kod tekrarýndan kurtarýr her demo managerda var.
    #region Singleton

    public static Modeling3dDemoManager Instance { get; private set; }
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    private void Awake()
    {
        Singleton();
    }
    void Start()
    {
        if (!ArePermissionsGranted(REQUIRED_PERMISSIONS))
        {
            Permission.RequestUserPermissions(REQUIRED_PERMISSIONS);
        }
        HMSAccountKitManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnLoginFailure;

        Debug.Log(TAG + "LogIn");

        // HMSAccountKitManager.Instance.SignIn();
        HMSModeling3dKitManager.Instance.AuthWithApiKey("DAEDAGtU1aO9158M5/1uIFS57ZkwmQhmep73W5AFzVyw63yA9I5JjgAZdEq5A03pFZp4OGcBORXVd6OBKFwRvye5GoqR2lTbW216qA==");

    }

    private bool ArePermissionsGranted(string[] permissions)
    {
        foreach (string permission in permissions)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                return false;
            }
        }
        return true;
    }


    // Start is called before the first frame update
    //public void Start()
    //{
    //    Debug.Log(TAG + NOT_LOGGED_IN);
    //    authServiceManager = HMSAuthServiceManager.Instance;
    //    authServiceManager.OnSignInSuccess = OnAuthServiceSignInSuccess;
    //    authServiceManager.OnSignInFailed = OnAuthServiceSignInFailed;

    //    if (authServiceManager.GetCurrentUser() != null)
    //    {
    //        user = authServiceManager.GetCurrentUser();

    //        string log = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
    //        Debug.Log(TAG + log);
    //    }
    //    else
    //    {
    //        SignInWithHuaweiAccount();
    //    }

    //}

    //private void OnAuthServiceSignInFailed(HMSException error)
    //{
    //    Debug.Log(TAG + LOGIN_ERROR);
    //}

    //private void OnAuthServiceSignInSuccess(SignInResult signInResult)
    //{
    //    user = signInResult.GetUser();
    //    string log = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
    //    Debug.Log(TAG + log);
    //    user.GetToken(true).AddOnSuccessListener(OnTokenResultSuccess).AddOnFailureListener(OnTokenResultFailed);
    //}

    //private void OnTokenResultSuccess(TokenResult tokenResult)
    //{
    //    modelling3dKitManager.Init();

    //    modelling3dKitManager.AuthWithAccessToken(tokenResult.Token);
    //    Debug.Log(TAG  + " TokenResult - " + tokenResult.Token);
    //}

    //private void OnTokenResultFailed(HMSException exception)
    //{
    //    Debug.Log(TAG + LOGIN_ERROR + " TokenResult " + exception.Message);

    //}

    //public void SignInWithHuaweiAccount()
    //{
    //    HMSAccountKitManager.Instance.OnSignInSuccess = OnAccountKitLoginSuccess;
    //    HMSAccountKitManager.Instance.OnSignInFailed = OnAuthSericeSignInFailed;
    //    HMSAccountKitManager.Instance.SignIn();
    //}

    //private void OnAccountKitLoginSuccess(AuthAccount authHuaweiId)
    //{
    //    AGConnectAuthCredential credential = HwIdAuthProvider.CredentialWithToken(authHuaweiId.AccessToken);
    //    authServiceManager.SignIn(credential);

    //    modelling3dKitManager = HMSModeling3dKitManager.Instance;

    //    Debug.Log(TAG + "OnAccountKitLoginSuccess");

    //}

    //private void OnAuthSericeSignInFailed(HMSException error)
    //{
    //    Debug.Log(TAG + LOGIN_ERROR + " OnAuthSericeSignInFailed");
    //}



    //void Start()
    //{
    //    try
    //    {
    //        HMSModeling3dKitManager.Instance.AuthWithAccessToken("eyJraWQiOiJnaHNtNWx3MmlzUlFITVNOUDRGU0NIaFRPTzJWdkZBTiIsInR5cCI6IkpXVCIsImFsZyI6IkhTMjU2In0.eyJzdWIiOiIxMTAyMjQ2MjU3MDc1NzAzMjk2IiwiZG4iOjEsImNsaWVudF90eXBlIjoxLCJleHAiOjE2ODA3NDg5NDQsImlhdCI6MTY4MDU3NjE0NH0.lQT4HXNrfLM3lWbFEsBfGEXNi5yDQEIXb9brFw4rY4c");
    //    }
    //    catch (System.Exception ex)
    //    {

    //        Debug.LogWarning(TAG + "Apikey " + ex.Message);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLoginSuccess(AuthAccount authHuaweiId)
    {
        Debug.Log(string.Format(LOGGED_IN, authHuaweiId.DisplayName) + " " + authHuaweiId.AccessToken);
       // HMSModeling3dKitManager.Instance.AuthWithAccessToken(authHuaweiId.AccessToken);
    }

    public void OnLoginFailure(HMSException error)
    {
        Debug.Log(LOGIN_ERROR);
    }



    public void test()
    {
        var settings =  HMSModeling3dKitManager.Instance.Create3DReconstructionEngine();
        var initResult = HMSModeling3dKitManager.Instance.InitTask(settings);
        PlayerPrefs.SetString("currentTaskId", initResult.TaskId);
        //HMSModeling3dKitManager.Instance.UploadFile(settings,null);
        //HMSModeling3dKitManager.Instance.DeleteTask(settings.TaskId);

    }

    public void DownloadFile()
    {

        var config = new Modeling3dReconstructDownloadConfig.Factory().SetModelFormat(Modeling3dReconstructConstants.ModelFormat.OBJ)
                                                                        .SetTextureMode(Modeling3dReconstructConstants.TextureMode.PBR)
                                                                            .Create();
        

        HMSModeling3dKitManager.Instance.DownloadFile(config, "1128044818459787648", null);

    }

    public void PreviewFile()
    {
        var config = new Modeling3dReconstructPreviewConfig.Factory().SetTextureMode(Modeling3dReconstructConstants.TextureMode.PBR).Create();

        HMSModeling3dKitManager.Instance.PreviewFile(config, "1128044818459787648");
                                                                        
    }

    public void QueryTask()
    {
        Debug.Log($"Current TaskId {PlayerPrefs.GetString("currentTaskId")}");
        HMSModeling3dKitManager.Instance.QueryTask(PlayerPrefs.GetString("currentTaskId"));
    }

    public void Create3DCaptureImage()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");


        HMSModeling3dKitManager.Instance.Create3DCaptureImageEngine(context);
    }



}
