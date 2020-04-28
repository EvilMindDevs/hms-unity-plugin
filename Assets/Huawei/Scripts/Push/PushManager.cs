using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PushManager : MonoBehaviour, IPushListener
{

    private string pushToken;
    private Text remoteMessageText;

    // Start is called before the first frame update
    void Start()
    {
        var appId = AGConnectServicesConfig.FromContext().GetString("client/app_id");
        pushToken = HmsInstanceId.GetInstance().GetToken(appId, "HCM");
        remoteMessageText = GameObject.Find("RemoteMessageText").GetComponent<Text>();
    }

    public void OnNewToken(string token)
    {
        if (pushToken == null)
        {
            pushToken = token;
        }
    }

    public void OnTokenError(Exception e)
    {
        Debug.Log("Error asking for Push token");
        Debug.Log(e.StackTrace);
    }

    public void OnMessageReceived(RemoteMessage remoteMessage)
    {
        var id = remoteMessage.MessageId;
        var from = remoteMessage.From;
        var to = remoteMessage.To;
        var data = remoteMessage.Data;
        remoteMessageText.text = $"ID: {id}\nFrom: {from}\nTo: {to}\nData: {data}";
    }
}
