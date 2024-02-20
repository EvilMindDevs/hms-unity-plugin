using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Analystics;
using HuaweiMobileServices.Utils;

namespace HmsPlugin
{
    public class HMSAnalyticsKitManager : HMSManagerSingleton<HMSAnalyticsKitManager>
    {
        private readonly string TAG = "[HMS] : HMSAnalyticsKitManager";
        private HiAnalyticsInstance hiAnalyticsInstance;
        private AndroidJavaObject activity;

        public HMSAnalyticsKitManager()
        {
            HMSManagerStart.Start(TAG, true, InitializeAnalyticsInstant);
        }

        void InitializeAnalyticsInstant()
        {
            Debug.Log($"{TAG} InitializeAnalyticsInstant");
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
            Debug.Log($"{TAG} : Analytics Kits Event Id:{eventID} Key:{key} Value:{value}");
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
            Debug.Log($"{TAG} : Analytics Kits Event Id:{eventID}");
            foreach (var item in values)
                Debug.Log($"{TAG} : Analytics Kits Key: {item.Key}, Value: {item.Value}");
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
            Debug.Log($"{TAG} : Analytics Kits Event Id:{eventID} Key:{key} Value:{value}");
            hiAnalyticsInstance.OnEvent(eventID, bundleUnity);
        }

    }
}
