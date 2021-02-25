using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin.Extensions
{
    public static class ActionExtension
    {
        public static void InvokeSafe(this Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        public static void InvokeSafe<T1>(this Action<T1> action, T1 value)
        {
            if (action != null)
            {
                action.Invoke(value);
            }
        }

        public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 value1, T2 value2)
        {
            if (action != null)
            {
                action.Invoke(value1, value2);
            }
        }
    }
}

