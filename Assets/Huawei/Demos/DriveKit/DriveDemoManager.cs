using System;
using System.Collections.Generic;
using HmsPlugin;
using HuaweiMobileServices.Drive;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class DriveDemoManager : MonoBehaviour
{
    private readonly string TAG = "[HMS] DriveDemoManager ";

    public Text description;

    void Start()
    {
        HMSAccountKitManager.Instance.OnSignInSuccess = OnSignInSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnSignInFailed;

        HMSAccountKitManager.Instance.SignInDrive(AccountAuthManager.GetService(GetAccountAuthParams()));

        description.text = string.Empty;
    }

    private void OnSignInFailed(HMSException exception)
    {
        Debug.LogError(TAG+"OnSignInFailed HMSException:" + exception.WrappedExceptionMessage);
    }

    private void OnSignInSuccess(AuthAccount authAccount)
    {
        Debug.Log(TAG+"OnSignInSuccess");
        if (CredentialManager.GetInstance().InitDrive(authAccount)) 
        {
            description.text = "Drive init success";
        }
        else 
        {
            description.text = "Drive init failed";
        }
    }

    public void GetAboutOnClick()
    {
        About about = HMSDriveKitManager.Instance.GetAbout();
        description.text = (about == null) ? "GetAbout Failed" : "GetAbout Success";
    }

    public void CreateDirectoryOnClick()
    {
        File file = HMSDriveKitManager.Instance.CreateDirectory();
        description.text = (file == null) ? "CreateDirectory Failed" : "CreateDirectory Success file.ID:" + file.GetId();
    }

    public void CreateFileOnClick()
    {
        string fileName = "testFile.txt";
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        File file = HMSDriveKitManager.Instance.CreateFiles(MimeType.MimeTypeFromSuffix(".txt"), filePath);
        description.text = (file == null) ? "CreateFiles Failed" : "CreateFiles Success file.ID:" + file.GetId();
    }

    public void ListCommentsOnClick()
    {
        List<Comment> commentList = HMSDriveKitManager.Instance.ListComments();
        description.text = (commentList.Count == 0) ? "ListComments Failed or there is no comment" : "ListComments Success commentList.Count:" + commentList.Count;
    }

    public void CreateCommentsOnClick()
    {
        Comment comment = HMSDriveKitManager.Instance.CreateComments();
        description.text = (comment == null) ? "CreateComments Failed" : "CreateComments Success comment.ID:" + comment.GetId();
    }

    public AccountAuthParams GetAccountAuthParams()
    {
        List<Scope> scopeList = new List<Scope>
        {
            new Scope(Drive.DriveScopes.SCOPE_DRIVE_FILE),
            new Scope(Drive.DriveScopes.SCOPE_DRIVE_APPDATA)
        };

        AccountAuthParams authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM)
            .SetAccessToken()
            .SetIdToken()
            .SetScopeList(scopeList)
            .CreateParams();

        description.text = "authParams is null -> " + (authParams == null).ToString() + "\n";
        return authParams;
    }

}
