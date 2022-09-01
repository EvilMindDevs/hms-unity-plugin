using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

namespace HmsPlugin
{
    public class ScanKitToggleEditor : ToggleEditor, IDrawer
    {

        public const string ScanKitEnabled = "Scan";

        public ScanKitToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(ScanKitEnabled);
            _toggle = new Toggle.Toggle("Scan", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(ScanKitEnabled, value);
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
            //throw new NotImplementedException(); Not Implemented because not needed.
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(ScanKitEnabled));
            }
        }
    }
}
