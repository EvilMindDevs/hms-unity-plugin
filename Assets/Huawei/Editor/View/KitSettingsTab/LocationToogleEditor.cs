using UnityEngine;

namespace HmsPlugin
{
    public class LocationToggleEditor : ToggleEditor, IDrawer
    {
        private IDependentToggle _dependentToggle;

        public const string LocationEnabled = "Location";
        public LocationToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(LocationEnabled);
            _toggle = new Toggle.Toggle("Location", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(LocationEnabled, value);

        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            if (_dependentToggle != null)
                _dependentToggle.SetToggle();
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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(LocationEnabled));
            }
        }
    }
}
