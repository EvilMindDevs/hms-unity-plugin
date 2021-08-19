using HmsPlugin.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.TextArea
{
    public class TextArea : IDrawer, ITextField
    {
        private string _text;
        private int? fieldWidth;
        private int? fieldHeight;

        private bool clearFocus;

        public TextArea(string text)
        {
            _text = text;
        }

        public string GetCurrentText()
        {
            return _text;
        }

        public void SetCurrentText(string text)
        {
            _text = text;
            clearFocus = true;
        }

        public void ClearInput()
        {
            _text = "";
        }

        public TextArea SetFieldWidth(int width)
        {
            fieldWidth = width;
            return this;
        }

        public TextArea SetFieldHeight(int height)
        {
            fieldHeight = height;
            return this;
        }

        public void Draw()
        {
            if (clearFocus)
            {
                GUI.FocusControl(null);
                clearFocus = false;
            }

            Rect rect = EditorGUILayout.GetControlRect();

            if (fieldWidth.HasValue)
                rect.width = fieldWidth.Value;

            if (fieldHeight.HasValue)
                rect.height = fieldHeight.Value;

            EditorGUI.SelectableLabel(rect, _text, style: EditorStyles.textField);
            
        }
    }
}
