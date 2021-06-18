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
        private Toggle.Toggle _toggle;

        public const string AuthEnabled = "Auth";

        public AuthToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AuthEnabled);
            _toggle = new Toggle.Toggle("Auth", enabled, OnStateChanged, true);
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
    }
}
