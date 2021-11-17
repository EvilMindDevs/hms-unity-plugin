using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    internal class AuthToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        public const string AuthEnabled = "Auth";

        public AuthToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AuthEnabled);
            _toggle = new Toggle.Toggle("Auth", enabled, OnStateChanged, true);
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
                if (HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBToggleEditor.CloudDBEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "CloudDB is dependent on Auth Service. Please disable CloudDB first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                DestroyManagers();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(AuthEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(AuthEnabled, true);
            CreateManagers();
        }

        public override void CreateManagers()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
            if (GameObject.FindObjectOfType<HMSAuthServiceManager>() == null)
            {
                GameObject obj = new GameObject("HMSAuthServiceManager");
                obj.AddComponent<HMSAuthServiceManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var authManagers = GameObject.FindObjectsOfType<HMSAuthServiceManager>();
            if (authManagers.Length > 0)
            {
                for (int i = 0; i < authManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(authManagers[i].gameObject);
                }
            }
            Enabled = false;
        }

        public override void DisableManagers(bool removeTabs)
        {
            var authManagers = GameObject.FindObjectsOfType<HMSAuthServiceManager>();
            if (authManagers.Length > 0)
            {
                for (int i = 0; i < authManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(authManagers[i].gameObject);
                }
            }
        }


        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AuthEnabled));
            }
        }
    }
}
