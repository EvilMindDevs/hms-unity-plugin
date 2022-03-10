using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HuaweiMobileServices.Utils
{
    public class HMSKitManager : MonoBehaviour
    {
        private static HMSKitManager _instance;

        private static bool _instanceExists;

        private static Thread _mainThread;
        private static object _lockObject = new object();
        private static readonly Queue<Action> _actions = new Queue<Action>();

        public static bool isMainThread => Thread.CurrentThread == _mainThread;

        public static bool InstanceExists { get => _instanceExists; set => _instanceExists = value; }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
                _instanceExists = true;
                _mainThread = Thread.CurrentThread;
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _instanceExists = false;
            }
        }

        void Update()
        {
            lock (_lockObject)
            {
                while (_actions.Count > 0)
                {
                    _actions.Dequeue()();
                }
            }
        }

        public static void CreateHMSKitManager()
        {
            lock (_lockObject)
            {
                if (_instanceExists) return;
                Debug.Log("[HMSKitManager] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                _instance = new GameObject("[HMSKitManager]").AddComponent<HMSKitManager>();
            }
        }

        /*public static HMSKitManager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance != null)
                        return _instance;

                    var instances = FindObjectsOfType<HMSKitManager>();
                    var count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1)
                            return _instance = instances[0];
                        Debug.LogWarning($"[{nameof(HMSKitManager)}] There should never be more than one {nameof(HMSKitManager)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        return _instance = instances[0];
                    }
                    Debug.Log($"[HMSKitManager] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"[HMSKitManager]").AddComponent<HMSKitManager>();
                }
            }
        }*/

        /// <summary>
        /// Queues an action to be invoked on the main game thread.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        public static void InvokeAsync(Action action)
        {
            if (!_instanceExists)
            {
                Debug.LogError("[HMSKitManager]: No HMSKitManager exists in the scene. Actions will not be invoked!");
                return;
            }

            if (isMainThread)
            {
                action();
                Debug.unityLogger.logEnabled = false;
            }
            else
            {
                lock (_lockObject)
                {
                    _actions.Enqueue(action);
                }
            }
        }

        /// <summary>
        /// Queues an action to be invoked on the main game thread and blocks the
        /// current thread until the action has been executed.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        public static void Invoke(Action action)
        {
            if (!_instanceExists)
            {
                Debug.LogError("[HMSKitManager]: No HMSKitManager exists in the scene. Actions will not be invoked!");
                return;
            }

            bool hasRun = false;

            InvokeAsync(() =>
            {
                action();
                hasRun = true;
            });

            while (!hasRun)
            {
                Thread.Sleep(5);
            }
        }
    }
}

