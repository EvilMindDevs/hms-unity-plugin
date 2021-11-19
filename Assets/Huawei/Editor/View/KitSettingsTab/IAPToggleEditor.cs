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
            _toggle = new Toggle.Toggle("IAP", enabled, OnStateChanged, true);
            Enabled = enabled;
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                CreateManagers();
            }
            else
            {
                DestroyManagers();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(IAPKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            if (_tabBar != null && _tabView != null)
                _tabBar.AddTab(_tabView);

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
            if (_tabBar != null && _tabView != null)
                _tabBar.RemoveTab(_tabView);
            Enabled = false;
        }

        public override void DisableManagers(bool removeTabs)
        {
            var iapManagers = GameObject.FindObjectsOfType<HMSIAPManager>();
            if (iapManagers.Length > 0)
            {
                for (int i = 0; i < iapManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(iapManagers[i].gameObject);
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(IAPKitEnabled));
            }
        }
    }
}
