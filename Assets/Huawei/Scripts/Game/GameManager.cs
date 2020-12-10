using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager GetInstance(string name = "GameManager") => GameObject.Find(name).GetComponent<GameManager>();

        private AccountManager accountManager;
        private SaveGameManager saveGameManager;
        private LeaderboardManager leaderboardManager;

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
            accountManager = AccountManager.GetInstance();
            saveGameManager = SaveGameManager.GetInstance();
            Init();
        }

        private void Init()
        {
            Debug.Log("HMS GAMES init");
            authService = accountManager.GetGameAuthService();

            ITask<AuthHuaweiId> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                accountManager.HuaweiId = result;
                Debug.Log("HMS GAMES: Setted app");
                IJosAppsClient josAppsClient = JosApps.GetJosAppsClient(accountManager.HuaweiId);
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
            saveGameManager.SavedGameAuth();
            saveGameManager.GetArchivesClient();
            //Leaderboard Initilize
            leaderboardManager.rankingsClient = Games.GetRankingsClient(accountManager.HuaweiId);
        }
        public void GetPlayerInfo()
        {
            if (accountManager.HuaweiId != null)
            {
                IPlayersClient playersClient = Games.GetPlayersClient(accountManager.HuaweiId);
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