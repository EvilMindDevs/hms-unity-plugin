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
    private readonly string TAG = "[HMS] GameDemoManager ";

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
        HMSGameServiceManager.Instance.OnGetPlayerInfoFailure = OnGetPlayerInfoFailure;
        HMSAchievementsManager.Instance.OnShowAchievementsSuccess = OnShowAchievementsSuccess;
        HMSAchievementsManager.Instance.OnShowAchievementsFailure = OnShowAchievementsFailure;
        HMSAchievementsManager.Instance.OnRevealAchievementSuccess = OnRevealAchievementSuccess;
        HMSAchievementsManager.Instance.OnRevealAchievementFailure = OnRevealAchievementFailure;
        HMSAchievementsManager.Instance.OnIncreaseStepAchievementSuccess = OnIncreaseStepAchievementSuccess;
        HMSAchievementsManager.Instance.OnIncreaseStepAchievementFailure = OnIncreaseStepAchievementFailure;
        HMSAchievementsManager.Instance.OnUnlockAchievementSuccess = OnUnlockAchievementSuccess;
        HMSAchievementsManager.Instance.OnUnlockAchievementFailure = OnUnlockAchievementFailure;

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
        Debug.Log(TAG + " GetMaxImageSize");

        ITask<int> detailSizeTask = HMSSaveGameManager.Instance.GetMaxImageSize();
        detailSizeTask.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMSP:] GetMaxImageSize Success " + result);
            imageSize.text = string.Format(MAX_IMAGE_SIZE, result);
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMSP:] GetMaxImageSize Failed");
        });
    }

    public void GetMaxFileSize()
    {
        Debug.Log(TAG + " GetMaxFileSize");

        ITask<int> detailSizeTask = HMSSaveGameManager.Instance.GetMaxFileSize();
        detailSizeTask.AddOnSuccessListener((result) =>
        {
            Debug.Log("[HMSP:] GetMaxFileSize Success " + result);
            fileSize.text = string.Format(MAX_FILE_SIZE, result);
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("[HMSP:] GetMaxFileSize Failed");
        });
    }

    public void CommitGame()
    {
        Debug.Log(TAG + " CommitGame");

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
            Debug.Log("[HMSP:] Fill box");
    }

    private void OnGetPlayerInfoSuccess(Player player)
    {
        Debug.Log("HMS Games: GetPlayerInfo SUCCESS");
    }

    private void OnGetPlayerInfoFailure(HMSException exception)
    {
        Debug.Log("HMS Games: GetPlayerInfo ERROR:" + exception.Message);
    }

    public void ShowArchive()
    {
        Debug.Log(TAG + " ShowArchive");

        HMSSaveGameManager.Instance.ShowArchive();
    }

    public void ShowAchievements()
    {
        Debug.Log(TAG + " ShowAchievements");

        HMSAchievementsManager.Instance.ShowAchievements();
    }

    private void OnShowAchievementsSuccess()
    {
        Debug.Log("HMS Games: ShowAchievements SUCCESS ");
    }

    private void OnShowAchievementsFailure(HMSException exception)
    {
        Debug.Log("HMS Games: ShowAchievements ERROR ");
    }

    public void GetAchievementsList()
    {
        HMSAchievementsManager.Instance.GetAchievementsList();
        HMSAchievementsManager.Instance.OnGetAchievementsListSuccess = OnGetAchievemenListSuccess;
        HMSAchievementsManager.Instance.OnGetAchievementsListFailure = OnGetAchievementListFailure;

    }

    private void OnGetAchievemenListSuccess(IList<Achievement> achievementList)
    {
        Debug.Log("HMS Games: GetAchievementsList SUCCESS ");
    }

    private void OnGetAchievementListFailure(HMSException error)
    {
        Debug.Log("HMS Games: GetAchievementsList ERROR ");
    }

    public void RevealAchievement(string achievementId)
    {
        HMSAchievementsManager.Instance.RevealAchievement(achievementId);
    }

    public void OnRevealAchievementSuccess()
    {
        Debug.Log("HMS Games: RevealAchievement SUCCESS ");
    }

    private void OnRevealAchievementFailure(HMSException error)
    {
        Debug.Log("HMS Games: RevealAchievement ERROR ");
    }

    public void IncreaseStepAchievement(string achievementId, int stepIncrement = 1)
    {
        HMSAchievementsManager.Instance.IncreaseStepAchievement(achievementId, stepIncrement);
    }

    private void OnIncreaseStepAchievementSuccess()
    {
        Debug.Log("HMS Games: IncreaseStepAchievement SUCCESS ");
    }

    private void OnIncreaseStepAchievementFailure(HMSException error)
    {
        Debug.Log("HMS Games: IncreaseStepAchievement ERROR ");
    }

    public void SetStepAchievement(string achievementId, int stepsNum)
    {
        HMSAchievementsManager.Instance.OnSetStepAchievementSuccess = OnSetStepAchievementSuccess;
        HMSAchievementsManager.Instance.OnSetStepAchievementFailure = OnSetStepAchievemenFailure;
        HMSAchievementsManager.Instance.SetStepAchievement(achievementId, stepsNum);
    }

    private void OnSetStepAchievementSuccess()
    {
        Debug.Log("HMS Games: SetStepAchievement SUCCESS ");
    }

    private void OnSetStepAchievemenFailure(HMSException error)
    {
        Debug.Log("HMS Games: SetStepAchievement ERROR ");
    }

    public void UnlockAchievement(string achievementId)
    {
        Debug.Log(TAG + " UnlockAchievement");

        HMSAchievementsManager.Instance.UnlockAchievement(achievementId);
    }

    private void OnUnlockAchievementSuccess()
    {
        Debug.Log("HMS Games: UnlockAchievement SUCCESS ");
    }

    private void OnUnlockAchievementFailure(HMSException error)
    {
        Debug.Log("HMS Games: UnlockAchievement ERROR ");
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
        Debug.Log("HMS Games: GetUserScoreShownOnLeaderboards SUCCESS ");
    }

    private void OnIsUserScoreShownOnLeaderboardsFailure(HMSException error)
    {
        Debug.Log("HMS Games: GetUserScoreShownOnLeaderboards ERROR ");
    }

    public void SetUserScoreShownOnLeaderboards(int active)
    {
        HMSLeaderboardManager.Instance.SetUserScoreShownOnLeaderboards(active);
        HMSLeaderboardManager.Instance.OnSetUserScoreShownOnLeaderboardsSuccess = OnSetUserScoreShownOnLeaderboardsSuccess;
        HMSLeaderboardManager.Instance.OnSetUserScoreShownOnLeaderboardsFailure = OnSetUserScoreShownOnLeaderboardsFailure;

    }

    private void OnSetUserScoreShownOnLeaderboardsSuccess(int active)
    {
        Debug.Log("HMS Games: SetUserScoreShownOnLeaderboards SUCCESS ");
    }

    private void OnSetUserScoreShownOnLeaderboardsFailure(HMSException exception)
    {
        Debug.Log("HMS Games: SetUserScoreShownOnLeaderboards ERROR ");
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
        Debug.Log("HMS Games: SubmitScore SUCCESS ");
    }

    private void OnSubmitScoreFailure(HMSException exception)
    {
        Debug.Log("HMS Games: SubmitScore ERROR ");

    }

    public void ShowLeaderboards()
    {
        Debug.Log(TAG + " ShowLeaderboards");

        HMSLeaderboardManager.Instance.ShowLeaderboards();
        HMSLeaderboardManager.Instance.OnShowLeaderboardsSuccess = OnShowLeaderboardsSuccess;
        HMSLeaderboardManager.Instance.OnShowLeaderboardsFailure = OnShowLeaderboardsFailure;

    }

    private void OnShowLeaderboardsSuccess()
    {
        Debug.Log("HMS Games: ShowLeaderboards SUCCESS ");

    }

    private void OnShowLeaderboardsFailure(HMSException error)
    {
        Debug.Log("HMS Games: ShowLeaderboards ERROR ");

    }

    public void GetLeaderboardsData(string leaderboardId)
    {
        HMSLeaderboardManager.Instance.GetLeaderboardData(leaderboardId);
        HMSLeaderboardManager.Instance.OnGetLeaderboardDataSuccess = OnGetLeaderboardDataSuccess;
        HMSLeaderboardManager.Instance.OnGetLeaderboardsDataFailure = OnGetLeaderboardsDataFailure;

    }

    private void OnGetLeaderboardDataSuccess(Ranking ranking)
    {
        Debug.Log("HMS Games: GetLeaderboardsData SUCCESS ");

    }
    private void OnGetLeaderboardsDataFailure(HMSException exception)
    {
        Debug.Log("HMS Games: GetLeaderboardsData ERROR ");

    }

    public void GetScoresFromLeaderboard(string leaderboardId, int timeDimension, int maxResults, int offsetPlayerRank, int pageDirection)
    {

        HMSLeaderboardManager.Instance.GetScoresFromLeaderboard(leaderboardId, timeDimension, maxResults, offsetPlayerRank, pageDirection);

        HMSLeaderboardManager.Instance.OnGetScoresFromLeaderboardSuccess = OnGetScoresFromLeaderboardSuccess;
        HMSLeaderboardManager.Instance.OnGetScoresFromLeaderboardFailure = OnGetScoresFromLeaderboardFailure;

    }

    private void OnGetScoresFromLeaderboardSuccess(RankingScores rankingScores)
    {
        Debug.Log("HMS Games: GetScoresFromLeaderboard SUCCESS ");
    }

    private void OnGetScoresFromLeaderboardFailure(HMSException error)
    {
        Debug.Log("HMS Games: GetScoresFromLeaderboard ERROR ");
    }
}
