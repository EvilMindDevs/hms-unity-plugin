using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDemoManager : MonoBehaviour
{
    [SerializeField] private GameObject fusedDemoPanel;
    [SerializeField] private GameObject activityDemoPanel;
    [SerializeField] private GameObject geofenceDemoPanel;
    [SerializeField] private GameObject geocodingDemoPanel;
    [SerializeField] private GameObject demoMenuPanel;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        DisplayDemoMenu();
    }

    public void DisplayDemoMenu()
    {
        fusedDemoPanel.SetActive(false);
        activityDemoPanel.SetActive(false);
        geofenceDemoPanel.SetActive(false);
        geocodingDemoPanel.SetActive(false);
        demoMenuPanel.SetActive(true);
    }

    public void DisplayFusedDemo()
    {
        fusedDemoPanel.SetActive(true);
        demoMenuPanel.SetActive(false);
    }

    public void DisplayActivityDemo()
    {
        activityDemoPanel.SetActive(true);
        demoMenuPanel.SetActive(false);
    }

    public void DisplayGeofenceDemo()
    {
        geofenceDemoPanel.SetActive(true);
        demoMenuPanel.SetActive(false);
    }

    public void DisplayGeocodingDemo()
    {
        geocodingDemoPanel.SetActive(true);
        demoMenuPanel.SetActive(false);
    }
}