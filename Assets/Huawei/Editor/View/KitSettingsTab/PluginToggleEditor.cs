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
        private TabBar _tabBar;
        private HMSLibrariesDrawer _librariesDrawer;

        public const string PluginEnabled = "PluginEnabled";

        public PluginToggleEditor(TabBar tabBar)
        {
            _tabBar = tabBar;
            _librariesDrawer = new HMSLibrariesDrawer(_tabBar);
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
                    enabledEditors.ForEach(f => f.EnableToggle());
                _librariesDrawer.CreateDrawer();
            }
            else
            {
                if (enabledEditors != null && enabledEditors.Count > 0)
                    enabledEditors.ForEach(c => c.RemoveToggleTabView(true));
                _librariesDrawer.DestroyDrawer();
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
