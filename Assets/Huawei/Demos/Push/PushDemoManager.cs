using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
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

    public void OnNewToken(string token, Bundle bundle)
    {
        Debug.Log($"[HMS] Push token from OnNewToken is {pushToken}");
        if (pushToken == null)
        {
            pushToken = token;
        }
    }

    public void OnTokenError(Exception exception, Bundle bundle)
    {
        Debug.Log("Error asking for Push token");
        Debug.Log(exception.StackTrace);
    }

    public void OnMessageSent(string msgId)
    {
        Debug.Log(msgId);
    }

    public void OnMessageDelivered(string msgId, Exception exception)
    {
        Debug.Log("Message Delivered");
        Debug.Log(exception.StackTrace + " , Message ID:" + msgId);
    }

    public void OnSendError(string msgId, Exception exception)
    {
        Debug.Log(exception.StackTrace + " , Message ID:" + msgId);
    }
}
