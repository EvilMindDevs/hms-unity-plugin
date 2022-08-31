using HmsPlugin;

using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class PushDemoManager : MonoBehaviour
{
    private string pushToken;
    private Text remoteMessageText, tokenText;

    private void Awake()
    {
        remoteMessageText = GameObject.Find("RemoteMessageText").GetComponent<Text>();
        tokenText = GameObject.Find("PushToken").GetComponent<Text>();
    }

    void Start()
    {
        /*
         * When using multiple kits, we recommend initializing the push kit with the coroutine.
         */
        Debug.Log("[HMS] Push Start");
        StartCoroutine(LateStart(0f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HMSPushKitManager.Instance.OnTokenSuccess = OnNewToken;
        HMSPushKitManager.Instance.OnTokenFailure = OnTokenError;
        HMSPushKitManager.Instance.OnTokenBundleSuccess = OnNewToken;
        HMSPushKitManager.Instance.OnTokenBundleFailure = OnTokenError;
        HMSPushKitManager.Instance.OnMessageSentSuccess = OnMessageSent;
        HMSPushKitManager.Instance.OnSendFailure = OnSendError;
        HMSPushKitManager.Instance.OnMessageDeliveredSuccess = OnMessageDelivered;
        HMSPushKitManager.Instance.OnMessageReceivedSuccess = OnMessageReceived;
        HMSPushKitManager.Instance.OnNotificationMessage = OnNotificationMessage;
        HMSPushKitManager.Instance.NotificationMessageOnStart = NotificationMessageOnStart;
        HMSPushKitManager.Instance.Init();
    }

    private void OnNotificationMessage(NotificationData data)
    {
        Debug.Log("[HMSPushDemo] CmdType: " + data.CmdType);
        Debug.Log("[HMSPushDemo] MsgId: " + data.MsgId);
        Debug.Log("[HMSPushDemo] NotifyId: " + data.NotifyId);
        Debug.Log("[HMSPushDemo] KeyValueJSON: " + data.KeyValueJSON);
    }

    private void NotificationMessageOnStart(NotificationData data)
    {
        Debug.Log("[HMSPushDemo] CmdType: " + data.CmdType);
        Debug.Log("[HMSPushDemo] MsgId: " + data.MsgId);
        Debug.Log("[HMSPushDemo] NotifyId: " + data.NotifyId);
        Debug.Log("[HMSPushDemo] KeyValueJSON: " + data.KeyValueJSON);
        /* TODO: Make your own logic here
         * notificationDataOnStart = data;
         */
    }

    public void OnNewToken(string token)
    {
        Debug.Log($"[HMS] Push token from OnNewToken is {token}");
        if (token != "")
        {
            pushToken = token;
            tokenText.text = "Push Token: " + pushToken;
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
        Debug.Log($"[HMS] Push token from OnNewToken is {token}");
        if (token != "")
        {
            pushToken = token;
            tokenText.text = "Push Token: " + pushToken;
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
