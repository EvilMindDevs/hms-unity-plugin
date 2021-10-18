
using System;
using UnityEngine;

namespace HmsPlugin.Toggle
{
    internal class ToggledTextFieldWithButton : HorizontalSequenceDrawer
    {
        private TextField.TextFieldWithAccept textFieldWithAccept;

        private Toggle _toggle;
        private DisabledDrawer _disabledDrawer;

        public ToggledTextFieldWithButton(string text, string initialTextValue, string buttonName, Action OnButtonClick, bool isChecked = false, Action<string> onTextValueChanged = null)
        {
            textFieldWithAccept = new TextField.TextFieldWithAccept(text, initialTextValue, buttonName, OnButtonClick);
            _toggle = new Toggle("", isChecked, OnStateChanged).SetLabelWidth(1);
            _disabledDrawer = new DisabledDrawer(textFieldWithAccept);

            AddDrawer(_toggle);
            AddDrawer(_disabledDrawer);
            _disabledDrawer.SetEnabled(!_toggle.IsChecked());
        }

        private void OnStateChanged(bool obj)
        {
            _disabledDrawer.SetEnabled(!obj);
        }

        public ToggledTextFieldWithButton SetLabelWidth(int width)
        {
            textFieldWithAccept.SetLabelWidth(width);
            return this;
        }

        public ToggledTextFieldWithButton SetButtonWidth(int width)
        {
            textFieldWithAccept.SetButtonWidth(width);
            return this;
        }
    }
}