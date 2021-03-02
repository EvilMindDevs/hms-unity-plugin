using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;

namespace HmsPlugin
{
    public class AdsToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;

        private const string AdsKitEnabled = "AdsKitEnabled";

        public AdsToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AdsKitEnabled);
            _tabView = HMSAdsTabFactory.CreateTab("Ads");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Ads", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(AdsKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}