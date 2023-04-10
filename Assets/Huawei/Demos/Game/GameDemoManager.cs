using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System;
using HmsPlugin;
using UnityEngine.UI;

public class GameDemoManager : MonoBehaviour
{
    private readonly string TAG = "[HMS] GameDemoManager: ";

    private bool customUnit = false;
    private const string MAX_FILE_SIZE = "Max File Size: {0}";
    private const string MAX_IMAGE_SIZE = "Max Image Size: {0}";

    private Text fileSize, imageSize;
    private InputField InputFieldDesc, InputFieldPlayedTime, InputFieldProgress;

    #region Singleton

    public static GameDemoManager Instance { get; private set; }
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    private void Awake()
    {
        Singleton();
    }

    void Start()
    {
        HMSGameServiceManager.Instance.OnGetPlayerInfoSuccess = OnGetPlayerInfoSuccess;
        HMSGameServiceManager.Instance.OnGetPlayerInfoFailure = OnFailureListener;
        HMSAchievementsManager.Instance.OnShowAchievementsSuccess = OnShowAchievementsSuccess;
        HMSAchievementsManager.Instance.OnShowAchievementsFailure = OnFailureListener;
        HMSAchievementsManager.Instance.OnRevealAchievementSuccess = OnRevealAchievementSuccess;
        HMSAchievementsManager.Instance.OnRevealAchievementFailure = OnFailureListener;
        HMSAchievementsManager.Instance.OnIncreaseStepAchievementSuccess = OnIncreaseStepAchievementSuccess;
        HMSAchievementsManager.Instance.OnIncreaseStepAchievementFailure = OnFailureListener;
        HMSAchievementsManager.Instance.OnUnlockAchievementSuccess = OnUnlockAchievementSuccess;
        HMSAchievementsManager.Instance.OnUnlockAchievementFailure = OnFailureListener;
        HMSGameServiceManager.Instance.OnGetUserPlayStateSuccess = OnGetUserPlayStateSuccess;
        HMSGameServiceManager.Instance.OnGetUserPlayStateFailure = OnFailureListener;
        HMSGameServiceManager.Instance.OnIsAllowContinuePlayGamesSuccess = OnIsAllowContinuePlayGamesSuccess;
        HMSGameServiceManager.Instance.OnIsAllowContinuePlayGamesFailure = OnFailureListener;

        fileSize = GameObject.Find("FileSize").GetComponent<Text>();
        imageSize = GameObject.Find("ImageSize").GetComponent<Text>();
        InputFieldDesc = GameObject.Find("Description").GetComponent<InputField>();
        InputFieldPlayedTime = GameObject.Find("PlayedTime").GetComponent<InputField>();
        InputFieldProgress = GameObject.Find("Progress").GetComponent<InputField>();

        //did you enable initialize on start => Unity Editor > Huawei > HMS kit settings > Game Service ?
        // HMSAccountKitManager.Instance.SignIn();
    }

    public void GetMaxImageSize()
    {
        Debug.Log(TAG + "GetMaxImageSize");

        ITask<int> detailSizeTask = HMSSaveGameManager.Instance.GetMaxImageSize();
        detailSizeTask.AddOnSuccessListener((result) =>
        {
            Debug.Log(TAG + "GetMaxImageSize Success " + result);
            imageSize.text = string.Format(MAX_IMAGE_SIZE, result);
        }).AddOnFailureListener((exception) =>
        {
            Debug.LogError(TAG + "GetMaxImageSize Failed:" + exception);
        });
    }

    public void GetMaxFileSize()
    {
        Debug.Log(TAG + "GetMaxFileSize");

        ITask<int> detailSizeTask = HMSSaveGameManager.Instance.GetMaxFileSize();
        detailSizeTask.AddOnSuccessListener((result) =>
        {
            Debug.Log(TAG + "GetMaxFileSize Success " + result);
            fileSize.text = string.Format(MAX_FILE_SIZE, result);
        }).AddOnFailureListener((exception) =>
        {
            Debug.LogError(TAG + "GetMaxFileSize Failed:" + exception);
        });
    }

    public void CommitGame()
    {
        Debug.Log(TAG + "CommitGame");

        //Example Image Path: give statics path of image on phone
        string ImagePath = Application.streamingAssetsPath;
        if (InputFieldDesc.text != null && InputFieldProgress.text != null && InputFieldPlayedTime.text != null)
        {
            string description = InputFieldDesc.text;
            long playedTime = long.Parse(InputFieldPlayedTime.text);
            long progress = long.Parse(InputFieldProgress.text);
            HMSSaveGameManager.Instance.Commit(description, playedTime, progress, ImagePath, "png");
        }
        else
            Debug.Log(TAG + "Fill box");
    }

    private void OnGetPlayerInfoSuccess(Player player)
    {
        Debug.Log(TAG + "GetPlayerInfo SUCCESS");
    }

    public void ShowArchive()
    {
        Debug.Log(TAG + "ShowArchive");

        HMSSaveGameManager.Instance.ShowArchive();
    }

    public void ShowAchievements()
    {
        Debug.Log(TAG + "ShowAchievements");

        HMSAchievementsManager.Instance.ShowAchievements();
    }

    private void OnShowAchievementsSuccess()
    {
        Debug.Log(TAG + "ShowAchievements SUCCESS.");
    }

    public void GetAchievementsList()
    {
        HMSAchievementsManager.Instance.GetAchievementsList();
        HMSAchievementsManager.Instance.OnGetAchievementsListSuccess = OnGetAchievemenListSuccess;
        HMSAchievementsManager.Instance.OnGetAchievementsListFailure = OnFailureListener;
    }

    private void OnGetAchievemenListSuccess(IList<Achievement> achievementList)
    {
        Debug.Log(TAG + "GetAchievementsList SUCCESS.");
    }

    private void OnFailureListener(HMSException exception)
    {
        Debug.LogError(TAG + "OnFailureListener ERROR:" + exception.Message);
    }

    private void OnIsAllowContinuePlayGamesSuccess(bool isAllow)
    {
        Debug.Log(TAG + "IsAllowContinuePlayGames SUCCESS. isAllow:"+ isAllow);
    }

    private void OnGetUserPlayStateSuccess(int state)
    {
        Debug.Log(TAG + "GetUserPlayState SUCCESS. state:" + state);
    }

    public void RevealAchievement(string achievementId)
    {
        HMSAchievementsManager.Instance.RevealAchievement(achievementId);
    }

    public void OnRevealAchievementSuccess()
    {
        Debug.Log(TAG + "RevealAchievement SUCCESS.");
    }

    public void IncreaseStepAchievement(string achievementId, int stepIncrement = 1)
    {
        HMSAchievementsManager.Instance.IncreaseStepAchievement(achievementId, stepIncrement);
    }

    private void OnIncreaseStepAchievementSuccess()
    {
        Debug.Log(TAG + "IncreaseStepAchievement SUCCESS.");
    }

    public void SetStepAchievement(string achievementId, int stepsNum)
    {
        HMSAchievementsManager.Instance.OnSetStepAchievementSuccess = OnSetStepAchievementSuccess;
        HMSAchievementsManager.Instance.OnSetStepAchievementFailure = OnFailureListener;
        HMSAchievementsManager.Instance.SetStepAchievement(achievementId, stepsNum);
    }

    private void OnSetStepAchievementSuccess()
    {
        Debug.Log(TAG + "SetStepAchievement SUCCESS.");
    }

    public void UnlockAchievement(string achievementId)
    {
        Debug.Log(TAG + "UnlockAchievement");

        HMSAchievementsManager.Instance.UnlockAchievement(achievementId);
    }

    private void OnUnlockAchievementSuccess()
    {
        Debug.Log(TAG + "UnlockAchievement SUCCESS.");
    }

    /******************  LEADERBOARDS  ********************/


    // Get User Score Shown
    public void GetUserScoreShownOnLeaderboards()
    {
        HMSLeaderboardManager.Instance.IsUserScoreShownOnLeaderboards();
        HMSLeaderboardManager.Instance.OnIsUserScoreShownOnLeaderboardsSuccess = OnIsUserScoreShownOnLeaderboardsSuccess;
        HMSLeaderboardManager.Instance.OnIsUserScoreShownOnLeaderboardsFailure = OnIsUserScoreShownOnLeaderboardsFailure;
    }

    private void OnIsUserScoreShownOnLeaderboardsSuccess(int i)
    {
        Debug.Log(TAG + "GetUserScoreShownOnLeaderboards SUCCESS.");
    }

    private void OnIsUserScoreShownOnLeaderboardsFailure(HMSException exception)
    {
        Debug.LogError(TAG + "GetUserScoreShownOnLeaderboards ERROR:" + exception);
    }

    public void SetUserScoreShownOnLeaderboards(int active)
    {
        HMSLeaderboardManager.Instance.SetUserScoreShownOnLeaderboards(active);
        HMSLeaderboardManager.Instance.OnSetUserScoreShownOnLeaderboardsSuccess = OnSetUserScoreShownOnLeaderboardsSuccess;
        HMSLeaderboardManager.Instance.OnSetUserScoreShownOnLeaderboardsFailure = OnSetUserScoreShownOnLeaderboardsFailure;

    }

    private void OnSetUserScoreShownOnLeaderboardsSuccess(int active)
    {
        Debug.Log(TAG + "SetUserScoreShownOnLeaderboards SUCCESS.");
    }

    private void OnSetUserScoreShownOnLeaderboardsFailure(HMSException exception)
    {
        Debug.LogError(TAG + "SetUserScoreShownOnLeaderboards ERROR:" + exception);
    }

    public void SubmitScore(string leaderboardId, long score, string scoreTips)
    {
        HMSLeaderboardManager.Instance.OnSubmitScoreSuccess = OnSubmitScoreSuccess;
        HMSLeaderboardManager.Instance.OnSubmitScoreFailure = OnSubmitScoreFailure;

        if (customUnit)
        {
            HMSLeaderboardManager.Instance.SubmitScore(leaderboardId, score, scoreTips);
        }
        else
        {
            HMSLeaderboardManager.Instance.SubmitScore(leaderboardId, score);
        }

    }

    private void OnSubmitScoreSuccess(ScoreSubmissionInfo scoreSubmission)
    {
        Debug.Log(TAG + "SubmitScore SUCCESS.");
    }

    private void OnSubmitScoreFailure(HMSException exception)
    {
        Debug.LogError(TAG + "SubmitScore ERROR:" + exception);

    }

    public void ShowLeaderboards()
    {
        Debug.Log(TAG + "ShowLeaderboards");

        HMSLeaderboardManager.Instance.ShowLeaderboards();
        HMSLeaderboardManager.Instance.OnShowLeaderboardsSuccess = OnShowLeaderboardsSuccess;
        HMSLeaderboardManager.Instance.OnShowLeaderboardsFailure = OnShowLeaderboardsFailure;

    }

    private void OnShowLeaderboardsSuccess()
    {
        Debug.Log(TAG + "ShowLeaderboards SUCCESS.");

    }

    private void OnShowLeaderboardsFailure(HMSException exception)
    {
        Debug.LogError(TAG + "ShowLeaderboards ERROR:" + exception);

    }

    public void GetLeaderboardsData(string leaderboardId)
    {
        HMSLeaderboardManager.Instance.GetLeaderboardData(leaderboardId);
        HMSLeaderboardManager.Instance.OnGetLeaderboardDataSuccess = OnGetLeaderboardDataSuccess;
        HMSLeaderboardManager.Instance.OnGetLeaderboardsDataFailure = OnGetLeaderboardsDataFailure;

    }

    private void OnGetLeaderboardDataSuccess(Ranking ranking)
    {
        Debug.Log(TAG + "GetLeaderboardsData SUCCESS.");

    }
    private void OnGetLeaderboardsDataFailure(HMSException exception)
    {
        Debug.LogError(TAG + "GetLeaderboardsData ERROR:" + exception);

    }

    public void GetScoresFromLeaderboard(string leaderboardId, int timeDimension, int maxResults, int offsetPlayerRank, int pageDirection)
    {

        HMSLeaderboardManager.Instance.GetScoresFromLeaderboard(leaderboardId, timeDimension, maxResults, offsetPlayerRank, pageDirection);

        HMSLeaderboardManager.Instance.OnGetScoresFromLeaderboardSuccess = OnGetScoresFromLeaderboardSuccess;
        HMSLeaderboardManager.Instance.OnGetScoresFromLeaderboardFailure = OnGetScoresFromLeaderboardFailure;

    }

    private void OnGetScoresFromLeaderboardSuccess(RankingScores rankingScores)
    {
        Debug.Log(TAG + "GetScoresFromLeaderboard SUCCESS.");
    }

    private void OnGetScoresFromLeaderboardFailure(HMSException exception)
    {
        Debug.LogError(TAG + "GetScoresFromLeaderboard ERROR:" + exception);
    }
}
