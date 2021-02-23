using System;
using HmsPlugin.Extensions;
using HmsPlugin.Window;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public static class LongProcess
    {
        public static void Run(Action action, string title, string message)
        {
            var window = ModalWindow.CreateWindow(title, new Vector2(250, 40), new Label.Label(message));

            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    action.InvokeSafe();
                    window.Close();
                };
            };
        }
    }
}