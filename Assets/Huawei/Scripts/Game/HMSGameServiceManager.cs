using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;
using static HuaweiConstants.HMSResponses;

namespace HmsPlugin
{
    public class HMSGameServiceManager : HMSManagerSingleton<HMSGameServiceManager>, ICheckUpdateCallback
    {
        public Action<Player> OnGetPlayerInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerInfoFailure { get; set; }
        public Action<string> OnSubmitPlayerEventSuccess { get; set; }
        public Action<HMSException> OnSubmitPlayerEventFailure { get; set; }
        public Action<PlayerExtraInfo> OnGetPlayerExtraInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerExtraInfoFailure { get; set; }
        public Action<AuthAccount> SignInSuccess { get; set; }
        public Action<HMSException> SignInFailure { get; set; }
        public Action<OnAppUpdateInfoRes> OnAppUpdateInfo { get; set; }

        private AccountAuthService authService;
        private AccountAuthParams authParams;
        private IBuoyClient buoyClient;
        private IPlayersClient playersClient;
        private IArchivesClient archivesClient;
        private IAppUpdateClient appUpdateClient;

        private bool forceUpdate;
        private bool showUpdateDialog;

        public HMSGameServiceManager()
        {
            Debug.Log($"[HMS] : HMSGameServiceManager Constructor");
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
        }

        private void OnAwake()
        {
            HuaweiMobileServicesUtil.SetApplication();
            if (HMSGameServiceSettings.Instance.Settings.GetBool(HMSGameServiceSettings.InitializeOnStart))
                Init();
            else
                Debug.LogWarning("[HMS] initOnStart is disable for GameService. Are you getting init first error? Call HMSGameServiceManager.Instance.Init() by yourself.");
        }

        public void Init()
        {
            Debug.Log("HMS GAMES init");
            authService = HMSAccountKitManager.Instance.GetGameAuthService();
            authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM_GAME).CreateParams();

            ITask<AuthAccount> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                archivesClient = HMSSaveGameManager.Instance.GetArchivesClient();
                InitJosApps(result);
                SignInSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("HMS GAMES: The app has not been authorized");
                authService.StartSignIn((auth) => { InitJosApps(auth); SignInSuccess?.Invoke(auth); }, (exc) => SignInFailure?.Invoke(exc));
                InitGameManagers();
            });
        }

        private void InitJosApps(AuthAccount result)
        {
            var appParams = new AppParams(authParams);
            HMSAccountKitManager.Instance.HuaweiId = result;
            Debug.Log("HMS GAMES: Setted app");
            IJosAppsClient josAppsClient = JosApps.GetJosAppsClient();
            Debug.Log("HMS GAMES: jossClient");
            josAppsClient.Init(appParams);

            Debug.Log("HMS GAMES: jossClient init");
            InitGameManagers();
        }

        public void InitGameManagers()
        {
            HMSSaveGameManager.Instance.SavedGameAuth();
            HMSLeaderboardManager.Instance.rankingsClient = Games.GetRankingsClient();
            HMSAchievementsManager.Instance.achievementsClient = Games.GetAchievementsClient();

            playersClient = Games.GetPlayersClient();
            archivesClient = Games.GetArchiveClient();
            buoyClient = Games.GetBuoyClient();
            appUpdateClient = JosApps.GetAppUpdateClient();
            CheckAppUpdate(true, false);
        }

        public void CheckAppUpdate(bool showAppUpdate, bool forceAppUpdate)
        {
            if (appUpdateClient != null)
            {
                showUpdateDialog = showAppUpdate;
                forceUpdate = forceAppUpdate;
                appUpdateClient.CheckAppUpdate(this);
            }
            else
            {
                Debug.LogError("[HMSGameManager] CheckAppUpdate AppUpdateClient is null.");
            }
        }

        public void ShowFloatWindow()
        {
            if (buoyClient != null)
                buoyClient.ShowFloatWindow();
            else
                Debug.LogError("[HMSGameManager] ShowFloatWindow BuoyClient is null.");
        }

        public void HideFloatWindow()
        {
            if (buoyClient != null)
                buoyClient.HideFloatWindow();
            else
                Debug.LogError("[HMSGameManager] HideFloatWindow BuoyClient is null.");
        }

        public void GetPlayerInfo()
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                ITask<Player> task = playersClient.CurrentPlayer;
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log("[HMSGameManager] GetPlayerInfo Success");
                    OnGetPlayerInfoSuccess?.Invoke(result);

                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError("[HMSGameManager]: GetPlayerInfo failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnGetPlayerInfoFailure?.Invoke(exception);
                });
            }
            else 
            {
                Debug.LogError("[HMSGameManager]: GetPlayerInfo failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetPlayerInfoFailure?.Invoke(new HMSException("GetPlayerInfo failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void SubmitPlayerEvent(string playerId, string eventId, string eventType)
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                var task = playersClient.SubmitPlayerEvent(playerId, eventId, eventType);
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log("[HMSGameManager] SubmitPlayerEvent Success");
                    OnSubmitPlayerEventSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError("[HMSGameManager]: SubmitPlayerEvent failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnSubmitPlayerEventFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError("[HMSGameManager]: SubmitPlayerEvent failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetPlayerInfoFailure?.Invoke(new HMSException("SubmitPlayerEvent failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void GetPlayerExtraInfo(string transactionId)
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                var task = playersClient.GetPlayerExtraInfo(transactionId);
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log("[HMSGameManager] GetPlayerExtraInfo Success");
                    OnGetPlayerExtraInfoSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError("[HMSGameManager]: GetPlayerExtraInfo failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnGetPlayerExtraInfoFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError("[HMSGameManager]: GetPlayerExtraInfo failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetPlayerInfoFailure?.Invoke(new HMSException("GetPlayerExtraInfo failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void OnUpdateInfo(AndroidIntent intent)
        {
            int status = intent.GetIntExtra("status");
            int rtnCode = intent.GetIntExtra("failcause");
            string rtnMessage = intent.GetStringExtra("failreason");
            bool isExit = intent.GetBoolExtra("compulsoryUpdateCancel", false);
            int buttonStatus = intent.GetIntExtra("buttonstatus");

            var apkUpgradeInfo = intent.Intent.Call<AndroidJavaObject>("getSerializableExtra", "updatesdk_update_info");
            if (apkUpgradeInfo != null && showUpdateDialog)
            {
                appUpdateClient.ShowUpdateDialog(apkUpgradeInfo, forceUpdate);
            }
            OnAppUpdateInfo?.Invoke(new OnAppUpdateInfoRes(status, rtnCode, rtnMessage, isExit, buttonStatus));

            AppUpdateStatusCode _statusCode = (AppUpdateStatusCode)Enum.Parse(typeof(AppUpdateStatusCode), status.ToString());
            AppUpdateRtnCode _rtnCode = (AppUpdateRtnCode)Enum.Parse(typeof(AppUpdateRtnCode), rtnCode.ToString());
            AppUpdateButtonStatus _buttonStatus = (AppUpdateButtonStatus)Enum.Parse(typeof(AppUpdateButtonStatus), buttonStatus.ToString());

            Debug.Log("[HMSGameManager] OnUpdateInfo, status: " + _statusCode + ", rtnCode: " + _rtnCode + ", rtnMessage: " + rtnMessage + ", buttonStatus: " + _buttonStatus + ", isExit: " + isExit);
        }

        public void OnMarketInstallInfo(AndroidIntent intent)
        {
            Debug.Log("[HMSGameManager] OnMarketInstallInfo Called");
        }

        public void OnMarketStoreError(int responseCode)
        {
            Debug.LogError($"[HMSGameManager] OnMarketStoreError. Response Code:{responseCode}");
        }

        public void OnUpdateStoreError(int responseCode)
        {
            Debug.LogError($"[HMSGameManager] OnUpdateStoreError. Response Code:{responseCode}");
        }

        public class OnAppUpdateInfoRes
        {
            public OnAppUpdateInfoRes(int status, int rtnCode, string rtnMessage, bool isExit, int buttonStatus)
            {
                Status = status;
                RtnCode = rtnCode;
                RtnMessage = rtnMessage;
                IsExit = isExit;
                ButtonStatus = buttonStatus;
            }

            public int Status { get; set; }
            public int RtnCode { get; set; }
            public string RtnMessage { get; set; }
            public bool IsExit { get; set; }
            public int ButtonStatus { get; set; }
        }
    }
}