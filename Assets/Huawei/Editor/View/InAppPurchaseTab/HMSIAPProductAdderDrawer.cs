using HmsPlugin.Dropdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSIAPProductAdderDrawer : VerticalSequenceDrawer
    {
        private readonly IIAPProductManipulator _productManipulator;

        private TextField.TextFieldBase _identifierTextField;
        private EnumDropdown _typeDropdown;

        public HMSIAPProductAdderDrawer(IIAPProductManipulator productManipulator)
        {
            _productManipulator = productManipulator;

            _identifierTextField = new TextField.TextField("Identifier: ", "").SetLabelWidth(70);
            _typeDropdown = new EnumDropdown(HMSIAPProductType.Consumable);

            SetupSequence();
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(_identifierTextField, new Space(10), _typeDropdown, new Space(15), new Button.Button("Add", OnAddProduct).SetWidth(100)));
        }

        private void OnAddProduct()
        {
            string identifier = _identifierTextField.GetCurrentText();
            HMSIAPProductType type = (HMSIAPProductType)_typeDropdown.GetCurrentValue();

            _productManipulator.AddProduct(identifier, type);
            _identifierTextField.ClearInput();
        }
    }
}
