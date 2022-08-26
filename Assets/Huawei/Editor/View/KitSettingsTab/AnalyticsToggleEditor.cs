using UnityEditor;

namespace HmsPlugin
{
    public class AnalyticsToggleEditor : ToggleEditor, IDrawer, IDependentToggle
    {
        public const string AnalyticsKitEnabled = "AnalyticsKit";

        public AnalyticsToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AnalyticsKitEnabled);
            _toggle = new Toggle.Toggle("Analytics Kit", enabled, OnStateChanged, true);
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
                //TODO: Improve handling here.
                if (HMSMainEditorSettings.Instance.Settings.GetBool(CrashToggleEditor.CrashKitEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "CrashKit is dependent on AnalyticsKit. Please disable CrashKit first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                else if (HMSMainEditorSettings.Instance.Settings.GetBool(RemoteConfigToggleEditor.RemoteConfigEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "Remote Config is dependent on AnalyticsKit. Please disable Remote Config first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }
                else if (HMSMainEditorSettings.Instance.Settings.GetBool(AppLinkingToggleEditor.AppLinkingEnabled))
                {
                    EditorUtility.DisplayDialog("Error", "App Linking is dependent on AnalyticsKit. Please disable App Linking first.", "OK");
                    _toggle.SetChecked(true);
                    return;
                }

                DisableToggle();

            }
            HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public void SetToggle()
        {
            _toggle.SetChecked(true);
            HMSMainEditorSettings.Instance.Settings.SetBool(AnalyticsKitEnabled, true);
            EnableToggle();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
            Enabled = true;
        }

        public override void DisableToggle()
        {
            Enabled = false;
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            //throw new NotImplementedException();
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(AnalyticsKitEnabled));
            }
        }
    }

    public interface IDependentToggle
    {
        void SetToggle();
    }
}