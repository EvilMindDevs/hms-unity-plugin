using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class AppMessagingToggleEditor : ToggleEditor, IDrawer
    {
        public const string AppMessagingEnabled = "AppMessaging";

        public AppMessagingToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AppMessagingEnabled);
            _toggle = new Toggle.Toggle("App Messaging", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(AppMessagingEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
            if (GameObject.FindObjectOfType<HMSAppMessagingManager>() == null)
            {
                GameObject obj = new GameObject("HMSAppMessagingManager");
                obj.AddComponent<HMSAppMessagingManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var appMessagingManagers = GameObject.FindObjectsOfType<HMSAppMessagingManager>();
            if (appMessagingManagers.Length > 0)
            {
                for (int i = 0; i < appMessagingManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(appMessagingManagers[i].gameObject);
                }
            }
            Enabled = false;
        }

        public override void DisableManagers(bool removeTabs)
        {
            var appMessagingManagers = GameObject.FindObjectsOfType<HMSAppMessagingManager>();
            if (appMessagingManagers.Length > 0)
            {
                for (int i = 0; i < appMessagingManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(appMessagingManagers[i].gameObject);
                }
            }
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AppMessagingEnabled));
            }
        }
    }
}
