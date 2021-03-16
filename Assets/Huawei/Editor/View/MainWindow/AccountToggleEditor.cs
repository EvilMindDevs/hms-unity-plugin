using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;

namespace HmsPlugin
{
    public class AccountToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string AccountKitEnabled = "AccountKit";

        public AccountToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AccountKitEnabled);
            _toggle = new Toggle.Toggle("Account", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(AccountKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}