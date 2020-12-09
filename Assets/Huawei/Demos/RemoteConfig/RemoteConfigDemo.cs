using HuaweiMobileServices.RemoteConfig;
using HuaweiMobileServices.Utils;
using System;
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
        remoteConfigManager.OnFecthSuccess = OnFecthSuccess;
        remoteConfigManager.OnFecthFailure = OnFecthFailure;
        remoteConfigManager.Fetch();
    }

    private void OnFecthSuccess(ConfigValues config)
    {
        remoteConfigManager.Apply(config);
        Debug.Log($"[{TAG}]: fetch() Success");
    }

    private void OnFecthFailure(HMSException exception)
    {
        Debug.Log($"[{TAG}]: fetch() Failed Error Code => {exception.ErrorCode} Message => {exception.WrappedExceptionMessage}");
    }

    public void GetMergedAll()
    {
        Dictionary<string, object> dictionary = remoteConfigManager.GetMergedAll();
        countOfVariables.text = $"Count of Variables : {dictionary.Count}";
    }

    public void ClearAll()
    {
        remoteConfigManager.ClearAll();
        GetMergedAll();
    }

    public void ApplyDefault()
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("Key", "Value");
        dictionary.Add("Key1", true);
        dictionary.Add("Key2", 5);
        dictionary.Add("Key3", 1.8); 
        remoteConfigManager.ApplyDefault(dictionary);
        GetMergedAll();
    }

    public void ApplyDefaultXml()
    {
        remoteConfigManager.ApplyDefault("xml/remoteConfig");
        GetMergedAll();
    }

    public void LoadLastFetched()
    {
        Debug.Log($"[{TAG}]: LoadLastFetched {remoteConfigManager.LoadLastFetched().getValueAsString("abc")}");
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
