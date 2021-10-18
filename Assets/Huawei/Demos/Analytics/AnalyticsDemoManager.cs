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
        [SerializeField]
        private InputField eventIdField;

        [SerializeField]
        private InputField keyField;

        [SerializeField]
        private InputField valueField;

        public void SendEvent()
        {
            if (string.IsNullOrEmpty(eventIdField.text) && string.IsNullOrEmpty(keyField.text) && string.IsNullOrEmpty(valueField.text))
            {
                Debug.Log("[HMS]: Fill Fields");
            }
            else
            {
                HMSAnalyticsManager.Instance.SendEventWithBundle(eventIdField.text, keyField.text, valueField.text);
            }
        }
    }
}

