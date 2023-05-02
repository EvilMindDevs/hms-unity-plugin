using UnityEngine;

namespace HmsPlugin
{
    public class Modeling3dKitToggleEditor : ToggleEditor, IDrawer
    {
        public const string Modeling3dKitEnabled = "Modeling3dkit";


        public Modeling3dKitToggleEditor()
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(Modeling3dKitEnabled);
            _toggle = new Toggle.Toggle("Modeling 3D Kit", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(Modeling3dKitEnabled, value);
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
            Enabled = false;
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(Modeling3dKitEnabled));
            }
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            Debug.Log("Not Implemented");
        }
    }
}
