using System;
using UnityEngine;

namespace HmsPlugin
{
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
        private static Lazy<T> _instance = new Lazy<T>(() => new T());

        public static T Instance
        {
            get => _instance.Value;
            set => _instance = new Lazy<T>(() => value);
        }
    }
}
