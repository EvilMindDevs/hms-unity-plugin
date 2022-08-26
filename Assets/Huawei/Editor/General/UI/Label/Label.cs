using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Label
{
    public class Label : IDrawer
    {
        private string _text;
        private string _tooltip;
        private GUIContent _guiContent;

        private GUIStyle _style;
        private FontStyle? _fontStyle;
        private bool _richTextEnabled = false;
        private bool _stretchWidth = false;
        private int? _fontSize;

        public Label(string text = null, string tooltip = null, GUIContent guiContent = null)
        {
            _text = text;
            _tooltip = tooltip;
            _guiContent = guiContent;
        }

        public Label()
        {
        }


        public virtual void Draw()
        {
            if (_text != null || _guiContent != null)
            {
                Draw(_text, _tooltip,_guiContent);
            }
        }

        public Label SetText(string text)
        {
            _text = text;
            return this;
        }

        public string GetText()
        {
            return _text;
        }

        public Label SetBold(bool bold)
        {
            SetFontStyle(bold ? FontStyle.Bold : FontStyle.Normal);
            return this;
        }

        public Label SetStretchWidth(bool status)
        {
            if (_style != null)
            {
                _style.stretchWidth = status;
                _stretchWidth = status;
            }
            return this;
        }

        public Label SetStretchHeight(bool status)
        {
            if (_style != null)
            {
                _style.stretchHeight = status;
            }
            return this;
        }

        public Label SetFontStyle(FontStyle fontStyle)
        {
            _fontStyle = fontStyle;
            if (_style != null)
            {
                _style.fontStyle = _fontStyle.Value;
            }

            return this;
        }

        public Label SetFontSize(int size)
        {
            _fontSize = size;
            return this;
        }

        public Label SetStyle(GUIStyle style)
        {
            _style = style;
            return this;
        }

        public Label EnableRichText()
        {
            _richTextEnabled = true;
            return this;
        }

        protected void Draw(string text, string tooltip, GUIContent guiContent)
        {
            if (_style == null)
            {
                _style = new GUIStyle(EditorStyles.label);
                if (_fontStyle != null)
                {
                    _style.fontStyle = _fontStyle.Value;
                }
            }

            _style.stretchWidth = _stretchWidth;
            _style.richText = _richTextEnabled;
            if (_fontSize.HasValue)
                _style.fontSize = _fontSize.Value;

            if (guiContent != null) 
            {
                GUILayout.Label(guiContent, _style);
            }
            else if (string.IsNullOrEmpty(tooltip))
            {
                GUILayout.Label(new GUIContent(text), _style);
            }
            else
            {
                GUILayout.Label(new GUIContent(text, tooltip), _style);
            }
        }
    }
}