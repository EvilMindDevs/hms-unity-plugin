using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class RemoteConfigToggleEditor : ToggleEditor, IDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;
        private IDependentToggle _dependentToggle;

        public const string RemoteConfigEnabled = "RemoteConfig";
        public RemoteConfigToggleEditor(TabBar tabBar, IDependentToggle analyticsToggle)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(RemoteConfigEnabled);
            _dependentToggle = analyticsToggle;
            _tabView = HMSRemoteConfigTabFactory.CreateTab("Remote Config");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Remote Configuration*", enabled, OnStateChanged, true).SetTooltip("Remote Config is dependent on Analytics Kit.");
            Enabled = enabled;
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(RemoteConfigEnabled, value);
            if (value)
            {
                EnableToggle();
            }
            else
            {
                DisableToggle();
            }
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

            if (_tabBar != null && _tabView != null)
                _tabBar.AddTab(_tabView);
            Enabled = true;
        }

        public override void DisableToggle()
        {
            if (_tabBar != null && _tabView != null)
                _tabBar.RemoveTab(_tabView);
            Enabled = false;
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            if (removeTabs)
            {
                if (_tabBar != null && _tabView != null)
                    _tabBar.RemoveTab(_tabView);
            }
            else
            {
                if (_tabBar != null && _tabView != null)
                    _tabBar.AddTab(_tabView);
            }
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(RemoteConfigEnabled));
            }
        }
    }
}
