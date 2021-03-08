using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class GameServiceToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;

        private const string GameServiceEnabled = "GameServiceEnabled";

        public GameServiceToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(GameServiceEnabled);
            _tabView = HMSGameServiceTabFactory.CreateTab("Game Service");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Game Service", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(GameServiceEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
