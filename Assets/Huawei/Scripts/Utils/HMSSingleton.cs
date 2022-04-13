using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMSSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<T>();
            if (instance == null)
                Debug.LogError("Singleton<" + typeof(T) + "> instance has been not found.");
            return instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
            instance = this as T;
        else if (instance != this)
            Destroy(this);
    }
}

public class HMSEditorSingleton<T> where T : new()
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }
}

public class HMSManagerSingleton<T> where T : new()
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }
}
