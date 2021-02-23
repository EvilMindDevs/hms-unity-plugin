using System;

namespace HmsPlugin.TextField
{
    public class TextFieldWithAccept : ITextField, IDrawer
    {
        private TextField _textField;
        private HorizontalSequenceDrawer _editLine;

        public TextFieldWithAccept(string label, string initialValue, string buttonText, Action onPressed)
        {
            _editLine = new HorizontalSequenceDrawer();
            _textField = new TextField(label, initialValue);
            var returnChecker = new ReturnChecker(_textField);
            returnChecker.ReturnPressed += onPressed;
            _editLine.AddDrawer(returnChecker);
            _editLine.AddDrawer(new Button.Button(buttonText, onPressed));
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

        public void ClearInput()
        {
            _textField.ClearInput();
        }

        public void SetCurrentText(string text)
        {
            _textField.SetCurrentText(text);
        }
    }
}