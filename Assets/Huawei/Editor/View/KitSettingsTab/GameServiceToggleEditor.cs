using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class GameServiceToggleEditor : ToggleEditor, IDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;
        private IDependentToggle _dependentToggle;

        public const string GameServiceEnabled = "GameService";

        public GameServiceToggleEditor(TabBar tabBar, IDependentToggle dependentToggle)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(GameServiceEnabled);
            _dependentToggle = dependentToggle;
            _tabView = HMSGameServiceTabFactory.CreateTab("Game Service");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Game Service*", enabled, OnStateChanged, true).SetTooltip("Game Service is dependent on AccountKit.");
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
            HMSMainEditorSettings.Instance.Settings.SetBool(GameServiceEnabled, value);
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

            if (_dependentToggle != null)
                _dependentToggle.SetToggle();
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(GameServiceEnabled));
            }
        }
    }
}
