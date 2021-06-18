using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class AccountToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        private Toggle.Toggle _toggle;

        public const string AccountKitEnabled = "AccountKit";

        public AccountToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AccountKitEnabled);
            _toggle = new Toggle.Toggle("Account", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                CreateManagers();
            }
            else
            {
                if (HMSMainEditorSettings.Instance.Settings.GetBool(GameServiceToggleEditor.GameServiceEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "Game Service is dependent on AccountKit. Please disable Game Service first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }

                DestroyManagers();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(AccountKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(AccountKitEnabled, true);
            CreateManagers();
        }

        public override void CreateManagers()
        {
            if (GameObject.FindObjectOfType<HMSAccountManager>() == null)
            {
                GameObject obj = new GameObject("HMSAccountManager");
                obj.AddComponent<HMSAccountManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var accountManagers = GameObject.FindObjectsOfType<HMSAccountManager>();
            if (accountManagers.Length > 0)
            {
                for (int i = 0; i < accountManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(accountManagers[i].gameObject);
                }
            }
            Enabled = false;
        }
    }
}