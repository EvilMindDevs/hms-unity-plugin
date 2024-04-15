using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSExceptionHandler : HMSManagerSingleton<HMSExceptionHandler>
    {
        private const string FAQ_BASE_URL = "https://evilminddevs.gitbook.io/hms-unity-plugin_";
        private const string TAG = "[HMSException]";
        private readonly Dictionary<string, string> _exceptionToFaqURL = new Dictionary<string, string>();
        private enum ExceptionName
        {
            QuickStartUrl,
            Faq6003Error,
            Faq6004Error,
            FaqAds0Error,
            FaqAds2Error,
            FaqAds3Error,
            Faq7018Error
        }
        private static class ExceptionLog
        {
            public static Dictionary<ExceptionName, Uri> Urls = new Dictionary<ExceptionName, Uri>
            {
                { ExceptionName.QuickStartUrl , GetUrl("/getting-started/quick-start") },
                { ExceptionName.Faq6003Error, GetUrl("/support/faq#what-should-i-do-if-a-6003-error-occurs") },
                { ExceptionName.Faq6004Error, GetUrl("/support/faq#why-is-result-code-6004-returned") },
                { ExceptionName.FaqAds0Error, GetUrl("/support/faq#why-i-am-getting-0-inner-error-while-using-a-ds-kit") },
                { ExceptionName.FaqAds2Error, GetUrl("/support/faq#why-i-am-getting-2-network_error-while-using-a-ds-kit") },
                { ExceptionName.FaqAds3Error, GetUrl("/support/faq#why-i-am-getting-3-no_ad-error-while-using-a-ds-kit") },
                { ExceptionName.Faq7018Error, GetUrl("/support/faq#why-i-am-getting-the-7018-game_state_not_init-error") }
            };

            private static Uri GetUrl(string path) => new Uri($"{FAQ_BASE_URL}{path}");
        }
        public HMSExceptionHandler()
        {
            RegisterExceptions();
            Application.logMessageReceivedThreaded += HandleLog;
        }
        ~HMSExceptionHandler()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!(type == LogType.Exception || type == LogType.Error)) return;

            var exceptionEntry = _exceptionToFaqURL.FirstOrDefault(entry =>
            {
                var comparisonType = StringComparison.InvariantCultureIgnoreCase;
                return logString.IndexOf(entry.Key, comparisonType) >= 0 ||
                       stackTrace.IndexOf(entry.Key, comparisonType) >= 0;
            });

            if (exceptionEntry.Key == null) return;

            Debug.LogError($"{TAG}: FAQ => {exceptionEntry.Value}");
        }
        public void HandleLogForListener(string logString, string stackTrace, LogType type)
        {
            HandleLog(logString, stackTrace, type);
            Application.logMessageReceivedThreaded -= HandleLog;
            Debug.LogError($"{logString} {stackTrace}");
            Application.logMessageReceivedThreaded += HandleLog;
        }

        #region Register Exceptions
        private void RegisterExceptions()
        {
            RegisterCommonExceptions();
            RegisterAccountExceptions();
            RegisterAdsExceptions();
            RegisterGameServiceExceptions();
        }
        private void RegisterCommonExceptions()
        {
            AddException("UnityEngine._AndroidJNIHelper.GetSignature",
                         CreateMessage("This below error is caused by that you try to run plugin on Editor. Please you switch to android platform and try on your android device again. Please check {0} for more information.", ExceptionName.QuickStartUrl));

            AddException("6004", CreateMessage("Did you enable any service? Please check {0} for more information.", ExceptionName.Faq6004Error));
        }
        private void RegisterAccountExceptions()
        {
            AddException("6003", CreateMessage("The below 6003 error is caused by inconsistent certificate fingerprint configurations. Please check {0} for more information.", ExceptionName.Faq6003Error));
        }
        private void RegisterAdsExceptions()
        {
            string adsError2 = "This below error is caused Are you testing with no HMS Core installed device(non-Huawei)? Please check {0} for more information.";
            AddException("reason:2", CreateMessage(adsError2, ExceptionName.FaqAds2Error));
            AddException("error $2", CreateMessage(adsError2, ExceptionName.FaqAds2Error));
            AddException("Error Code: 2", CreateMessage(adsError2, ExceptionName.FaqAds2Error));

            AddException("SplashAdLoadFailed. Error Code: 3", CreateMessage("This below error is caused The ad request is sent successfully, but the server returns a response indicating no available ad assets. Please check {0} for more information.", ExceptionName.FaqAds3Error));

            AddException("Error Code: 0", CreateMessage("Why I am getting 0 - INNER error while using Ads kit?. Please check {0} for more information.", ExceptionName.FaqAds0Error));

        }
        private void RegisterGameServiceExceptions()
        {
            AddException("7018", CreateMessage("Why I am getting the 7018 GAME_STATE_NOT_INIT error? Please check {0} for more information.", ExceptionName.Faq7018Error));
        }

        // Register other exceptions in a similar way...
        #endregion

        #region Helper Methods
        private void AddException(string key, string message)
        {
            _exceptionToFaqURL.Add(key, message);
        }
        private string CreateMessage(string template, ExceptionName link)
        {
            return string.Format(template, ExceptionLog.Urls[link]);
        }
        #endregion
    }
}
