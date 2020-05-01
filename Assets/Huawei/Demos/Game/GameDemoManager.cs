using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System;

public class GameDemoManager : MonoBehaviour
{

    private bool achievements = true;
    private  bool leaderboards = true;
    private bool customUnit = false;

    
    IAchievementsClient achievementsClient;
    IRankingsClient rankingsClient;

    GameManager gameManager;
    LeaderboardManager leaderboardManager;
    AchievementsManager achievementsManager;

    void Start()
    {
        gameManager = GameManager.Instance;

        leaderboardManager = LeaderboardManager.Instance;

        achievementsManager = AchievementsManager.Instance;
        achievementsManager.OnShowAchievementsSuccess = OnShowAchievementsSuccess;
        achievementsManager.OnShowAchievementsFailure = OnShowAchievementsFailure;
        achievementsManager.OnRevealAchievementSuccess = OnRevealAchievementSuccess;
        achievementsManager.OnRevealAchievementFailure = OnRevealAchievementFailure;
        achievementsManager.OnIncreaseStepAchievementSuccess = OnIncreaseStepAchievementSuccess;
        achievementsManager.OnIncreaseStepAchievementFailure = OnIncreaseStepAchievementFailure;
        achievementsManager.OnUnlockAchievementSuccess = OnUnlockAchievementSuccess;
        achievementsManager.OnUnlockAchievementFailure = OnUnlockAchievementFailure;
    }


    
    // SHOW ACHIEVEMENTS
    public void ShowAchievements() {

        achievementsManager.ShowAchievements();
    }

    private void OnShowAchievementsSuccess()
    {

    }

    private void OnShowAchievementsFailure(HMSException exception)
    {

    }

    // GET ACHIEVEMENT LIST

    public void GetAchievementsList()
    {
        achievementsManager.GetAchievementsList();
        // ToDo callbacks
        // achievementsManager.OnGetAchievementListSuccess = OnGetAchievemenListSuccess;
        // achievementsManager.OnGetAchievementListFailure = OnGetAchievementListFailure;
    }

    private void OnGetAchievemenListSuccess()
    {

    }

    private void OnGetAchievementListFailure()
    {

    }

    // REVEAL ACHIEVEMENT
    public void RevealAchievement(string achievementId)
    {
        achievementsManager.RevealAchievement(achievementId);
    }

    public void OnRevealAchievementSuccess()
    {

    }

    private void OnRevealAchievementFailure(HMSException error)
    {

    }

    // INCREASE STEP ACHIEVEMENT
    public void IncreaseStepAchievement(string achievementId, int stepIncrement = 1)
    {
        achievementsManager.IncreaseStepAchievement(achievementId, stepIncrement);
    }

    private void OnIncreaseStepAchievementSuccess()
    {

    }

    private void OnIncreaseStepAchievementFailure(HMSException error)
    {

    }

    // Set Step Achivement
    public void SetStepAchievement(string achievementId, int stepsNum)
    {
        achievementsManager.SetStepAchievement(achievementId, stepsNum);
    }

    private void OnSetStepAchievementSuccess()
    {

    }

    private void OnSetStepAchievemenFailure(HMSException error)
    {

    }

    // Unlock Achievement

    public void UnlockAchievement(string achievementId) {
        achievementsManager.UnlockAchievement(achievementId);
    }

    private void OnUnlockAchievementSuccess()
    {

    }

    private void OnUnlockAchievementFailure(HMSException error)
    { 

    }

    /******************  LEADERBOARDS  ********************/

    public void IsUserScoreShownOnLeaderboards()
    {

       /// Todo Hay que cambiar en el manager isUser por el getUser
       /// 
       leaderboardManager.IsUserScoreShownOnLeaderboards();
        leaderboardManager.OnIsUserScoreShownOnLeaderboardsSuccess = OnIsUserScoreShownOnLeaderboardsSuccess;
       leaderboardManager.OnIsUserScoreShownOnLeaderboardsFailure = OnIsUserScoreShownOnLeaderboardsFailure;
    }

    private void OnIsUserScoreShownOnLeaderboardsSuccess(int i)
    {

    }

    private void OnIsUserScoreShownOnLeaderboardsFailure(HMSException error)
    {

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
