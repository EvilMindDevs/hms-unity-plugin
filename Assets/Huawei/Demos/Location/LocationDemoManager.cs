using HuaweiMobileServices.Location;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HuaweiMobileServices.Location.Activity;
using UnityEngine.Android;

public class LocationDemoManager : MonoBehaviour
{
    private ActivityIdentificationService _activityIdentificationService;
    private AndroidPendingIntent _pendingIntent;
    [SerializeField] private Text statusText;
    
    

    public void RequestActivityUpdates()
    {
        _activityIdentificationService = ActivityIdentification.GetService();

        if (_activityIdentificationService == null)
        {
            Debug.Log("Enes ActivityIdentificationService is null");
        }
        else
        {
            Debug.Log("Enes ActivityIdentificationService not  null");

        }

        _pendingIntent = LocationBroadcastReceiver.GetPendingIntent();
        if (_pendingIntent == null)
        {
            Debug.Log("Enes _pendingIntent is null");
        }
        else
        {
            Debug.Log("Enes _pendingIntent is not null");

        }

        Debug.Log("Enes LocationDemoManager1");

        _activityIdentificationService.CreateActivityIdentificationUpdates(5000, _pendingIntent)
            .AddOnSuccessListener((type) => { Debug.Log("Enes [Location Demo]: Complete " + type.ToString()); })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes [Location Demo]: Fail123" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes Error code: " + exception.ErrorCode);
            });
        Debug.Log("Enes LocationDemoManager2");
        
        // Create an information object for switching from the static state to the activity state.
        ActivityConversionInfo activityConversionInfoStillEnter = new ActivityConversionInfo(
            ActivityIdentificationData.STILL, ActivityConversionInfo.ENTER_ACTIVITY_CONVERSION);
        // Create an information object for switching from the activity state to the static state. 
        ActivityConversionInfo activityConversionInfoStillExit = new ActivityConversionInfo(
            ActivityIdentificationData.STILL, ActivityConversionInfo.EXIT_ACTIVITY_CONVERSION);
        
        ActivityConversionRequest request = new ActivityConversionRequest();
        Debug.Log("Enes LocationDemoManager3.6");
        
        var list = new ActivityConversionInfo[] { activityConversionInfoStillEnter, activityConversionInfoStillExit };
        request.SetActivityConversions(list);
        _activityIdentificationService.CreateActivityConversionUpdates(request, _pendingIntent)
            .AddOnSuccessListener((type) => { Debug.Log("Enes [Location Demo]: request complete " + type.ToString()); })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes [Location Demo]: Fail466" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes Error code: " + exception.ErrorCode);
            });
        Debug.Log("Enes LocationDemoManager3");
    }

    public void GetActivityUpdates()
    {      
        
        var activityIdentificationResponse = LocationBroadcastReceiver.GetActivityIdentificationResponse();
        Debug.Log("Enes [Location Demo]:  activityIdentificationResponse " + activityIdentificationResponse.ToString());

        Debug.Log("Enes [Location Demo]:  GetActivityUpdates " );
        
        if (activityIdentificationResponse != null)
        {
            activityIdentificationResponse.SetTime(123132);
            Debug.Log("123 Enes [Location Demo]:  SetTime " );

            activityIdentificationResponse.SetElapsedTimeFromReboot(123132);
            Debug.Log("123 Enes [Location Demo]:  SetElapsedTimeFromReboot " );
            activityIdentificationResponse.GetTime();
            Debug.Log("123 Enes [Location Demo]: activityIdentificationResponse " +activityIdentificationResponse.GetTime());
            activityIdentificationResponse.GetActivityPossibility(ActivityIdentificationData.BIKE);
            Debug.Log("123 Enes [Location Demo]: activityIdentificationResponse " +  activityIdentificationResponse.GetActivityPossibility(ActivityIdentificationData.BIKE));
            activityIdentificationResponse.GetElapsedTimeFromReboot();
            Debug.Log("123 Enes [Location Demo]: activityIdentificationResponse " + activityIdentificationResponse.GetElapsedTimeFromReboot());
            activityIdentificationResponse.GetMostActivityIdentification();
            Debug.Log("123 Enes [Location Demo]: activityIdentificationResponse " + activityIdentificationResponse.GetMostActivityIdentification());

            activityIdentificationResponse.GetActivityIdentificationDatas();
            Debug.Log("123 Enes [Location Demo]: GetActivityConversionDatas " + activityIdentificationResponse.GetActivityIdentificationDatas());

            foreach (var item in activityIdentificationResponse.GetActivityIdentificationDatas())
            {
                Debug.Log("123 Enes [Location Demo]: GetActivityConversionDatas item" + item.GetPossibility());
                Debug.Log("123 Enes [Location Demo]: GetActivityConversionDatas item" + item.GetIdentificationActivity());
                item.SetPossibility(2);
                Debug.Log("123 Enes [Location Demo]: GetActivityConversionDatas item yeni" + item.GetPossibility());
            }
        }

        var activityConversionResponse = LocationBroadcastReceiver.GetActivityConversionResponse();
        
        Debug.Log("Enes [Location Demo]:  GetActivityUpdates2 " );

        Debug.Log("Enes [Location Demo]:  activityConversionResponse " + activityConversionResponse.ToString());

        if (activityConversionResponse != null)
        {
            activityConversionResponse.GetActivityConversionDatas();
            Debug.Log("Enes [Location Demo]: activityConversionResponse " + activityConversionResponse.ToString());
        }
   
        else
        {
            Debug.Log("Enes [Location Demo] 123 activityIdentificationResponse is null again");
        }
        
        
        Debug.Log("Enes [Location Demo]: IsValidType " +
                  ActivityIdentificationData.IsValidType(ActivityIdentificationData.BIKE));

        var list = LocationBroadcastReceiver.GetActivityList();
        foreach (var item in list)
        {
            Debug.Log("Yeni Enes [Location Demo]: GetPossibility " + item.GetPossibility());
            Debug.Log("Yeni Enes [Location Demo]: GetIdentificationActivity " + item.GetIdentificationActivity());
            Debug.Log("Yeni Enes [Location Demo]: GetActivityList item " + item.ToString());
            statusText.text += item.ToString() + "\n";
            item.SetPossibility(61);
            item.SetIdentificationActivity(ActivityIdentificationData.BIKE);
            Debug.Log("Yeni Enes [Location Demo]: SetPossibility");
            Debug.Log("Yeni Enes [Location Demo]: GetIdentificationActivity " + item.GetIdentificationActivity());
            Debug.Log("Yeni Enes [Location Demo]: GetPossibility " + item.GetPossibility());
            
        }

        var conversionDataList = LocationBroadcastReceiver.GetConversionDataList();
        if (conversionDataList != null)
        {
            foreach (var conversionData in conversionDataList)
            {
                Debug.Log("Enes [Location Demo]: GetConversionType " + conversionData.GetConversionType);
                Debug.Log("Enes [Location Demo]: GetActivityType " + conversionData.GetActivityType);
                Debug.Log("Enes [Location Demo]: GetElapsedTimeFromReboot " + conversionData.GetElapsedTimeFromReboot);
                Debug.Log(
                    "Enes [Location Demo]: GetConversionDataList conversionData item " + conversionData.ToString());
                statusText.text += conversionData.ToString() + "\n";
            }
        }
        else
        {
            Debug.Log("Enes [Location Demo]: GetConversionDataList is null");
        }
        ActivityConversionInfo activityConversionInfoStillEnter = new ActivityConversionInfo(
            ActivityIdentificationData.STILL, ActivityConversionInfo.ENTER_ACTIVITY_CONVERSION);
        // Create an information object for switching from the activity state to the static state. 
        ActivityConversionInfo activityConversionInfoStillExit = new ActivityConversionInfo(
            ActivityIdentificationData.STILL, ActivityConversionInfo.EXIT_ACTIVITY_CONVERSION);

        Debug.Log("Enes LocationDemoManager3 yeni deneme");
        Debug.Log("Enes LocationDemoManager3 yeni deneme" + activityConversionInfoStillEnter.GetActivityType + " " +
                  activityConversionInfoStillEnter.GetConversionType + " " +
                  activityConversionInfoStillEnter.ToString());
        
        ActivityConversionInfo.Builder activityTransition = new ActivityConversionInfo.Builder();
        Debug.Log("Enes LocationDemoManager345645665yeni deneme"+ activityTransition.ToString());

        activityTransition.SetActivityType(ActivityIdentificationData.WALKING);
        Debug.Log("Enes LocationDemoManager345645665yeni deneme"+ activityTransition.ToString());

        activityTransition.SetConversionType(ActivityConversionInfo.ENTER_ACTIVITY_CONVERSION);
        Debug.Log("Enes LocationDemoManager345645665yeni deneme"+ activityTransition.ToString());

        activityTransition.Build();
        Debug.Log("Enes LocationDemoManager345645665yeni deneme"+ activityTransition.ToString());
        
        
        activityConversionInfoStillEnter.SetActivityType(ActivityIdentificationData.RUNNING);
        activityConversionInfoStillEnter.SetConversionType(ActivityConversionInfo.EXIT_ACTIVITY_CONVERSION);
        
        Debug.Log("Enes LocationDemoManager3 yeni deneme" + activityConversionInfoStillEnter.GetActivityType + " " +
                  activityConversionInfoStillEnter.GetConversionType + " " +
                  activityConversionInfoStillEnter.ToString());
        
        Debug.Log("Enes LocationDemoManager3 yeni deneme" + activityConversionInfoStillExit.GetActivityType + " " +
                  activityConversionInfoStillExit.GetConversionType + " " + activityConversionInfoStillExit.ToString());

        List<ActivityConversionInfo> activityConversionInfos = new List<ActivityConversionInfo>();
        activityConversionInfos.Add(activityConversionInfoStillEnter);
        activityConversionInfos.Add(activityConversionInfoStillExit);
        // Create an activity conversion request body instance.
        Debug.Log("Enes LocationDemoManager3.5");

        
        Debug.Log("Enes LocationDemoManager4");


        Debug.Log("Enes LocationDemoManager5");
    }

    public void ShopActivityUpdates()
    {
        _activityIdentificationService.DeleteActivityIdentificationUpdates( _pendingIntent)
            .AddOnSuccessListener((type) => { Debug.Log("Enes [Location Demo]: DeleteActivityIdentificationUpdates request complete " + type.ToString()); })
            .AddOnFailureListener((exception) =>
            {
                Debug.Log("Enes [Location Demo]: DeleteActivityIdentificationUpdates Fail" + exception.WrappedCauseMessage + " " +
                          exception.WrappedExceptionMessage + "Enes Error code: " + exception.ErrorCode);
            });
        
        
        
    }
}