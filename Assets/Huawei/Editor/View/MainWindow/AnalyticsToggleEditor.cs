using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;

namespace HmsPlugin
{
    public class AnalyticsToggleEditor : IDrawer, IDependentToggle
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
            if (!value && HMSMainEditorSettings.Instance.Settings.GetBool(CrashToggleEditor.CrashKitEnabled))
            {
                EditorUtility.DisplayDialog("Error", "CrashKit is dependent on AnalyticsKit. Please disable CrashKit first.", "OK");
                _toggle.SetChecked(true);
            }
            else
            {
                HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, value);
            }
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle(bool value)
        {
            _toggle.SetChecked(value);
            HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, value);
        }
    }

    public interface IDependentToggle
    {
        void SetToggle(bool value);
    }
}