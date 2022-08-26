using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    internal class AuthToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        public const string AuthEnabled = "Auth";

        public AuthToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AuthEnabled);
            _toggle = new Toggle.Toggle("Auth Service", enabled, OnStateChanged, true);
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
                if (HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBToggleEditor.CloudDBEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "CloudDB is dependent on Auth Service. Please disable CloudDB first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                DisableToggle();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(AuthEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(AuthEnabled, true);
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AuthEnabled));
            }
        }
    }
}
