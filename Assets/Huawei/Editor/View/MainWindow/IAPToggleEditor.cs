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
        private Toggle.Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;

        public const string IAPKitEnabled = "IAPKit";

        public IAPToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(IAPKitEnabled);
            _tabView = HMSIAPTabFactory.CreateTab("IAP");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("IAP", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                _tabBar.AddTab(_tabView);
                CreateManagers();
            }
            else
            {
                DestroyManagers();
                _tabBar.RemoveTab(_tabView);
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(IAPKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            if (GameObject.FindObjectOfType<HMSIAPManager>() == null)
            {
                GameObject obj = new GameObject("HMSIAPManager");
                obj.AddComponent<HMSIAPManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var iapManagers = GameObject.FindObjectsOfType<HMSIAPManager>();
            if (iapManagers.Length > 0)
            {
                for (int i = 0; i < iapManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(iapManagers[i].gameObject);
                }
            }
            Enabled = false;
        }
    }
}
