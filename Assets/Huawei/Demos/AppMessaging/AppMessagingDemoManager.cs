using HuaweiMobileServices.AppMessaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppMessagingDemoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AGConnectAppMessaging appMessaging = AGConnectAppMessaging.Instance;
        appMessaging.AddOnClickListener(OnMessageClickFunction);
        appMessaging.AddOnDisplayListener(OnMessageDisplayFunction);
        appMessaging.AddOnDismissListener(OnMessageDissmissFunction);
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
