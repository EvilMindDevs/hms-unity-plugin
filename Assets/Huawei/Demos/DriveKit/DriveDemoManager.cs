using HmsPlugin;

using HuaweiMobileServices.Drive;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;

public class DriveDemoManager : MonoBehaviour
{
    private readonly string TAG = "[HMS] DriveDemoManager ";

    public static Action<string> DriveKitLog;

    #region Singleton

    public static DriveDemoManager Instance { get; private set; }
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
        HMSAccountKitManager.Instance.OnSignInSuccess = OnSignInSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnSignInFailed;

        HMSAccountKitManager.Instance.SignInDrive(AccountAuthManager.GetService(GetAccountAuthParams()));

        DriveKitLog?.Invoke(string.Empty);
    }

    private void OnSignInFailed(HMSException exception)
    {
        Debug.LogError(TAG + "OnSignInFailed HMSException:" + exception.WrappedExceptionMessage);
    }

    private void OnSignInSuccess(AuthAccount authAccount)
    {
        Debug.Log(TAG + "OnSignInSuccess");
        if (CredentialManager.GetInstance().InitDrive(authAccount))
        {
            DriveKitLog?.Invoke("Drive init success");
        }
        else
        {
            DriveKitLog?.Invoke("Drive init failed");
        }
    }

    public void GetAboutOnClick()
    {
        Debug.Log(TAG + " GetAboutOnClick");

        About about = HMSDriveKitManager.Instance.GetAbout();
        string log = (about == null) ? "GetAbout Failed" : "GetAbout Success";
        DriveKitLog?.Invoke(log);
    }

    public void CreateDirectoryOnClick()
    {
        Debug.Log(TAG + " CreateDirectoryOnClick");

        File file = HMSDriveKitManager.Instance.CreateDirectory();
        string log = (file == null) ? "CreateDirectory Failed" : "CreateDirectory Success file.ID:" + file.GetId();
        DriveKitLog?.Invoke(log);
    }

    public void CreateFileOnClick()
    {
        Debug.Log(TAG + " CreateFileOnClick");

        string fileName = "testFile.txt";
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        File file = HMSDriveKitManager.Instance.CreateFiles(MimeType.MimeTypeFromSuffix(".txt"), filePath);
        string log = (file == null) ? "CreateFiles Failed" : "CreateFiles Success file.ID:" + file.GetId();
        DriveKitLog?.Invoke(log);
    }

    public void ListCommentsOnClick()
    {
        Debug.Log(TAG + " ListCommentsOnClick");

        List<Comment> commentList = HMSDriveKitManager.Instance.ListComments();
        string log = (commentList.Count == 0) ? "ListComments Failed or there is no comment" : "ListComments Success commentList.Count:" + commentList.Count;
        DriveKitLog?.Invoke(log);
    }

    public void CreateCommentsOnClick()
    {
        Debug.Log(TAG + " CreateCommentsOnClick");

        Comment comment = HMSDriveKitManager.Instance.CreateComments();
        string log = (comment == null) ? "CreateComments Failed" : "CreateComments Success comment.ID:" + comment.GetId();
        DriveKitLog?.Invoke(log);
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

        string log = "authParams is null -> " + (authParams == null).ToString() + "\n";
        DriveKitLog?.Invoke(log);

        return authParams;
    }

}
