using System;
using System.Threading.Tasks;
using HuaweiMobileServices.Ads;
using HuaweiMobileServices.Base;
using UnityEngine;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Location;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;

public class FusedDemo : MonoBehaviour
{
    LocationRequest mLocationRequest;

    // Define a fusedLocationProviderClient object.
    private FusedLocationProviderClient fusedLocationProviderClient;
    private AndroidPendingIntent _pendingIntent;
    private SettingsClient mSettingsClient;


    public Action<LocationResult> OnLocationResult { get; set; }
    public Action<LocationAvailability> OnLocationAvailability { get; set; }

// Instantiate the fusedLocationProviderClient object.
    private void ApplyForLocationPermission()
    {
        LocationBroadcastReceiver.ApplyActivityRecognitionPermissions();
        LocationPermissions.ApplyBackgroundLocationPermissions();
        LocationPermissions.RequestLocationPermission();
    }


    void Start()
    {
        ApplyForLocationPermission();
        fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient();
        LocationRequest mLocationRequest = new LocationRequest();
        mLocationRequest.SetInterval(10000);
        LocationCallback mLocationCallback;
        // mLocationCallback = new LocationCallback() {        
        //     public override void onLocationResult(LocationResult mLocationResult) {        
        //     if (mLocationResult != null) {        
        //     // Obtain the location information.
        //     List<HWLocation> mHWLocations = mLocationResult.getHWLocationList();
        //     for (HWLocation mHWLocation : mHWLocations) {
        //     Map<String, Object> maps = mHWLocation.getExtraInfo();
        //     parseIndoorLocation(maps);
        // }
        Debug.Log("Enes1 LocationCallBackListener 2200002 ");

        LocationCallBackListener callBack = new LocationCallBackListener(this);
        Debug.Log("Enes1 LocationCallBackListener 2 ");

        var locationCallback = new LocationCallback(callBack);
        Debug.Log("Enes1 LocationCallBackListener 3 ");

        // fusedLocationProviderClient.RequestLocationUpdates(mLocationRequest, locationCallback, Looper.GetMainLooper())
        //     .AddOnSuccessListener(
        //         (update) =>
        //         {
        //             Debug.Log("Enes1 LocationCallBackListener sucess " + update
        //                 .ToString());
        //         })
        //     .AddOnFailureListener((exception) =>
        //     {
        //         Debug.Log("Enes1 LocationCallBackListener Fail" + exception.WrappedCauseMessage + " " +
        //                   exception.WrappedExceptionMessage + "Enes1 RequestLocationUpdates Error code: " +
        //                   exception.ErrorCode);
        //     });
        // Debug.Log("Enes1 LocationCallBackListener 4 ");


        ///

            LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
            builder.AddLocationRequest(mLocationRequest);
            LocationSettingsRequest locationSettingsRequest = builder.Build();
            // Before requesting location update, invoke checkLocationSettings to check device settings.
            ITask<LocationSettingsResponse> locationSettingsResponseTask =
                mSettingsClient.CheckLocationSettings(locationSettingsRequest).AddOnSuccessListener(
                        (update) =>
                        {
                            Debug.Log("Enes1 LocationCallBackListener sucess1 " + update
                                .ToString());

                            fusedLocationProviderClient
                                .RequestLocationUpdates(mLocationRequest, locationCallback, Looper.GetMainLooper())
                                .AddOnSuccessListener(
                                    (update) =>
                                    {
                                        Debug.Log("Enes1 LocationCallBackListener sucess2 " + update
                                            .ToString());
                                    })
                                .AddOnFailureListener((exception) =>
                                {
                                    Debug.Log("Enes1 LocationCallBackListener Fail" + exception.WrappedCauseMessage +
                                              " " +
                                              exception.WrappedExceptionMessage +
                                              "Enes1 RequestLocationUpdates Error code: " +
                                              exception.ErrorCode);
                                });
                        })
                    .AddOnFailureListener((exception) =>
                    {
                        Debug.Log("Enes1 LocationCallBackListener Fail" + exception.WrappedCauseMessage + " " +
                                  exception.WrappedExceptionMessage + "Enes1 RequestLocationUpdates Error code: " +
                                  exception.ErrorCode);
                    });
            Debug.Log("Enes1 LocationCallBackListener3 4 ");
        //

        // Debug.Log("Enes1 FusedLocationDemo servise2");
        //
        // SettingsClient settingsClient = LocationServices.GetSettingsClient();
        // SetLocationMode();
        //
        // CheckLocationSettings(settingsClient);
        // Debug.Log("Enes1 fusedLocationProviderClient servise1 is basladı");
        // fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient();
        //
        // Debug.Log("Enes1 fusedLocationProviderClient servise1 İS" + fusedLocationProviderClient.FlushLocations()
        //     .AddOnSuccessListener((fusedLocationProviderClient) =>
        //     {
        //         Debug.Log("Enes1 fusedLocationProviderClient servise1 complete");
        //     })
        //     .AddOnFailureListener((exception) =>
        //     {
        //         Debug.Log("Enes1 fusedLocationProviderClient: Fail123" + exception.WrappedCauseMessage + " " +
        //                   exception.WrappedExceptionMessage + "Enes1 Error code: " + exception.ErrorCode);
        //     }));
        //
        // Debug.Log("Enes1 fusedLocationProviderClient servise2 İS" + fusedLocationProviderClient.GetLastLocation());
        //
        // Debug.Log("Enes1 fusedLocationProviderClient servise3 İS" +
        //           fusedLocationProviderClient.GetLastLocationWithAddress(mLocationRequest));
        //
        // Debug.Log("Enes1 fusedLocationProviderClient servise4 İS" +
        //           fusedLocationProviderClient.GetLocationAvailability());
        //
        // Debug.Log("Enes1 fusedLocationProviderClient servise5 İS" +
        //           fusedLocationProviderClient.GetLocationAvailability());
    }

    private void SetLocationMode()
    {
        Debug.Log("Enes1 SetLocationMode servise1 is basladı");

        mLocationRequest = LocationRequest.Create();
    }

    private void CheckLocationSettings(SettingsClient settingsClient)
    {
        LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
        Debug.Log("Enes1 CheckLocationSettings servise1");

        builder.AddLocationRequest(mLocationRequest);
        Debug.Log("Enes1 CheckLocationSettings servise2");

        LocationSettingsRequest locationSettingsRequest = builder.Build();


        settingsClient.CheckLocationSettings(locationSettingsRequest)
            .AddOnSuccessListener((locationSettingResponse) =>
            {
                LocationSettingsStates locationSettingsStates = locationSettingResponse.GetLocationSettingsStates();
            })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Fuse Demo]: Fail123" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 Error code: " + exception.ErrorCode);
            });
        Debug.Log("Enes1 [Fuse Demo]10: Complete ");
    }

    public void EnableBackgroundLocation()
    {
        // LocationCallback mLocationCallback;
        // mLocationCallback = new LocationCallback().OnLocationAvailability(LocationAvailability.ExtractLocationAvailability());
        //
        //

        _pendingIntent = LocationBroadcastReceiver.GetPendingIntent();

        fusedLocationProviderClient.RequestLocationUpdates(mLocationRequest, _pendingIntent).AddOnSuccessListener(
                (update) =>
                {
                    Debug.Log("Enes1 [Fuse Demo]10: Complete RequestLocationUpdates sucess " + update
                        .ToString());
                })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Fuse Demo]: RequestLocationUpdates Fail" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 RequestLocationUpdates Error code: " +
                          exception.ErrorCode);
            });

        fusedLocationProviderClient.RemoveLocationUpdates(_pendingIntent).AddOnSuccessListener((update) =>
            {
                Debug.Log("Enes1 [Fuse Demo]11: Complete RemoveLocationUpdates sucess ");
            })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Fuse Demo]11: RemoveLocationUpdates Fail" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 RemoveLocationUpdates Error code: " +
                          exception.ErrorCode);
            });

        fusedLocationProviderClient.SetMockMode(true).AddOnSuccessListener((update) =>
            {
                Debug.Log("Enes1 [Fuse Demo]12: Complete SetMockMode sucess ");
            })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Fuse Demo]13: SetMockMode Fail" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 SetMockMode Error code: " + exception.ErrorCode);
            });

        // mockLocation.SetLongitude(118.76);
        // mockLocation.SetLatitude(31.98);
        //         fusedLocationProviderClient.SetMockLocation(mockLocation).AddOnSuccessListener((update) =>
        //             {
        //                 Debug.Log("Enes1 [Fuse Demo]15: Complete SetMockLocation sucess ");
        //             })
        //             .AddOnFailureListener((exception) =>
        //             {
        //                 Debug.Log("Enes1 [Fuse Demo]15: SetMockLocation Fail" + exception.WrappedCauseMessage + " " +
        //                           exception.WrappedExceptionMessage + "Enes1 SetMockLocation Error code: " + exception.ErrorCode);
        //             });
        //         Debug.Log("Enes1 [Fuse Demo]15: GetLastLocation SetMockMode sucess mucket"+mockLocation );

        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.GPS_PROVIDER );
        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.EXTRA_PROVIDER_NAME );
        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.KEY_LOCATION_CHANGED );
        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.EXTRA_LOCATION_ENABLED );
        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.KEY_PROVIDER_ENABLED );
        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.EXTRA_PROVIDER_ENABLED );
        // Debug.Log("Enes1 [Fuse Demo]16: GetLastLocation SetMockMode sucess mucket"+LocationManager.KEY_PROXIMITY_ENTERING );

        // mLocationCallback.GetCallback().OnLocationResult(locationResult =>
        // {
        //     Debug.Log("Enes1 [Fuse Demo]11: Complete "+locationResult.LastLocation);
        // }).OnLocationAvailability(locationAvailabilityResult =>
        // {
        //     Debug.Log("Enes1 [Fuse Demo]12: Complete "+locationAvailabilityResult.IsLocationAvailable);
        // });
        //

        //    
        //    var mLocationAva = new LocationAvailability();
        //    var locationResult = new LocationResult();
        //    
        // mLocationCallback.OnLocationAvailability(mLocationAva);
        // mLocationCallback.OnLocationResult(locationResult);
    }

    public void InDoorLocationService()
    {
//         LocationRequest locationRequest = new LocationRequest();
// // Set the location update interval, in milliseconds.
//         locationRequest.SetInterval(1000);
//
//         LocationBridge.GetLocationResult();
//         // if (locationRequest != null)
//         // {
//         //     Debug.Log("Enes1 [Fuse Demo]20: Complete "+locationResult.GetLastLocation().ToString());
//         //
//         // }
//         // else
//         // {
//         //     Debug.Log("Enes1 [Fuse Demo]20: null");
//         // }
//
//         //
//         // Func<LocationCallback, int> Toplam = ToplamMetodu;
//         //
//         // 	
//         // Func<LocationCallback,LocationAvailability> locationAvailability = (LocationCallback locationCallback) =>
//         //     
//         //     
//         //     
//         //                        LocationCallback locationCallback;
//         // locationCallback = new LocationCallback();
//         // locationCallback.OnLocationAvailability();


        var mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient();
        _pendingIntent = LocationBroadcastReceiver.GetPendingIntent();

        //var locationCallback = new LocationCallback();

        //
        // locationCallback.OnLocationAvailability( () =>
        // {
        //     Debug.Log("Enes1 OnLocationAvailability 11111 ");
        // }, (exception) =>
        // {
        //     Debug.Log("Enes1 OnLocationAvailability 2222 ");
        //     Debug.LogError("Enes1 [OnLocationAvailability]: OnLocationAvailability failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
        // });


        //   locationCallback.OnLocationAvailability();
        Debug.Log("Enes1 GetLocationResult 000 ");

        // mFusedLocationProviderClient.RequestLocationUpdates(mLocationRequest, locationCallback, Looper.GetMainLooper())
        //     .AddOnSuccessListener(
        //         (update) =>
        //         {
        //             Debug.Log("Enes1 23423423423: Complete RequestLocationUpdates RequestLocationUpdates " + update
        //                 .ToString());
        //         })
        //     .AddOnFailureListener((exception) =>
        //     {
        //         Debug.Log("Enes1 23424234: RequestLocationUpdates Fail" + exception.WrappedCauseMessage + " " +
        //                   exception.WrappedExceptionMessage + "Enes1 RequestLocationUpdates Error code: " +
        //                   exception.ErrorCode);
        //     });

        Debug.Log("Enes1 RequestLocationUpdates 1100011 ");


        //var asd =mFusedLocationProviderClient.GetLastLocation();
        // Debug.Log("Enes1 RequestLocationUpdates 1100011 "+asd.ToString());


        var locationAvailability = mFusedLocationProviderClient.GetLocationAvailability();


        var mSettingsClient = LocationServices.GetSettingsClient();
        mLocationRequest = new LocationRequest();
        // Sets the interval for location update (unit: Millisecond)
        mLocationRequest.SetInterval(5000);
        // mLocationRequest
        Debug.Log("Enes1 GetLocationResult 111 ");


        //  Debug.Log("Enes1 GetLocationResult 155 " + locationCallback.ToString());


        LocationBridge.GetLocationResult();

        Debug.Log("Enes1 GetLocationResult 222 ");
    }

    public class LocationCallBackListener : ILocationCallback
    {
        private readonly FusedDemo _fusedDemo;

        public LocationCallBackListener(FusedDemo fusedDemo)
        {
            _fusedDemo = fusedDemo;
        }

        public void OnLocationResult(LocationResult result)
        {
            _fusedDemo.OnLocationResult?.Invoke(result);
            Debug.Log("Enes1 LocationCallBackListener 222 " + result.GetLastLocation().ToString());
        }

        public void OnLocationAvailability(LocationAvailability availability)
        {
            _fusedDemo.OnLocationAvailability?.Invoke(availability);
            Debug.Log("Enes1 LocationCallBackListener 222 " + availability.IsLocationAvailable);
        }
    }
}