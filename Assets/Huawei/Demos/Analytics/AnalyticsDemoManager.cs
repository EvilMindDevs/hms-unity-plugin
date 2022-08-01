using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Analystics;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;
using System.Net.Mail;

namespace HmsPlugin
{
    public class AnalyticsDemoManager : MonoBehaviour
    {
        private readonly string TAG = "[HMS] AnalyticsDemoManager ";

        #region Singleton

        public static AnalyticsDemoManager Instance { get; private set; }
        private void Singleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion

        private void Awake()
        {
            Singleton();
        }

        public void SendEvent(string eventIdFieldText, string keyFieldtext, string valueFieldText)
        {
            if (string.IsNullOrEmpty(eventIdFieldText) && string.IsNullOrEmpty(keyFieldtext) && string.IsNullOrEmpty(valueFieldText))
            {
                Debug.Log(TAG+": Fill Fields");
            }
            else
            {
                Debug.Log(TAG+eventIdFieldText + " " + keyFieldtext + " " + valueFieldText);
                HMSAnalyticsKitManager.Instance.SendEventWithBundle(eventIdFieldText, keyFieldtext, valueFieldText);
            }
        }

    }
}

