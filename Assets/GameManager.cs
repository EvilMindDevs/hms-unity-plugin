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

        SignIn();

        initGame();
        

        if (achievements)
        {
            initAchievements();
        }

        if (leaderboards)
        {
            initLeaderboards();
        }

      

        
    }

    void initGame()
    {
        IJosAppsClient josAppsClient = JosApps.GetJosAppsClient(authHuaweiId);

        josAppsClient.Init();
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
        IHuaweiIdAuthService authService = HuaweiIdAuthManager.GetService(authParams);
        ITask<AuthHuaweiId> task = HuaweiIdAuthManager.ParseAuthResultFromIntent(authService.SignInIntent);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMSP:] SingIn Success");
            authHuaweiId = result;
            Debug.Log("[HMSP:] huaweiId:" + authHuaweiId);


        }).AddOnFailureListener((exception) =>
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

    /******************  ACHIEVEMENTS  ********************/
    public void ShowAchievements() {

        AuthHuaweiId authHuaweiId = new AuthHuaweiId();

        IAchievementsClient achievementsClient = Games.GetAchievementsClient(authHuaweiId);

        ITask<AndroidIntent> task = achievementsClient.ShowAchievementListIntent;

        task.AddOnSuccessListener((intent) =>
        {

            Debug.Log("[HMS GAMES:] ShowAchievements SUCCESS");
            // result start ActivityForResult


        }).AddOnFailureListener((exception) =>
        {

            Debug.Log("[HMS GAMES:] ShowAchievements ERROR");

        });

    }

    public void GetAchievementsList()
    {
        ITask<IList<Achievement>> task = achievementsClient.GetAchievementList(true);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] GetAchievementsList SUCCESS");
            foreach (Achievement achievement in result) {
                Debug.Log("Achievement " + achievement.Id + "-" + achievement.DisplayName);
            }
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] GetAchievementsList ERROR");
        });
    }


    public void RevealAchievement(string achievementId)
    {

        ITask<HuaweiMobileServices.Utils.Void> task = achievementsClient.VisualizeWithResult(achievementId);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] RevealAchievement SUCCESS");
            
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] RevealAchievement ERROR");
        });
    }
    public void IncreaseStepAchievement(string achievementId, int stepIncrement)
    {
        ITask<bool> task = achievementsClient.GrowWithResult(achievementId, stepIncrement);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] IncreaseStepAchievement SUCCESS" + result);

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] IncreaseStepAchievement ERROR");
        });
    }
    
    public void SetStepAchievement(string achievementId, int stepsNum)
    {
        ITask<bool> task = achievementsClient.MakeStepsWithResult(achievementId, stepsNum);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] SetStepAchievement SUCCESS" + result);

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] SetStepAchievement ERROR");
        });
    }

    public void UnlockAchievement(string achievementId) {
        ITask<HuaweiMobileServices.Utils.Void> task = achievementsClient.ReachWithResult(achievementId);

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] UnlockAchievements SUCCESS" );

        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMS GAMES] UnlockAchievements ERROR");
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
        ITask<AndroidIntent> task = rankingsClient.TotalRankingsIntent;

        task.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMS GAMES] ShowLeaderboards SUCCESS");

            // ToDo: Launch StartActivity

        }).AddOnFailureListener((exception) =>
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
