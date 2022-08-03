using HuaweiMobileServices.Location;
using HuaweiMobileServices.Location.Activity;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using Huawei.Scripts.Location;

namespace Huawei.Demos.Location
{
    public class ActivityIdentificationDemo : MonoBehaviour
    {
        private static string TAG = "ActivityIdentificationDemo";
        private ActivityIdentificationService _activityIdentificationService;
        private ActivityConversionRequest _request;
        private AndroidPendingIntent _pendingIntent;
        [SerializeField] private Text activityIdentificationDataText;

        private void Start()
        {
            HMSLocationManager.Instance.RequestActivityRecognitionPermissions();
            LocationReceiver.Instance.onReceive += OnReceive;

            InitClient();
        }

        private void InitClient()
        {
            _activityIdentificationService = ActivityIdentification.GetService();
            _pendingIntent = HMSLocationManager.Instance.GetPendingIntentFromLocation();
            LocationReceiver.Instance.SetLocationBroadcastListener();
            SetParameterForActivityConversion();
        }

        private void OnReceive(AndroidIntent intent)
        {
            Debug.Log($"{TAG} OnReceive success");

            var isIntentUsed = false;
            StringBuilder stringBuilder = new StringBuilder();
            if (LocationReceiver.isListenActivityIdentification)
            {
                Debug.Log($"{TAG} ActivityIdentification OnReceive success");
                isIntentUsed = true;
                
                var activityIdentificationResponse = ActivityIdentificationResponse.GetDataFromIntent(intent);

                if (activityIdentificationResponse != null)
                {
                    foreach (var activityIdentificationData in
                             activityIdentificationResponse.GetActivityIdentificationDatas())
                    {
                        stringBuilder
                            .Append("\nIdentificationActivity = " +
                                    GetActivityName(activityIdentificationData.GetIdentificationActivity()))
                            .Append(" Possibility " + activityIdentificationData.GetPossibility());

                        activityIdentificationDataText.text += stringBuilder.ToString();
                    }
                }
                else
                {
                    Debug.LogError($"{TAG} activityIdentificationResponse is null");
                }
            }

            if (LocationReceiver.isListenActivityConversion)
            {
                Debug.Log($"{TAG} ActivityConversionResponse OnReceive success");
                isIntentUsed = true;

                ActivityConversionResponse activityConversionResponse =
                    ActivityConversionResponse.GetDataFromIntent(intent);
                if (activityConversionResponse != null)
                {
                    var conversionDataList = activityConversionResponse.GetActivityConversionDatas();

                    activityIdentificationDataText.text = "";
                    foreach (var activityConversionData in conversionDataList)
                    {
                        stringBuilder
                            .Append("\nActivityType = " +
                                    activityConversionData.GetActivityType)
                            .Append(" Conversion Type " + activityConversionData.GetConversionType)
                            .Append(" Get Time From Reboot " + activityConversionData.GetElapsedTimeFromReboot);
                        activityIdentificationDataText.text += stringBuilder.ToString();
                    }
                }
                else
                {
                    Debug.LogError($"{TAG} activityConversionResponse is null");
                }
            }

            if (!isIntentUsed) Debug.LogError($"{TAG} OnReceive called but not used");
        }

        #region ActivityConversion

        private void SetParameterForActivityConversion()
        {
            ActivityConversionInfo activityConversionInfoStillEnter = new ActivityConversionInfo(
                ActivityIdentificationData.STILL, ActivityConversionInfo.ENTER_ACTIVITY_CONVERSION);

            // Create an information object for switching from the activity state to the static state. 
            ActivityConversionInfo activityConversionInfoStillExit = new ActivityConversionInfo(
                ActivityIdentificationData.FOOT, ActivityConversionInfo.EXIT_ACTIVITY_CONVERSION);
            List<ActivityConversionInfo> activityConversionInfos = new List<ActivityConversionInfo>();
            activityConversionInfos.Add(activityConversionInfoStillEnter);
            activityConversionInfos.Add(activityConversionInfoStillExit);

            // Create an activity conversion request body instance.
            _request = new ActivityConversionRequest();
            _request.SetActivityConversions(activityConversionInfos);

        }

        public void CreateActivityConversionUpdates()
        {
            LocationReceiver.AddConversionListener();

            Debug.Log($"{TAG} CreateActivityConversionUpdates clicked");
            _activityIdentificationService.CreateActivityConversionUpdates(_request, _pendingIntent)
                .AddOnSuccessListener(type => { Debug.Log($"{TAG} CreateActivityConversionUpdates Successful"); })
                .AddOnFailureListener(exception =>
                {
                    Debug.LogError(
                        $"{TAG} CreateActivityConversionUpdates Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
                });
        }

        public void DeleteActivityConversionUpdates()
        {
            LocationReceiver.RemoveConversionListener();

            Debug.Log($"{TAG} DeleteActivityConversionUpdates clicked");
            _activityIdentificationService.DeleteActivityConversionUpdates(_pendingIntent)
                .AddOnSuccessListener(type => { Debug.Log($"{TAG} DeleteActivityConversionUpdates Successful"); })
                .AddOnFailureListener(exception =>
                {
                    Debug.LogError(
                        $"{TAG} DeleteActivityConversionUpdates Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
                });
        }

        #endregion

        #region ActivityIdentification

        private string GetActivityName(int activity)
        {
            switch (activity)
            {
                case 100:
                    return "VEHICLE";
                case 101:
                    return "BIKE";
                case 102:
                    return "FOOT";
                case 103:
                    return "STILL";
                case 104:
                    return "OTHERS";
                case 107:
                    return "WALKING";
                case 108:
                    return "RUNNING";
                default:
                    return "UNKNOWN";
            }
        }

        public void RequestActivityIdentificationUpdates()
        {
            LocationReceiver.AddIdentificationListener();

            _activityIdentificationService.CreateActivityIdentificationUpdates(5000, _pendingIntent)
                .AddOnSuccessListener(type => { Debug.Log($"{TAG} CreateActivityIdentificationUpdates Successful"); })
                .AddOnFailureListener(exception =>
                {
                    Debug.LogError(
                        $"{TAG} CreateActivityIdentificationUpdates Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
                });
        }

        public void DeleteActivityIdentificationUpdates()
        {
            LocationReceiver.RemoveIdentificationListener();

            _activityIdentificationService.DeleteActivityIdentificationUpdates(_pendingIntent)
                .AddOnSuccessListener(type => { Debug.Log($"{TAG} DeleteActivityIdentificationUpdates Successful"); })
                .AddOnFailureListener(exception =>
                {
                    Debug.LogError(
                        $"{TAG} ) Exception {exception.WrappedCauseMessage} with error code: {exception.ErrorCode}");
                });
        }

        #endregion

        private void OnDestroy()
        {
            DeleteActivityIdentificationUpdates();
            DeleteActivityConversionUpdates();
        }
    }
}