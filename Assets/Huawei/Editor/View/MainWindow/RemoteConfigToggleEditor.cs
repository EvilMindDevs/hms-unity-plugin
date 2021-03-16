using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class RemoteConfigToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;

        public const string RemoteConfigEnabled = "RemoteConfig";

        public RemoteConfigToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(RemoteConfigEnabled);
            _tabView = HMSRemoteConfigTabFactory.CreateTab("Remote Config");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Remote Config", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                _tabBar.AddTab(_tabView);
            }
            else
            {
                _tabBar.RemoteTab(_tabView);
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(RemoteConfigEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
