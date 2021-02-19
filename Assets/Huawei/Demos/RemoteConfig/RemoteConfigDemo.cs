using HuaweiMobileServices.RemoteConfig;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RemoteConfigDemo : MonoBehaviour
{
    private Text countOfVariables;
    string TAG = "RemoteConfig Demo";

    void Start()
    {
        countOfVariables = GameObject.Find("countOfVariables").GetComponent<Text>();
    }

    public void Fetch()
    {
        HMSRemoteConfigManager.Instance.OnFecthSuccess = OnFecthSuccess;
        HMSRemoteConfigManager.Instance.OnFecthFailure = OnFecthFailure;
        HMSRemoteConfigManager.Instance.Fetch();
    }

    private void OnFecthSuccess(ConfigValues config)
    {
        HMSRemoteConfigManager.Instance.Apply(config);
        Debug.Log($"[{TAG}]: fetch() Success");
    }

    private void OnFecthFailure(HMSException exception)
    {
        Debug.Log($"[{TAG}]: fetch() Failed Error Code => {exception.ErrorCode} Message => {exception.WrappedExceptionMessage}");
    }

    public void GetMergedAll()
    {
        Dictionary<string, object> dictionary = HMSRemoteConfigManager.Instance.GetMergedAll();
        countOfVariables.text = $"Count of Variables : {dictionary.Count}";
    }

    public void ClearAll()
    {
        HMSRemoteConfigManager.Instance.ClearAll();
        GetMergedAll();
    }

    public void ApplyDefault()
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("Key", "Value");
        dictionary.Add("Key1", true);
        dictionary.Add("Key2", 5);
        dictionary.Add("Key3", 1.8);
        HMSRemoteConfigManager.Instance.ApplyDefault(dictionary);
        GetMergedAll();
    }

    public void ApplyDefaultXml()
    {
        HMSRemoteConfigManager.Instance.ApplyDefault("xml/remoteConfig");
        GetMergedAll();
    }

    public void LoadLastFetched()
    {
        Debug.Log($"[{TAG}]: LoadLastFetched {HMSRemoteConfigManager.Instance.LoadLastFetched().getValueAsString("abc")}");
    }

    public void DeveloperMode(bool val)
    {
        HMSRemoteConfigManager.Instance.SetDeveloperMode(val);
    }

    public void GetSource()
    {
        Debug.Log($"[{TAG}]: GetSource(Key) {HMSRemoteConfigManager.Instance.GetSource("Key")}");
    }
}
