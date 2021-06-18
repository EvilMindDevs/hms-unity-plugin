using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class AnalyticsToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        private Toggle.Toggle _toggle;

        public const string AnalyticsKitEnabled = "AnalyticsKit";

        public AnalyticsToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AnalyticsKitEnabled);
            _toggle = new Toggle.Toggle("Analytics", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                CreateManagers();
            }
            else
            {
                //TODO: Improve handling here.
                if (HMSMainEditorSettings.Instance.Settings.GetBool(CrashToggleEditor.CrashKitEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "CrashKit is dependent on AnalyticsKit. Please disable CrashKit first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                else if (HMSMainEditorSettings.Instance.Settings.GetBool(RemoteConfigToggleEditor.RemoteConfigEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "Remote Config is dependent on AnalyticsKit. Please disable Remote Config first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }

                DestroyManagers();

            }
            HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, true);
            CreateManagers();
        }

        public override void CreateManagers()
        {
            if (GameObject.FindObjectOfType<HMSAnalyticsManager>() == null)
            {
                GameObject obj = new GameObject("HMSAnalyticsManager");
                obj.AddComponent<HMSAnalyticsManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var analyticManagers = GameObject.FindObjectsOfType<HMSAnalyticsManager>();
            if (analyticManagers.Length > 0)
            {
                for (int i = 0; i < analyticManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(analyticManagers[i].gameObject);
                }
            }
            Enabled = false;
        }
    }

    public interface IDependentToggle
    {
        void SetToggle();
    }
}