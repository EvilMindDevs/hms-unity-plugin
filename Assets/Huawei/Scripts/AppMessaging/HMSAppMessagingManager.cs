using HuaweiMobileServices.AppMessaging;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HMSAppMessagingManager : HMSManagerSingleton<HMSAppMessagingManager>
{
    public Action<AppMessage> OnMessageClicked { get; set; }
    public Action<AppMessage> OnMessageDisplay { get; set; }

    public Action<AppMessage, DismissType> OnMessageDissmiss { get; set; }
    public Action<AAIDResult> AAIDResultAction { get; set; }

    public HMSAppMessagingManager()
    {
        Debug.Log($"[HMS] : HMSAppMessagingManager Constructor");
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(OnAwake);
    }

    private void OnAwake()
    {
        Debug.Log("AppMessaging: OnAwake");
        HmsInstanceId inst = HmsInstanceId.GetInstance();
        ITask<AAIDResult> idResult = inst.AAID;
        idResult.AddOnSuccessListener((result) =>
        {
            AAIDResult AAIDResult = result;
            Debug.Log("AppMessaging: " + result.Id);
            AAIDResultAction?.Invoke(result);
        }).AddOnFailureListener((exception) =>
        {

        });
        OnMessageClicked = OnMessageClickFunction;
        OnMessageDisplay = OnMessageDisplayFunction;
        OnMessageDissmiss = OnMessageDissmissFunction;
        AGConnectAppMessaging appMessaging = AGConnectAppMessaging.Instance;
        appMessaging.AddOnClickListener(OnMessageClicked);
        appMessaging.AddOnDisplayListener(OnMessageDisplay);
        appMessaging.AddOnDismissListener(OnMessageDissmiss);
        appMessaging.SetForceFetch();
    }
    private void OnMessageClickFunction(AppMessage obj)
    {
        Debug.Log("AppMessaging  OnMessageClickFunction");
    }
    private void OnMessageDisplayFunction(AppMessage obj)
    {
        Debug.Log("AppMessaging OnMessageDisplayFunction" + obj.MessageType);
    }

    private void OnMessageDissmissFunction(AppMessage obj, DismissType dismissType)
    {
        Debug.Log("AppMessaging OnMessageDissmissFunction" + obj.MessageType);
    }
}

