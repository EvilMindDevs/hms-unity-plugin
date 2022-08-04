using System;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Geofences;
using HuaweiMobileServices.Utils;
using UnityEngine;

namespace Huawei.Scripts.Location
{
    public class GeofenceReceiver : HMSManagerSingleton<GeofenceReceiver>
    {
        private static string TAG = "GeofenceDemo";

        public Action<AndroidIntent> onReceive;

        public void SetGeofenceBroadcastListener()
        {
            GeofenceBroadcastReceiver.SetGeofenceBroadcastListener(
                new BroadcastListener
                    (GeofenceBroadcastListener_onReceive));
        }

        private void GeofenceBroadcastListener_onReceive(AndroidIntent intent)
        {
            Debug.LogError($"{TAG} [HMS] GeofenceBroadcastListener_onReceive ");
            onReceive?.Invoke(intent);
        }
    }
}