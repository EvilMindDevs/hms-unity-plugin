using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class CloudDBToggleEditor : ToggleEditor, IDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;
        private IDependentToggle _dependentToggle;

        public const string CloudDBEnabled = "CloudDB";

        public CloudDBToggleEditor(TabBar tabBar, IDependentToggle dependentToggle)
        {
            _dependentToggle = dependentToggle;
            _tabView = HMSCloudDBTabFactory.CreateTab("Cloud DB");
            _tabBar = tabBar;

            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBEnabled);
            _toggle = new Toggle.Toggle("Cloud DB*", enabled, OnStateChanged, true).SetTooltip("CloudDB is dependent on Auth Service.");
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
            HMSMainEditorSettings.Instance.Settings.SetBool(CloudDBEnabled, value);
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBEnabled));
            }
        }
    }
}
