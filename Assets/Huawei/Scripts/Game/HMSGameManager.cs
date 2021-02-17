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

        // change
        //public static GameManager GetInstance(string name = "GameManager") => GameObject.Find(name).GetComponent<GameManager>();

        public Action<Player> OnGetPlayerInfoSuccess { get; set; }
        public Action<HMSException> OnGetPlayerInfoFailure { get; set; }
        public Action<AuthHuaweiId> SignInSuccess { get; set; }
        public Action<HMSException> SignInFailure { get; set; }

        private HuaweiIdAuthService authService;
        // Make sure user already signed in!
        public void Start()
        {
            Debug.Log("HMS GAMES: Game init");
            HuaweiMobileServicesUtil.SetApplication();
            //saveGameManager = SaveGameManager.GetInstance();
            Init();
        }

        private void Init()
        {
            Debug.Log("HMS GAMES init");
            authService = HMSAccountManager.Instance.GetGameAuthService();

            ITask<AuthHuaweiId> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                HMSAccountManager.Instance.HuaweiId = result;
                Debug.Log("HMS GAMES: Setted app");
                IJosAppsClient josAppsClient = JosApps.GetJosAppsClient(HMSAccountManager.Instance.HuaweiId);
                Debug.Log("HMS GAMES: jossClient");
                josAppsClient.Init();
                Debug.Log("HMS GAMES: jossClient init");
                InitGameMAnagers();

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("HMS GAMES: The app has not been authorized");
                authService.StartSignIn(SignInSuccess,SignInFailure);
                InitGameMAnagers();
            });
        }
        public void InitGameMAnagers()
        {
            //SavedGame Initilize
            HMSSaveGameManager.Instance.SavedGameAuth();
            HMSSaveGameManager.Instance.GetArchivesClient();
            //Leaderboard Initilize
            HMSLeaderboardManager.Instance.rankingsClient = Games.GetRankingsClient(HMSAccountManager.Instance.HuaweiId);
        }
        public void GetPlayerInfo()
        {
            if (HMSAccountManager.Instance.HuaweiId != null)
            {
                IPlayersClient playersClient = Games.GetPlayersClient(HMSAccountManager.Instance.HuaweiId);
                ITask<Player> task = playersClient.CurrentPlayer;
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