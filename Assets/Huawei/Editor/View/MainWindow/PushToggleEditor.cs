using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;

namespace HmsPlugin
{
    public class PushToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string PushKitEnabled = "PushKit";

        public PushToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(PushKitEnabled);
            _toggle = new Toggle.Toggle("Push", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(PushKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}