using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class AdsToggleEditor : ToggleEditor, IDrawer
    {
        private Toggle.Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;

        public const string AdsKitEnabled = "AdsKit";

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
                CreateManagers();
            }
            else
            {
                _tabBar.RemoveTab(_tabView);
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
    }
}