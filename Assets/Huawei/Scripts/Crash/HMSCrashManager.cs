using HuaweiMobileServices.Crash;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace HmsPlugin
{
    public class HMSCrashManager : HMSManagerSingleton<HMSCrashManager>
    {
        IAGConnectCrash agConnectCrash;

        private readonly string TAG = "[HMS] HMSCrashManager ";

        public HMSCrashManager()
        {
            HMSManagerStart.Start(OnAwake, TAG);
        }

        private void OnAwake()
        {
            Debug.Log($"{TAG}: Crash OnAwake - Initialized");
            agConnectCrash = AGConnectCrash.GetInstance();
        }

        //Crash Collection enable/disable method used on AnalyticsDemo scene with enable/disable radio button configuration
        public void EnableCrashCollection(bool value)
        {
            agConnectCrash.EnableCrashCollection(value);
            Debug.Log($"{TAG}enableCrashCollection {value}");
        }

        public void TestCrash()
        {
            Debug.Log($"{TAG}TestCrash");
            Utils.ForceCrash(0);
        }

        enum Log
        {
            DEBUG = 3,
            INFO = 4,
            WARN = 5,
            ERROR = 6,
        }

        public void CustomReport()
        {
            agConnectCrash.SetUserId("testuser");
            agConnectCrash.Log((int)Log.DEBUG, "set debug log.");
            agConnectCrash.Log((int)Log.INFO, "set info log.");
            agConnectCrash.Log((int)Log.WARN, "set warning log.");
            agConnectCrash.Log((int)Log.ERROR, "set error log.");
            agConnectCrash.SetCustomKey("stringKey", "Hello world");
            agConnectCrash.SetCustomKey("booleanKey", false);
            agConnectCrash.SetCustomKey("doubleKey", 1.1);
            agConnectCrash.SetCustomKey("floatKey", 1.1f);
            agConnectCrash.SetCustomKey("intKey", 0);
            agConnectCrash.SetCustomKey("longKey", 11L);
            Debug.Log($"{TAG}CustomReport");
        }
    }
}
