using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private const string NAME = "GameManager";

    public static GameManager Instance => GameObject.Find(NAME).GetComponent<GameManager>();

    private AccountManager accountManager;

    public Action<Player> OnGetPlayerInfoSuccess { get; set; }
    public Action<HMSException> OnGetPlayerInfoFailure { get; set; }

    // Make sure user already signed in!
    public void Start()
    {
        Debug.Log("HMS GAMES: Game init");
        HuaweiMobileServicesUtil.SetApplication();
        accountManager = AccountManager.Instance;
        Init();
    }

    private void Init()
    {
        Debug.Log("HMS GAMES init");
        if (accountManager.HuaweiId != null)
        {
            Debug.Log("HMS GAMES: Setted app");
            IJosAppsClient josAppsClient = JosApps.GetJosAppsClient(accountManager.HuaweiId);
            Debug.Log("HMS GAMES: jossClient");
            josAppsClient.Init();
            Debug.Log("HMS GAMES: jossClient init");
        }
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
