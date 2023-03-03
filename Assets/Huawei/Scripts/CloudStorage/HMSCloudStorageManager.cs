using HuaweiMobileServices.Base;
using HuaweiMobileServices.CloudStorage;
using HuaweiMobileServices.Utils;

using System;

using UnityEngine;
using UnityEngine.Android;

using static HuaweiMobileServices.CloudStorage.DownloadTask;
using static HuaweiMobileServices.CloudStorage.UploadTask;

namespace HmsPlugin
{
    public class HMSCloudStorageManager : HMSManagerSingleton<HMSCloudStorageManager>
    {
        private readonly string TAG = "[HMS] HMSCloudStorageManager";
        private AGCStorageManagement mAGCStorageManagement;

        public Action<HMSException> OnAllFailureListeners { get; set; }

        public Action<UploadResult> OnUploadFileSuccess { get; set; }
        public Action<float> OnUploadFileProgress { get; set; }
        public Action<float> OnUploadPaused { get; set; }
        public Action OnUploadCanceled { get; set; }
        public Action OnUploadCompleted { get; set; }
        public Action<HMSException> OnUploadFileFailure { get; set; }

        public Action<DownloadResult> OnDownloadFileSuccess { get; set; }
        public Action<float> OnDownloadFileProgress { get; set; }
        public Action<float> OnDownloadPaused { get; set; }
        public Action OnDownloadCanceled { get; set; }
        public Action<float> OnDownloadCompleted { get; set; }
        public Action<HMSException> OnDownloadFileFailure { get; set; }

        public Action<FileMetadata> OnGetFileMetadataSuccess { get; set; }
        public Action<HMSException> OnGetFileMetadataFailure { get; set; }

        public Action<FileMetadata> OnUpdateFileMetadataSuccess { get; set; }
        public Action<HMSException> OnUpdateFileMetadataFailure { get; set; }

        public Action<ListResult> OnGetFileListSuccess { get; set; }
        public Action<HMSException> OnGetFileListFailure { get; set; }

        public Action OnDeleteFileSuccess { get; set; }
        public Action<HMSException> OnDeleteFileFailure { get; set; }

        public Action<byte[]> OnGetImageByteArray { get; set; }
        public Action<HMSException> OnGetImageByteArrayFailure { get; set; }

        public HMSCloudStorageManager()
        {
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();

            HMSDispatcher.InvokeAsync(InitAGCStorageManagement);
        }

        private void InitAGCStorageManagement()
        {
            mAGCStorageManagement = AGCStorageManagement.GetInstance();
        }

        //Requires modifying manifest file https://evilminddevs.gitbook.io/hms-unity-plugin/kits-and-services/cloud-storage/guides-and-references#modifying-the-androidmanifest-file
        public static void CheckRequestUserPermissionForCloudStorage()
        {
            if (Permission.HasUserAuthorizedPermission("android.permission.READ_EXTERNAL_STORAGE"))
            {
                Debug.LogWarning("android.permission.READ_EXTERNAL_STORAGE requesting");
                Permission.RequestUserPermission("android.permission.READ_EXTERNAL_STORAGE");
            }

            if (Permission.HasUserAuthorizedPermission("android.permission.WRITE_EXTERNAL_STORAGE"))
            {
                Debug.LogWarning("android.permission.WRITE_EXTERNAL_STORAGE requesting");
                Permission.RequestUserPermission("android.permission.WRITE_EXTERNAL_STORAGE");
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
        }

        //https://developer.huawei.com/consumer/en/codelabsPortal/carddetails/CloudStorage-Android-Hard
        public UploadTask UploadFile(string filePathInDevice, string filePathInCloudStorage)
        {
            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }

            var file = new HuaweiMobileServices.Utils.java.io.File(filePathInDevice);

            if (!file.Exists())
            {
                Debug.LogError($"{TAG} UploadFile error, filePath notExists. filePathWithFileName:" + filePathInDevice);
                return null;
            }

            StorageReference storageReference = mAGCStorageManagement.GetStorageReference(filePathInCloudStorage);
            UploadTask uploadTask = storageReference.PutFile(file);

            uploadTask.AddOnSuccessListener(result =>
            {
                Debug.Log($"{TAG} successfully UploadFile");
                OnUploadFileSuccess?.Invoke((UploadResult)result);
            });

            uploadTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} UploadFile failed:" + exception);
                OnUploadFileFailure?.Invoke(exception);
                OnAllFailureListeners?.Invoke(exception);
            });

            uploadTask.AddOnProgressListener((val) =>
            {
                var _result = (UploadResult)val;
                var percentage = (float)_result.BytesTransferred / (float)_result.TotalByteCount;
                percentage *= 100f;
                OnUploadFileProgress?.Invoke(percentage);
            });

            uploadTask.AddOnPausedListener((val) =>
            {
                var _result = (UploadResult)val;
                var percentage = (float)_result.BytesTransferred / (float)_result.TotalByteCount;
                percentage *= 100f;
                OnUploadPaused?.Invoke(percentage);
            });

            uploadTask.AddOnCanceledListener(() =>
            {
                OnUploadCanceled?.Invoke();
            });

            uploadTask.AddOnCompleteListener((val) =>
            {
                OnUploadCompleted?.Invoke();
            });

            return uploadTask;
        }

        public DownloadTask DownloadFile(string fileName, string filePathInCloudStorage = "", string whereToDownload = "")
        {
            Debug.Log($"{TAG} Download File");

            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }

            string downloadDirectory = System.IO.Path.Combine(Application.persistentDataPath + "", whereToDownload);

            var file = new HuaweiMobileServices.Utils.java.io.File(downloadDirectory + fileName);

            StorageReference storageReference = mAGCStorageManagement.GetStorageReference(filePathInCloudStorage + fileName);
            DownloadTask downloadTask = storageReference.GetFile(file);

            Debug.Log($"filePathInCloudStorage + fileName {filePathInCloudStorage + fileName}");
            Debug.Log($"downloadDirectory + fileName {downloadDirectory + fileName}");


            downloadTask.AddOnSuccessListener(result =>
            {
                var _result = (DownloadResult)result;

                Debug.Log($"{TAG} successfully DownloadFile , {_result.TotalByteCount}");

                OnDownloadFileSuccess?.Invoke((DownloadResult)result);
            });

            downloadTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} DownloadFile failed:" + exception);
                OnDownloadFileFailure?.Invoke(exception);
                OnAllFailureListeners?.Invoke(exception);
            });

            downloadTask.AddOnProgressListener((val) =>
            {
                var _result = (DownloadResult)val;
                var percentage = (float)_result.BytesTransferred / (float)_result.TotalByteCount;
                percentage *= 100f;
                OnDownloadFileProgress?.Invoke(percentage);
            });

            downloadTask.AddOnPausedListener((val) =>
            {
                var _result = (DownloadResult)val;
                var percentage = (float)_result.BytesTransferred / (float)_result.TotalByteCount;
                percentage *= 100f;
                OnDownloadPaused?.Invoke(percentage);
            });

            downloadTask.AddOnCanceledListener(() =>
            {
                OnDownloadCanceled?.Invoke();
            });

            downloadTask.AddOnCompleteListener((val) =>
            {
                var _result = (DownloadResult)val;
                var percentage = (float)_result.BytesTransferred / (float)_result.TotalByteCount;
                percentage *= 100f;
                OnDownloadCompleted?.Invoke(percentage);
            });

            return downloadTask;

        }

        public void GetFileMetadata(string filePathInCloudStorage = "/files/test/testImage.jpg", StorageReference storageReference = null)
        {
            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }
            if (storageReference == null)
            {
                storageReference = mAGCStorageManagement.GetStorageReference(filePathInCloudStorage);
            }

            ITask<FileMetadata> fileMetadataTask = storageReference.GetFileMetadata();
            fileMetadataTask.AddOnSuccessListener(fileMetaData =>
            {
                Debug.Log($"{TAG} successfully GetFileMetadata");
                OnGetFileMetadataSuccess?.Invoke(fileMetaData);
            });
            fileMetadataTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} GetFileMetadata failed:" + exception);
                OnGetFileMetadataFailure?.Invoke(exception);
                OnAllFailureListeners?.Invoke(exception);
            });
        }

        public void UpdateFileMetadata(FileMetadata fMetaData, string filePathInCloudStorage = "/files/test/testImage.jpg")
        {
            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }
            StorageReference storageReference = mAGCStorageManagement.GetStorageReference(filePathInCloudStorage);
            ITask<FileMetadata> fileMetadataTask = storageReference.UpdateFileMetadata(fMetaData);
            fileMetadataTask.AddOnSuccessListener(fileMetaData =>
            {
                Debug.Log($"{TAG} successfully UpdateFileMetadata");
                OnUpdateFileMetadataSuccess?.Invoke(fileMetaData);
            });

            fileMetadataTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} UpdateFileMetadata failed:" + exception);
                OnUpdateFileMetadataFailure?.Invoke(exception);
                OnAllFailureListeners?.Invoke(exception);
            });


        }

        public void GetFileList(string folderPathInCloudStorage = "/files/test/")
        {
            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }
            StorageReference storageReference = mAGCStorageManagement.GetStorageReference(folderPathInCloudStorage);
            ITask<ListResult> listResultTask = null;
            listResultTask = storageReference.List(100);

            listResultTask.AddOnSuccessListener(listResult =>
            {
                Debug.Log($"{TAG} successfully GetFileList");
                OnGetFileListSuccess?.Invoke(listResult);
            });
            listResultTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} GetFileList failed:" + exception);
                OnGetFileListFailure?.Invoke(exception);
                OnAllFailureListeners?.Invoke(exception);
            });
        }

        public void DeleteFile(string filePathInCloudStorage)
        {
            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }
            StorageReference storageReference = mAGCStorageManagement.GetStorageReference(filePathInCloudStorage);
            ITask<HuaweiMobileServices.Utils.Void> deleteTask = storageReference.Delete();

            deleteTask.AddOnSuccessListener(v =>
            {
                Debug.Log($"{TAG} successfully DeleteFile");
                OnDeleteFileSuccess?.Invoke();
            });

            deleteTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} DeleteFile failed:" + exception);
                OnDeleteFileFailure?.Invoke(exception);
                OnAllFailureListeners?.Invoke(exception);
            });
        }

        public void GetImageByteArray(string filePathInCloudStorage, long maxBytes = 99999999)
        {
            StorageReference storageReference = null;

            if (mAGCStorageManagement == null)
            {
                InitAGCStorageManagement();
            }
            if (storageReference == null)
            {
                storageReference = mAGCStorageManagement.GetStorageReference(filePathInCloudStorage);
            }

            var byteArrayTask = storageReference.GetBytes(maxBytes);

            byteArrayTask.AddOnSuccessListener(byteArray =>
            {
                Debug.Log($"{TAG} GetBytes Successfully");
                OnGetImageByteArray?.Invoke(byteArray);
            });
            byteArrayTask.AddOnFailureListener(exception =>
            {
                Debug.LogError($"{TAG} GetFileMetadata failed:" + exception);
                OnGetImageByteArrayFailure?.Invoke(exception);
            });
        }

        private static AndroidJavaClass sJavaClass = new AndroidJavaClass("org.m0skit0.android.hms.unity.storage.StoragePermissions");

        public static void RequestPermission()
        {
            sJavaClass.CallStatic("requestStoragePermissions", new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
            //sJavaClass.CallStatic("requestStoragePermissions", AndroidContext.ApplicationContext);
        }

    }
}
