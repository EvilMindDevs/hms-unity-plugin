using System;
using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Activity;
using HuaweiMobileServices.Utils;
using UnityEngine;

namespace Huawei.Demos.Location
{
    public class ActivityIdentificationDemo : MonoBehaviour
    {
        private static String TAG = "ActivityIdentificationDemo";
        private ActivityIdentificationService _activityIdentificationService;
        private AndroidPendingIntent _pendingIntent;
        private AndroidIntent _androidIntent;

        private void Start()
        {
            HMSLocationManager.Instance.RequestActivityRecognitionPermissions();
            HMSLocationManager.Instance.onReceive += OnReceive;

            InitClient();
        }

        private void InitClient()
        {
            _activityIdentificationService = ActivityIdentification.GetService();
            _pendingIntent = HMSLocationManager.Instance.GetPendingIntent();
        }

        private void OnReceive(AndroidIntent intent)
        {
            Debug.Log($"{TAG} ActivityIdentification OnReceive success");
            _androidIntent = intent;
            var activityIdentificationResponse = ActivityConversionResponse.GetDataFromIntent(intent);
            var activityIdentificationResponse2 = ActivityIdentificationResponse.GetDataFromIntent(intent);
            foreach (var activityConversionData in activityIdentificationResponse.GetActivityConversionDatas())
            {
                Debug.Log(
                    $"{TAG} ActivityIdentification OnReceive activityConversionData: {activityConversionData.ToString()}");

                foreach (var activityConversionData2 in activityIdentificationResponse2.GetActivityConversionDatas())
                {
                    Debug.Log(
                        $"{TAG} ActivityIdentification OnReceive activityConversionData: {activityConversionData2.ToString()}");
                }
            }
        }

        public void RequestActivityIdentificationUpdates()
        {
            HMSLocationManager.Instance.SetLocationBroadcastListener();

            _activityIdentificationService.CreateActivityIdentificationUpdates(5000, _pendingIntent)
                .AddOnSuccessListener(type => { Debug.Log($"{TAG} CreateActivityIdentificationUpdates Successful"); })
                .AddOnFailureListener(exception =>
                {
                    Debug.LogError(
                        $"{TAG} CreateActivityIdentificationUpdates Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
                });
        }
    }
}