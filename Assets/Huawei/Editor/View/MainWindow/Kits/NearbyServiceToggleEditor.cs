using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class NearbyServiceToggleEditor : ToggleEditor, IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string NearbyServiceEnabled = "NearbyService";

        public NearbyServiceToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(NearbyServiceEnabled);
            _toggle = new Toggle.Toggle("Nearby Service", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(NearbyServiceEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            if (GameObject.FindObjectOfType<HMSNearbyServiceManager>() == null)
            {
                GameObject obj = new GameObject("HMSNearbyServiceManager");
                obj.AddComponent<HMSNearbyServiceManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var nearbyServiceManagers = GameObject.FindObjectsOfType<HMSNearbyServiceManager>();
            if (nearbyServiceManagers.Length > 0)
            {
                for (int i = 0; i < nearbyServiceManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(nearbyServiceManagers[i].gameObject);
                }
            }
            Enabled = false;
        }
    }
}
