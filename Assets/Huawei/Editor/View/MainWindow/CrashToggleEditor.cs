using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class CrashToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;
        private IDependentToggle _dependentToggle;

        public const string CrashKitEnabled = "CrashKit";

        public CrashToggleEditor(IDependentToggle analyticsToggle)
        {
            _dependentToggle = analyticsToggle;
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(CrashKitEnabled);
            _toggle = new Toggle.Toggle("Crash*", enabled, OnStateChanged, true).SetTooltip("Crash Kit is dependent on Analytics Kit.");
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(CrashKitEnabled, value);
            if (value)
            {
                _dependentToggle.SetToggle(value);
            }
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
