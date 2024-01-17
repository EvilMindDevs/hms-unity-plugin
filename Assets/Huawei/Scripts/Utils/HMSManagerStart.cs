using System;
using HuaweiMobileServices.Utils;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSManagerStart
    {
        public static void Start(string TAG, bool useDispatcher = true, Action CustomAction = null)
        {
            Debug.Log($"{TAG} Constructor");
            var exceptionHandler = HMSExceptionHandler.Instance;

            if (useDispatcher && !HMSDispatcher.InstanceExists)
            {
                HMSDispatcher.CreateDispatcher();
            }
            if (CustomAction != null) HMSDispatcher.InvokeAsync(CustomAction);

        }

        public static void Start(Action OnAwake, string TAG)
        {
            Start(TAG);
            HMSDispatcher.InvokeAsync(OnAwake);
        }

        public static void Start(Action OnAwake, Action OnStart, string TAG, Action CustomAction = null)
        {
            Start(OnAwake, TAG);
            HMSDispatcher.InvokeAsync(OnStart);
            if (CustomAction != null) HMSDispatcher.InvokeAsync(CustomAction);
        }

    }
}
