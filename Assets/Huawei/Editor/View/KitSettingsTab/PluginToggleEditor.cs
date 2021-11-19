using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace HmsPlugin
{
    public class PluginToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string PluginEnabled = "PluginEnabled";

        public PluginToggleEditor()
        {
            bool enabled = HMSPluginSettings.Instance.Settings.GetBool(PluginEnabled, true);
            _toggle = new Toggle.Toggle("Enable Plugin", enabled, OnStateChanged, true);
            RefreshDrawer(!enabled);
        }

        private void OnStateChanged(bool value)
        {
            var enabledEditors = HMSMainKitsTabFactory.GetEnabledEditors();
            HMSPluginSettings.Instance.Settings.SetBool(PluginEnabled, value);
            if (value)
            {
                if (enabledEditors != null && enabledEditors.Count > 0)
                    enabledEditors.ForEach(f => f.CreateManagers());
            }
            else
            {
                if (enabledEditors != null && enabledEditors.Count > 0)
                    enabledEditors.ForEach(c => c.DisableManagers(true));
            }
            RefreshDrawer(value);
        }

        public void RefreshDrawer(bool value)
        {
            if (HMSMainKitsTabFactory._disabledDrawer != null)
            {
                HMSMainKitsTabFactory._disabledDrawer.SetEnabled(!value);
            }
        }

        public void RefreshToggle()
        {
            if (_toggle != null)
                _toggle.SetChecked(HMSPluginSettings.Instance.Settings.GetBool(PluginEnabled));
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
