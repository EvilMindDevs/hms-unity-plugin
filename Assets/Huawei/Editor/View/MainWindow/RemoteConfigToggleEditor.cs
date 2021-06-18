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
        private Toggle.Toggle _toggle;
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
        }

        private void OnStateChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(RemoteConfigEnabled, value);
            if (value)
            {
                CreateManagers();
                _dependentToggle.SetToggle();
                _tabBar.AddTab(_tabView);
            }
            else
            {
                DestroyManagers();
                _tabBar.RemoveTab(_tabView);
            }
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
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
            Enabled = false;
        }
    }
}
