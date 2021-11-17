using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class AdsToggleEditor : ToggleEditor, IDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;

        public const string AdsKitEnabled = "AdsKit";

        public AdsToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AdsKitEnabled);
            _tabView = HMSAdsTabFactory.CreateTab("Ads");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Ads", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(AdsKitEnabled, value);
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

            if (GameObject.FindObjectOfType<HMSAdsKitManager>() == null)
            {
                GameObject obj = new GameObject("HMSAdsKitManager");
                obj.AddComponent<HMSAdsKitManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            if (_tabBar != null && _tabView != null)
                _tabBar.RemoveTab(_tabView);

            var adsKitManagers = GameObject.FindObjectsOfType<HMSAdsKitManager>();
            if (adsKitManagers.Length > 0)
            {
                for (int i = 0; i < adsKitManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(adsKitManagers[i].gameObject);
                }
            }
            Enabled = false;
        }

        public override void DisableManagers(bool removeTabs)
        {
            var adsKitManagers = GameObject.FindObjectsOfType<HMSAdsKitManager>();
            if (adsKitManagers.Length > 0)
            {
                for (int i = 0; i < adsKitManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(adsKitManagers[i].gameObject);
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AdsKitEnabled));
            }
        }
    }
}