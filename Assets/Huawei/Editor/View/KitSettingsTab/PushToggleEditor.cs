using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class PushToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        public const string PushKitEnabled = "PushKit";

        public PushToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(PushKitEnabled);
            _toggle = new Toggle.Toggle("Push Kit", enabled, OnStateChanged, true);
            Enabled = enabled;
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                EnableToggle();
            }
            else
            {
                if (HMSMainEditorSettings.Instance.Settings.GetBool(DriveKitToggleEditor.DriveKitEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "DriveKit is dependent on PushKit. Please disable DriveKit first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                DisableToggle();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(PushKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
            Enabled = true;
        }

        public override void DisableToggle()
        {
            Enabled = false;
        }
        public override void RemoveToggleTabView(bool removeTabs)
        {
            //throw new NotImplementedException();
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(PushKitEnabled));
            }
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(PushKitEnabled, true);
            EnableToggle();
        }
    }
}