using HmsPlugin.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Dropdown
{
    public class StringDropdown : IDrawer, IDropdown
    {
        public event Action OnChangedSelection;

        private readonly string[] _options;
        private readonly string _text;

        private int _index;

        public StringDropdown(string[] options, int initialIndex, string text = "")
        {
            _options = options;
            _text = text;
            _index = initialIndex;

            Debug.Assert(options.Length > 0);
            Debug.Assert(initialIndex < options.Length);
        }

        public int GetCurrentIndex()
        {
            return _index;
        }

        public void Draw()
        {
            var prevIndex = _index;

            _index = EditorGUILayout.Popup(_text, _index, _options);

            if (!Equals(prevIndex, _index))
            {
                OnChangedSelection.InvokeSafe();
            }
        }
    }
}