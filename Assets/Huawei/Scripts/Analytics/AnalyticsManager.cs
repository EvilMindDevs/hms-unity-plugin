using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Analystics;
using HuaweiMobileServices.Utils;
using System;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager GetInstance(string name = "AnalyticsManager") => GameObject.Find(name).GetComponent<AnalyticsManager>();

    private HiAnalyticsInstance instance;
    void InitilizeAnalyticsInstane()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        HiAnalyticsTools.enableLog();
        instance = HiAnalytics.getInstance(activity);
        instance.setAnalyticsEnabled(true);

    }
    public void SendEventWithBundle(String eventID, String key, String value)
    {
        Bundle bundleUnity = new Bundle();
        bundleUnity.putString(key, value);
        instance.onEvent(eventID, bundleUnity);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitilizeAnalyticsInstane();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
