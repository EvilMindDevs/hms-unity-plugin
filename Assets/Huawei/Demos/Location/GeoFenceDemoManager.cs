using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Utils;
using HuaweiMobileServices.Location.Geofences;
using HuaweiMobileServices.Location.Location;
using UnityEngine.Android;


public class GeoFenceDemoManager : MonoBehaviour
{
    private GeofenceService geofenceService;
    private List<string> idList = new List<string>();
    private static List<Geofence> geofenceList = new List<Geofence>();
    private string TAG;

    private AndroidPendingIntent _pendingIntent;

    private void ApplyForLocationPermission()
    {
      //  LocationPermissions.ApplyActivityRecognitionPermissions();
    }

    void ApplyForFineLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    void Start()
    {
        ApplyForLocationPermission();
        // ApplyForFineLocationPermission();
        // ApplyForCoarseLocationPermission();
       // LocationPermissions.ApplyBackgroundLocationPermissions();
        LocationPermissions.RequestLocationPermission();
    }

    private void ApplyForCoarseLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
    }

    public void startthings()
    {
        geofenceService = LocationServices.GetGeofenceService();
        _pendingIntent = GeoFenceBroadcastReceiver.GetPendingIntent();

        Debug.Log("Enes1 geofence servise" + geofenceService.ToString());
        // Obtain a PendingIntent object.
        Debug.Log("Enes1 geofence servise1");

        Debug.Log("Enes1 geofence servise2");


        Debug.Log("Enes1 geofence servise3");

        var geofenceRequest = GetAddGeofenceRequest();

        Debug.Log("Enes1 geofence servise4 GELMYOR");
        if (geofenceRequest == null)
        {
            Debug.Log("Enes1 ActivityIdentificationService is null");
        }
        else
        {
            Debug.Log("Enes1 geofence GetCoordinateType" + geofenceRequest.GetCoordinateType);
            Debug.Log("Enes1 geofence GetInitConversions" + geofenceRequest.GetInitConversions);
            Debug.Log("Enes1 ActivityIdentificationService not  null");
        }


        if (_pendingIntent == null)
        {
            Debug.Log("Enes _pendingIntent is null");
        }
        else
        {
            Debug.Log("Enes _pendingIntent is not null");
        }

        Debug.Log("Enes1 geofence GetPendingIntent");


        CreateGeofenceListRequst(geofenceRequest);
        Debug.Log("Enes1 geofence servise4");
    }

    public void GetGeofenceData()
    {
        Debug.Log("Enes1 geofence servise5");

        var geofenceData = GeoFenceBroadcastReceiver.GetGeofenceData();
        Debug.Log("Enes1 geofence servise6");

        if (geofenceData != null)
        {
            Debug.Log("Enes1 GeofenceData GetConversion: " + geofenceData.GetConversion);
            Debug.Log("Enes1 GeofenceData GetErrorCode: " + geofenceData.GetErrorCode);
            Debug.Log("Enes1 GeofenceData GetConvertingLocation: " + geofenceData.GetConvertingLocation());
            Debug.Log("Enes1 GeofenceData IsFailure: " + geofenceData.IsFailure);
            Debug.Log("Enes1 GeofenceData GetConversion: " + geofenceData.IsSuccess);
            var Enes = geofenceData.GetConvertingGeofenceList();
            if (Enes != null)
            {
                foreach (var geofence in Enes)
                {
                    Debug.Log("Enes1 GeofenceData bu: " + geofence);
                }
            }
            else
            {
                Debug.Log("Enes1 GeofenceData bu: null");
            }
        }
        else
        {
            Debug.Log("Enes GeofenceData nulll: ");
        }
    }

    private void CreateGeofenceListRequst(GeofenceRequest geofenceRequest)
    {
        geofenceService.CreateGeofenceList(geofenceRequest, _pendingIntent)
            .AddOnSuccessListener((type) => { Debug.Log("Enes1 [Geofence Demo]: Complete " + type.ToString()); })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Geofence Demo]: Fail123" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 Error code: " + exception.ErrorCode);
            });
        }

    private void RemoveGeofenceListRequst()
    {
        geofenceService.DeleteGeofenceList(_pendingIntent)
            .AddOnSuccessListener((type) => { Debug.Log("Enes1 [Geofence Demo]: Complete " + type.ToString()); })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes1 [Geofence Demo]: Fail123" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes1 Error code: " + exception.ErrorCode);
            });
    }

    public void CreateGeofenceInstance()
    {
        Debug.Log("Enes1 CreateGeofenceInstance enter: ");
        var asd = new Geofence.Builder()
            .SetUniqueId("mGeofence")
            .SetValidContinueTime(10000)
            // Pass the longitude and latitude of the geofence center, and radius of the circle geofence (unit: meters).
            .SetRoundArea(-6.914744f, 107.609810f, 100f)
            // Trigger a callback when the user enters or leaves the geofence.
            .SetConversions(Geofence.ENTER_GEOFENCE_CONVERSION | Geofence.EXIT_GEOFENCE_CONVERSION)
            .Build();
        Debug.Log("Enes1 CreateGeofenceInstance enter2: ");

        geofenceList.Add(asd);
        Debug.Log("Enes1 CreateGeofenceInstance enter3: + geofenceList.Count: " + geofenceList.Count);
        Debug.Log("Enes1 CreateGeofenceInstance enter3: + geofenceList.Count: " + geofenceList[0].GetUniqueId());

        // geofenceList.Add(new Geofence.Builder()
        //     .SetUniqueId("mGeofence")
        //     .SetValidContinueTime(10000)
        //     // Pass the longitude and latitude of the geofence center, and radius of the circle geofence (unit: meters).
        //     .SetRoundArea(-6.914744f, 107.609810f, 100f)
        //     // Trigger a callback when the user enters or leaves the geofence.
        //     .SetConversions(Geofence.ENTER_GEOFENCE_CONVERSION | Geofence.EXIT_GEOFENCE_CONVERSION)
        //     .Build());
        idList.Add("mGeofence");
        foreach (var geofence in geofenceList)
        {
            Debug.Log("Enes1 geofence: " + geofence.ToString());
        }
    }


    private GeofenceRequest GetAddGeofenceRequest()
    {
        Debug.Log("Enes1 GetAddGeofenceRequest servise1");

        GeofenceRequest.Builder builder = new GeofenceRequest.Builder();
        Debug.Log("Enes1 GetAddGeofenceRequest servise2");

        // Trigger a callback immediately after the geofence is added, if the user is already within the geofence.
        builder.SetInitConversions(GeofenceRequest.ENTER_INIT_CONVERSION);
        Debug.Log("Enes1 CreateGeofenceInstance enter3: + geofenceList.Count: " + geofenceList.Count);

        Debug.Log("Enes1 GetAddGeofenceRequest servise3");
        builder.CreateGeofenceList(geofenceList);
        if (geofenceList.Count <= 0)
        {
            Debug.Log("Enes1 CreateGeofenceInstance enter3: + geofenceList.Count: 000000000000000000000");
        }

        Debug.Log("Enes1 CreateGeofenceInstance enter3: + geofenceList.Count: " + geofenceList.Count);
        Debug.Log("Enes1 CreateGeofenceInstance enter3: + geofenceList.Count: " + geofenceList.Capacity);

        return builder.Build();
    }


    // Update is called once per frame
    void Update()
    {
    }
}