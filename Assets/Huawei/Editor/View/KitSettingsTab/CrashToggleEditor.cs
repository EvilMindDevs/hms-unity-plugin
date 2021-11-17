using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class CrashToggleEditor : ToggleEditor, IDrawer
    {
        private IDependentToggle _dependentToggle;

        public const string CrashKitEnabled = "CrashKit";
        public CrashToggleEditor(IDependentToggle analyticsToggle)
        {
            _dependentToggle = analyticsToggle;
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(CrashKitEnabled);
            _toggle = new Toggle.Toggle("Crash*", enabled, OnStateChanged, true).SetTooltip("Crash Kit is dependent on Analytics Kit.");
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
            HMSMainEditorSettings.Instance.Settings.SetBool(CrashKitEnabled, value);
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
            if (GameObject.FindObjectOfType<HMSCrashManager>() == null)
            {
                GameObject obj = new GameObject("HMSCrashManager");
                obj.AddComponent<HMSCrashManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
            var crashManagers = GameObject.FindObjectsOfType<HMSCrashManager>();
            if (crashManagers.Length > 0)
            {
                for (int i = 0; i < crashManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(crashManagers[i].gameObject);
                }
            }
            Enabled = false;
        }

        public override void DisableManagers(bool removeTabs)
        {
            var crashManagers = GameObject.FindObjectsOfType<HMSCrashManager>();
            if (crashManagers.Length > 0)
            {
                for (int i = 0; i < crashManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(crashManagers[i].gameObject);
                }
            }
        }


        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(CrashKitEnabled));
            }
        }
    }
}
