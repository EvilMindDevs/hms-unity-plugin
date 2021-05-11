using HmsPlugin;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PushDemoManager : MonoBehaviour
{
    private string pushToken;
    [SerializeField]
    private Text remoteMessageText, tokenText;

    private NotificationData notificationDataOnStart;

    void Start()
    {
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

        Debug.Log($"[HMS] Push token from GetToken is {pushToken}");
        tokenText.text = "Push Token: " + pushToken;
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
