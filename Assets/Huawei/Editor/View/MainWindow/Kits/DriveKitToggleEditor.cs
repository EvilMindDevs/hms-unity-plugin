using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class DriveKitToggleEditor : ToggleEditor, IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string DriveKitEnabled = "DriveKit";

        public DriveKitToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(DriveKitEnabled);
            _toggle = new Toggle.Toggle("Drive Kit", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                //_tabBar.AddTab(_tabView);
                //if (GameObject.FindObjectOfType<HMSDriveKitManager>() == null)
                //{
                //    GameObject obj = new GameObject("HMSAdsKitManager");
                //    obj.AddComponent<HMSAdsKitManager>();
                //    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                //}
            }
            else
            {
                //var adsKitManagers = GameObject.FindObjectsOfType<HMSAdsKitManager>();
                //if (adsKitManagers.Length > 0)
                //{
                //    for (int i = 0; i < adsKitManagers.Length; i++)
                //    {
                //        GameObject.DestroyImmediate(adsKitManagers[i].gameObject);
                //    }
                //}
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(DriveKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            //throw new NotImplementedException();
        }

        public override void DestroyManagers()
        {
            //throw new NotImplementedException();
        }
    }
}
