﻿using System.Collections;
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
            Destroy(this);
    }
}