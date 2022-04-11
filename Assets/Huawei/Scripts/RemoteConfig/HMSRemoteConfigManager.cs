using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.RemoteConfig;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System.Xml.Linq;
using System.Xml;
using HmsPlugin;

public class HMSRemoteConfigManager : HMSManagerSingleton<HMSRemoteConfigManager>
{
    string TAG = "HMSRemoteConfig Manager";

    public Action<ConfigValues> OnFecthSuccess { get; set; }
    public Action<HMSException> OnFecthFailure { get; set; }

    IAGConnectConfig agc = null;

    public HMSRemoteConfigManager()
    {
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(OnAwake);
    }

    private void OnAwake()
    {
        GetInstance();
        if (HMSRemoteConfigSettings.Instance != null)
        {
            SetDeveloperMode(HMSRemoteConfigSettings.Instance.Settings.GetBool(HMSRemoteConfigSettings.DeveloperMode));
            if (HMSRemoteConfigSettings.Instance.Settings.GetBool(HMSRemoteConfigSettings.ApplyDefaultValues))
            {
                var values = HMSRemoteDefaultValueSettings.Instance.GetDefaultValues();
                if (values != null && values.Count > 0)
                {
                    ApplyDefault(values);
                }
            }
        }
        Debug.Log($"[{TAG}]: Start() ");
    }

    //getInstance() Obtains an instance of AGConnectConfig.
    public void GetInstance()
    {
        if (agc == null) agc = AGConnectConfig.GetInstance();
        Debug.Log($"[{TAG}]: GetInstance() {agc}");
    }

    //applyDefault(int resId) Sets a default value for a parameter.
    public void ApplyDefault(Dictionary<string, object> dictionary)
    {
        if (agc != null) agc.ApplyDefault(dictionary);
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
            Debug.Log("[HMSRemoteConfigManager] Fetch success.");
            OnFecthSuccess?.Invoke(configValues);
        });
        x.AddOnFailureListener((exception) =>
        {
            Debug.LogError("[HMSRemoteConfigManager]: Fetch failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
            OnFecthFailure?.Invoke(exception);
        });
    }

    //fetch(long intervalSeconds) Fetches latest parameter values from Remote Configuration at 
    //a customized interval. If the method is called within an interval, cached data is returned.
    public void Fetch(long intervalSeconds)
    {
        ITask<ConfigValues> x = agc.Fetch(intervalSeconds);
        x.AddOnSuccessListener((configValues) =>
        {
            OnFecthSuccess?.Invoke(configValues);
        });
        x.AddOnFailureListener((exception) =>
        {
            OnFecthFailure?.Invoke(exception);
        });
    }

    //getMergedAll() Returns all values obtained after the combination of the default values and 
    //values in Remote Configuration.
    public Dictionary<string, object> GetMergedAll()
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        if (agc != null) dictionary = agc.GetMergedAll();
        /*foreach(KeyValuePair<string, object> kvp in values)
        {
            Debug.Log($"[Remote Config]: key:{kvp.Key} / value:{kvp.Value}");
        }*/
        return dictionary;
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

    //Returns the value for a key.
    public bool GetValueAsBoolean(string paramString) => agc.GetValueAsBoolean(paramString);
    public byte[] GetValueAsByteArray(string paramString) => agc.GetValueAsByteArray(paramString);
    public double GetValueAsDouble(string paramString) => agc.GetValueAsDouble(paramString);
    public long GetValueAsLong(string paramString) => agc.GetValueAsLong(paramString);
    public string GetValueAsString(string paramString) => agc.GetValueAsString(paramString);

}
