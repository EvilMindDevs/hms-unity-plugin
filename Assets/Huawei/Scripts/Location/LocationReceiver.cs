using System;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Utils;
using UnityEngine;

namespace Huawei.Scripts.Location
{
    public class LocationReceiver : HMSManagerSingleton<LocationReceiver>
    {
        private static string TAG = "LocationReceiver";
        
        public Action<AndroidIntent> onReceive;

        public static bool isListenActivityIdentification;

        public static bool isListenActivityConversion;

        public void SetLocationBroadcastListener()
        {
            LocationBroadcastReceiver.SetLocationCallbackListener(
                new BroadcastListener
                    (LocationBroadcastListener_onReceive));
        }

        private void LocationBroadcastListener_onReceive(AndroidIntent intent)
        {
            Debug.LogError($"{TAG} [HMS] LocationBroadcastListener_onReceive ");
            onReceive?.Invoke(intent);
        }

        public static void AddConversionListener() => isListenActivityConversion = true;

        public static void RemoveConversionListener() => isListenActivityConversion = false;

        public static void AddIdentificationListener() => isListenActivityIdentification = true;

        public static void RemoveIdentificationListener() => isListenActivityIdentification = false;
    }
}