using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace HmsPlugin
{
    public class HMSPushKitManager : HMSManagerSingleton<HMSPushKitManager>, IPushListener
    {
        public Action<string> OnTokenSuccess { get; set; }
        public Action<Exception> OnTokenFailure { get; set; }
        public Action<string, Bundle> OnTokenBundleSuccess { get; set; }
        public Action<Exception, Bundle> OnTokenBundleFailure { get; set; }
        public Action<string> OnMessageSentSuccess { get; set; }
        public Action<string, Exception> OnSendFailure { get; set; }
        public Action<string, Exception> OnMessageDeliveredSuccess { get; set; }
        public Action<RemoteMessage> OnMessageReceivedSuccess { get; set; }
        public Action<NotificationData> OnNotificationMessage { get; set; }
        public Action<NotificationData> NotificationMessageOnStart { get; set; }

        public NotificationData notificationDataOnStart;

        public HMSPushKitManager()
        {
            Debug.Log($"[HMS] : HMSPushKitManager Constructor");
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
        }

        private void OnAwake()
        {
            Debug.Log($"[HMS] : HMSPushKitManager OnAwake");
            PushManager.Listener = this;
            notificationDataOnStart = PushManager.NotificationDataOnStart;
        }

        public void Init() 
        {
            if (notificationDataOnStart.NotifyId != -1)
            {
                NotificationMessageOnStart?.Invoke(notificationDataOnStart);
            }
            PushManager.RegisterOnNotificationMessage((data) =>
            {
                OnNotificationMessage?.Invoke(data);
            });

            var token = PushManager.Token;
            Debug.Log($"[HMS] Push token from GetToken is {token}");
            if (token != null)
            {
                OnTokenSuccess?.Invoke(token);
            }
        }

        public void OnNewToken(string token)
        {
            Debug.Log($"[HMS] Push token from OnNewToken is {token}");
            if (token != null)
            {
                OnTokenSuccess?.Invoke(token);
            }
        }

        public void OnTokenError(Exception e)
        {
            Debug.Log("Error asking for Push token");
            Debug.Log(e.StackTrace);
            OnTokenFailure?.Invoke(e);
        }

        // This method only gets triggered if Data Message is sent by Push Kit Server/AGC.
        public void OnMessageReceived(RemoteMessage remoteMessage)
        {
            Debug.Log("[HMSPushKit] Data Message received");
            Debug.Log("Data: " + remoteMessage.Data);
            OnMessageReceivedSuccess?.Invoke(remoteMessage);
        }

        public void OnNewToken(string token, Bundle bundle)
        {
            OnTokenBundleSuccess?.Invoke(token, bundle);
        }

        public void OnTokenError(Exception exception, Bundle bundle)
        {
            OnTokenBundleFailure?.Invoke(exception, bundle);
        }

        public void OnMessageSent(string msgId)
        {
            OnMessageSentSuccess?.Invoke(msgId);
        }

        public void OnMessageDelivered(string msgId, Exception exception)
        {
            OnMessageDeliveredSuccess?.Invoke(msgId, exception);
        }

        public void OnSendError(string msgId, Exception exception)
        {
            OnSendFailure?.Invoke(msgId, exception);
        }
        public void TurnOnPush()
        {
            HmsMessaging.GetInstance().TurnOnPush().AddOnSuccessListener((type) =>
            {
                Debug.Log("[HMSPlugin]: TurnOnPush Complete");

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMSPlugin]: TurnOnPush Failed" + exception.Message);

            });
        }
        public void TurnOffPush()
        {
            HmsMessaging.GetInstance().TurnOffPush().AddOnSuccessListener((type) =>
            {
                Debug.Log("[HMSPlugin]: TurnOffPush Complete");

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMSPlugin]: TurnOffPush Failed" + exception.Message);

            });
        }
        public void DeleteToken()
        {
            HmsInstanceId.GetInstance().DeleteToken(MetadataHelper.AppId, HmsMessaging.DEFAULT_TOKEN_SCOPE);
        }
    }
}
