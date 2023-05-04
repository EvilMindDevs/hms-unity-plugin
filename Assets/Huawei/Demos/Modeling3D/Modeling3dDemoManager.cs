using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HmsPlugin;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;


public class Modeling3dDemoManager : MonoBehaviour
{
    ///It is necessary to add permissions in the Android manifest. Assets\Huawei\Plugins\Android\HMSUnityModelingKit.plugin\AndroidManifest.xml
    #region Definations
    private static readonly string[] REQUIRED_PERMISSIONS = new string[]{
        Permission.Camera,
        "android.permission.INTERNET",
        "android.permission.ACCESS_NETWORK_STATE",
        Permission.ExternalStorageWrite,
        Permission.ExternalStorageRead,
    };

    //Getting apikey in agconnect json file.
    private const string YOUR_API_KEY = "";
    private const string TASK_LIST_PREFS_KEY = "3dTaskList";
    private const string TAG = "[HMS] Modeling3dDemoManager ";

    private readonly PlayerPrefsJsonDatabase<Modeling3dDTO> modeling3dTaskEntity = new PlayerPrefsJsonDatabase<Modeling3dDTO>(TASK_LIST_PREFS_KEY);
    private string currentUploadFilePath;
    private string currentDownloadFilePath;
    private string modelFormat;
    private int? textureMode;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject taskList;
    [SerializeField] private GameObject taskItemPrefab;
    #endregion
    /// TODO: This singleton can be written as a generic class to extend MonoBehavior. It saves code repetition and is available in every demo manager.r.
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

    #region Methods
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

        HMSModeling3dKitManager.Instance.OnUploadProgress = OnUploadProgress;
        HMSModeling3dKitManager.Instance.OnResultUpload = OnResultUpload;
        HMSModeling3dKitManager.Instance.OnDownloadProgress = OnDownloadProgress;
        HMSModeling3dKitManager.Instance.OnResultDownload = OnResultDownload;
        //HMSModeling3dKitManager.Instance.OnResultPreview = OnResultPreview;

        Debug.Log(TAG + "LogIn By API key");

        HMSModeling3dKitManager.Instance.AuthWithApiKey(YOUR_API_KEY);

    }
    public void OnUploadProgress(string taskId, double progress, AndroidJavaObject obj) {
        progressBar.current = Mathf.RoundToInt((float)progress);

        HMSDispatcher.Invoke(() =>
        {
            var data = new Modeling3dDTO();
            var currentTaskId = PlayerPrefs.GetString("currentTaskId");
            data = modeling3dTaskEntity.Find(currentTaskId);
            data.TaskId = taskId;
            data.Status = $"UploadProgress";
            data.Type = 1;
            modeling3dTaskEntity.Update(data);
            PlayerPrefs.SetString("currentTaskId", taskId);
        });

    }
    public void OnResultUpload(string taskId, Modeling3dReconstructUploadResult result, AndroidJavaObject obj)
    {
        Debug.Log($"{TAG} Upload Result Android Obj: {obj?.GetRawObject()} and TaskId: {taskId} and Result: {result}");
        if(result.Complate == true)
        {
            HMSDispatcher.Invoke(() =>
            {
                var data = new Modeling3dDTO();
                var currentTaskId = PlayerPrefs.GetString("currentTaskId");
                data = modeling3dTaskEntity.Find(currentTaskId);
                data.TaskId = taskId;
                data.Status = $"Complated: {result.Complate}";
                data.UploadFilePath = currentUploadFilePath;
                data.Type = 1;
                modeling3dTaskEntity.Update(data);
                PlayerPrefs.SetString("currentTaskId", taskId);
                AndroidToast.MakeText("Upload Complated. Open Task List and Check It.").Show();
                progressBar.gameObject.SetActive(false);

            });
        }
        else
        {
            HMSDispatcher.Invoke(() =>
            {
                AndroidToast.MakeText("Upload Not Complated").Show();
            });
        }
    }
    public void OnDownloadProgress(string taskId, double progress, AndroidJavaObject obj)
    {
        progressBar.current = Mathf.RoundToInt((float)progress);

        HMSDispatcher.Invoke(() =>
        {
            var data = new Modeling3dDTO();
            var currentTaskId = PlayerPrefs.GetString("currentTaskId");
            data = modeling3dTaskEntity.Find(currentTaskId);
            data.TaskId = taskId;
            data.Status = $"DownloadProgress";
            data.Type = 2;
            modeling3dTaskEntity.Update(data);
            PlayerPrefs.SetString("currentTaskId", taskId);
        });
    }
    public void OnResultDownload(string taskId, Modeling3dReconstructDownloadResult result, AndroidJavaObject obj)
    {
        Debug.Log($"{TAG} Download Result Android Obj: {obj?.GetRawObject()} and TaskId: {taskId} and Result: {result}");
        if (result.Complete == true)
        {
            HMSDispatcher.Invoke(() =>
            {
                var data = new Modeling3dDTO();
                var currentTaskId = PlayerPrefs.GetString("currentTaskId");
                data = modeling3dTaskEntity.Find(currentTaskId);
                data.TaskId = taskId;
                data.Status = $"Complated: {result.Complete}";
                data.Type = 2;
                data.DownloadFilePath = currentDownloadFilePath;
                modeling3dTaskEntity.Update(data);
                PlayerPrefs.SetString("currentTaskId", taskId);
                AndroidToast.MakeText($"Download Complated.\n Download Path: {currentDownloadFilePath}").Show();
                progressBar.gameObject.SetActive(false);

            });
        }
        else
        {
            HMSDispatcher.Invoke(() =>
            {
                AndroidToast.MakeText("Download Not Complated").Show();
            });
        }

    }
    public void OnResultPreview(string taskId, AndroidJavaObject obj)
    {
        Debug.Log($"{TAG} Preview Result Android Obj: {obj?.GetRawObject()} and TaskId: {taskId}");

        HMSDispatcher.Invoke(() =>
        {
            var data = new Modeling3dDTO();
            var currentTaskId = PlayerPrefs.GetString("currentTaskId");
            data = modeling3dTaskEntity.Find(currentTaskId);
            data.TaskId = taskId;
            data.Status = $"Previewed";
            data.Type = 3;
            modeling3dTaskEntity.Update(data);
            PlayerPrefs.SetString("currentTaskId", taskId);
            AndroidToast.MakeText("Preview Complated").Show();

        });
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
    public void UploadFile()
    {
        AndroidFolderPicker.mOnSuccessListener = OnUploadSuccessFolderPicker;
        AndroidFolderPicker.OpenFolderPicker();

    }
    public void OnUploadSuccessFolderPicker(AndroidIntent androidIntent)
    {
        currentUploadFilePath = androidIntent.GetData().GetPath;
        var settings = HMSModeling3dKitManager.Instance.Create3DReconstructionEngine();

        progressBar.gameObject.SetActive(true);
        progressBar.progressType = "Uploading";
        progressBar.current = 0;
        HMSModeling3dKitManager.Instance.UploadFile(settings, currentUploadFilePath);
        AndroidToast.MakeText("Start Uploading...").Show();

    }
    public void DownloadFile(string TaskId, string ModelFormat = null, int? TextureMode = null)
    {
        PlayerPrefs.SetString("currentTaskId", TaskId);
        this.modelFormat = ModelFormat;
        this.textureMode = TextureMode;
        AndroidFolderPicker.mOnSuccessListener = OnDownloadSuccessFolderPicker;
        AndroidFolderPicker.OpenFolderPicker();
    }
    public void OnDownloadSuccessFolderPicker(AndroidIntent androidIntent)
    {
        currentDownloadFilePath = androidIntent.GetData().GetPath;
        var config = new Modeling3dReconstructDownloadConfig.Factory().SetModelFormat(this.modelFormat ?? Modeling3dReconstructConstants.ModelFormat.OBJ)
                                                                        .SetTextureMode(this.textureMode ?? Modeling3dReconstructConstants.TextureMode.PBR)
                                                                            .Create();

        progressBar.gameObject.SetActive(true);
        progressBar.progressType = "Downloading";
        progressBar.current = 0;
        HMSModeling3dKitManager.Instance.DownloadFile(config, PlayerPrefs.GetString("currentTaskId"), currentDownloadFilePath);
        AndroidToast.MakeText("Start Downloading...\n Please Back to Main Menu.").Show();

    }
    public void PreviewFile(string TaskId, int? TextureMode = null)
    {
        var config = new Modeling3dReconstructPreviewConfig.Factory().SetTextureMode(TextureMode ?? Modeling3dReconstructConstants.TextureMode.PBR).Create();

        if (string.IsNullOrWhiteSpace(TaskId))
        {
            TaskId = PlayerPrefs.GetString("currentTaskId");
        }

        HMSModeling3dKitManager.Instance.PreviewFile(config, TaskId);
        AndroidToast.MakeText("Start Previewing...").Show();


    }
    public Modeling3dReconstructQueryResult QueryTask(string taskId = null)
    {
        if (taskId == null)
        {
            taskId = PlayerPrefs.GetString("currentTaskId");
        }
        Debug.Log($"Current TaskId {taskId}");
        return HMSModeling3dKitManager.Instance.QueryTask(taskId);
    }
    public void Create3DCaptureImage()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        HMSModeling3dKitManager.Instance.Create3DCaptureImageEngine(context);
    }
    public void Cancel3dModelReconstruct()
    {
        var modeling3D = modeling3dTaskEntity.Find(PlayerPrefs.GetString("currentTaskId"));
        var result = 1;

        if (modeling3D.Type == 1) {
            result = HMSModeling3dKitManager.Instance.CancelUpload3dReconstruct(modeling3D.TaskId);

        }
        else if(modeling3D.Type == 2)
        {
            result = HMSModeling3dKitManager.Instance.CancelDownload3dReconstruct(modeling3D.TaskId);
        }

        if (result == 1)
        {
            Debug.LogError(TAG + "Cancel failed"); 
        }
        else
        {
            Debug.Log(TAG+"Canceled successfully");
            progressBar.progressType = "Cancelling";
            HMSDispatcher.Invoke(() =>
            {
                Invoke("CancelOperationWait", 5);
            });
        }

    }
    private void CancelOperationWait()
    {
        progressBar.progressType = "Cancelled";
        progressBar.current = 0;
        progressBar.gameObject.SetActive(false);
        var getData = modeling3dTaskEntity.Find(PlayerPrefs.GetString("currentTaskId"));
        getData.Status = "Cancelled";
        modeling3dTaskEntity.Update(getData);
        AndroidToast.MakeText("Cancelled Upload/Download File.").Show();
    }
    public void OpenTaskList()
    {
        var taskListItem = taskItemPrefab.GetComponent<TaskListDisplay>();
        taskListItem.RefreshAllTask();
        TaskList();
    }
    public void TaskList()
    {
        var taskListItem = taskItemPrefab.GetComponent<TaskListDisplay>();
        taskList.gameObject.SetActive(true);
        var taskListItemParentObj = GameObject.Find("TaskList/Body/ScrollView/Panel/Body");
        var noDataText = FindObjectsOfType<Text>().FirstOrDefault(t => t.name == "NoDataText");
        var taskListData = modeling3dTaskEntity.GetAll();
        Debug.LogFormat(TAG + "PlayerPrefsJsonDatabase Count {0}", taskListData.Count);
        noDataText.gameObject.SetActive(taskListData.Count == 0 ? true : false);

        DestoryChildrenByType(taskListItemParentObj, typeof(TaskListDisplay));
        CreateTaskListItem(taskListData, taskListItem, taskListItemParentObj);

    }
    private void CreateTaskListItem(List<Modeling3dDTO> taskListData, TaskListDisplay taskListItem, GameObject taskListItemParentObj){
        foreach (var task in taskListData)
        {
            taskListItem.Name.text = $"{task.TaskId} \n {task.Name}";
            taskListItem.Status.text = task.Status;
            taskListItem.Image.color = Color.white;
            if(!string.IsNullOrWhiteSpace(task.CoverImagePath)){
                taskListItem.RawImage.gameObject.SetActive(true);
                byte[] bytes = File.ReadAllBytes(task.CoverImagePath);
                Texture2D texture = new Texture2D(100, 100);
                texture.LoadImage(bytes);
                taskListItem.RawImage.texture = texture;
            }
            taskListItem.HasDownloaded.gameObject.SetActive(!string.IsNullOrWhiteSpace(task.DownloadFilePath));
            var successCondition = task.Status.Contains("3") && task.Status.Contains("success");
            taskListItem.PreviewButton.gameObject.SetActive(successCondition);
            taskListItem.DownloadButton.gameObject.SetActive(successCondition);

            taskListItem.TaskId = task.TaskId;
            bool hasChildren = taskListItemParentObj.transform.childCount > 0;
            var obj = Instantiate(taskItemPrefab, taskListItemParentObj.transform);
            if (hasChildren)
            {
                //get last child
                var lastChild = taskListItemParentObj.transform.GetChild(taskListItemParentObj.transform.childCount - 2);
                //change transform according to last child
                obj.transform.SetSiblingIndex(lastChild.GetSiblingIndex() + 1);
                obj.transform.SetPositionAndRotation(new Vector3(lastChild.position.x, lastChild.position.y - 150, lastChild.position.z), lastChild.rotation);
            }
            
        }
    }
    private void DestoryChildrenByType(GameObject parent, Type type)
    {
        foreach (var child in parent.GetComponentsInChildren(type))
        {
            child.transform.parent = null;
            Destroy(child);
        }
    }
    public void CloseTaskList()
    {
        taskList.gameObject.SetActive(false);
    }
    public void DeleteTask(string taskId = null)
    {
        if (taskId == null)
        {
            taskId = PlayerPrefs.GetString("currentTaskId");
        }
        Debug.Log($"Current TaskId {taskId}");
        HMSModeling3dKitManager.Instance.DeleteTask(taskId);
    }

    private void RequestUserPermissions(string[] permissions)
    {
        foreach (string permission in permissions)
        {
            Permission.RequestUserPermission(permission);
        }
    }
    #endregion
}
