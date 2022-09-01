using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class IAPToggleEditor : ToggleEditor, IDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;

        public const string IAPKitEnabled = "IAPKit";

        public IAPToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(IAPKitEnabled);
            _tabView = HMSIAPTabFactory.CreateTab("IAP");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("In-App Purchases", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(IAPKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(IAPKitEnabled));
            }
        }
    }
}
