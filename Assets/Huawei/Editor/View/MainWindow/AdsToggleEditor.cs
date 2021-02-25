using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;

namespace HmsPlugin
{
    public class AdsToggleEditor : IDrawer
    {
        private Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;

        public AdsToggleEditor(TabBar tabBar)
        {
            _toggle = new Toggle("Ads", false, OnStateChanged);
            _tabBar = tabBar;
            _tabView = HMSAdsTabFactory.CreateTab("Ads");
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
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}