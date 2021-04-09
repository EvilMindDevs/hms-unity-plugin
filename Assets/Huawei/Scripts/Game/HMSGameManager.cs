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
        public Action<AuthAccount> SignInSuccess { get; set; }
        public Action<HMSException> SignInFailure { get; set; }

        private AccountAuthService authService;

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
            //SavedGame Initilize
            HMSSaveGameManager.Instance.SavedGameAuth();
            HMSSaveGameManager.Instance.GetArchivesClient();
            //Leaderboard Initilize
            HMSLeaderboardManager.Instance.rankingsClient = Games.GetRankingsClient();
            //Achievements Initilize
            HMSAchievementsManager.Instance.achievementsClient = Games.GetAchievementsClient();
        }

        public void GetPlayerInfo()
        {
            if (HMSAccountManager.Instance.HuaweiId != null)
            {
                IPlayersClient playersClient = Games.GetPlayersClient();
                ITask<HuaweiMobileServices.Game.Player> task = playersClient.CurrentPlayer;
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log("[HMSP:] GetPlayerInfo Success");
                    OnGetPlayerInfoSuccess?.Invoke(result);

                }).AddOnFailureListener((exception) =>
                {
                    Debug.Log("[HMSP:] GetPlayerInfo Failed");
                    OnGetPlayerInfoFailure?.Invoke(exception);
                });
            }
        }
    }
}