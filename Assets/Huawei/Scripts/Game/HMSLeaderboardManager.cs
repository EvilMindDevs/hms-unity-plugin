using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSLeaderboardManager : HMSSingleton<HMSLeaderboardManager>
    {

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

        public void IsUserScoreShownOnLeaderboards()
        {
            ITask<int> task = rankingsClient.GetRankingSwitchStatus();
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSLeaderboardManager] isUserScoreShownOnLeaderboards SUCCESS" + result);
                OnIsUserScoreShownOnLeaderboardsSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: IsUserScoreShownOnLeaderboards failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnIsUserScoreShownOnLeaderboardsFailure?.Invoke(exception);
            });

        }

        public void SetUserScoreShownOnLeaderboards(int active)
        {
            ITask<int> task = rankingsClient.SetRankingSwitchStatus(active);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSLeaderboardManager] SetUserScoreShownOnLeaderboards SUCCESS" + result);
                OnSetUserScoreShownOnLeaderboardsSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: SetUserScoreShownOnLeaderboards failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnSetUserScoreShownOnLeaderboardsFailure?.Invoke(exception);
            });
        }

        public void SubmitScore(string leaderboardId, long score)
        {
            ITask<ScoreSubmissionInfo> task = rankingsClient.SubmitScoreWithResult(leaderboardId, score);
            task.AddOnSuccessListener((scoreInfo) =>
            {
                Debug.Log("[HMSLeaderboardManager] SubmitScore SUCCESS");
                OnSubmitScoreSuccess?.Invoke(scoreInfo);
            }).AddOnFailureListener((error) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: SubmitScore failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                OnSubmitScoreFailure?.Invoke(error);
            });
        }

        public void SubmitScore(string leaderboardId, long score, string scoreTips)
        {
            ITask<ScoreSubmissionInfo> task = rankingsClient.SubmitScoreWithResult(leaderboardId, score, scoreTips);
            task.AddOnSuccessListener((scoreInfo) =>
            {
                Debug.Log("[HMSLeaderboardManager] SubmitScore SUCCESS");
                OnSubmitScoreSuccess?.Invoke(scoreInfo);
            }).AddOnFailureListener((error) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: SubmitScore failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                OnSubmitScoreFailure?.Invoke(error);
            });
        }

        public void ShowLeaderboards()
        {
            rankingsClient.ShowTotalRankings(() =>
            {
                Debug.Log("[HMSLeaderboardManager] ShowLeaderboards SUCCESS");
                OnShowLeaderboardsSuccess?.Invoke();

            }, (exception) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: ShowLeaderboards failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnShowLeaderboardsFailure?.Invoke(exception);
            });
        }

        public void GetLeaderboardsData()
        {
            ITask<IList<Ranking>> task = rankingsClient.GetRankingSummary(true);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSLeaderboardManager] GetLeaderboardsData SUCCESS");
                OnGetLeaderboardsDataSuccess?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: GetLeaderboardsData failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnGetLeaderboardsDataFailure?.Invoke(exception);
            });
        }

        public void GetLeaderboardData(string leaderboardId)
        {
            ITask<Ranking> task = rankingsClient.GetRankingSummary(leaderboardId, true);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSLeaderboardManager] GetLeaderboardsData SUCCESS");
                OnGetLeaderboardDataSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: GetLeaderboardData failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnGetLeaderboardDataFailure?.Invoke(exception);
            });
        }

        public void GetScoresFromLeaderboard(string leaderboardId, int timeDimension, int maxResults, int offsetPlayerRank, int pageDirection)
        {

            ITask<RankingScores> task =
                rankingsClient.GetRankingTopScores(leaderboardId, timeDimension, maxResults, offsetPlayerRank, pageDirection);

            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSLeaderboardManager] GetScoresFromLeaderboard SUCCESS");
                OnGetScoresFromLeaderboardSuccess?.Invoke(result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSLeaderboardManager]: GetScoresFromLeaderboard failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnGetScoresFromLeaderboardFailure?.Invoke(exception);
            });
        }
    }
}