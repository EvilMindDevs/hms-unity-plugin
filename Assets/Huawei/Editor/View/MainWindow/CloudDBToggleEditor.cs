using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class CloudDBToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string CloudDBEnabled = "CloudDB";

        public CloudDBToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBEnabled);
            _toggle = new Toggle.Toggle("Cloud DB", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(CloudDBEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
