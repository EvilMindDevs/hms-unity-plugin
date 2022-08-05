using System;
using System.Text;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Location;
using UnityEngine;
using UnityEngine.UI;

namespace Huawei.Demos.Location
{
    public class FusedLocationDemo : MonoBehaviour
    {
        private static String TAG = "FusedLocationDemo";

        private LocationRequest locationRequest;
        private FusedLocationProviderClient fusedLocationProviderClient;
        private LocationCallback locationCallback;
        [SerializeField] private Text resultText;
        [SerializeField] private Text lastKnownLocationText;
        [SerializeField] private Text locationAvailabilityText;

        private void Start()
        {
            HMSLocationManager.Instance.RequestFineLocationPermission();
            HMSLocationManager.Instance.RequestCoarseLocationPermission();

            HMSLocationManager.Instance.onLocationResult += OnLocationResult;
            HMSLocationManager.Instance.onLocationAvailability += OnLocationAvailability;
        }

        private void OnLocationResult(LocationResult locationResult)
        {
            Debug.Log($"{TAG} Location Result success");
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var location in locationResult.GetLocations())
            {
                stringBuilder.Append(",\nLocation = ").Append("Latitude " + location.GetLatitude())
                    .Append(" Longitude " + location.GetLongitude());
                resultText.text += stringBuilder.ToString();
            }
        }

        private void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            var isLocationAvailable = locationAvailability.IsLocationAvailable;
            locationAvailabilityText.text = $"LocationStatus = {isLocationAvailable}";
        }

        private void CheckLocationSettings(LocationRequest locationRequest)
        {
            SettingsClient settingsClient = LocationServices.GetSettingsClient();
            LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
            builder.AddLocationRequest(locationRequest);

            LocationSettingsRequest locationSettingsRequest = builder.Build();

            settingsClient.CheckLocationSettings(locationSettingsRequest)
                .AddOnSuccessListener(locationSettingsResponse =>
                {
                    LocationSettingsStates locationSettingsStates =
                        locationSettingsResponse.GetLocationSettingsStates();
                    StringBuilder stringBuilder = new StringBuilder();

                    // Check whether the location function is enabled. 
                    stringBuilder.Append(",\nisLocationUsable=").Append(locationSettingsStates.IsLocationUsable());

                    // Check whether HMS Core (APK) is available. 
                    stringBuilder.Append(",\nisHMSLocationUsable=")
                        .Append(locationSettingsStates.IsHMSLocationUsable());

                    Debug.Log($"{TAG} checkLocationSetting onComplete:" + stringBuilder);
                })
                .AddOnFailureListener(exception =>
                {
                    Debug.Log($"checkLocationSetting onFailure, Cause Message: {exception.WrappedCauseMessage} " +
                              $"Exception message: {exception.WrappedExceptionMessage} Error code : {exception.ErrorCode}");
                });
        }

        public void RequestLocationUpdatesWithCallback()
        {
            try
            {
                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient();
                locationRequest = new LocationRequest();
                locationRequest.SetInterval(1000);

                if (locationCallback == null) locationCallback = HMSLocationManager.Instance.DefineLocationCallback();

                fusedLocationProviderClient
                    .RequestLocationUpdates(locationRequest, locationCallback, Looper.GetMainLooper())
                    .AddOnSuccessListener(
                        (update) => { Debug.Log($"{TAG} RequestLocationUpdatesWithCallback success"); })
                    .AddOnFailureListener((exception) =>
                    {
                        Debug.LogError($"{TAG} LocationCallBackListener Fail" + exception.WrappedCauseMessage + " " +
                                       exception.WrappedExceptionMessage +
                                       $"{TAG} RequestLocationUpdates Error code: " +
                                       exception.ErrorCode);
                    });
            }
            catch (Exception exception)
            {
                Debug.Log($"{TAG} {exception.Message}");
            }
        }

        public void RemoveLocationUpdatesWithCallback()
        {
            try
            {
                fusedLocationProviderClient.RemoveLocationUpdates(locationCallback)
                    .AddOnSuccessListener(
                        (update) => { Debug.Log($"{TAG} removeLocationUpdatesWithIntent onSuccess"); })
                    .AddOnFailureListener((exception) =>
                    {
                        Debug.LogError(
                            $"{TAG} removeLocationUpdatesWithIntent onFailure: {exception.WrappedExceptionMessage}");
                    });
            }
            catch (Exception exception)
            {
                Debug.LogError($"{TAG} removeLocationUpdatesWithIntent exception: {exception}");
            }
        }

        private void OnDestroy()
        {
            RemoveLocationUpdatesWithCallback();
        }

        public void GetLastKnownLocation()
        {
            fusedLocationProviderClient.GetLastLocation()
                .AddOnSuccessListener(location =>
                {
                    Debug.Log($"{TAG} GetLastKnownLocation success");
                    if (location != null)
                    {
                        lastKnownLocationText.text =
                            $"{TAG} GetLastKnownLocation Latitude: {location.GetLatitude()} Longitude: {location.GetLongitude()}";
                    }
                    else
                    {
                        Debug.LogError($"{TAG} GetLastKnownLocation not available");
                    }
                }).AddOnFailureListener(exception =>
                    {
                        Debug.LogError($"{TAG} GetLastKnownLocation exception: {exception}");
                    }
                );
        }
    }
}