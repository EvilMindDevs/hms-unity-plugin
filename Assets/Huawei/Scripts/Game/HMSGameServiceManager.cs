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
        private const string TAG = "[HMS] HMSGameServiceManager";
        public Action<Player> OnGetPlayerInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerInfoFailure { get; set; }
        public Action<string> OnSubmitPlayerEventSuccess { get; set; }
        public Action<HMSException> OnSubmitPlayerEventFailure { get; set; }
        public Action<PlayerExtraInfo> OnGetPlayerExtraInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerExtraInfoFailure { get; set; }
        public Action<AuthAccount> SignInSuccess { get; set; }
        public Action<HMSException> SignInFailure { get; set; }
        public Action<OnAppUpdateInfoRes> OnAppUpdateInfo { get; set; }
        public Action<int> OnGetUserPlayStateSuccess { get; set; }
        public Action<HMSException> OnGetUserPlayStateFailure { get; set; }
        public Action<bool> OnIsAllowContinuePlayGamesSuccess { get; set; }
        public Action<HMSException> OnIsAllowContinuePlayGamesFailure { get; set; }
        public Action<HuaweiMobileServices.Utils.Void> GameInitSuccess { get; set; }

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
            HMSManagerStart.Start(OnAwake, TAG);
        }

        private void OnAwake()
        {
            HuaweiMobileServicesUtil.SetApplication();
            if (HMSGameServiceSettings.Instance.Settings.GetBool(HMSGameServiceSettings.InitializeOnStart))
                Init();
            else
                Debug.LogWarning($"{TAG} initOnStart is disable for GameService. Are you getting init first error? Call HMSGameServiceManager.Instance.Init() by yourself.");
        }

        public void Init(IAntiAddictionCallback antiAddictionCallback = null, AuthAccount authAccount = null)
        {
            Debug.Log($"{TAG}: HMS GAMES init");

            if (authAccount != null) 
            {
                archivesClient = HMSSaveGameManager.Instance.GetArchivesClient();
                InitJosApps(authAccount, antiAddictionCallback);
                return;
            }
            authService = HMSAccountKitManager.Instance.GetGameAuthService();
            ITask<AuthAccount> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                archivesClient = HMSSaveGameManager.Instance.GetArchivesClient();
                InitJosApps(result, antiAddictionCallback);
                SignInSuccess?.Invoke(result);
                Debug.Log($"{TAG} HMS GAMES: SilentSignIn Success");

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"{TAG} HMS GAMES: The app has not been authorized");
                authService.StartSignIn((auth) => { InitJosApps(auth, antiAddictionCallback); SignInSuccess?.Invoke(auth); }, (exc) => SignInFailure?.Invoke(exc));
                InitGameManagers();
            });
        }

        private void InitJosApps(AuthAccount result, IAntiAddictionCallback antiAddictionCallback = null)
        {
            authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM_GAME).CreateParams();

            AppParams appParams = (antiAddictionCallback != null) 
                ? new AppParams(authParams, new AntiAddictionCallWrapper(antiAddictionCallback)) 
                : new AppParams(authParams);

            HMSAccountKitManager.Instance.HuaweiId = result;
            Debug.Log($"{TAG} HMS GAMES: Setted app");
            IJosAppsClient josAppsClient = JosApps.GetJosAppsClient();
            Debug.Log($"{TAG} HMS GAMES: jossClient");
            var init = josAppsClient.Init(appParams);

            init.AddOnSuccessListener((aVoid) => 
            {
                GameInitSuccess?.Invoke(aVoid);
            });

            Debug.Log($"{TAG} HMS GAMES: jossClient init");
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
                Debug.LogError($"{TAG} CheckAppUpdate AppUpdateClient is null.");
            }
        }

        public void ShowFloatWindow()
        {
            if (buoyClient != null)
                buoyClient.ShowFloatWindow();
            else
                Debug.LogError($"{TAG} ShowFloatWindow BuoyClient is null.");
        }

        public void HideFloatWindow()
        {
            if (buoyClient != null)
                buoyClient.HideFloatWindow();
            else
                Debug.LogError($"{TAG} HideFloatWindow BuoyClient is null.");
        }

        public void GetPlayerInfo()
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                ITask<Player> task = playersClient.GamePlayer;
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log($"{TAG} GetPlayerInfo Success");
                    OnGetPlayerInfoSuccess?.Invoke(result);

                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError($"{TAG}: GetPlayerInfo failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnGetPlayerInfoFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError($"{TAG}: GetPlayerInfo failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetPlayerInfoFailure?.Invoke(new HMSException($"{TAG}: GetPlayerInfo failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void SubmitPlayerEvent(string playerId, string eventId, string eventType)
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                var task = playersClient.SubmitPlayerEvent(playerId, eventId, eventType);
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log($"{TAG} SubmitPlayerEvent Success");
                    OnSubmitPlayerEventSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError($"{TAG}: SubmitPlayerEvent failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnSubmitPlayerEventFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError($"{TAG}: SubmitPlayerEvent failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetPlayerInfoFailure?.Invoke(new HMSException($"{TAG}: SubmitPlayerEvent failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void GetPlayerExtraInfo(string transactionId)
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                var task = playersClient.GetPlayerExtraInfo(transactionId);
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log($"{TAG}: GetPlayerExtraInfo Success");
                    OnGetPlayerExtraInfoSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError($"{TAG}: GetPlayerExtraInfo failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnGetPlayerExtraInfoFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError($"{TAG}: GetPlayerExtraInfo failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetPlayerInfoFailure?.Invoke(new HMSException("GetPlayerExtraInfo failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void GetUserPlayState()
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                var task = playersClient.UserPlayState;
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log($"{TAG}: GetUserPlayState Success");
                    OnGetUserPlayStateSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError($"{TAG}: GetUserPlayState failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnGetUserPlayStateFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError($"{TAG}: GetUserPlayState failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnGetUserPlayStateFailure?.Invoke(new HMSException("GetUserPlayState failed. HMSAccountKitManager.Instance.HuaweiId is null"));
            }
        }

        public void IsAllowContinuePlayGames()
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                var task = playersClient.IsAllowContinuePlayGames;
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log($"{TAG}: IsAllowContinuePlayGames Success");
                    OnIsAllowContinuePlayGamesSuccess?.Invoke(result);
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError($"{TAG}: IsAllowContinuePlayGames failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnIsAllowContinuePlayGamesFailure?.Invoke(exception);
                });
            }
            else
            {
                Debug.LogError($"{TAG}: IsAllowContinuePlayGames failed. HMSAccountKitManager.Instance.HuaweiId is null");
                OnIsAllowContinuePlayGamesFailure?.Invoke(new HMSException("IsAllowContinuePlayGames failed. HMSAccountKitManager.Instance.HuaweiId is null"));
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

            try
            {
                Debug.Log($"{TAG} OnUpdateInfo, status: {(AppUpdateStatusCode)status}," +
                    $" rtnCode: {(AppUpdateRtnCode)rtnCode}, rtnMessage: {rtnMessage}, buttonStatus: {(AppUpdateButtonStatus)buttonStatus}, isExit: {isExit}");
            }
            catch(Exception e)
            {
                Debug.LogError($"{TAG} Failed to parse status, rtnCode, or buttonStatus as their respective enums exception:{e.Message}");
            }
        }
        public void OnMarketInstallInfo(AndroidIntent intent)
        {
            Debug.Log($"{TAG}: OnMarketInstallInfo Called");
        }

        public void OnMarketStoreError(int responseCode)
        {
            Debug.LogError($"{TAG} OnMarketStoreError. Response Code:{responseCode}");
        }

        public void OnUpdateStoreError(int responseCode)
        {
            Debug.LogError($"{TAG} OnUpdateStoreError. Response Code:{responseCode}");
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
