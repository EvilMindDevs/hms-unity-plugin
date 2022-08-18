using HuaweiMobileServices.Base;
using HuaweiMobileServices.CloudStorage;
using HuaweiMobileServices.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HuaweiMobileServices.CloudStorage.UploadTask;

public class CloudStorageDemo : MonoBehaviour
{
    private readonly string TAG = "[HMS] HMSCloudStorageManager ";
    private AGCStorageManagement mAGCStorageManagement;

    // Start is called before the first frame update
    void Start()
    {
        InitAGCStorageManagement();
        uploadFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitAGCStorageManagement()
    {
        mAGCStorageManagement = AGCStorageManagement.GetInstance();
        Debug.Log(TAG + "2");
    }


    private void uploadFile()
    {
        if (mAGCStorageManagement == null)
        {
            InitAGCStorageManagement();
            Debug.Log(TAG + "1");
        }
        Debug.Log(TAG + "3");
        string fileName = "testFile.jpg";
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        HuaweiMobileServices.Utils.java.io.File file = new HuaweiMobileServices.Utils.java.io.File(filePath);
        Debug.Log(TAG + "4");

        if (file.Exists())
        {
            Debug.LogError(TAG + "createFile error, filePath notExists. fileName:" + filePath);
            return;
        }
        Debug.Log(TAG + "5");

        StorageReference storageReference = mAGCStorageManagement.GetStorageReference(Application.persistentDataPath);
        Debug.Log(TAG + "6");
        UploadTask uploadTask = storageReference.PutFile(file);
        Debug.Log(TAG + "7");
        
        Debug.Log(TAG + "7a");
       // IOnSuccessListener<UploadResult> a = new IOnSuccessListener<UploadResult>()
        
        Debug.Log(TAG + "7b");
        uploadTask.AddOnSuccessListener(Result => 
        {
            Debug.Log(TAG + "[HMS] !!!!!!!!!! AddOnSuccessListener");
        }); ;
        Debug.Log(TAG + "8");
        uploadTask.AddOnFailureListener(result =>
        {
            Debug.Log(TAG + "[HMS] !!!!!!!!!! AddOnFailureListenerSUCCESS");
        });


    }


    
}


    