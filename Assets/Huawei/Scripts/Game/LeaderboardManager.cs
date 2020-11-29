﻿using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class LeaderboardManager : MonoBehaviour
    {

    public static LeaderboardManager GetInstance(string name = "GameManager") => GameObject.Find(name).GetComponent<LeaderboardManager>();

    private AccountManager accountManager;
    public IRankingsClient rankingsClient;

        public Action<int> OnIsUserScoreShownOnLeaderboardsSuccess { get; set; }
        public Action<HMSException> OnIsUserScoreShownOnLeaderboardsFailure { get; set; }

        public Action<int> OnSetUserScoreShownOnLeaderboardsSuccess { get; set; }
        public Action<HMSException> OnSetUserScoreShownOnLeaderboardsFailure { get; set; }

        public Action OnShowLeaderboardsSuccess { get; set; }
        public Action<HMSException> OnShowLeaderboardsFailure { get; set; }

        public Action<IList<Ranking>> OnGetLeaderboardsDataSuccess { get; set; }
        public Action<HMSException> OnGetLeaderboardsDataFailure { get; set; }

        public Action<Ranking> OnGetLeaderboardDataSuccess { get; set; }
        public Action<HMSException> OnGetLeaderboardDataFailure { get; set; }

        public Action<RankingScores> OnGetScoresFromLeaderboardSuccess { get; set; }
        public Action<HMSException> OnGetScoresFromLeaderboardFailure { get; set; }

        public Action<ScoreSubmissionInfo> OnSubmitScoreSuccess { get; set; }
        public Action<HMSException> OnSubmitScoreFailure { get; set; }

        public void Start()
        {

        }

        public void IsUserScoreShownOnLeaderboards()
        {
            ITask<int> task = rankingsClient.GetRankingSwitchStatus();
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] isUserScoreShownOnLeaderboards SUCCESS" + result);
                OnIsUserScoreShownOnLeaderboardsSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS GAMES] isUserScoreShownOnLeaderboards ERROR");
                OnIsUserScoreShownOnLeaderboardsFailure?.Invoke(exception);
            });

        }

        public void SetUserScoreShownOnLeaderboards(int active)
        {
            ITask<int> task = rankingsClient.SetRankingSwitchStatus(1);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] SetUserScoreShownOnLeaderboards SUCCESS" + result);
                OnSetUserScoreShownOnLeaderboardsSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS GAMES] SetUserScoreShownOnLeaderboards ERROR");
                OnSetUserScoreShownOnLeaderboardsFailure?.Invoke(exception);
            });
        }

        public void SubmitScore(string leaderboardId, long score)
        {
            ITask<ScoreSubmissionInfo> task = rankingsClient.SubmitScoreWithResult(leaderboardId, score);
            task.AddOnSuccessListener((scoreInfo) =>
            {
                OnSubmitScoreSuccess?.Invoke(scoreInfo);
            }).AddOnFailureListener((error) =>
            {
                OnSubmitScoreFailure?.Invoke(error);
            });
        }

        public void SubmitScore(string leaderboardId, long score, string scoreTips)
        {
            ITask<ScoreSubmissionInfo> task = rankingsClient.SubmitScoreWithResult(leaderboardId, score, scoreTips);
            task.AddOnSuccessListener((scoreInfo) =>
            {
                OnSubmitScoreSuccess?.Invoke(scoreInfo);
            }).AddOnFailureListener((error) =>
            {
                OnSubmitScoreFailure?.Invoke(error);
            });
        }

        public void ShowLeaderboards()
        {
            rankingsClient.ShowTotalRankings(() =>
            {
                Debug.Log("[HMS GAMES] ShowLeaderboards SUCCESS");
                OnShowLeaderboardsSuccess?.Invoke();

            }, (exception) =>
            {
                Debug.Log("[HMS GAMES] ShowLeaderboards ERROR");
                OnShowLeaderboardsFailure?.Invoke(exception);
            });
        }

        public void GetLeaderboardsData()
        {
            ITask<IList<Ranking>> task = rankingsClient.GetRankingSummary(true);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] GetLeaderboardsData SUCCESS");
                OnGetLeaderboardsDataSuccess?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS GAMES] GetLeaderboardsData ERROR");
                OnGetLeaderboardsDataFailure?.Invoke(exception);
            });
        }

        public void GetLeaderboardData(string leaderboardId)
        {
            ITask<Ranking> task = rankingsClient.GetRankingSummary(leaderboardId, true);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] GetLeaderboardsData SUCCESS");
                OnGetLeaderboardDataSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS GAMES] GetLeaderboardsData ERROR");
                OnGetLeaderboardDataFailure?.Invoke(exception);
            });
        }

        public void GetScoresFromLeaderboard(string leaderboardId, int timeDimension, int maxResults, int offsetPlayerRank, int pageDirection)
        {

            ITask<RankingScores> task =
                rankingsClient.GetRankingTopScores(leaderboardId, timeDimension, maxResults, offsetPlayerRank, pageDirection);

            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] GetScoresFromLeaderboard SUCCESS");
                OnGetScoresFromLeaderboardSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS GAMES] GetScoresFromLeaderboard ERROR");
                OnGetScoresFromLeaderboardFailure?.Invoke(exception);
            });
        }
    }
}