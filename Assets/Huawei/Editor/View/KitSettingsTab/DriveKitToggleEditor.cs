using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace HmsPlugin
{
    public class DriveKitToggleEditor : ToggleEditor, IDrawer
    {
        private List<IDependentToggle> _dependentToggleList;

        public const string DriveKitEnabled = "DriveKit";

        public DriveKitToggleEditor(IDependentToggle _dependent1, IDependentToggle _dependent2)
        {
            _dependentToggleList = new List<IDependentToggle>();
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(DriveKitEnabled);
            _dependentToggleList.Add(_dependent1);
            _dependentToggleList.Add(_dependent2);
            _toggle = new Toggle.Toggle("Drive Kit*", enabled, OnStateChanged, true).SetTooltip("DriveKit is dependent on AccountKit and PushKit");
            Enabled = enabled;
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                EnableToggle();
            }
            else
            {
                DisableToggle();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(DriveKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            if (_dependentToggleList.Count > 1)
            {
                foreach (IDependentToggle _dependentToggle in _dependentToggleList) {
                    _dependentToggle.SetToggle();
                }
            }

            Enabled = true;

        }

        public override void DisableToggle()
        {
            Enabled = false;
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            //throw new NotImplementedException(); Not Implemented because not needed.
        }

        public override void RefreshToggles()
        {
            if (_toggle != null )
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(DriveKitEnabled));
            }
        }
    }
}
