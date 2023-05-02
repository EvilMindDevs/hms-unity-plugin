using System.Collections;
using System.Collections.Generic;
using HuaweiMobileServices.Utils;
using HuaweiMobileServices.Modeling3D;
using UnityEngine;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using System;
using HmsPlugin;
using System.Linq;
using HuaweiMobileServices.Modeling3D.ModelingCaptureSdk;
using UnityEngine.Android;
using HuaweiMobileServices.Base;
using System.Threading;

public class HMSModeling3dKitManager : HMSManagerSingleton<HMSModeling3dKitManager>
{
    private readonly string TAG = "[HMS] HMSModeling3dKitManager ";
    private ReconstructApplication reconstructApplication;
    private Modeling3dReconstructEngine modeling3DReconstructEngine;

    public Action<string, double, AndroidJavaObject> OnProgress;
    public Action<string, int, string> OnError;
    public Action<string, Modeling3dReconstructDownloadResult, AndroidJavaObject> OnResultDownload;
    public Action<string, Modeling3dReconstructUploadResult, AndroidJavaObject> OnResultUpload;
    public Action<string, AndroidJavaObject> OnResultPreview;

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

    public Modeling3dReconstructSetting Create3DReconstructionEngine()
    {
        Modeling3dReconstructSetting.Factory factory = new Modeling3dReconstructSetting.Factory();

        var modeling3dSettings = factory.SetReconstructMode(Modeling3dReconstructConstants.ReconstructMode.PICTURE)
                                            .SetTextureMode(Modeling3dReconstructConstants.TextureMode.PBR)
                                            .SetFaceLevel(Modeling3dReconstructConstants.FaceLevel.HIGH)
                                            .Create();

        Debug.Log(TAG + "Modelling Settings " + "FaceLevel:" +modeling3dSettings.FaceLevel + 
                                                    " TaskType:" + modeling3dSettings.FaceLevel + 
                                                        " TextureMode:" + modeling3dSettings.TextureMode + 
                                                            " ReconstuctMode:" + modeling3dSettings.ReconstructMode + 
                                                                " TaskId:" + modeling3dSettings.TaskId);


        

        return modeling3dSettings;
    }

    public void Create3DCaptureImageEngine(AndroidJavaObject context)
    {
        
        var modeling3dCaptureImageEngine = Modeling3dCaptureImageEngine.GetInstance();

        var setting = new Modeling3dCaptureSetting.Factory().SetAzimuthNum(30).SetLatitudeNum(3).SetRadius(2).Create();

        modeling3dCaptureImageEngine.SetCaptureConfig(setting);

        string fileSavePath = Application.persistentDataPath;

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

        var listener = new HuaweiMobileServices.Modeling3D.ModelingCaptureSdk.Modeling3dCaptureImageListener(new HmsPlugin.Modeling3dCaptureImageListener(OnProgressCaptureImage,OnErrorCaptureImage,OnResultCaptureImage));
       


        modeling3dCaptureImageEngine.CaptureImage(fileSavePath, context, listener);

        Debug.Log(TAG + "Capture Image");

    }

    public Modeling3dReconstructInitResult InitTask(Modeling3dReconstructSetting setting)
    {
        var modeling3DReconstructInitResult = modeling3DReconstructEngine.InitTask(setting);

        modeling3DReconstructInitResult.GetType().GetProperties().ToList().ForEach(x => Debug.Log(TAG + "Modelling Init Result " + x.Name + ":" + x.GetValue(modeling3DReconstructInitResult)));

        return modeling3DReconstructInitResult;
    }

    public void UploadFile(Modeling3dReconstructSetting setting, string path)
    {
        var modeling3DReconstructInitResult = modeling3DReconstructEngine.InitTask(setting);

        string taskId = modeling3DReconstructInitResult.TaskId;

        OnProgress += (taskId, progress, obj) =>
        {
            Debug.Log(TAG + "OnUploadProgress taskId:" + taskId + " progress:" + progress);
        };

        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
        };

        OnResultUpload += (taskId, result, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + result.TaskId + " result:" + result);
        };

        var listener = new Modeling3dReconstructUploadListener(new Modeling3dReconstructUploadORDownloadORPreviewListener(OnProgress, OnError, OnResultUpload));

        modeling3DReconstructEngine.SetReconstructUploadListener(listener);

        string uploadsPath = path ?? Application.persistentDataPath;

        Debug.Log(TAG + "Enviroment " + uploadsPath);

        modeling3DReconstructEngine.UploadFile(taskId, uploadsPath);

    }

    public void DownloadFile(Modeling3dReconstructDownloadConfig config, string taskId, string path)
    {
        OnProgress += (taskId, progress, obj) =>
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
        string downloadsPath = path ?? Application.persistentDataPath;
        //System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Download");

        var listener = new Modeling3dReconstructDownloadListener(new Modeling3dReconstructUploadORDownloadORPreviewListener(OnProgress, OnError, OnResultDownload));

        modeling3DReconstructEngine.SetReconstructDownloadListener(listener);        

        modeling3DReconstructEngine.DownloadModelWithConfig(taskId, downloadsPath, config);

    }

    public void PreviewFile(Modeling3dReconstructPreviewConfig config, string taskId)
    {
        OnError += (taskId, errorCode, errorMessage) =>
        {
            Debug.LogError(TAG + "OnError taskId:" + taskId + " errorCode:" + errorCode + " errorMessage:" + errorMessage);
        };

        OnResultPreview += (taskId, obj) =>
        {
            Debug.Log(TAG + "OnResult taskId:" + taskId + " result:" + obj);
        };
        var listener = new Modeling3dReconstructPreviewListener(new Modeling3dReconstructUploadORDownloadORPreviewListener(OnError, OnResultPreview));

        modeling3DReconstructEngine.PreviewModelWithConfig(taskId,config, listener);
    }

    public void QueryTask(string taskId)
    {
        var reconstructTaskUtils = Modeling3dReconstructTaskUtils.GetInstance();

        var result = reconstructTaskUtils.QueryTask(taskId);

        Debug.Log(TAG + "QueryTask result status:" + result.Status + " taskId:" + result.TaskId + " RedCode:" + result.RetCode + " RedMessage:" + result.RetMessage + " RedConstMessage:" + result?.ReconstructFailMessage);

        Debug.Log(TAG + "QueryTask ModelFormat Count : " + result?.GetModelFormat()?.Count ?? "0");
        DeleteTask(taskId);

    }

    public void DeleteTask(string taskId)
    {
        Modeling3dReconstructTaskUtils.GetInstance().DeleteTask(taskId);

        Debug.Log(TAG + string.Format("Task Deleted {0}", taskId));
    }

    public void CheckRequestUserPermissionForModelling3D()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Debug.LogWarning("android.permission.READ_EXTERNAL_STORAGE requesting");
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.LogWarning("android.permission.WRITE_EXTERNAL_STORAGE requesting");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        if (Permission.HasUserAuthorizedPermission("android.permission.ACCESS_NETWORK_STATE"))
        {
            Debug.LogWarning("android.permission.ACCESS_NETWORK_STATE requesting");
            Permission.RequestUserPermission("android.permission.ACCESS_NETWORK_STATE");
        }

        if (Permission.HasUserAuthorizedPermission("android.permission.INTERNET"))
        {
            Debug.LogWarning("android.permission.INTERNET requesting");
            Permission.RequestUserPermission("android.permission.INTERNET");
        }
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.LogWarning("android.permission.CAMERA requesting");
            Permission.RequestUserPermission(Permission.Camera);
        }
    }



}

