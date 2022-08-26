using UnityEngine;

namespace HmsPlugin
{
    public class CloudStorageToggleEditor : ToggleEditor, IDrawer
    {
        //private IDependentToggle _dependentToggle;

        public const string CloudStorageEnabled = "CloudStorage";
        public CloudStorageToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(CloudStorageEnabled);
            _toggle = new Toggle.Toggle("Cloud Storage", enabled, OnStateChanged, true);//.SetTooltip("");
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
            HMSMainEditorSettings.Instance.Settings.SetBool(CloudStorageEnabled, value);

        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            Enabled = true;
        }

        public override void DisableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
            Enabled = false;
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            Debug.Log("Not Implemented");
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(CloudStorageEnabled));
            }
        }
    }
}
