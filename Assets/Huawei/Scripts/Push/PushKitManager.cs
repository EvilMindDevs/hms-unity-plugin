using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace HmsPlugin
{
    public class PushKitManager : MonoBehaviour, IPushListener
    {

        public Action<string> OnTokenSuccess { get; set; }
        public Action<Exception> OnTokenFailure { get; set; }

        public Action<RemoteMessage> OnMessageReceivedSuccess { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            PushManager.Listener = this;
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

        public void OnMessageReceived(RemoteMessage remoteMessage)
        {
            OnMessageReceivedSuccess?.Invoke(remoteMessage);
        }
    }
}