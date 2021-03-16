using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;

namespace HmsPlugin
{
    public class AnalyticsToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string AnalyticsKitEnabled = "AnalyticsKit";

        public AnalyticsToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AnalyticsKitEnabled);
            _toggle = new Toggle.Toggle("Analytics", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}