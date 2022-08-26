using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class AccountToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        public const string AccountKitEnabled = "AccountKit";

        public AccountToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AccountKitEnabled);
            _toggle = new Toggle.Toggle("Account Kit", enabled, OnStateChanged, true);
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
                if (HMSMainEditorSettings.Instance.Settings.GetBool(GameServiceToggleEditor.GameServiceEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "Game Service is dependent on AccountKit. Please disable Game Service first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                if (HMSMainEditorSettings.Instance.Settings.GetBool(DriveKitToggleEditor.DriveKitEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "DriveKit is dependent on AccountKit. Please disable DriveKit first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }

                DisableToggle();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(AccountKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(AccountKitEnabled, true);
            EnableToggle();
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AccountKitEnabled));
            }
        }
    }
}