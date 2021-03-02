using System;
using HmsPlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.TextField
{
    public interface ITextField
    {
        string GetCurrentText();
        void ClearInput();
        void SetCurrentText(string text);
    }

    public class TextField : IDrawer, ITextField
    {
        private string _label = null;
        private string _text;
        private int? labelWidth;

        public event Action<string> OnValueChanged;

        public TextField(string initialValue, Action<string> onValueChanged = null)
        {
            _text = initialValue;
            OnValueChanged += onValueChanged;
        }

        public TextField(string label, string initialValue, Action<string> onValueChanged = null) : this(initialValue, onValueChanged)
        {
            _label = label;
        }

        public string GetCurrentText()
        {
            return _text;
        }

        public void SetCurrentText(string text)
        {
            _text = text;
        }

        public void ClearInput()
        {
            _text = "";
            OnValueChanged.InvokeSafe(_text);
        }

        public TextField SetLabelWidth(int width)
        {
            labelWidth = width;
            return this;
        }

        public void Draw()
        {
            Rect rect = EditorGUILayout.GetControlRect();

            var tmpText = _text;

            var labelWidthTmp = EditorGUIUtility.labelWidth;
            if (labelWidth.HasValue)
            {
                EditorGUIUtility.labelWidth = labelWidth.Value;
            }

            if (_label != null)
            {
                _text = EditorGUI.TextField(rect, _label, _text);
            }
            else
            {
                _text = EditorGUI.TextField(rect, _text);
            }

            EditorGUIUtility.labelWidth = labelWidthTmp;

            if (_text != tmpText)
            {
                OnValueChanged.InvokeSafe(_text);
            }
        }
    }
}