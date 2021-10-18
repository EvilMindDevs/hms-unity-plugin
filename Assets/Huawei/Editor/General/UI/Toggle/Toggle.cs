using System;
using HmsPlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Toggle
{
    public class Toggle : IDrawer
    {
        private bool _checked;
        private string _text;
        private Action<bool> _onStateChanged;
        private string _tooltip = string.Empty; // string.Empty is the disabled state of the tooltip
        private int? labelWidth;

        public Toggle(string text, bool isChecked = false, Action<bool> onStateChanged = null, bool invokeOnStart = false)
        {
            _text = text;
            _checked = isChecked;
            _onStateChanged = onStateChanged;
            if (invokeOnStart)
                _onStateChanged.InvokeSafe(_checked);
        }

        public Toggle SetTooltip(string tooltipMessage)
        {
            _tooltip = tooltipMessage;
            return this;
        }

        public bool IsChecked()
        {
            return _checked;
        }

        public Toggle SetChecked(bool isChecked)
        {
            _checked = isChecked;
            return this;
        }

        public Toggle SetLabelWidth(int width)
        {
            labelWidth = width;
            return this;
        }

        public void Draw()
        {
            var state = _checked;

            var labelWidthTmp = EditorGUIUtility.labelWidth;
            if (labelWidth.HasValue)
            {
                EditorGUIUtility.labelWidth = labelWidth.Value;
            }

            _checked = EditorGUILayout.Toggle(new GUIContent(_text, _tooltip), _checked, GUILayout.ExpandWidth(false));

            EditorGUIUtility.labelWidth = labelWidthTmp;

            if (state != _checked)
            {
                _onStateChanged.InvokeSafe(_checked);
            }
        }
    }
}