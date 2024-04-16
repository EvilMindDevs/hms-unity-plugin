using HmsPlugin.Button;
using HmsPlugin.TextField;
using UnityEngine;

namespace HmsPlugin
{
    internal class ModelingSettingsDrawer : VerticalSequenceDrawer
    {
        private TextField.TextFieldWithAccept _keyAPITextField;

        private HMSSettings _settings;

        public ModelingSettingsDrawer()
        {
            _settings = HMSModelingKitSettings.Instance.Settings;
            _keyAPITextField = new TextFieldWithAccept("KeyAPI", _settings.Get(HMSModelingKitSettings.ModelingKeyAPI),
                "Save", OnKeyAPISaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);

            SetupSequence();
        }
        private void OnKeyAPISaveButtonClick()
        {
            _settings.Set(HMSModelingKitSettings.ModelingKeyAPI, _keyAPITextField.GetCurrentText());
        }

        private void SetupSequence()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.cyan;
            AddDrawer(new Space(3));
            AddDrawer(new Space(3));
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("KeyAPI").SetBold(true),
                new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_keyAPITextField);
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine()));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("\tNeed help? You can refer to"), new Clickable(new Label.Label("Guides & References").SetBold(true)
                .SetStyle(style).SetFontSize(10),
                () => { Application.OpenURL("https://evilminddevs.gitbook.io/hms-unity-plugin_/kits-and-services/3d-modeling-kit/guides-and-references"); })
                    , new Label.Label(".")));
        }
    }
}
