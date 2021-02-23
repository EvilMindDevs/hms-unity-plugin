using System;
using HmsPlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Window
{
    public class DisplayDialog
    {
        public static void Create(string title, string message, string ok, Action onOk)
        {
            if (EditorUtility.DisplayDialog(title, message, ok))
            {
                onOk.InvokeSafe();
            }
        }

        public static void Create(string title, string message, string ok, Action onOk, string cancel, Action onCancel = null)
        {
            if (EditorUtility.DisplayDialog(title, message, ok, cancel))
            {
                onOk.InvokeSafe();
            }
            else
            {
                onCancel.InvokeSafe();
            }
        }

        public static void Create(string title, string message, string ok, Action onOk, string cancel, Action onCancel, string alt, Action onAlt)
        {
            int option = EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);

            switch (option)
            {
                case 0: // ok
                    onOk.InvokeSafe();
                    break;

                case 1: // cancel
                    onCancel.InvokeSafe();
                    break;

                case 2: // alt
                    onAlt.InvokeSafe();
                    break;

                default:
                    Debug.LogError("Unrecognized option");
                    break;
            }
        }

    }

}