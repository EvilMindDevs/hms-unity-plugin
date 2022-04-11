using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class CrashToggleEditor : ToggleEditor, IDrawer
    {
        private IDependentToggle _dependentToggle;

        public const string CrashKitEnabled = "CrashKit";
        public CrashToggleEditor(IDependentToggle analyticsToggle)
        {
            _dependentToggle = analyticsToggle;
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(CrashKitEnabled);
            _toggle = new Toggle.Toggle("Crash*", enabled, OnStateChanged, true).SetTooltip("Crash Kit is dependent on Analytics Kit.");
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
                DisableToggle();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(CrashKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            if (_dependentToggle != null)
                _dependentToggle.SetToggle();
            Enabled = true;
        }

        public override void DisableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(CrashKitEnabled));
            }
        }
    }
}
