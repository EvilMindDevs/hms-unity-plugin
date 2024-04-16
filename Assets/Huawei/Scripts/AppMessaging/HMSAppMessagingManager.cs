using HuaweiMobileServices.AppMessaging;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSAppMessagingManager : HMSManagerSingleton<HMSAppMessagingManager>
    {
        private const string TAG = "[HMS] HMSAppMessagingManager";
        public Action<AppMessage> OnMessageClicked { get; set; }
        public Action<AppMessage> OnMessageDisplay { get; set; }
        public Action<AppMessage, DismissType> OnMessageDismiss { get; set; }
        public Action<AAIDResult> AAIDResultAction { get; set; }

        public HMSAppMessagingManager()
        {
            HMSManagerStart.Start(OnAwake, TAG);
        }

        private void OnAwake()
        {
            Debug.Log($"{TAG}: OnAwake");
            HmsInstanceId inst = HmsInstanceId.GetInstance();
            ITask<AAIDResult> idResult = inst.AAID;
            idResult.AddOnSuccessListener((result) =>
            {
                AAIDResult AAIDResult = result;
                Debug.Log($"{TAG}: result.Id");
                AAIDResultAction?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError($"{TAG}: exception.Message");
            });
            OnMessageClicked = OnMessageClickFunction;
            OnMessageDisplay = OnMessageDisplayFunction;
            OnMessageDismiss = OnMessageDismissFunction;
            AGConnectAppMessaging appMessaging = AGConnectAppMessaging.Instance;
            appMessaging.AddOnClickListener(OnMessageClicked);
            appMessaging.AddOnDisplayListener(OnMessageDisplay);
            appMessaging.AddOnDismissListener(OnMessageDismiss);
            appMessaging.SetForceFetch();
        }
        private void OnMessageClickFunction(AppMessage obj)
        {
            Debug.Log($"{TAG} OnMessageClickFunction");
        }
        private void OnMessageDisplayFunction(AppMessage obj)
        {
            Debug.Log($"{TAG} OnMessageDisplayFunction" + obj.MessageType);
        }

        private void OnMessageDismissFunction(AppMessage obj, DismissType dismissType)
        {
            Debug.Log($"{TAG} OnMessageDismissFunction" + obj.MessageType);
        }
    }

}
