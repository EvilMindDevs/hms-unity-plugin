using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSAchievementsManager : HMSManagerSingleton<HMSAchievementsManager>
    {

        public IAchievementsClient achievementsClient;
        public Action OnShowAchievementsSuccess { get; set; }
        public Action<HMSException> OnShowAchievementsFailure { get; set; }

        public Action<IList<Achievement>> OnGetAchievementsListSuccess { get; set; }
        public Action<HMSException> OnGetAchievementsListFailure { get; set; }

        public Action OnRevealAchievementSuccess { get; set; }
        public Action<HMSException> OnRevealAchievementFailure { get; set; }

        public Action OnIncreaseStepAchievementSuccess { get; set; }
        public Action<HMSException> OnIncreaseStepAchievementFailure { get; set; }

        public Action OnSetStepAchievementSuccess { get; set; }
        public Action<HMSException> OnSetStepAchievementFailure { get; set; }

        public Action OnUnlockAchievementSuccess { get; set; }
        public Action<HMSException> OnUnlockAchievementFailure { get; set; }

        public void ShowAchievements()
        {
            if (HMSAccountKitManager.Instance.HuaweiId != null)
            {
                IAchievementsClient achievementsClient = Games.GetAchievementsClient();
                achievementsClient.ShowAchievementList(() =>
                {
                    Debug.Log("[HMS GAMES:] ShowAchievements SUCCESS");
                    OnShowAchievementsSuccess?.Invoke();
                }, (exception) =>
                {
                    Debug.LogError("[HMSAchievementsManager]: Show Achievements failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                    OnShowAchievementsFailure?.Invoke(exception);
                });
            }
        }

        public void GetAchievementsList()
        {
            ITask<IList<Achievement>> task = achievementsClient.GetAchievementList(true);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] GetAchievementsList SUCCESS");
                OnGetAchievementsListSuccess?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSAchievementsManager]: GetAchievementsList failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnGetAchievementsListFailure?.Invoke(exception);
            });
        }

        public void Visualize(string achievementId)
        {
            achievementsClient.Visualize(achievementId);
            Debug.Log("[HMS GAMES] Visualize");
        }

        public void RevealAchievement(string achievementId)
        {
            ITask<HuaweiMobileServices.Utils.Void> task = achievementsClient.VisualizeWithResult(achievementId);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] RevealAchievement SUCCESS");
                OnRevealAchievementSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSAchievementsManager]: RevealAchievement failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnRevealAchievementFailure?.Invoke(exception);
            });
        }

        public void Grow(string achievementId, int stepIncrement)
        {
            achievementsClient.Grow(achievementId, stepIncrement);
            Debug.Log("[HMS GAMES] Grow");
        }

        public void IncreaseStepAchievement(string achievementId, int stepIncrement)
        {
            ITask<bool> task = achievementsClient.GrowWithResult(achievementId, stepIncrement);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] IncreaseStepAchievement SUCCESS" + result);
                OnIncreaseStepAchievementSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSAchievementsManager]: IncreaseStepAchievement failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnIncreaseStepAchievementFailure?.Invoke(exception);
            });
        }

        public void MakeSteps(string achievementId, int stepIncrement)
        {
            achievementsClient.MakeSteps(achievementId, stepIncrement);
            Debug.Log("[HMS GAMES] MakeSteps");
        }

        public void SetStepAchievement(string achievementId, int stepsNum)
        {
            ITask<bool> task = achievementsClient.MakeStepsWithResult(achievementId, stepsNum);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] SetStepAchievement SUCCESS" + result);
                OnSetStepAchievementSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSAchievementsManager]: SetStepAchievement failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnSetStepAchievementFailure?.Invoke(exception);
            });
        }

        public void Reach(string achievementId)
        {
            achievementsClient.Reach(achievementId);
            Debug.Log("[HMS GAMES] Reach");
        }

        public void UnlockAchievement(string achievementId)
        {
            ITask<HuaweiMobileServices.Utils.Void> task = achievementsClient.ReachWithResult(achievementId);
            task.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS GAMES] UnlockAchievements SUCCESS");
                OnUnlockAchievementSuccess?.Invoke();

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSAchievementsManager]: UnlockAchievement failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnUnlockAchievementFailure?.Invoke(exception);
            });
        }
    }
}
