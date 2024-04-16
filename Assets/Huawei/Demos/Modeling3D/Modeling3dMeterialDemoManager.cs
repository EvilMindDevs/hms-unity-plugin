using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Modeling3D.MeterialGenerateSdk.Cloud;
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

    private string TASKID = "";
    private Modeling3dTextureSetting settings;

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
        string APIKEY = HMSModelingKitSettings.Instance.Settings.Get(HMSModelingKitSettings.ModelingKeyAPI);
        HMSModeling3dKitManager.Instance.AuthWithApiKey(APIKEY);
        settings = HMSModeling3dKitManager.Instance.Create3dTextureEngine();
        HMSModeling3dKitManager.Instance.OnResult3dTextureUpload += OnMaterialUploadResult;
        HMSModeling3dKitManager.Instance.OnUploadProgress += OnUploadProgress;
        HMSModeling3dKitManager.Instance.OnError += OnError;
    }
    public void OnLoginSuccess(AuthAccount authHuaweiId)
    {
        Debug.Log(TAG + string.Format(LOGGED_IN, authHuaweiId.DisplayName) + " " + authHuaweiId.AccessToken);
        // HMSModeling3dKitManager.Instance.AuthWithAccessToken(authHuaweiId.AccessToken); //ForAuthService
    }

    public void OnLoginFailure(HMSException error)
    {
        Debug.LogError(TAG + LOGIN_ERROR + "error:" + error);
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

    #region Upload Material
    private void OnMaterialUploadResult(string TaskId, Modeling3dTextureUploadResult result, JavaObject @object)
    {
        Debug.Log(TAG + "UploadResult TaskId:" + TaskId + " -- result.TaskId:" + result.TaskId);
        TASKID = TaskId;
    }
    private void OnError(string taskId, int errorCode, string errorMessage)
    {
        AndroidToast.MakeText("Upload error " + errorMessage).Show();
        Debug.LogError(TAG + "Upload error -- TaskID:" + taskId + " Code:" + errorCode + " msg:" + errorMessage);
    }
    private void OnUploadProgress(string taskId, double progress, AndroidJavaObject obj)
    {
        Debug.Log(TAG + " OnUploadProgress:" + progress);
    }
    public void UploadFile()
    {
        AndroidToast.MakeText("Number of Input Images: 1 - 5 images").Show();
        AndroidFolderPicker.mOnSuccessListener = OnUploadSuccessFolderPicker;
        AndroidFolderPicker.OpenFolderPicker();
    }
    public void OnUploadSuccessFolderPicker(AndroidIntent androidIntent)
    {
        var currentUploadFilePath = androidIntent.GetData().GetPath;
        HMSModeling3dKitManager.Instance.AsyncUploadFile(settings, currentUploadFilePath);
        AndroidToast.MakeText("Start Uploading...").Show();
    }
    #endregion
    #region Download Material
    public void DownloadFile()
    {
        AndroidFolderPicker.mOnSuccessListener = OnDownloadSuccessFolderPicker;
        AndroidFolderPicker.OpenFolderPicker();
    }
    public void OnDownloadSuccessFolderPicker(AndroidIntent androidIntent)
    {
        HMSModeling3dKitManager.Instance.Create3dTextureEngine();
        var currentDownloadFilePath = androidIntent.GetData().GetPath;
        HMSModeling3dKitManager.Instance.AsyncDownloadFile(TASKID, currentDownloadFilePath);
        AndroidToast.MakeText("Start Downloading...").Show();
    }
    #endregion
    #region Query Task
    public void QueryTask()
    {
        Debug.Log($"{TAG} QueryTask Current TaskId {TASKID}");
        //HMSModeling3dKitManager.Instance.QueryTaskRestrictStatusModeling3dTexture(TASKID);
        Modeling3dTextureQueryResult result = HMSModeling3dKitManager.Instance.QueryTaskModeling3dTexture(TASKID);
        AndroidToast.MakeText("TaskID:" + TASKID + "\n Status:" + result.Status + " " + HMSModeling3dKitManager.Instance.IdentifyProgressStatus(result.Status)).Show();

    }
    #endregion

    public void Create3DCaptureImage()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        HMSModeling3dKitManager.Instance.Create3DCaptureImageEngine(context);
    }

    public void MaterialPreview()
    {
        Debug.Log($"{TAG} Material preview TaskId:{TASKID}");
        HMSModeling3dKitManager.Instance.PreviewFile3dTexture(TASKID);
    }
}
