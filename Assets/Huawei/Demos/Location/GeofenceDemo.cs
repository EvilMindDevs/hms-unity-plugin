using System;
using HuaweiMobileServices.Location.Geofences;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using Huawei.Scripts.Location;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Location;

public class GeofenceDemo : MonoBehaviour
{
    private static string TAG = "GeofenceDemo";

    [SerializeField] private Text resultText;
    [SerializeField] private Text latitudeText;
    [SerializeField] private Text longitudeText;

    private GeofenceService geofenceService;
    private List<string> idList;
    private List<Geofence> geofenceList;
    private AndroidPendingIntent pendingIntent;
    private double latitude;
    private double longitude;
    private FusedLocationProviderClient fusedLocationProviderClient;
    private LocationRequest locationRequest;
    private LocationCallback locationCallback;

    private void Start()
    {
        HMSLocationManager.Instance.RequestFineLocationPermission();
        HMSLocationManager.Instance.RequestCoarseLocationPermission();
        HMSLocationManager.Instance.RequestBackgroundLocationPermissions();
        
        GeofenceReceiver.Instance.onReceive += OnReceive;
        HMSLocationManager.Instance.onLocationResult += OnLocationResult;
        GeofenceReceiver.Instance.SetGeofenceBroadcastListener();
    }

    public void GetLocation()
    {
        HMSLocationManager.Instance.DefineLocationCallback();

        fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient();
        locationRequest = new LocationRequest();
        locationRequest.SetInterval(10000);
        locationRequest.SetNumUpdates(1);

        if (locationCallback == null) locationCallback = HMSLocationManager.Instance.DefineLocationCallback();

        fusedLocationProviderClient
            .RequestLocationUpdates(locationRequest, locationCallback, Looper.GetMainLooper())
            .AddOnSuccessListener(
                (update) => { Debug.Log($"{TAG} RequestLocationUpdates success"); })
            .AddOnFailureListener((exception) =>
            {
                Debug.LogError($"{TAG} RequestLocationUpdates Fail " + exception.WrappedCauseMessage + " " +
                               exception.WrappedExceptionMessage + " HMS RequestLocationUpdates Error code: " +
                               exception.ErrorCode);
            });
    }

    private void OnLocationResult(LocationResult locationResult)
    {
        Debug.Log($"{TAG} OnLocationResult success");
        
        var location = locationResult.GetLastLocation();
        latitude = location.GetLatitude();
        longitude = location.GetLongitude();

        latitudeText.text = latitude.ToString();
        longitudeText.text = longitude.ToString();
    }

    private void OnReceive(AndroidIntent intent)
    {
        Debug.Log($"{TAG} OnReceive success");

        GeofenceData geofenceData = GeofenceData.GetDataFromIntent(intent);

        DisplayGeofenceData(geofenceData);
    }

    private void DisplayGeofenceData(GeofenceData geofenceData)
    {
        if (geofenceData != null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var conversion = geofenceData.GetConversion;
            var status = geofenceData.IsSuccess;
            var errorCode = geofenceData.GetErrorCode;
            var mLocation = geofenceData.GetConvertingLocation();

            stringBuilder.Append("\nConversion = " + conversion)
                .Append("\nStatus = " + status)
                .Append("\nerrorCode= " + errorCode)
                .Append("\nLocation= ")
                .Append("Latitude " + mLocation.GetLatitude() + "\nLongitude " + mLocation.GetLongitude());

            resultText.text = stringBuilder.ToString();
        }
    }

    private void InitGeofenceServiceClient()
    {
        // Create a GeofenceService instance.
        geofenceService = LocationServices.GetGeofenceService();

        // Obtain a PendingIntent object.
        pendingIntent = HMSLocationManager.Instance.GetPendingIntentFromGeofence();
        idList = new List<String>();
        geofenceList = new List<Geofence>();
    }

    private void CreateGeofence()
    {
        geofenceList.Add(new Geofence.Builder()
            .SetUniqueId("mGeofence")
            .SetValidContinueTime(1000000)
            .SetDwellDelayTime(10000)
            .SetNotificationInterval(100)

            // Pass the longitude and latitude of the geofence center, and radius of the circle geofence (unit: meters).
            .SetRoundArea(latitude, longitude, 60f)
            // Trigger a callback when the user enters or leaves the geofence.
            .SetConversions(5)
            .Build());

        idList.Add("mGeofence");
    }

    private GeofenceRequest GetAddGeofenceRequest()
    {
        GeofenceRequest.Builder builder = new GeofenceRequest.Builder();
        builder.CreateGeofenceList(geofenceList);
        builder.SetInitConversions(5);
        return builder.Build();
    }

    public void InitGeofence()
    {
        InitGeofenceServiceClient();
        CreateGeofence();
    }

    public void RequestAddGeofence()
    {
        var geofenceRequest = GetAddGeofenceRequest();

        geofenceService.CreateGeofenceList(geofenceRequest, pendingIntent)
            .AddOnSuccessListener(type =>
            {
                resultText.text= "GeofenceList Successfully Created ";
                Debug.Log($"{TAG} CreateGeofenceList Successful");
            })
            .AddOnFailureListener(exception =>
            {
                Debug.LogError(
                    $"{TAG} CreateGeofenceList Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
            });
    }

    public void DeleteGeofence()
    {
        geofenceService.DeleteGeofenceList(pendingIntent)
            .AddOnSuccessListener(type =>
            {
                resultText.text= "GeofenceList Successfully Deleted ";
                Debug.Log($"{TAG} DeleteGeofenceList Successful");
            })
            .AddOnFailureListener(exception =>
            {
                Debug.LogError(
                    $"{TAG} DeleteGeofenceList Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
            });
    }
}