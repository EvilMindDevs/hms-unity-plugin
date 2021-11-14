using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Image
{
    public class SpriteImage : IDrawer
    {
        private string _label;
        private string _tooltip = null;
        private Sprite _sprite = null;
        public event Action<Sprite> OnValueChanged;

        public SpriteImage(Sprite initialValue, string label, Action<Sprite> onValueChanged = null)
        {
            _sprite = initialValue;
            _label = label;
            OnValueChanged += onValueChanged;
        }

        public SpriteImage SetTooltip(string tooltip)
        {
            _tooltip = tooltip;
            return this;
        }

        public void Draw()
        {
            Rect rect = EditorGUILayout.GetControlRect();

            var tmpImage = _sprite;

            if (_tooltip != null)
            {
                _sprite = (Sprite)EditorGUI.ObjectField(rect, new GUIContent(_label, _tooltip), _sprite, typeof(Sprite), false);
            }

            else
            {
                _sprite = (Sprite)EditorGUI.ObjectField(rect, _label, _sprite, typeof(Sprite), false);
            }

            if (_sprite != tmpImage)
            {
                OnValueChanged.InvokeSafe(_sprite);
            }
        }
    }
}
