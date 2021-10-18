using System;

namespace HmsPlugin.TextField
{
    public class TextFieldWithAccept : ITextField, IDrawer
    {
        private TextField _textField;
        private HorizontalSequenceDrawer _editLine;
        private Button.Button _button;

        public TextFieldWithAccept(string label, string initialValue, string buttonText, Action onPressed)
        {
            _editLine = new HorizontalSequenceDrawer();
            _textField = new TextField(label, initialValue);
            _button = new Button.Button(buttonText, onPressed);
            var returnChecker = new ReturnChecker(_textField);
            returnChecker.ReturnPressed += onPressed;
            _editLine.AddDrawer(returnChecker);
            _editLine.AddDrawer(_button);
        }

        public TextFieldWithAccept(string initialValue, string buttonText, Action onPressed) : this(null, initialValue,
            buttonText, onPressed)
        {
        }

        public void Draw()
        {
            _editLine.Draw();
        }

        public string GetCurrentText()
        {
            return _textField.GetCurrentText();
        }

        public TextFieldWithAccept SetLabelWidth(int width)
        {
            _textField.SetLabelWidth(width);
            return this;
        }

        public TextFieldWithAccept SetButtonWidth(int width)
        {
            _button.SetWidth(width);
            return this;
        }

        public void ClearInput()
        {
            _textField.ClearInput();
        }

        public void SetCurrentText(string text)
        {
            _textField.SetCurrentText(text);
        }

        public TextFieldWithAccept AddTooltip(string value)
        {
            _textField.SetTooltip(value);
            return this;
        }
    }
}