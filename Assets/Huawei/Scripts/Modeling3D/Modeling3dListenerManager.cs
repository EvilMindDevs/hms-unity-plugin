using HuaweiMobileServices.Modeling3D.MeterialGenerateSdk.Cloud;
using HuaweiMobileServices.Modeling3D.ObjReconstructSdk.Cloud;
using System;
using UnityEngine;
using static HuaweiMobileServices.Modeling3D.MeterialGenerateSdk.Cloud.Modeling3dTextureDownloadListener;
using static HuaweiMobileServices.Modeling3D.MeterialGenerateSdk.Cloud.Modeling3dTexturePreviewListener;
using static HuaweiMobileServices.Modeling3D.MeterialGenerateSdk.Cloud.Modeling3dTextureUploadListener;
using static HuaweiMobileServices.Modeling3D.ModelingCaptureSdk.Modeling3dCaptureImageListener;

namespace HmsPlugin
{
    public class Modeling3dReconstructUploadORDownloadORPreviewListener : IModeling3dReconstructUploadListener, IModeling3dReconstructDownloadListener, IModeling3dReconstructPreviewListener
    {
        private Action<string, double, AndroidJavaObject> Progress;
        private Action<string, int, string> Error;
        private Action<string, Modeling3dReconstructUploadResult, AndroidJavaObject> ResultUpload;
        private Action<string, Modeling3dReconstructDownloadResult, AndroidJavaObject> ResultDownload;
        private Action<string, AndroidJavaObject> ResultPreview;

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
        private Action<int> Progress;
        private Action<int, string> Error;
        private Action Result;


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

    public class Modeling3dTextureUploadORDownloadORPreviewListener : IModeling3dTextureUploadListener, IModeling3dTextureDownloadListener, IModeling3dTexturePreviewListener
    {
        private Action<string, double, AndroidJavaObject> Progress;
        private Action<string, int, string> Error;
        private Action<string, Modeling3dTextureUploadResult, AndroidJavaObject> UploadResult;
        private Action<string, Modeling3dTextureDownloadResult, AndroidJavaObject> DownloadResult;
        private Action<string, AndroidJavaObject> PreviewResult;

        public Modeling3dTextureUploadORDownloadORPreviewListener(Action<string, double, AndroidJavaObject> Progress,
                                                    Action<string, int, string> Error
                                                    , Action<string, Modeling3dTextureUploadResult, AndroidJavaObject> UploadResult)
        {
            this.Progress = Progress;
            this.Error = Error;
            this.UploadResult = UploadResult;
        }
        public Modeling3dTextureUploadORDownloadORPreviewListener(Action<string, double, AndroidJavaObject> Progress,
                                            Action<string, int, string> Error
                                            , Action<string, Modeling3dTextureDownloadResult, AndroidJavaObject> DownloadResult)
        {
            this.Progress = Progress;
            this.Error = Error;
            this.DownloadResult = DownloadResult;
        }

        public Modeling3dTextureUploadORDownloadORPreviewListener(Action<string, double, AndroidJavaObject> Progress,
                                            Action<string, int, string> Error
                                            , Action<string, AndroidJavaObject> PreviewResult)
        {
            this.Progress = Progress;
            this.Error = Error;
            this.PreviewResult = PreviewResult;
        }
        public void onDownloadProgress(string taskId, double progress, AndroidJavaObject javaObject)
        {
            Progress?.Invoke(taskId, progress, javaObject);
        }

        public void onError(string taskId, int errorCode, string errorMessage)
        {
            Error?.Invoke(taskId, errorCode, errorMessage);
        }

        public void onResult(string taskId, Modeling3dTextureUploadResult result, AndroidJavaObject javaObject)
        {
            UploadResult?.Invoke(taskId, result, javaObject);
        }

        public void onResult(string taskId, Modeling3dTextureDownloadResult result, AndroidJavaObject javaObject)
        {
            DownloadResult?.Invoke(taskId, result, javaObject);
        }

        public void onResult(string taskId, AndroidJavaObject javaObject)
        {
            PreviewResult?.Invoke(taskId, javaObject);
        }

        public void onUploadProgress(string taskId, double progress, AndroidJavaObject javaObject)
        {
            Progress?.Invoke(taskId, progress, javaObject);
        }
        
    }
}
