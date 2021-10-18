﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Analystics;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;
using System.Net.Mail;

namespace HmsPlugin
{
<<<<<<< HEAD
    public class AnalyticsDemoManager: MonoBehaviour
    {
        private AnalyticsManager analyticsManager;
        InputField eventID, key, value;
        void InitilizeAnalyticsInstane()
        {
            eventID = GameObject.Find("EventId").GetComponent<InputField>();
            key = GameObject.Find("Param1").GetComponent<InputField>();
            value = GameObject.Find("Param2").GetComponent<InputField>();

        }
        public void SendEvent()
        {
            SendEvent(eventID.text, key.text, value.text);
        }
        void SendEvent(string eventID, string key, string value)
        {
            if(string.IsNullOrEmpty(eventID) && string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                Debug.Log("[HMS]: Fill Fields"); 
            }
            else
            {
                analyticsManager.SendEventWithBundle(eventID, key, value);
            }
        }
 
        // Start is called before the first frame update
        void Start()
        {
            InitilizeAnalyticsInstane();
            analyticsManager = AnalyticsManager.GetInstance();
        }

        // Update is called once per frame
        void Update()
        {

        }
=======
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
>>>>>>> EvilMindDevs-master
    }
}

