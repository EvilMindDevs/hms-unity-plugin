using UnityEngine;

namespace HmsPlugin
{
    public class AppLinkingToggleEditor : ToggleEditor, IDrawer
    {
        private IDependentToggle _dependentToggle;

        public const string AppLinkingEnabled = "AppLinking";
        public AppLinkingToggleEditor(IDependentToggle analyticsToggle)
        {
            _dependentToggle = analyticsToggle;
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AppLinkingEnabled);
            _toggle = new Toggle.Toggle("App Linking*", enabled, OnStateChanged, true).SetTooltip("AppLinking is dependent on Analytics Kit.");
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
            HMSMainEditorSettings.Instance.Settings.SetBool(AppLinkingEnabled, value);

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
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AppLinkingEnabled));
            }
        }
    }
}
