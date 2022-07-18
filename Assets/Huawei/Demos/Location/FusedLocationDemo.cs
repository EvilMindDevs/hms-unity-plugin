using UnityEngine;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Location;

public class FusedLocationDemo : MonoBehaviour
{
    LocationRequest mLocationRequest;

    // Define a fusedLocationProviderClient object.
    private FusedLocationProviderClient fusedLocationProviderClient;

// Instantiate the fusedLocationProviderClient object.
    private void ApplyForLocationPermission()
    {
        LocationBroadcastReceiver.ApplyActivityRecognitionPermissions();
    }


    void Start()
    {
        ApplyForLocationPermission();
        Debug.Log("Enes1 FusedLocationDemo servise1");

        LocationPermissions.ApplyBackgroundLocationPermissions();

        LocationPermissions.RequestLocationPermission();


        Debug.Log("Enes1 FusedLocationDemo servise2");

        SettingsClient settingsClient = LocationServices.GetSettingsClient();
        Debug.Log("Enes1 FusedLocationDemo servise3");


        CheckLocationSettings(settingsClient);
        Debug.Log("Enes1 FusedLocationDemo servise4");

        fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient();
        Debug.Log("Enes1 FusedLocationDemo servise5");

        SetLocationMode();
        Debug.Log("Enes1 FusedLocationDemo servise6");
    }

    private void SetLocationMode()
    {
        LocationRequest mLocationRequest = new LocationRequest();
        mLocationRequest.SetNumUpdates(1);
    }

    private void CheckLocationSettings(SettingsClient settingsClient)
    {
        LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
        mLocationRequest = new LocationRequest();
        Debug.Log("Enes1 CheckLocationSettings servise1");

        builder.AddLocationRequest(mLocationRequest);
        Debug.Log("Enes1 CheckLocationSettings servise2");

        LocationSettingsRequest locationSettingsRequest = builder.Build();

        settingsClient.CheckLocationSettings(locationSettingsRequest)
            .AddOnSuccessListener((locationSettingResponse) =>
            {
                Debug.Log("Enes1 [Fuse Demo234]: Complete " + locationSettingResponse.ToString());
                LocationSettingsStates locationSettingsStates = locationSettingResponse.GetLocationSettingsStates();
                Debug.Log("Enes1 [Fuse Demo234]: Complete " + locationSettingResponse.ToString());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsBleUsable());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsGnssPresent());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsGnssUsable());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsLocationPresent());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsLocationUsable());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsNetworkLocationPresent());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsNetworkLocationUsable());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsHMSLocationPresent());
                Debug.Log("Enes1 [Fuse Demo]: Complete " + locationSettingsStates.IsHMSLocationUsable());
            })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Fuse Demo]: Fail123" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 Error code: " + exception.ErrorCode);
            });
    }

    // Update is called once per frame
    void Update()
    {
    }
}