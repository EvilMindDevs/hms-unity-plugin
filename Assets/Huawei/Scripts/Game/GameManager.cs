using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System;

public class GameManager : MonoBehaviour
{

    public bool achievements;
    public bool leaderboards;
    public bool customUnit;

    AuthHuaweiId authHuaweiId;
    IAchievementsClient achievementsClient;
    IRankingsClient rankingsClient;

    void Start()
    {
        Debug.Log("HMS GAMES: Game init");
        HuaweiMobileServicesUtil.SetApplication();
        SignIn();
    }

    public void Init(AuthHuaweiId authHuaweiId)
    {
        Debug.Log("HMS GAMES init");
        Debug.Log("HMS GAMES: Setted app");
        IJosAppsClient josAppsClient = JosApps.GetJosAppsClient(authHuaweiId);
        Debug.Log("HMS GAMES: jossClient");
        josAppsClient.Init();
        Debug.Log("HMS GAMES: jossClient init");
    }

    private void initLeaderboards()
    {
        Debug.Log("HMS GAMES: Achievements init");
        rankingsClient = Games.GetRankingsClient(authHuaweiId);

    }

    private void initAchievements() {
        Debug.Log("HMS GAMES: Achievements init");
        achievementsClient = Games.GetAchievementsClient(authHuaweiId);
    }


    public void SignIn()
    {
        
        HuaweiIdAuthParams authParams = new HuaweiIdAuthParamsHelper(HuaweiIdAuthParams.DEFAULT_AUTH_REQUEST_PARAM_GAME).CreateParams();

        HuaweiIdAuthManager.GetService(authParams).StartSignIn(
            (result) =>
            {
                Debug.Log("[HMSP:] SingIn Success");
                authHuaweiId = result;
                Debug.Log("[HMSP:] huaweiId:" + authHuaweiId);
                Init();

                if (achievements)
                {
                    initAchievements();
                }

                if (leaderboards)
                {
                    initLeaderboards();
                }

            }
        , (exception) =>
        {
            Debug.Log("[HMSP:] SignIn Failed");

        });
        
    }

    public void GetPlayerInfo()
    {
        IPlayersClient playersClient = Games.GetPlayersClient(authHuaweiId);
        ITask<Player> task = playersClient.CurrentPlayer;

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMSP:] GetPlayerInfo Success");
            
            Debug.Log("[HMSP:] player:" + result.ToString());


        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMSP:] GetPlayerInfo Failed");

        });
    }

    /******************  LEADERBOARDS  ********************/

    public void IsUserScoreShownOnLeaderboards()
    {
        ITask<int> task = rankingsClient.GetRankingSwitchStatus();

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] isUserScoreShownOnLeaderboards SUCCESS" + result);

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] isUserScoreShownOnLeaderboards ERROR");
        });

    }

    public void SetUserScoreShownOnLeaderboards(int active)
    {
        ITask<int> task = rankingsClient.SetRankingSwitchStatus(1);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] SetUserScoreShownOnLeaderboards SUCCESS" + result);

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] SetUserScoreShownOnLeaderboards ERROR");
        });
    }

    public void SubmitScore(string leaderboardId, long score, string scoreTips)
    {
        if (customUnit)
        {
            ITask<ScoreSubmissionInfo> scoreTask = rankingsClient.SubmitScoreWithResult(leaderboardId, score, scoreTips);
        }
        else
        {
            ITask<ScoreSubmissionInfo> scoreTask = rankingsClient.SubmitScoreWithResult(leaderboardId, score);
        }
        
    }

    public void ShowLeaderboards()
    {
        rankingsClient.ShowTotalRankings(() =>
        {
            Debug.Log("[HMS GAMES] ShowLeaderboards SUCCESS");


        }, (exception) =>
        {
            Debug.Log("[HMS GAMES] ShowLeaderboards ERROR");
        });

       
    }

    public void GetLeaderboardsData()
    {
        ITask<IList<Ranking>> task = rankingsClient.GetRankingSummary(true);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] GetLeaderboardsData SUCCESS");

            foreach (Ranking ranking in result)
            {
                Debug.Log("[HMS GAMES] Received " + ranking.RankingDisplayName + "data");
            }

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] GetLeaderboardsData ERROR");
        });
    }

    public void GetLeaderboardData(string leaderboardId)
    {
        ITask<Ranking> task = rankingsClient.GetRankingSummary(leaderboardId,true);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] GetLeaderboardsData SUCCESS");

            Debug.Log("[HMS GAMES] Received " + result.RankingDisplayName + "data");

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] GetLeaderboardsData ERROR");
        });
    }

    public void GetScoresFromLeaderboard(string leaderboardId, int timeDimension, int maxResults, int offsetPlayerRank, int pageDirection)
    {

        ITask<RankingScores> task
        = rankingsClient.GetRankingTopScores(leaderboardId, timeDimension, maxResults, offsetPlayerRank, pageDirection);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] GetScoresFromLeaderboard SUCCESS");


        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] GetScoresFromLeaderboard ERROR");
        });
    }
        
}
