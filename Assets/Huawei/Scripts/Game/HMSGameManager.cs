using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSGameManager : HMSSingleton<HMSGameManager>
    {
        public Action<HuaweiMobileServices.Game.Player> OnGetPlayerInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerInfoFailure { get; set; }
        public Action<string> OnSubmitPlayerEventSuccess { get; set; }
        public Action<HMSException> OnSubmitPlayerEventFailure { get; set; }
        public Action<HuaweiMobileServices.Game.PlayerExtraInfo> OnGetPlayerExtraInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerExtraInfoFailure { get; set; }
        public Action<AuthAccount> SignInSuccess { get; set; }
        public Action<HMSException> SignInFailure { get; set; }

        private AccountAuthService authService;
        private IBuoyClient buoyClient;
        private IPlayersClient playersClient;
        private IArchivesClient archivesClient;

        public void Start()
        {
            HuaweiMobileServicesUtil.SetApplication();
            if (HMSGameServiceSettings.Instance.Settings.GetBool(HMSGameServiceSettings.InitializeOnStart))
                Init();
        }

        public void Init()
        {
            Debug.Log("HMS GAMES init");
            authService = HMSAccountManager.Instance.GetGameAuthService();

            ITask<AuthAccount> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                HMSAccountManager.Instance.HuaweiId = result;
                Debug.Log("HMS GAMES: Setted app");
                IJosAppsClient josAppsClient = JosApps.GetJosAppsClient();
                Debug.Log("HMS GAMES: jossClient");
                josAppsClient.Init();
                Debug.Log("HMS GAMES: jossClient init");
                InitGameManagers();
                SignInSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {

                Debug.Log("HMS GAMES: The app has not been authorized");
                authService.StartSignIn(SignInSuccess, SignInFailure);
                InitGameManagers();
            });
        }

        public void InitGameManagers()
        {
            HMSSaveGameManager.Instance.SavedGameAuth();
            HMSLeaderboardManager.Instance.rankingsClient = Games.GetRankingsClient();
            HMSAchievementsManager.Instance.achievementsClient = Games.GetAchievementsClient();

            playersClient = Games.GetPlayersClient();
            archivesClient = Games.GetArchiveClient();
            buoyClient = Games.GetBuoyClient();
        }

        public void ShowFloatWindow()
        {
            buoyClient.ShowFloatWindow();
        }

        public void HideFloatWindow()
        {
            buoyClient.HideFloatWindow();
        }

        public void GetPlayerInfo()
        {
            if (HMSAccountManager.Instance.HuaweiId != null)
            {
                ITask<HuaweiMobileServices.Game.Player> task = playersClient.CurrentPlayer;
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
        }

        public void SubmitPlayerEvent(string playerId, string eventId, string eventType)
        {
            if (HMSAccountManager.Instance.HuaweiId != null)
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
        }

        public void GetPlayerExtraInfo(string transactionId)
        {
            if (HMSAccountManager.Instance.HuaweiId != null)
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
        }
    }
}