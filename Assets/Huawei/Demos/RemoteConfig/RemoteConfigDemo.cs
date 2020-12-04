using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RemoteConfigDemo : MonoBehaviour
{
    private Text countOfVariables;
    private RemoteConfigManager remoteConfigManager;
    string TAG = "RemoteConfig Demo";

    void Start()
    {
        countOfVariables = GameObject.Find("countOfVariables").GetComponent<Text>();
        remoteConfigManager = new RemoteConfigManager();
        remoteConfigManager.GetInstance();
    }

    public void Fetch()
    {
        remoteConfigManager.Fetch();
    }

    public void GetMergedAll()
    {
        Dictionary<string, object> map = remoteConfigManager.GetMergedAll();
        foreach (var el in map)
        {
            Debug.Log($"[{TAG}]: {el.Key} = {el.Value}");
        }
        countOfVariables.text = $"Count of Variables : {map.Count}";
    }

    public void ClearAll()
    {
        remoteConfigManager.ClearAll();
        GetMergedAll();
    }

    public void ApplyDefault()
    {
        Dictionary<string, object> map = new Dictionary<string, object>();
        map.Add("Key", "Value");
        map.Add("Key1", "Value1");
        remoteConfigManager.ApplyDefault(map);
        GetMergedAll();
    }

    public void ApplyDefaultXml()
    {
        remoteConfigManager.ApplyDefault("xml/remoteConfig");
        GetMergedAll();
    }

    public void LoadLastFetched()
    {
        Debug.Log($"[{TAG}]: LoadLastFetched {remoteConfigManager.LoadLastFetched().getValueAsString("Key")}");
    }

    public void DeveloperMode(bool val)
    {
        remoteConfigManager.SetDeveloperMode(val);
    }

    public void GetSource()
    {
        Debug.Log($"[{TAG}]: GetSource(Key) {remoteConfigManager.GetSource("Key")}");
    }
}
