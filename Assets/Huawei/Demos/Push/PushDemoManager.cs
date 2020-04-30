using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PushDemoManager : MonoBehaviour, IPushListener
{

    private string pushToken;
    private Text remoteMessageText;

    // Start is called before the first frame update
    void Start()
    {
        remoteMessageText = GameObject.Find("RemoteMessageText").GetComponent<Text>();
        PushManager.Listener = this;
        pushToken = PushManager.Token;
        Debug.Log($"[HMS] Push token from GetToken is {pushToken}");
    }

    public void OnNewToken(string token)
    {
        Debug.Log($"[HMS] Push token from OnNewToken is {pushToken}");
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
