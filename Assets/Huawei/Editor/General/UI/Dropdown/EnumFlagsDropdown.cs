using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace HmsPlugin.Dropdown
{
    public class EnumFlagsDropdown : IDrawer
    {
        private Enum _value;
        private readonly string _text;

        public EnumFlagsDropdown(Enum initialValue, string text = "")
        {
            _value = initialValue;
            _text = text;
        }

        public Enum GetCurrentValue() => _value;

        public void Draw()
        {
            _value = EditorGUILayout.EnumFlagsField(label: _text, _value);
        }
    }
}
