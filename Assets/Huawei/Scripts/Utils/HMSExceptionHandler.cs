using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSExceptionHandler : HMSManagerSingleton<HMSExceptionHandler>
    {
        public bool LogApiEnabled {get; set;} = true;
        private const string API_LOG_URL = "YOUR_API_SEND_LOG_URL";
        private const string TAG = "[HMSException]";
        private readonly Dictionary<string, string> _exceptionToFaqURL = new Dictionary<string, string>();

        private static class ExceptionLog
        {
            public const string QUICK_START_URL = "https://evilminddevs.gitbook.io/hms-unity-plugin_/getting-started/quick-start";
            public const string FAQ_6003_ERROR = "https://evilminddevs.gitbook.io/hms-unity-plugin_/support/faq#what-should-i-do-if-a-6003-error-occurs";
            public const string FAQ_ADS_ERROR_2 = "https://evilminddevs.gitbook.io/hms-unity-plugin_/support/faq#why-i-am-getting-2-network_error-while-using-a-ds-kit";
            public const string FAQ_ADS_ERROR_3 = "https://evilminddevs.gitbook.io/hms-unity-plugin_/support/faq#why-i-am-getting-3-no_ad-error-while-using-a-ds-kit";
        }

        public HMSExceptionHandler()
        {
            RegisterExceptions();
            Application.logMessageReceived += HandleLog;
        }
        ~HMSExceptionHandler()
        {
            Application.logMessageReceived -= HandleLog;
        }
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!(type == LogType.Exception || type == LogType.Error)) return;
            
            var exceptionEntry = _exceptionToFaqURL.FirstOrDefault(entry => 
                                                                logString.Contains(entry.Key, System.StringComparison.InvariantCultureIgnoreCase)||  
                                                                stackTrace.Contains(entry.Key, System.StringComparison.InvariantCultureIgnoreCase));

            if (exceptionEntry.Key == null) return;

            Debug.LogError($"{TAG}: FAQ: {exceptionEntry.Value}");
            if (LogApiEnabled)
            {   
                ///TODO: Send log to your api
            }
        }
        private void RegisterExceptions()
        {
            RegisterCommonExceptions();
            RegisterAccountExceptions();
            RegisterAdsExceptions();
        }

        private void RegisterCommonExceptions()
        {
            string commonError = "This below error is caused by that you try to run plugin on Editor. Please you switch to android platform and try on your android device again. Please check {0} for more information.";
            _exceptionToFaqURL.Add("UnityEngine._AndroidJNIHelper.GetSignature", string.Format(commonError, ExceptionLog.QUICK_START_URL));
            _exceptionToFaqURL.Add("SpecificException2", string.Format(commonError, ExceptionLog.QUICK_START_URL));
        }
        private void RegisterAccountExceptions()
        {
            string accountError = "The below 6003 error is caused by inconsistent certificate fingerprint configurations. Please check {0} for more information.";
            _exceptionToFaqURL.Add("6003", string.Format(accountError, ExceptionLog.FAQ_6003_ERROR));
        }
        private void RegisterAdsExceptions()
        {
            string adsError2 = "This below error is caused Are you testing with no HMS Core installed device(non-Huawei)? Please check {0} for more information.";
            _exceptionToFaqURL.Add("reason:2", string.Format(adsError2, ExceptionLog.FAQ_ADS_ERROR_2));
            _exceptionToFaqURL.Add("error $2", string.Format(adsError2, ExceptionLog.FAQ_ADS_ERROR_2));
            _exceptionToFaqURL.Add("Error Code: 2", string.Format(adsError2, ExceptionLog.FAQ_ADS_ERROR_2));

            string adsError3 = "This below error is caused The ad request is sent successfully, but the server returns a response indicating no available ad assets. Please check {0} for more information.";
            _exceptionToFaqURL.Add("Error Code: 3", string.Format(adsError3, ExceptionLog.FAQ_ADS_ERROR_3));
        }

    }
}