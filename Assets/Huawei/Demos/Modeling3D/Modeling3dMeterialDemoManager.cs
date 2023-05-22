using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.Android;

public class Modeling3dMeterialDemoManager : MonoBehaviour
{
    private static readonly string[] REQUIRED_PERMISSIONS = new string[]{
        Permission.Camera,
        "android.permission.INTERNET",
        "android.permission.ACCESS_NETWORK_STATE",
        Permission.ExternalStorageWrite,
        Permission.ExternalStorageRead
    };

    private readonly string TAG = "[HMS] Modeling3dMeterialDemoManager ";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGIN_ERROR = "Error or cancelled login";


    #region Singleton

    public static Modeling3dMeterialDemoManager Instance { get; private set; }
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
            RequestUserPermissions(REQUIRED_PERMISSIONS);
        }
        HMSAccountKitManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnLoginFailure;

        Debug.Log(TAG + "LogIn");

        // HMSAccountKitManager.Instance.SignIn();
        //Getting apikey in agconnect json file.
        HMSModeling3dKitManager.Instance.AuthWithApiKey("DAEDAGtU1aO9158M5/1uIFS57ZkwmQhmep73W5AFzVyw63yA9I5JjgAZdEq5A03pFZp4OGcBORXVd6OBKFwRvye5GoqR2lTbW216qA==");

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

    private void RequestUserPermissions(string[] permissions)
    {
        foreach (string permission in permissions)
        {
            Permission.RequestUserPermission(permission);
        }
    }

    public void UploadFile()
    {
        var settings = HMSModeling3dKitManager.Instance.Create3dTextureEngine();
        var initResult = HMSModeling3dKitManager.Instance.InitTask(settings);
        PlayerPrefs.SetString("currentTaskId", initResult.TaskId);
        HMSModeling3dKitManager.Instance.AsyncUploadFile(settings,null);
    }
    public void QueryTask()
    {
        var currTaskId = PlayerPrefs.GetString("currentTaskId");
        Debug.Log($"Current TaskId {currTaskId}");
        HMSModeling3dKitManager.Instance.QueryTaskRestrictStatusModeling3dTexture(currTaskId);
        HMSModeling3dKitManager.Instance.QueryTaskModeling3dTexture(currTaskId);
    }

    public void Create3DCaptureImage()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");


        HMSModeling3dKitManager.Instance.Create3DCaptureImageEngine(context);
    }

}
