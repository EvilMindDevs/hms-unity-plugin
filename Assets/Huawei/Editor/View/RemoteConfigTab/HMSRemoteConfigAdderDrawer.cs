using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HmsPlugin.Button;
using HmsPlugin.TextField;

namespace HmsPlugin
{
    public class HMSRemoteConfigAdderDrawer : VerticalSequenceDrawer
    {
        private readonly IDefaultValueManipulator _defaultValueManipulator;
        private TextField.TextFieldBase _keyNameTextField;
        private TextField.TextFieldBase _keyValueTextField;

        public HMSRemoteConfigAdderDrawer(IDefaultValueManipulator defaultValueManipulator)
        {
            _defaultValueManipulator = defaultValueManipulator;
            _keyNameTextField = new TextField.TextField("Key Name:", "").SetLabelWidth(70);
            _keyValueTextField = new TextField.TextField("Key Value:", "").SetLabelWidth(70);

            SetupSequence();
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(_keyNameTextField, new Space(10), _keyValueTextField, new Space(15), new Button.Button("Add", OnAddDefaultValuePressed).SetWidth(100)));
        }

        private void OnAddDefaultValuePressed()
        {
            var keyName = _keyNameTextField.GetCurrentText();
            var keyValue = _keyValueTextField.GetCurrentText();

            _defaultValueManipulator.AddDefaultValue(keyName, keyValue);
            _keyNameTextField.ClearInput();
            _keyValueTextField.ClearInput();
        }
    }
}
