using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using System;
using UnityEditor;
using UnityEngine;
using static HuaweiMobileServices.Modeling3D.ModelingCaptureSdk.Modeling3dCaptureImageListener;

namespace HmsPlugin
{
    public class Modeling3dReconstructUploadORDownloadORPreviewListener : IModeling3dReconstructUploadListener, IModeling3dReconstructDownloadListener, IModeling3dReconstructPreviewListener
    {
        Action<string, double, AndroidJavaObject> Progress;
        Action<string, int, string> Error;
        Action<string, Modeling3dReconstructUploadResult, AndroidJavaObject> ResultUpload;
        Action<string, Modeling3dReconstructDownloadResult, AndroidJavaObject> ResultDownload;
        Action<string, AndroidJavaObject> ResultPreview;

        public Modeling3dReconstructUploadORDownloadORPreviewListener(Action<string, double, AndroidJavaObject> UploadProgress,
                                                    Action<string, int, string> Error,
                                                     Action<string, Modeling3dReconstructUploadResult, AndroidJavaObject> ResultUpload)
        {
            this.Progress = UploadProgress;
            this.Error = Error;
            this.ResultUpload = ResultUpload;
        }

        public Modeling3dReconstructUploadORDownloadORPreviewListener(Action<string, double, AndroidJavaObject> DownloadProgress,
                                             Action<string, int, string> Error,
                                              Action<string, Modeling3dReconstructDownloadResult, AndroidJavaObject> ResultDownload)
        {
            this.Progress = DownloadProgress;
            this.Error = Error;
            this.ResultDownload = ResultDownload;
        }

        public Modeling3dReconstructUploadORDownloadORPreviewListener(Action<string, int, string> Error,
                                                                Action<string, AndroidJavaObject> ResultPreview)
        {
            this.Error = Error;
            this.ResultPreview = ResultPreview;
        }

        public void onDownloadProgress(string taskId, double progress, AndroidJavaObject javaObject)
        {
            Progress?.Invoke(taskId, progress, javaObject);
        }

        public void onError(string taskId, int errorCode, string errorMessage)
        {
            Error?.Invoke(taskId, errorCode, errorMessage);
        }

        public void onResult(string taskId, Modeling3dReconstructUploadResult result, AndroidJavaObject javaObject)
        {
            ResultUpload?.Invoke(taskId, result, javaObject);
        }

        public void onResult(string taskId, Modeling3dReconstructDownloadResult result, AndroidJavaObject javaObject)
        {
            ResultDownload?.Invoke(taskId, result, javaObject);
        }

        public void onResult(string taskId, AndroidJavaObject javaObject)
        {
            ResultPreview?.Invoke(taskId, javaObject);
        }

        public void onUploadProgress(string taskId, double progress, AndroidJavaObject javaObject)
        {
            Progress?.Invoke(taskId, progress, javaObject);
        }
    }

    public class Modeling3dCaptureImageListener : IModeling3dCaptureImageListener
    {
        Action<int> Progress;
        Action<int, string> Error;
        Action Result;


        public Modeling3dCaptureImageListener(Action<int> Progress,
                                                    Action<int, string> Error,
                                                     Action Result)
        {
            this.Progress = Progress;
            this.Error = Error;
            this.Result = Result;
        }
        public void onError(int errorCode, string message)
        {
            Error?.Invoke(errorCode, message); ;
        }

        public void onProgress(int progress)
        {
            Progress?.Invoke(progress);
        }

        public void onResult()
        {
            Result?.Invoke();
        }
    }
}
