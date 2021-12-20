using HmsPlugin.Extensions;
using System;
using UnityEditor;

namespace HmsPlugin.Dropdown
{
    public class EnumDropdown : IDropdown<Enum>, IDrawer
    {
        private Enum _value;
        private readonly string _text;
        public event Action<Enum> OnChangedSelection;

        public EnumDropdown(Enum initialValue, string text = "")
        {
            _value = initialValue;
            _text = text;
        }

        public Enum GetCurrentValue()
        {
            return _value;
        }

        public void Draw()
        {
            var prevValue = _value;

            _value = EditorGUILayout.EnumPopup(_text, _value);

            if (!Equals(prevValue, _value))
            {
                OnChangedSelection.InvokeSafe(_value);
            }
        }
    }
}
