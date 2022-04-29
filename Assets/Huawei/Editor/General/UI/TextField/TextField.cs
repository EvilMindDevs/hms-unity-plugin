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

    public class TextFieldBase : IDrawer, ITextField
    {
        private string _label = null;
        private string _text;
        private string _tooltip;
        private int? labelWidth;
        private int? fieldWidth;
        private int? fieldHeight;

        protected Delegate OnValueChanged;

        public TextFieldBase(string initialValue, Delegate onValueChanged = null)
        {
            _text = initialValue;
            OnValueChanged = onValueChanged;
        }

        public TextFieldBase(string label, string initialValue, Delegate onValueChanged = null) : this(initialValue, onValueChanged)
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

        public void SetTooltip(string tooltip)
        {
            _tooltip = tooltip;
        }

        public void ClearInput()
        {
            _text = "";
            
            if(OnValueChanged != null)
                OnValueChanged.DynamicInvoke(_text);
        }

        public TextFieldBase SetLabelWidth(int width)
        {
            labelWidth = width;
            return this;
        }

        public TextFieldBase SetFieldWidth(int width)
        {
            fieldWidth = width;
            return this;
        }

        public TextFieldBase SetFieldHeight(int height)
        {
            fieldHeight = height;
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

            if (fieldWidth.HasValue)
            {
                rect.width = fieldWidth.Value;
            }

            if (fieldHeight.HasValue)
            {
                rect.height = fieldHeight.Value;
            }

            if (_label != null)
            {
                _text = EditorGUI.TextField(rect, new GUIContent(_label, _tooltip), _text);
            }
            else
            {
                _text = EditorGUI.TextField(rect, _text);
            }

            EditorGUIUtility.labelWidth = labelWidthTmp;

            if (_text != tmpText)
            {
                InvokeCallback(_text);
            }
        }

        protected virtual void InvokeCallback(string text)
        {
            OnValueChanged?.DynamicInvoke(text);
        }

    }

    public class TextField : TextFieldBase
    {
        public TextField(string label, string initialValue, Action<string> onValueChanged = null) : base(label, initialValue, onValueChanged)
        {

        }

        public TextField(string initialValue, Action<string> onValueChanged = null) : base(initialValue, onValueChanged)
        {

        }

        public TextField SetdWidth(int width)
        {
            SetFieldWidth(width);
            return this;
        }

        public TextField SetHeight(int height)
        {
            SetFieldHeight(height);
            return this;
        }

    }

    public class TextFieldWithData<T> : TextFieldBase
    {
        private T _data;

        public TextFieldWithData(string label, string initialValue, Action<T, string> onValueChanged, T data) : base(label, initialValue, onValueChanged)
        {
            _data = data;
        }

        public TextFieldWithData(string initialValue, Action<T, string> onValueChanged, T data) : base(initialValue, onValueChanged)
        {
            _data = data;
        }

        protected override void InvokeCallback(string text)
        {
            OnValueChanged.DynamicInvoke(_data, text);
        }
    }
}