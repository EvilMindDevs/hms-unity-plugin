using HmsPlugin;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameAddictionPreventionDemoManager : MonoBehaviour
{
    private const string TAG = "[HMS] GameAddictionPreventionDemoManager";

    [SerializeField]
    Text display, btn;
    void Start()
    {
        //1th Way
        //Enable initOnStart in KitSettings > GameService.
        //HMSGameServiceManager will handle signIn process
        /*HMSGameServiceManager.Instance.SignInSuccess = OnSignInSuccess;
        HMSGameServiceManager.Instance.SignInFailure = OnSignInFailed;*/

        //2nd Way
        //Disable initOnStart in KitSettings>GameService.
        //Use account kit to handle signIn process
        //Run HMSGameServiceManager.Instance.Init() after user signIn succesfully
        HMSAccountKitManager.Instance.OnSignInSuccess = OnSignInSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnSignInFailed;
        HMSAccountKitManager.Instance.SignIn(HMSAccountKitManager.Instance.GetGameAuthService());

        //3rd Way
        //Disable initOnStart in KitSettings > GameService.
        //HMSGameServiceManager will handle signIn process
        //Run HMSGameServiceManager.Instance.Init()
        /*HMSGameServiceManager.Instance.SignInSuccess = OnSignInSuccess;
        HMSGameServiceManager.Instance.SignInFailure = OnSignInFailed;
        //Disable line 54 same with 39
        HMSGameServiceManager.Instance.GameInitSuccess = GameInitSuccess;
        HMSGameServiceManager.Instance.Init();*/

/*
        //GAME Trial
        //According to the latest notice issued by National Press and Publication Administration, all online games cannot be provided to users who have not registered in real name in any form(including the guest mode).
        //If your game uses the guest mode, modification is required.Huawei has disabled the previous game trial mode. If you have enabled the mode, disable it as soon as possible.
        //The sample code for the previous game trial mode is as follows: 
        IPlayersClient client = Games.GetPlayersClient();
        client.SetGameTrialProcess(onTrialTimeout, onCheckRealNameResult);
*/
    }
    private void OnSignInFailed(HMSException exception)
    {
        //Games released in the Chinese mainland must support the identity verification function.
        //When a player signs in to your game using a HUAWEI ID, prompt the player to perform identity verification. You do not need to develop an identity verification system.
        //HMSAccountKitManager.Instance.SignIn(HMSAccountKitManager.Instance.GetGameAuthService());
        Debug.LogError($"{TAG} OnSignInFailed:{exception}");
        display.text = "OnSignInFailed:\n" + exception;
    }
    private void OnSignInSuccess(AuthAccount account)
    {
        Debug.Log($"{TAG} OnSignInSuccess:{account.DisplayName}, {account.ServiceCountryCode}");
        HMSGameServiceManager.Instance.GameInitSuccess = GameInitSuccess;
        HMSGameServiceManager.Instance.Init(new AntiAddictionCallback(onExit), account);
        btn.text = "SignOut";
    }
    private void GameInitSuccess(HuaweiMobileServices.Utils.Void @void)
    {
        Debug.Log($"{TAG} GameInitSuccess");
        ITask<Player> task = Games.GetPlayersClient().GamePlayer;
        task.AddOnSuccessListener((Player player) => 
        {
            Debug.Log($"{TAG} task.AddOnSuccessListener:{player.Level}, {player.DisplayName}");
            display.text = "Welcome: \n" + player.DisplayName;
        });
        task.AddOnFailureListener((HMSException exception) => 
        {
            Debug.Log($"{TAG} task.AddOnFailureListener:{exception}");
            display.text = "getGamePlayerFailed:\n" + exception;
        });
        Debug.Log($"{TAG} end of the GameInitSuccess");
    }
    public void onSignButtonClick()
    {
        if(btn.text.Equals("SignIn"))
            HMSAccountKitManager.Instance.SignIn(HMSAccountKitManager.Instance.GetGameAuthService());
        else
        {
            HMSAccountKitManager.Instance.SignOut();
            btn.text = "SignIn";
            display.text = "Please SignIn";
        }
    }
    public void onShowFloatWindowClicked()
    {
        Debug.Log($"{TAG} onShowFloatWindowClicked");
        HMSGameServiceManager.Instance.ShowFloatWindow();
    }
    private void onExit()
    {
        Debug.Log($"{TAG} onExit");
        display.text = "onExit";
        //Exits game addiction prevention.
        //This method is triggered when a minor's played time exceeds the upper limit.
        //You must implement the game exit function in this method, for example, saving game progress and calling the HUAWEI ID sign-out API.
    }
    private void onCheckRealNameResult(bool res)
    {
        Debug.Log($"{TAG} onCheckRealNameResult:{res}");
    }
    private void onTrialTimeout()
    {
        Debug.Log($"{TAG} onTrialTimeout");
    }
}

public class AntiAddictionCallback : IAntiAddictionCallback
{
    public Action onExit;
    public AntiAddictionCallback(Action onExit)
    {
        this.onExit = onExit;
    }
    public void OnExit()
    {
        onExit?.Invoke();
    }
}
