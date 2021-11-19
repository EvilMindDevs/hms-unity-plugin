using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Button
{
    public class ButtonBase : IDrawer
    {
        protected Delegate _onPress;

        protected string _text;
        protected Texture _image;
        protected int _width = 9999;
        private GUIStyle _style;
        private Color? _bgColor;

        public ButtonBase(string text, Delegate OnPress)
        {
            _text = text;
            _onPress = OnPress;
        }

        public ButtonBase(Texture image, Delegate OnPress)
        {
            _image = image;
            _onPress = OnPress;
        }

        public ButtonBase SetWidth(int width)
        {
            _width = width;
            return this;
        }

        public ButtonBase SetText(string text)
        {
            _text = text;
            return this;
        }

        public ButtonBase SetStyle(GUIStyle style)
        {
            _style = style;
            return this;
        }

        public ButtonBase SetBGColor(Color color)
        {
            _bgColor = color;
            return this;
        }


        public void Draw()
        {
            if (_style == null)
            {
                _style = GUI.skin.button;
            }
            if (_text != null)
            {
                Color currentBgColor = GUI.backgroundColor;
                if (_bgColor.HasValue)
                {
                    GUI.backgroundColor = _bgColor.Value;
                }

                if (GUILayout.Button(_text, _style, GUILayout.MaxWidth(_width)))
                {
                    EditorApplication.delayCall += InvokeCallback;
                }

                if (_bgColor.HasValue)
                {
                    GUI.backgroundColor = currentBgColor;
                }
            }
            else if (_image != null)
            {
                if (GUILayout.Button(_image, _style, GUILayout.MaxWidth(_width)))
                {
                    EditorApplication.delayCall += InvokeCallback;
                }
            }
        }

        protected virtual void InvokeCallback()
        {
            _onPress.DynamicInvoke();
        }
    }

    public class ButtonWithData<T> : ButtonBase
    {
        private T _data;

        public ButtonWithData(string text, Action<T> OnPress, T data) : base(text, OnPress)
        {
            _data = data;
        }

        public ButtonWithData(Texture image, Action<T> OnPress, T data) : base(image, OnPress)
        {
            _data = data;
        }

        protected override void InvokeCallback()
        {
            _onPress.DynamicInvoke(_data);
        }
    }

    public class Button : ButtonBase
    {
        public Button(string text, Action OnPress) : base(text, OnPress)
        {
        }

        public Button(Texture image, Action OnPress) : base(image, OnPress)
        {
        }
    }

    public class ButtonInfo<T> where T : class
    {
        private readonly Action<T> _onButtonPress;
        private readonly string _text;
        private readonly int _width;

        public ButtonInfo(string text, int width, Action<T> onButtonPress)
        {
            _onButtonPress = onButtonPress;
            _text = text;
            _width = width;
        }

        public ButtonBase CreateButton(T data)
        {
            return new ButtonWithData<T>(_text, _onButtonPress, data).SetWidth(_width);
        }
    }
}

