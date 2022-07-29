using System;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Geofences;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.Android;
using HuaweiMobileServices.Location.Location;

public class HMSLocationManager : HMSManagerSingleton<HMSLocationManager>
{
    public Action<LocationResult> onLocationResult;
    public Action<LocationAvailability> onLocationAvailability;

    public HMSLocationManager()
    {
        Debug.Log("[HMS] HMSAdsKitManager Constructor");
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(OnAwake);
        HMSDispatcher.InvokeAsync(OnStart);
    }

    private void OnAwake()
    {
        Debug.Log("[HMS] HMSLocationManager OnAwake");
    }

    private void OnStart()
    {
        Debug.Log("[HMS] HMSLocationManager OnStart");
    }

    public AndroidPendingIntent GetPendingIntentFromLocation() => LocationBroadcastReceiver.GetPendingIntent();
    public AndroidPendingIntent GetPendingIntentFromGeofence() => GeofenceBroadcastReceiver.GetPendingIntent();

    #region FusedLocation

    public LocationCallback DefineLocationCallback()
    {
        Debug.Log("[HMS] HMSLocationManager DefineLocationCallback");

        LocationBridge.SetLocationCallbackListener
        (new LocationCallbackListener
            (LocationCallbackListener_onLocationResult, LocationCallbackListener_onLocationAvailability));

        return LocationBridge.GetLocationResult();
    }

    private void LocationCallbackListener_onLocationResult(LocationResult locationResult)
    {
        Debug.LogError("Enes1 [HMS] LocationCallbackListener_onLocationResult ");
        onLocationResult?.Invoke(locationResult);
    }

    private void LocationCallbackListener_onLocationAvailability(LocationAvailability locationAvailability)
    {
        Debug.LogError("Enes1 [HMS] LocationCallbackListener_onLocationAvailability ");
        onLocationAvailability?.Invoke(locationAvailability);
    }

    #endregion

    #region Permissions

    public void ApplyForAllLocationPermissions()
    {
        LocationPermissions.RequestActivityRecognitionPermissions();
        LocationPermissions.RequestBackgroundLocationPermissions();
        LocationPermissions.RequestLocationPermission();
        RequestFineLocationPermission();
        RequestCoarseLocationPermission();
    }

    public void RequestActivityRecognitionPermissions() => LocationPermissions.RequestActivityRecognitionPermissions();

    public void RequestBackgroundLocationPermissions() => LocationPermissions.RequestBackgroundLocationPermissions();

    public void RequestFineLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    public void RequestCoarseLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
    }

    #endregion
}