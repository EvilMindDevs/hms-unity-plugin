using HuaweiMobileServices.Game;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class GameDemoUIView : MonoBehaviour
{

    private Button Btn_ShowAchievements;
    private Button Btn_ShowLeaderboards;
    private Button Btn_UnlockAchievement;

    private Button Btn_ShowSavedGames;
    private Button Btn_Commit;
    private Button Btn_GetImage;
    private Button Btn_GetFile;

    #region Monobehaviour

    private void Awake()
    {
        Btn_ShowAchievements = GameObject.Find("Btn_ShowAchievements").GetComponent<Button>();
        Btn_ShowLeaderboards = GameObject.Find("Btn_ShowLeaderboards").GetComponent<Button>();
        Btn_UnlockAchievement = GameObject.Find("Btn_UnlockAchievement").GetComponent<Button>();
        Btn_ShowSavedGames = GameObject.Find("Btn_ShowSavedGames").GetComponent<Button>();
        Btn_Commit = GameObject.Find("Btn_Commit").GetComponent<Button>();
        Btn_GetImage = GameObject.Find("GetImage").GetComponent<Button>();
        Btn_GetFile = GameObject.Find("GetFile").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_ShowAchievements.onClick.AddListener(ButtonClick_ShowAchievements);
        Btn_ShowLeaderboards.onClick.AddListener(ButtonClick_ShowLeaderboards);
        Btn_UnlockAchievement.onClick.AddListener(ButtonClick_UnlockAchievement);
        Btn_ShowSavedGames.onClick.AddListener(ButtonClick_ShowSavedGames);
        Btn_Commit.onClick.AddListener(ButtonClick_Commit);
        Btn_GetImage.onClick.AddListener(ButtonClick_GetImage);
        Btn_GetFile.onClick.AddListener(ButtonClick_GetFile);
    }

    private void OnDisable()
    {
        Btn_ShowAchievements.onClick.RemoveListener(ButtonClick_ShowAchievements);
        Btn_ShowLeaderboards.onClick.RemoveListener(ButtonClick_ShowLeaderboards);
        Btn_UnlockAchievement.onClick.RemoveListener(ButtonClick_UnlockAchievement);
        Btn_ShowSavedGames.onClick.RemoveListener(ButtonClick_ShowSavedGames);
        Btn_Commit.onClick.RemoveListener(ButtonClick_Commit);
        Btn_GetImage.onClick.RemoveListener(ButtonClick_GetImage);
        Btn_GetFile.onClick.RemoveListener(ButtonClick_GetFile);
    }

    #endregion

    #region Button Events

    private void ButtonClick_ShowAchievements()
    {
        GameDemoManager.Instance.ShowAchievements();
    }

    private void ButtonClick_ShowLeaderboards()
    {
        GameDemoManager.Instance.ShowLeaderboards();
    }

    private void ButtonClick_UnlockAchievement()
    {
        GameDemoManager.Instance.UnlockAchievement("tutorial");
    }

    private void ButtonClick_ShowSavedGames()
    {
        GameDemoManager.Instance.ShowArchive();
    }

    private void ButtonClick_Commit()
    {
        GameDemoManager.Instance.CommitGame();
    }

    private void ButtonClick_GetImage()
    {
        GameDemoManager.Instance.GetMaxImageSize();
    }

    private void ButtonClick_GetFile()
    {
        GameDemoManager.Instance.GetMaxFileSize();
    }

    #endregion

}
