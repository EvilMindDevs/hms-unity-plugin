using UnityEngine;
using System;
using System.Linq;
using HmsPlugin;
using HuaweiMobileServices.Utils;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using HuaweiMobileServices.Modeling3D.ModelingCaptureSdk;
using HuaweiMobileServices.Modeling3D.MeterialGenerateSdk.Cloud;
using HuaweiMobileServices.Modeling3D.MeterialGenerateSdk;
using System.IO;
using System.Collections;

public class HMSModeling3dKitManager : HMSManagerSingleton<HMSModeling3dKitManager>
{

    private readonly string TAG = "[HMS] HMSModeling3dKitManager ";
    private const string TASK_LIST_PREFS_KEY = "3dTaskList";

    private readonly PlayerPrefsJsonDatabase<Modeling3dDTO> modeling3dTaskEntity = new PlayerPrefsJsonDatabase<Modeling3dDTO>(TASK_LIST_PREFS_KEY);

    private ReconstructApplication reconstructApplication;
    private Modeling3dReconstructEngine modeling3DReconstructEngine;
    private Modeling3dTextureEngine modeling3DTextureEngine;

    public Action<string, double, AndroidJavaObject> OnUploadProgress;
    public Action<string, double, AndroidJavaObject> OnDownloadProgress;
    public Action<string, int, string> OnError;
    public Action<string, Modeling3dReconstructDownloadResult, AndroidJavaObject> OnResultDownload;
    public Action<string, Modeling3dReconstructUploadResult, AndroidJavaObject> OnResultUpload;
    public Action<string, AndroidJavaObject> OnResultPreview;
    public Action<string, Modeling3dTextureDownloadResult, AndroidJavaObject> OnResult3dTextureDownload;
    public Action<string, Modeling3dTextureUploadResult, AndroidJavaObject> OnResult3dTextureUpload;
    public Action<string, AndroidJavaObject> OnResult3dTexturePreview;
    public Action OnResultCaptureImage;
    public Action<int, string> OnErrorCaptureImage;
    public Action<int> OnProgressCaptureImage;


    public HMSModeling3dKitManager()
    {
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(OnAwake);
    }
    private void OnAwake()
    {
        Init();
    }
    public void Init()
    {
        try
        {
            reconstructApplication = ReconstructApplication.GetInstance();
            modeling3DReconstructEngine = Modeling3dReconstructEngine.GetInstance();
            Debug.Log(TAG + "Init: " + reconstructApplication);
            
        }
        catch (System.Exception ex)
        {

            Debug.LogError(TAG + ex.Message);
        }


    }
    public void AuthWithAccessToken(string accessToken)
    {
        reconstructApplication.SetAccessToken(accessToken);
    }
    public void AuthWithApiKey(string apiKey)
    {
        reconstructApplication.SetApiKey(apiKey);
    }

    #region Modeling3dReconstruct Part
    public Modeling3dReconstructSetting Create3DReconstructionEngine(int? ReconstructMode = null, int? TextureMode = null, int? FaceLevel = null )
    {
        Modeling3dReconstructSetting.Factory factory = new Modeling3dReconstructSetting.Factory();
        SetDefaultParameters(ref ReconstructMode, ref TextureMode, ref FaceLevel);


        var modeling3dSettings = factory.SetReconstructMode((int)ReconstructMode)
                                            .SetTextureMode((int)TextureMode)
                                            .SetFaceLevel((int)FaceLevel)
                                            .Create();

        Debug.Log(TAG + "Modelling Settings " + "FaceLevel:" +modeling3dSettings.FaceLevel + 
                                                    " TaskType:" + modeling3dSettings.FaceLevel + 
                                                        " TextureMode:" + modeling3dSettings.TextureMode + 
                                                            " ReconstuctMode:" + modeling3dSettings.ReconstructMode + 
                                                                " TaskId:" + modeling3dSettings.TaskId);


        

        return modeling3dSettings;
    }
    private void SetDefaultParameters(ref int? ReconstructMode, ref int? TextureMode, ref int? FaceLevel)
    {
        if(ReconstructMode == null) 
        {
            ReconstructMode = Modeling3dReconstructConstants.ReconstructMode.PICTURE;
        }

        if (TextureMode == null)
        {
            TextureMode = Modeling3dReconstructConstants.TextureMode.PBR;
        }

        if (FaceLevel == null)
        {
            FaceLevel = Modeling3dReconstructConstants.FaceLevel.HIGH;
        }
    }
    public Modeling3dReconstructInitResult InitTask(Modeling3dReconstructSetting setting)
    {
        var modeling3DReconstructInitResult = modeling3DReconstructEngine.InitTask(setting);

        modeling3DReconstructInitResult.GetType().GetProperties().ToList().ForEach(x => Debug.Log(TAG + "Modelling Init Result " + x.Name + ":" + x.GetValue(modeling3DReconstructInitResult)));

        return modeling3DReconstructInitResult;
    }
    public void UploadFile(Modeling3dReconstructSetting setting, string path = null)
    {
        var modeling3DReconstructInitResult = InitTask(setting);

        string initTaskId = modeling3DReconstructInitResult.TaskId;

        Debug.Log(TAG + " Return TaskId: " + initTaskId);
        PlayerPrefs.SetString("currentTaskId", initTaskId);

        var taskName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + initTaskId.Substring(initTaskId.Length - 4);
        Debug.Log($"{TAG}{taskName}");

        OnUploadProgress += (taskId, progress, obj) =>
        {
            //check prograss is integer or not
            if (progress % 1 == 0)
                Debug.Log($"{TAG} OnUploadProgress TaskId: {taskId} and Progress: {progress}");
        };

        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
            HMSDispatcher.InvokeAsync(() =>
            {
                AndroidToast.MakeText($"{errorMessage} - {errorCode}").Show();
            });
        };

        OnResultUpload += (taskId, result, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + result.TaskId + " result:" + result);

        };

        var listener = new Modeling3dReconstructUploadListener(new Modeling3dReconstructUploadORDownloadORPreviewListener(OnUploadProgress, OnError, OnResultUpload));

        modeling3DReconstructEngine.SetReconstructUploadListener(listener);

        string uploadsPath =  Application.persistentDataPath;
        if(!string.IsNullOrWhiteSpace(path))
        {
            uploadsPath = Application.persistentDataPath.Split('/').Take(4).Aggregate((a, b) => a + "/" + b) + "/" + path.Split(':').Last();
        }
        Debug.Log(TAG + "Enviroment " + uploadsPath);


        //get all files in uploads folder cross platform
        string file = Directory.GetFiles(uploadsPath).FirstOrDefault();

        Debug.Log(TAG + "UploadFile: " + file);



        modeling3dTaskEntity.Insert(new Modeling3dDTO()
        {
            TaskId = initTaskId,
            Name = taskName,
            Status = "Uploading",
            Type = 1,
            CoverImagePath = file
        });
        RequestPermission();
        modeling3DReconstructEngine.UploadFile(initTaskId, uploadsPath);

    }
    public IEnumerator RequestPermission()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
            while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
            {
                yield return null;
            }
        }
        Debug.Log("Permission Granted");
    }
    public void DownloadFile(Modeling3dReconstructDownloadConfig config, string taskID, string path)
    {
        OnDownloadProgress += (taskId, progress, obj) =>
        {
            if (progress % 1 == 0)
                Debug.Log(TAG + "OnDownloadProgress taskId:" + taskId + " progress:" + progress);
        };

        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
            HMSDispatcher.InvokeAsync(() =>
            {
                AndroidToast.MakeText($"{errorMessage} - {errorCode}").Show();
            });
        };

        OnResultDownload += (taskId, result, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + taskId + " result:" + result);
        };

        string downloadsPath = path ?? Path.Combine(Application.persistentDataPath, "images");

        if (!string.IsNullOrWhiteSpace(path))
        {
            downloadsPath = Application.persistentDataPath.Split('/').Take(4).Aggregate((a, b) => a + "/" + b) + "/" + path.Split(':').Last();
        }

        Debug.Log(TAG + "DownloadsPath: " + downloadsPath);

        PlayerPrefs.SetString("currentTaskId", taskID);


        var listener = new Modeling3dReconstructDownloadListener(new Modeling3dReconstructUploadORDownloadORPreviewListener(OnDownloadProgress, OnError, OnResultDownload));

        modeling3DReconstructEngine.SetReconstructDownloadListener(listener);        

        modeling3DReconstructEngine.DownloadModelWithConfig(taskID, downloadsPath, config);

    }
    public void PreviewFile(Modeling3dReconstructPreviewConfig config, string taskID)
    {
        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
            HMSDispatcher.InvokeAsync(() =>
            {
                AndroidToast.MakeText($"{errorMessage} - {errorCode}").Show();
            });
        };

        OnResultPreview += (taskId, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + taskId + " result:" + obj);
        };
        var listener = new Modeling3dReconstructPreviewListener(new Modeling3dReconstructUploadORDownloadORPreviewListener(OnError, OnResultPreview));
        PlayerPrefs.SetString("currentTaskId", taskID);

        modeling3DReconstructEngine.PreviewModelWithConfig(taskID, config, listener);
    }
    public Modeling3dReconstructQueryResult QueryTask(string taskId)
    {
        var reconstructTaskUtils = Modeling3dReconstructTaskUtils.GetInstance();

        var result = reconstructTaskUtils.QueryTask(taskId);

        var restrictStatus = QueryTaskRestrictStatus(taskId);

        Debug.Log(TAG + "QueryTask result status:" + result.Status + " taskId:" + result.TaskId + " RedCode:" + result.RetCode + " RedMessage:" + result.RetMessage + " RedConstMessage:" + result?.ReconstructFailMessage);

        Debug.Log(TAG + "QueryTask Restrict Status: " + restrictStatus);
        
        return result;

    }
    public int QueryTaskRestrictStatus(string taskId)
    {
        var reconstructTaskUtils = Modeling3dReconstructTaskUtils.GetInstance();
        var result = reconstructTaskUtils.QueryTaskRestrictStatus(taskId);
        Debug.LogFormat(TAG + "Query Task Restrict Status: {0}", result);
        return result;

    }
    public void DeleteTask(string taskId)
    {
        var result = Modeling3dReconstructTaskUtils.GetInstance().DeleteTask(taskId);

        var text = string.Format("Task Deleted {0}. Result: {1}", taskId, result);

        AndroidToast.MakeText(text).Show();

        Debug.Log(TAG + text);
    }
    public int CancelUpload3dReconstruct(string taskId)
    {
        Debug.LogFormat(TAG + "CancelUpload3dReconstruct taskId: {0}", taskId);
        var result = modeling3DReconstructEngine.CancelUpload(taskId);
        return result;
    }
    public int CancelDownload3dReconstruct(string taskId)
    {
        Debug.LogFormat(TAG + "CancelDownload3dReconstruct taskId: {0}", taskId);
        var result = modeling3DReconstructEngine.CancelDownload(taskId);
        return result;
    }
    #endregion

    #region Modeling3dCapture Part
    public void Create3DCaptureImageEngine(AndroidJavaObject context)
    {

        var modeling3dCaptureImageEngine = Modeling3dCaptureImageEngine.GetInstance();

        var setting = new Modeling3dCaptureSetting.Factory().SetAzimuthNum(30).SetLatitudeNum(3).SetRadius(2).Create();

        modeling3dCaptureImageEngine.SetCaptureConfig(setting);


        string fileSavePath = Application.persistentDataPath + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

        OnProgressCaptureImage += (progress) =>
        {
            Debug.Log(TAG + "OnProgressCaptureImage progress:" + progress);
        };

        OnErrorCaptureImage += (errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError errorCode:" + errorCode + " errorMessage:" + errorMessage);
        };

        OnResultCaptureImage += () =>
        {
            Debug.Log(TAG + "OnResult");
        };

        var listener = new HuaweiMobileServices.Modeling3D.ModelingCaptureSdk.Modeling3dCaptureImageListener(new HmsPlugin.Modeling3dCaptureImageListener(OnProgressCaptureImage, OnErrorCaptureImage, OnResultCaptureImage));



        modeling3dCaptureImageEngine.CaptureImage(fileSavePath, context, listener);

        Debug.Log(TAG + "Capture Image");

    }
    #endregion

    #region MaterialGenerate Part
    public Modeling3dTextureSetting Create3dTextureEngine()
    {
        modeling3DTextureEngine = Modeling3dTextureEngine.GetInstance();

        var settings = new Modeling3dTextureSetting.Factory().SetTextureMode(Modeling3dTextureConstants.AlgorithmMode.AI).Create();

        Debug.LogFormat(TAG + "Create3dTextureEngine settings texture mode: {0}", settings.TextureMode);
        

        return settings;
    }
    public Modeling3dTextureInitResult InitTask(Modeling3dTextureSetting setting)
    {
        var modeling3DTextureInitResult = modeling3DTextureEngine.InitTask(setting);

        modeling3DTextureInitResult.GetType().GetProperties().ToList().ForEach(x => Debug.Log(TAG + "Modelling Init Result " + x.Name + ":" + x.GetValue(modeling3DTextureInitResult)));

        return modeling3DTextureInitResult;
    }
    public string AsyncUploadFile(Modeling3dTextureSetting setting, string uploadPath)
    {
        var modeling3DTextureInitResult = InitTask(setting);

        string taskID = modeling3DTextureInitResult.TaskId;

        OnUploadProgress += (taskId, progress, obj) =>
        {
            Debug.Log(TAG + "OnUploadProgress taskId:" + taskId + " progress:" + progress);
        };

        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
        };

        OnResult3dTextureUpload += (taskId, result, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + result.TaskId + " result:" + result);
        };

        var listener = new Modeling3dTextureUploadListener(new Modeling3dTextureUploadORDownloadORPreviewListener(OnUploadProgress, OnError, OnResult3dTextureUpload));

        modeling3DTextureEngine.SetTextureUploadListener(listener);

        string uploadsPath = uploadPath ?? Application.persistentDataPath;
        Debug.LogFormat(TAG + "AsyncUploadFile uploadPath: {0}", uploadsPath);

        modeling3DTextureEngine.AsyncUploadFile(taskID, uploadsPath);

        Debug.LogFormat(TAG + "AsyncUploadFile taskId: {0}", taskID);

        return taskID;
    }
    public void AsyncDownloadFile(string taskID, string savePath)
    {
        OnDownloadProgress += (taskId, progress, obj) =>
        {
            Debug.Log(TAG + "OnDownloadProgress taskId:" + taskId + " progress:" + progress);
        };

        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
        };

        OnResultDownload += (taskId, result, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + taskId + " result:" + result);
        };
        string downloadsPath = savePath ?? Application.persistentDataPath;

        var listener = new Modeling3dTextureDownloadListener(new Modeling3dTextureUploadORDownloadORPreviewListener(OnDownloadProgress, OnError, OnResult3dTextureDownload));

        modeling3DTextureEngine.SetTextureDownloadListener(listener);

        modeling3DTextureEngine.AsyncDownloadTexture(taskID, downloadsPath);

        Debug.LogFormat(TAG + "AsyncDownloadFile taskId: {0} savePath: {1}", taskID, downloadsPath);

    }
    public void PreviewFile3dTexture(string taskID)
    {
        OnDownloadProgress += (taskId, progress, obj) =>
        {
            Debug.Log(TAG + "OnDownloadProgress taskId:" + taskId + " progress:" + progress);
        };

        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
        };

        OnResult3dTexturePreview += (taskId, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + taskId + " result:" + obj);
        };

        var listener = new Modeling3dTexturePreviewListener(new Modeling3dTextureUploadORDownloadORPreviewListener(OnDownloadProgress, OnError, OnResult3dTexturePreview));

    }
    public Modeling3dTextureQueryResult QueryTaskModeling3dTexture(string taskId)
    {
        var texture3dTaskUtils = Modeling3dTextureTaskUtils.GetInstance();

        var result = texture3dTaskUtils.QueryTask(taskId);

        Debug.Log(TAG + " Modeling3dTexture QueryTask result status:" + result.Status + " taskId:" + result.TaskId + " RedCode:" + result.RetCode + " RedMessage:" + result.RetMsg);

        return result;
    }
    public void DeleteTaskModeling3dTexture(string taskId)
    {
        Modeling3dTextureTaskUtils.GetInstance().DeleteTask(taskId);

        Debug.Log(TAG + string.Format("Modeling3dTexture Task Deleted {0}", taskId));
    }
    public int QueryTaskRestrictStatusModeling3dTexture(string taskId)
    {
        var reconstructTaskUtils = Modeling3dTextureTaskUtils.GetInstance();
        var result = reconstructTaskUtils.QueryTaskRestrictStatus(taskId);
        Debug.LogFormat(TAG + "Modeling3dTexture Query Task Restrict Status: {0}", result);
        return result;

    }
    public int SetTaskRestrictStatusModeling3dTexture(string taskId, int restrictStatus)
    {
        var reconstructTaskUtils = Modeling3dTextureTaskUtils.GetInstance();
        var result = reconstructTaskUtils.SetTaskRestrictStatus(taskId, restrictStatus);
        Debug.LogFormat(TAG + "Modeling3dTexture Set Task Restrict Status: {0}", result);
        return result;
    }
    /* Call the synchronous API, passing the file path of a single image, path for saving the texture map, and configurator.
        Call the synchronous API to obtain the generated texture maps in real time.*/
    public int SyncGenerateTexture(string imagePath, string downloadPath, Modeling3dTextureSetting setting)
    {
        int result = modeling3DTextureEngine.SyncGenerateTexture(imagePath, downloadPath, setting);

        return result;
    }
    #endregion
}

