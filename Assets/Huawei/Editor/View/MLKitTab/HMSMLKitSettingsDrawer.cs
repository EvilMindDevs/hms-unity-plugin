using HmsPlugin.TextField;

namespace HmsPlugin
{

    internal class HMSMLKitSettingsDrawer : VerticalSequenceDrawer
    {
        private bool translateIsActive = false;
        private bool textToSpeechIsActive = false;
        private TextField.TextFieldWithAccept _keyAPITextField;

        private Toggle.Toggle _enableTranslateToggle;
        private Toggle.Toggle _enableTextToSpeechToggle;
        private HMSSettings _settings;

        public HMSMLKitSettingsDrawer()
        {
            _settings = HMSMLKitSettings.Instance.Settings;

            _keyAPITextField = new TextFieldWithAccept("KeyAPI", _settings.Get(HMSMLKitSettings.MLKeyAPI),
                "Save", OnKeyAPISaveButtonClick).SetLabelWidth(0).SetButtonWidth(100);

            translateIsActive = _settings.GetBool(HMSMLKitSettings.EnableTranslateModule);
            textToSpeechIsActive = _settings.GetBool(HMSMLKitSettings.EnableTextToSpeechModule);
            AddDrawer(new VerticalSequenceDrawer(
                new HorizontalSequenceDrawer(new Spacer(), new Label.Label("- ML Kit Modules -").SetBold(true), new Spacer()),
                new HorizontalSequenceDrawer(new HorizontalLine())
            ));

            SetupSequence();
        }

        private void TranslateModuleDrawer()
        {
            AddDrawer(new VerticalSequenceDrawer(
                new HorizontalSequenceDrawer(new Label.Label("Translate Module").SetBold(true)),
                new HorizontalSequenceDrawer(new Spacer()),
                new HorizontalSequenceDrawer(new Label.Label("Translate Module enables you to translate text between 50+ languages."))
            ));
            AddDrawer(new Space(3));

            _enableTranslateToggle = new Toggle.Toggle("Enable Translate Module", translateIsActive, OnTranslateToggleChanged, false);
            AddDrawer(_enableTranslateToggle);
            AddDrawer(new HorizontalLine());

        }

        private void TextToSpeechModuleDrawer()
        {
            AddDrawer(new VerticalSequenceDrawer(
                new HorizontalSequenceDrawer(new Label.Label("Text To Speech Module").SetBold(true)),
                new HorizontalSequenceDrawer(new Spacer()),
                new HorizontalSequenceDrawer(new Label.Label("Text To Speech Module enables you to convert text to speech."))

            ));
            AddDrawer(new Space(3));
            _enableTextToSpeechToggle = new Toggle.Toggle("Enable TextToSpeech Module", textToSpeechIsActive, OnTextToSpeechToggleChanged, false).SetLabelWidth(180);
            AddDrawer(_enableTextToSpeechToggle);
            AddDrawer(new HorizontalLine());
        }

        private void KeyAPIDrawer()
        {
            AddDrawer(new VerticalSequenceDrawer(
                new HorizontalSequenceDrawer(new Label.Label("Key API").SetBold(true)),
                new HorizontalSequenceDrawer(new Spacer()),
                new HorizontalSequenceDrawer(new Label.Label("You can get your API key from Huawei Developer Console OR  agconnect-services.json file."))
            ));
            AddDrawer(new Space(3));
            AddDrawer(_keyAPITextField);
            AddDrawer(new HorizontalLine());
        }

        private void SetupSequence()
        {
            KeyAPIDrawer();
            TranslateModuleDrawer();
            TextToSpeechModuleDrawer();
        }
        private void OnTranslateToggleChanged(bool value)
        {
            translateIsActive = value;

            _settings.SetBool(HMSMLKitSettings.EnableTranslateModule, translateIsActive);
        }

        private void OnTextToSpeechToggleChanged(bool value)
        {
            textToSpeechIsActive = value;
            _settings.SetBool(HMSMLKitSettings.EnableTextToSpeechModule, textToSpeechIsActive);
        }

        private void OnKeyAPISaveButtonClick()
        {
            _settings.Set(HMSMLKitSettings.MLKeyAPI, _keyAPITextField.GetCurrentText());
        }
    }


}
