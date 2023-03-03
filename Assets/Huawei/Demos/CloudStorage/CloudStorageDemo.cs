using HmsPlugin;

using HuaweiMobileServices.CloudStorage;
using HuaweiMobileServices.Utils;

using System;

using UnityEngine;
using UnityEngine.UI;

using static HuaweiMobileServices.CloudStorage.DownloadTask;

public class CloudStorageDemo : MonoBehaviour
{
    private readonly string TAG = "[HMS] HMSCloudStorageDemo";

    StorageTask<DownloadTask.DownloadResult> currentDownloadTask;
    StorageTask<UploadTask.UploadResult> currentUploadTask;

    [SerializeField]
    private Text txtPercentage;

    void Start()
    {
        //Requires modifying manifest file https://evilminddevs.gitbook.io/hms-unity-plugin/kits-and-services/cloud-storage/guides-and-references#modifying-the-androidmanifest-file
        HMSCloudStorageManager.CheckRequestUserPermissionForCloudStorage();

        HMSCloudStorageManager.Instance.OnDownloadFileSuccess = OnDownloadFileSuccess;
        HMSCloudStorageManager.Instance.OnDownloadFileProgress = OnDownloadFileProgress;
        HMSCloudStorageManager.Instance.OnDownloadPaused = OnDownloadPaused;
        HMSCloudStorageManager.Instance.OnDownloadCompleted = OnDownloadCompleted;
        HMSCloudStorageManager.Instance.OnDownloadCanceled = OnDownloadCanceled;

        HMSCloudStorageManager.Instance.OnUploadFileSuccess = OnUploadFileSuccess;
        HMSCloudStorageManager.Instance.OnUploadFileProgress = OnUploadFileProgress;
        HMSCloudStorageManager.Instance.OnUploadPaused = OnUploadPaused;
        HMSCloudStorageManager.Instance.OnUploadCompleted = OnUploadCompleted;
        HMSCloudStorageManager.Instance.OnUploadCanceled = OnUploadCanceled;

        HMSCloudStorageManager.Instance.OnDeleteFileSuccess = OnDeleteFileSuccess;
        HMSCloudStorageManager.Instance.OnAllFailureListeners = FailureListener;
        HMSCloudStorageManager.Instance.OnGetImageByteArray = OnGetImageByteArray;
        HMSCloudStorageManager.Instance.OnGetImageByteArrayFailure = OnGetImageByteArrayFailure;
    }


    public void UploadFile()
    {
        string filePath =
            System.IO.Path.Combine(Application.persistentDataPath, "testFile.jpg");//example: \HUAWEI P40 lite\Internal storage\Android\data\com.samet.reffapp.huawei\files\testFile.jpg

        filePath = Application.persistentDataPath + "/gameovercanvas";

        currentUploadTask = HMSCloudStorageManager.Instance.UploadFile(filePathInDevice: filePath, "gameovercanvas");
    }

    public void DownloadFile()
    {
        currentDownloadTask = HMSCloudStorageManager.Instance.DownloadFile("testFile.jpg");
    }

    public void DeleteFile()
    {
        var filePathInCloudStorage = "/files/test/testImage.jpg";

        HMSCloudStorageManager.Instance.DeleteFile(filePathInCloudStorage);
    }

    public void Pause()
    {
        if (currentDownloadTask != null)
            currentDownloadTask.Pause();

        if (currentUploadTask != null)
            currentUploadTask.Pause();
    }

    public void Resume()
    {
        if (currentDownloadTask != null)
            currentDownloadTask.Resume();

        if (currentUploadTask != null)
            currentUploadTask.Resume();
    }

    public void Cancel()
    {
        if (currentDownloadTask != null)
            currentDownloadTask.Cancel();

        if (currentUploadTask != null)
            currentUploadTask.Cancel();
    }


    public void GetByteArray()
    {
        var filePathInCloudStorage = "/files/test/testImage.jpg";

        HMSCloudStorageManager.Instance.GetImageByteArray(filePathInCloudStorage);
    }

    #region Callbacks

    private void OnUploadCanceled()
    {
        Debug.Log($"{TAG} Upload Canceled");
        txtPercentage.text = "Upload Canceled";

    }

    private void OnUploadCompleted()
    {
        Debug.Log($"{TAG} Upload Completed");
        txtPercentage.text = "Upload Completed";

    }

    private void OnUploadPaused(float percentage)
    {
        txtPercentage.text = "% " + percentage.ToString();
        Debug.Log($"{TAG} UploadPaused , BytesTransferred percentage % : {percentage}");
    }

    private void OnUploadFileProgress(float percentage)
    {
        txtPercentage.text = "% " + percentage.ToString();
        Debug.Log($"{TAG} UploadFileProgress , BytesTransferred percentage % : {percentage}");
    }

    private void OnDeleteFileSuccess()
    {
        Debug.Log($"{TAG} Delete File Success");
        PrintDescription("Delete File Success");
    }

    private void OnDownloadFileSuccess(DownloadTask.DownloadResult result)
    {
        Debug.Log($"{TAG} Download File Success:" + result.BytesTransferred);
        PrintDescription("Download File Success");
    }

    private void OnUploadFileSuccess(UploadTask.UploadResult result)
    {
        Debug.Log($"{TAG} Upload File Success:" + result.BytesTransferred);
        PrintDescription("Upload File Success");
    }

    private void FailureListener(HMSException exception)
    {
        Debug.LogError($"{TAG} FailureListener:" + exception);
        PrintDescription("Failed:" + exception);
    }

    private void PrintDescription(string text)
    {
        Text desc = GameObject.Find("Description").GetComponent<Text>();
        desc.text = text;
    }

    private void OnGetImageByteArrayFailure(HMSException exception)
    {
        Debug.LogError($"{TAG} OnGetImageByteArrayFailure:" + exception);
    }

    private void OnGetImageByteArray(byte[] imageByteArray)
    {
        Debug.Log($"{TAG} OnGetImageByteArray Success , image byte array length " + imageByteArray.Length);
    }

    private void OnDownloadCompleted(float percentage)
    {
        txtPercentage.text = "% " + percentage.ToString();
        Debug.Log($"{TAG} Download Completed percentage % : {percentage}");
    }

    private void OnDownloadCanceled()
    {
        Debug.Log($"{TAG} Download Canceled");

        txtPercentage.text = "Download Canceled ! ";

    }

    private void OnDownloadPaused(float percentage)
    {
        txtPercentage.text = "% " + percentage.ToString() + "Paused ";
        Debug.Log($"{TAG} Paused , BytesTransferred percentage % : {percentage}");
    }

    private void OnDownloadFileProgress(float percentage)
    {
        txtPercentage.text = "% " + percentage.ToString();
        Debug.Log($"{TAG} Progress , BytesTransferred percentage % : {percentage}");
    }

    #endregion


}


