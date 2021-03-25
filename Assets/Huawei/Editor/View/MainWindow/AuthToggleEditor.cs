using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    internal class AuthToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string AuthEnabled = "Auth";

        public AuthToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AuthEnabled);
            _toggle = new Toggle.Toggle("Auth", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(AuthEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
