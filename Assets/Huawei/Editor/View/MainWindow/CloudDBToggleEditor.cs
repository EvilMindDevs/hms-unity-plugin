using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class CloudDBToggleEditor : IDrawer
    {
        private Toggle.Toggle _toggle;
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
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                if (GameObject.FindObjectOfType<HMSCloudDBManager>() == null)
                {
                    GameObject obj = new GameObject("HMSCloudDBManager");
                    obj.AddComponent<HMSCloudDBManager>();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                _dependentToggle.SetToggle();
                _tabBar.AddTab(_tabView);
            }
            else
            {
                var cloudDBManagers = GameObject.FindObjectsOfType<HMSCloudDBManager>();
                if (cloudDBManagers.Length > 0)
                {
                    for (int i = 0; i < cloudDBManagers.Length; i++)
                    {
                        GameObject.DestroyImmediate(cloudDBManagers[i].gameObject);
                    }
                }
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(CloudDBEnabled, value);
            _tabBar.RemoveTab(_tabView);
        }

        public void Draw()
        {
            _toggle.Draw();
        }
    }
}
