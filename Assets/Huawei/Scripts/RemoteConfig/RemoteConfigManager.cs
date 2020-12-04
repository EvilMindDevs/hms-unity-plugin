using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.RemoteConfig;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System.Xml.Linq;
using System.Xml;


public class RemoteConfigManager : MonoBehaviour
{
    string TAG = "RemoteConfig Manager";

    public Action<ConfigValues> OnSetFecthSuccess { get; set; }
    public Action<HMSException> OnSetFecthFailure { get; set; }

    IAGConnectConfig agc = null;

    void Start()
    {
        GetInstance();
        Debug.Log($"[{TAG}]: Start() ");
    }

    //getInstance() Obtains an instance of AGConnectConfig.
    public void GetInstance()
    {
        if (agc == null) agc = AGConnectConfig.GetInstance();
        Debug.Log($"[{TAG}]: GetInstance() {agc}");
    }

    //applyDefault(int resId) Sets a default value for a parameter.
    public void ApplyDefault(Dictionary<String, System.Object> map)
    {
        if(agc != null) agc.ApplyDefault(map);
        Debug.Log($"[{TAG}]: applyDefault with Dictionary");
    }

    //applyDefault(Map<String, Object> map) Sets a default value for a parameter. This path must be in Resources folder.
    public void ApplyDefault(string xmlPath)
    {
        if (agc != null) agc.ApplyDefault(xmlPath);
        Debug.Log($"[{TAG}]: applyDefault({xmlPath})");
    }

    //apply(ConfigValues values) Applies parameter values.
    public void Apply(ConfigValues configValues)
    {
        if (agc != null) agc.Apply(configValues);
        Debug.Log($"[{TAG}]: apply");
    }

    //fetch() Fetches latest parameter values from Remote Configuration at the default 
    //interval of 12 hours. If the method is called within an interval, cached data is returned.
    public void Fetch()
    {
        ITask<ConfigValues> x = agc.Fetch();
        x.AddOnSuccessListener((configValues) =>
        {
            Debug.Log($"[{TAG}]: fetch() Success");
            agc.Apply(configValues);
            OnSetFecthSuccess?.Invoke(configValues);
        });
        x.AddOnFailureListener((exception) =>
        {
            Debug.Log($"[{TAG}]: fetch() Failed => " + exception.ErrorCode);
            Debug.Log($"[{TAG}]: fetch() Failed => " + exception.WrappedExceptionMessage);
            OnSetFecthFailure?.Invoke(exception);
        });
    }

    //fetch(long intervalSeconds) Fetches latest parameter values from Remote Configuration at 
    //a customized interval. If the method is called within an interval, cached data is returned.
    public void Fetch(long intervalSeconds)
    {
        ITask<ConfigValues> x = agc.Fetch(intervalSeconds);
        x.AddOnSuccessListener((configValues) =>
        {
            Debug.Log($"[{TAG}]: fetch() Success");
            Debug.Log($"[{TAG}]: fetch() Success => " + configValues.getValueAsString("abc"));
            agc.Apply(configValues);
            OnSetFecthSuccess?.Invoke(configValues);
        });
        x.AddOnFailureListener((exception) =>
        {
            Debug.Log($"[{TAG}]: fetch() Failed => " + exception.ErrorCode);
            Debug.Log($"[{TAG}]: fetch() Failed => " + exception.WrappedExceptionMessage);
            OnSetFecthFailure?.Invoke(exception);
        });
    }

    //getMergedAll() Returns all values obtained after the combination of the default values and 
    //values in Remote Configuration.
    public Dictionary<string, object> GetMergedAll()
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        if (agc != null) values = agc.GetMergedAll();
        return values;
    }

    //loadLastFetched() Obtains the cached data that is successfully fetched last time.
    public ConfigValues LoadLastFetched()
    {
        ConfigValues config = null;
        if (agc != null) config = agc.LoadLastFetched();
        return config;
    }

    //getSource(String key) Returns the source of a key.
    public string GetSource(string key)
    {
        string source = "";
        if (agc != null) source = agc.GetSource(key);
        return source;
    }

    //clearAll() Clears all cached data, including the data fetched from Remote Configuration and 
    //the default values passed.
    public void ClearAll()
    {
        if (agc != null) agc.ClearAll();
        Debug.Log($"[{TAG}]: clearAll()");
    }

    //setDeveloperMode(boolean isDeveloperMode) Enables the developer mode, in which the number 
    //of times that the client obtains data from Remote Configuration is not limited, and traffic 
    //control is still performed over the cloud.
    public void SetDeveloperMode(Boolean val)
    {
        if (agc != null) agc.DeveloperMode = val;
        Debug.Log($"[{TAG}]: setDeveloperMode({val})");
    }

}
