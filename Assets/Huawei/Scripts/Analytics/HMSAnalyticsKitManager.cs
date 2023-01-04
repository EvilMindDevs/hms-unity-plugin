using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Analystics;
using HuaweiMobileServices.Utils;
using System;
using System.Threading.Tasks;
using System.Threading;

public class HMSAnalyticsKitManager : HMSManagerSingleton<HMSAnalyticsKitManager>
{
    private HiAnalyticsInstance hiAnalyticsInstance;
    private AndroidJavaObject activity;

    public HMSAnalyticsKitManager()
    {
        Debug.Log($"[HMS] : HMSAnalyticsKitManager Constructor");
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(InitilizeAnalyticsInstane);
    }

    void InitilizeAnalyticsInstane()
    {
        Debug.Log("HMSAnalyticsKitManager: InitilizeAnalyticsInstane");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            HiAnalyticsTools.EnableLog();
            hiAnalyticsInstance = HiAnalytics.GetInstance(activity);
            hiAnalyticsInstance.SetAnalyticsEnabled(true);
        }));
    }

    public void SendEventWithBundle(string eventID, string key, string value) 
    {
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            _SendEventWithBundle(eventID, key, value);
        }));
    }

    void _SendEventWithBundle(string eventID, string key, string value)
    {
        Bundle bundleUnity = new Bundle();
        bundleUnity.PutString(key, value);
        Debug.Log($"[HMS] : Analytics Kits Event Id:{eventID} Key:{key} Value:{value}");
        hiAnalyticsInstance.OnEvent(eventID, bundleUnity);
    }

    public void SendEventWithBundle(string eventID, Dictionary<string, object> values)
    {
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            _SendEventWithBundle(eventID, values);
        }));
    }

    void _SendEventWithBundle(string eventID, Dictionary<string, object> values)
    {
        Bundle bundleUnity = new Bundle();
        foreach (var item in values)
        {
            if (item.Value is int)
            {
                bundleUnity.PutInt(item.Key, (int)item.Value);
            }
            else if (item.Value is string)
            {
                bundleUnity.PutString(item.Key, (string)item.Value);
            }
            else if (item.Value is bool)
            {
                bundleUnity.PutBoolean(item.Key, (bool)item.Value);
            }
        }

        Debug.Log($"[HMS] : Analytics Kits Event Id:{eventID}");
        foreach (var item in values)
            Debug.Log($"[HMS] : Analytics Kits Key: {item.Key}, Value: {item.Value}");
        hiAnalyticsInstance.OnEvent(eventID, bundleUnity);
    }

    public void SendEventWithBundle(string eventID, string key, int value)
    {
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            _SendEventWithBundle(eventID, key, value);
        }));
    }

    void _SendEventWithBundle(string eventID, string key, int value)
    {
        Bundle bundleUnity = new Bundle();
        bundleUnity.PutInt(key, value);
        Debug.Log($"[HMS] : Analytics Kits Event Id:{eventID} Key:{key} Value:{value}");
        hiAnalyticsInstance.OnEvent(eventID, bundleUnity);
    }

}
