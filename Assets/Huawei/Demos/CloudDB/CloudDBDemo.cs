using HmsPlugin;

using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.CloudDB;
using HuaweiMobileServices.Common;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;

public class CloudDBDemo : MonoBehaviour
{
    private string TAG = "[HMS] CloudDBDemo";
    private HMSAuthServiceManager authServiceManager = null;
    private AGConnectUser user = null;

    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGGED_IN_ANONYMOUSLY = "Anonymously Logged In";
    private const string LOGIN_ERROR = "Error or cancelled login";

    private HMSCloudDBManager cloudDBManager = null;
    private readonly string cloudDBZoneName = "QuickStartDemo";
    private readonly string BookInfoClass = "com.clouddbdemo.kb.huawei.BookInfo";
    private readonly string ObjectTypeInfoHelper = "com.clouddbdemo.kb.huawei.ObjectTypeInfoHelper";
    List<BookInfo> bookInfoList = null;

    public static Action<string> CloudDBDemoLog;

    public Action<CloudDBZoneSnapshot<BookInfo>> OnCloudDBZoneSnapshot { get; set; }
    public Action<AGConnectCloudDBException> OnCloudDBZoneSnapshotException { get; set; }

    #region Singleton

    public static CloudDBDemo Instance { get; private set; }
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

    public void Start()
    {
        CloudDBDemoLog?.Invoke(NOT_LOGGED_IN);

        authServiceManager = HMSAuthServiceManager.Instance;
        authServiceManager.OnSignInSuccess = OnAuthSericeSignInSuccess;
        authServiceManager.OnSignInFailed = OnAuthSericeSignInFailed;

        if (authServiceManager.GetCurrentUser() != null)
        {
            user = authServiceManager.GetCurrentUser();

            string log = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
            CloudDBDemoLog?.Invoke(log);
        }
        else
        {
            SignInWithHuaweiAccount();
        }

        cloudDBManager = HMSCloudDBManager.Instance;
        cloudDBManager.Initialize();
        cloudDBManager.GetInstance(AGConnectInstance.GetInstance(), AGConnectAuth.GetInstance());
    }

    private void OnAccountKitLoginSuccess(AuthAccount authHuaweiId)
    {
        AGConnectAuthCredential credential = HwIdAuthProvider.CredentialWithToken(authHuaweiId.AccessToken);
        authServiceManager.SignIn(credential);
    }

    public void SignInWithHuaweiAccount()
    {
        HMSAccountKitManager.Instance.OnSignInSuccess = OnAccountKitLoginSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnAuthSericeSignInFailed;
        HMSAccountKitManager.Instance.SignIn();
    }

    private void OnAuthSericeSignInFailed(HMSException error)
    {
        CloudDBDemoLog?.Invoke(LOGIN_ERROR);
    }

    private void OnAuthSericeSignInSuccess(SignInResult signInResult)
    {
        user = signInResult.GetUser();
        string log = user.IsAnonymous() ? LOGGED_IN_ANONYMOUSLY : string.Format(LOGGED_IN, user.DisplayName);
        CloudDBDemoLog?.Invoke(log);
    }

    public void CreateObjectType()
    {
        Debug.Log(TAG + " CreateObjectType");

        cloudDBManager.CreateObjectType(ObjectTypeInfoHelper);
    }

    public void GetCloudDBZoneConfigs()
    {
        Debug.Log(TAG + " GetCloudDBZoneConfigs");

        IList<CloudDBZoneConfig> CloudDBZoneConfigs = cloudDBManager.GetCloudDBZoneConfigs();
        Debug.Log($"{TAG} " + CloudDBZoneConfigs.Count);
    }

    public void OpenCloudDBZone()
    {
        Debug.Log(TAG + " OpenCloudDBZone");

        cloudDBManager.OpenCloudDBZone(cloudDBZoneName, CloudDBZoneConfig.CloudDBZoneSyncProperty.CLOUDDBZONE_CLOUD_CACHE, CloudDBZoneConfig.CloudDBZoneAccessProperty.CLOUDDBZONE_PUBLIC);
    }

    public void OpenCloudDBZone2()
    {
        Debug.Log(TAG + " OpenCloudDBZone2");

        cloudDBManager.OpenCloudDBZone2(cloudDBZoneName, CloudDBZoneConfig.CloudDBZoneSyncProperty.CLOUDDBZONE_CLOUD_CACHE, CloudDBZoneConfig.CloudDBZoneAccessProperty.CLOUDDBZONE_PUBLIC);
    }

    public void EnableNetwork()
    {
        Debug.Log(TAG + " EnableNetwork");

        cloudDBManager.EnableNetwork(cloudDBZoneName);
    }

    public void DisableNetwork()
    {
        Debug.Log(TAG + " DisableNetwork");

        cloudDBManager.DisableNetwork(cloudDBZoneName);
    }

    public void AddBookInfo()
    {
        Debug.Log(TAG + " AddBookInfo");

        BookInfo bookInfo = new BookInfo();
        bookInfo.Id = 1;
        bookInfo.BookName = "bookName";
        bookInfo.Author = "Author 1";
        cloudDBManager.ExecuteUpsert(bookInfo);
    }

    public void AddBookInfoList()
    {
        Debug.Log(TAG + " AddBookInfoList");

        IList<AndroidJavaObject> bookInfoList = new List<AndroidJavaObject>();

        BookInfo bookInfo1 = new BookInfo();
        bookInfo1.Id = 2;
        bookInfo1.Author = "Author 2";
        bookInfoList.Add(bookInfo1.GetObj());

        BookInfo bookInfo2 = new BookInfo();
        bookInfo2.Id = 3;
        bookInfo2.Author = "Author 3";
        bookInfoList.Add(bookInfo2.GetObj());

        cloudDBManager.ExecuteUpsert(bookInfoList);
    }

    public void UpdateBookInfo()
    {
        Debug.Log(TAG + " UpdateBookInfo");

        BookInfo bookInfo = new BookInfo();
        bookInfo.Id = 1;
        bookInfo.BookName = "bookName";
        bookInfo.Author = "Author 1";
        bookInfo.Price = 300;
        cloudDBManager.ExecuteUpsert(bookInfo);
    }

    public void DeleteBookInfo()
    {
        Debug.Log(TAG + " DeleteBookInfo");

        BookInfo bookInfo = new BookInfo();
        bookInfo.Id = 1;
        cloudDBManager.ExecuteDelete(bookInfo);
    }

    public void DeleteBookInfoList()
    {
        Debug.Log(TAG + " DeleteBookInfoList");

        IList<AndroidJavaObject> bookInfoList = new List<AndroidJavaObject>();

        BookInfo bookInfo1 = new BookInfo();
        bookInfo1.Id = 2;
        bookInfo1.Author = "Author 2";
        bookInfoList.Add(bookInfo1.GetObj());

        BookInfo bookInfo2 = new BookInfo();
        bookInfo2.Id = 3;
        bookInfo2.Author = "Author 3";
        bookInfoList.Add(bookInfo2.GetObj());

        cloudDBManager.ExecuteDelete(bookInfoList);
    }

    public void GetBookInfo()
    {
        Debug.Log(TAG + " GetBookInfo");

        CloudDBZoneQuery mCloudQuery = CloudDBZoneQuery.Where(new AndroidJavaClass(BookInfoClass));
        var cloudDBZoneQueryPolicy = CloudDBZoneQuery.CloudDBZoneQueryPolicy.CLOUDDBZONE_LOCAL_ONLY;

        HMSCloudDBManager.Instance.MCloudDBZone.ExecuteQuery<BookInfo>(mCloudQuery, cloudDBZoneQueryPolicy)
                .AddOnSuccessListener(snapshot =>
                {
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteQuery AddOnSuccessListener {snapshot}");

                    OnExecuteQuerySuccess(snapshot);

                }).AddOnFailureListener(exception =>
                {
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");

                    OnExecuteQueryFailed(exception);
                });

    }

    public void GetBookInfo2()
    {
        Debug.Log(TAG + " GetBookInfo2");

        CloudDBZoneQuery mCloudQuery = CloudDBZoneQuery.Where(new AndroidJavaClass(BookInfoClass));
        var cloudDBZoneQueryPolicy = CloudDBZoneQuery.CloudDBZoneQueryPolicy.CLOUDDBZONE_LOCAL_ONLY;

        HMSCloudDBManager.Instance.MCloudDBZone.ExecuteQueryUnsynced<BookInfo>(mCloudQuery)
                .AddOnSuccessListener(snapshot =>
                {
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteQuery AddOnSuccessListener {snapshot}");

                    OnExecuteQuerySuccess(snapshot);

                }).AddOnFailureListener(exception =>
                {
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");

                    OnExecuteQueryFailed(exception);
                });

    }

    #region Query

    private void OnExecuteQueryFailed(HMSException error) => Debug.Log($"{TAG} OnExecuteQueryFailed(HMSException error) => {error.WrappedExceptionMessage}");

    private void OnExecuteQuerySuccess(CloudDBZoneSnapshot<BookInfo> snapshot) => ProcessQueryResult(snapshot);

    private void ProcessQueryResult(CloudDBZoneSnapshot<BookInfo> snapshot)
    {
        CloudDBZoneObjectList<BookInfo> bookInfoCursor = snapshot.GetSnapshotObjects();
        bookInfoList = new List<BookInfo>();
        try
        {
            while (bookInfoCursor.HasNext())
            {
                BookInfo bookInfo = bookInfoCursor.Next();
                bookInfoList.Add(bookInfo);
                Debug.Log($"{TAG} bookInfoCursor.HasNext() {bookInfo.Id}  {bookInfo.Author}");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{TAG} processQueryResult:  Exception => " + e.Message);
        }
        finally
        {
            snapshot.Release();
        }
    }

    #endregion

    public void ExecuteSumQuery()
    {
        Debug.Log(TAG + " ExecuteSumQuery");

        CloudDBZoneQuery mCloudQuery = CloudDBZoneQuery.Where(new AndroidJavaClass(BookInfoClass));
        cloudDBManager.ExecuteSumQuery(mCloudQuery, "price", CloudDBZoneQuery.CloudDBZoneQueryPolicy.CLOUDDBZONE_LOCAL_ONLY);
    }

    public void ExecuteCountQuery()
    {
        Debug.Log(TAG + " ExecuteCountQuery");

        CloudDBZoneQuery mCloudQuery = CloudDBZoneQuery.Where(new AndroidJavaClass(BookInfoClass));
        cloudDBManager.ExecuteCountQuery(mCloudQuery, "price", CloudDBZoneQuery.CloudDBZoneQueryPolicy.CLOUDDBZONE_LOCAL_ONLY);
    }

    public void SubscribeSnapshot(CloudDBZoneQuery cloudDBZoneQuery, CloudDBZoneQuery.CloudDBZoneQueryPolicy cloudDBZoneQueryPolicy)
    {
        if (!HMSCloudDBManager.Instance.IsCloudDBActive)
            return;

        void OnOnCloudDBZoneSnapshot(CloudDBZoneSnapshot<BookInfo> snapshot)
        {
            ProcessQueryResult(snapshot);
        }

        void OnOnCloudDBZoneSnapshotException(AGConnectCloudDBException exception)
        {
            Debug.LogError(exception);
        }

        OnCloudDBZoneSnapshot += OnOnCloudDBZoneSnapshot;
        OnCloudDBZoneSnapshotException += OnOnCloudDBZoneSnapshotException;

        HMSCloudDBManager.Instance.MRegister = HMSCloudDBManager.Instance.MCloudDBZone.SubscribeSnapshot(cloudDBZoneQuery, cloudDBZoneQueryPolicy, OnCloudDBZoneSnapshot, OnCloudDBZoneSnapshotException);
    }

}
