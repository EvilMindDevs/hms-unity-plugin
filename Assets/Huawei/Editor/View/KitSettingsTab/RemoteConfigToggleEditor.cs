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
            _toggle = new Toggle.Toggle("Remote Config*", enabled, OnStateChanged, true).SetTooltip("Remote Config is dependent on Analytics Kit.");
            Enabled = enabled;
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(RemoteConfigEnabled, value);
            if (value)
            {
                CreateManagers();
            }
            else
            {
                DestroyManagers();
            }
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            if (_dependentToggle != null)
                _dependentToggle.SetToggle();

            if (_tabBar != null && _tabView != null)
                _tabBar.AddTab(_tabView);

            if (GameObject.FindObjectOfType<HMSRemoteConfigManager>() == null)
            {
                GameObject obj = new GameObject("HMSRemoteConfigManager");
                obj.AddComponent<HMSRemoteConfigManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var remoteConfigManagers = GameObject.FindObjectsOfType<HMSRemoteConfigManager>();
            if (remoteConfigManagers.Length > 0)
            {
                for (int i = 0; i < remoteConfigManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(remoteConfigManagers[i].gameObject);
                }
            }
            if (_tabBar != null && _tabView != null)
                _tabBar.RemoveTab(_tabView);
            Enabled = false;
        }

        public override void DisableManagers(bool removeTabs)
        {
            var remoteConfigManagers = GameObject.FindObjectsOfType<HMSRemoteConfigManager>();
            if (remoteConfigManagers.Length > 0)
            {
                for (int i = 0; i < remoteConfigManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(remoteConfigManagers[i].gameObject);
                }
            }
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
