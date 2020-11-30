using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HuaweiMobileServices.RemoteConfig;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System.Xml.Linq;
using System.Xml;


public class RemoteConfig : MonoBehaviour
{
    string TAG = "RemoteConfig Manager";

    public Action<ConfigValues> OnSetFecthSuccess { get; set; }
    public Action<HMSException> OnSetFecthFailure { get; set; }

    IAGConnectConfig agc = null;

    void Start()
    {
        getInstance();
        Debug.Log($"[{TAG}]: Start() ");
    }

    //getInstance() Obtains an instance of AGConnectConfig.
    public void getInstance()
    {
        if (agc == null) agc = AGConnectConfig.GetInstance();
        Debug.Log($"[{TAG}]: GetInstance() ");
    }

    //applyDefault(int resId) Sets a default value for a parameter.
    public void applyDefault(Dictionary<String, System.Object> map)
    {
        if(agc != null) agc.applyDefault(map);
        Debug.Log($"[{TAG}]: applyDefault with Dictionary");
    }

    //applyDefault(Map<String, Object> map) Sets a default value for a parameter. This path must be in Resources folder.(Example `xml/remoteConfig`)
    public void applyDefault(string xmlPath)
    {
        agc.applyDefault(xmlPath);
        Debug.Log($"[{TAG}]: applyDefault({xmlPath})");
    }

    //apply(ConfigValues values) Applies parameter values.
    public void apply(ConfigValues configValues)
    {
        if (agc != null) agc.apply(configValues);
        Debug.Log($"[{TAG}]: apply");
    }

    //fetch() Fetches latest parameter values from Remote Configuration at the default 
    //interval of 12 hours. If the method is called within an interval, cached data is returned.
    public void fetch()
    {
        ITask<ConfigValues> x = agc.fetch();
        x.AddOnSuccessListener((configValues) =>
        {
            Debug.Log($"[{TAG}]: fetch() Success");
            agc.apply(configValues);
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
    public void fetch(long intervalSeconds)
    {
        ITask<ConfigValues> x = agc.fetch(intervalSeconds);
        x.AddOnSuccessListener((configValues) =>
        {
            Debug.Log($"[{TAG}]: fetch() Success");
            Debug.Log($"[{TAG}]: fetch() Success => " + configValues.getValueAsString("abc"));
            agc.apply(configValues);
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
    public void getMergedAll()
    {
        Dictionary<String, System.Object> values = null;
        if (agc != null) values = agc.getMergedAll();
        Debug.Log($"[{TAG}]: getMergedAll() size {values.Count}");
    }

    //loadLastFetched() Obtains the cached data that is successfully fetched last time.
    public ConfigValues loadLastFetched()
    {
        return agc.loadLastFetched();
    }

    //getSource(String key) Returns the source of a key.
    public Constants.SOURCE getSource(string key)
    {
        return agc.getSource(key);
    }

    //clearAll() Clears all cached data, including the data fetched from Remote Configuration and 
    //the default values passed.
    public void clearAll()
    {
        if (agc != null) agc.clearAll();
        Debug.Log($"[{TAG}]: clearAll()");
    }

    //setDeveloperMode(boolean isDeveloperMode) Enables the developer mode, in which the number 
    //of times that the client obtains data from Remote Configuration is not limited, and traffic 
    //control is still performed over the cloud.
    public void setDeveloperMode(Boolean val)
    {
        if (agc != null) agc.setDeveloperMode(val);
        Debug.Log($"[{TAG}]: setDeveloperMode({val})");
    }

    

}
