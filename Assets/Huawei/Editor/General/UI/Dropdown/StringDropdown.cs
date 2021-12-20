using HmsPlugin.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Dropdown
{
    public class StringDropdown : IDrawer, IDropdown<int>
    {
        public event Action<int> OnChangedSelection;

        private string[] _options;
        private readonly string _text;

        private int _index;

        public StringDropdown(string[] options, int initialIndex, string text = "", Action<int> onChangedSelection = null)
        {
            _options = options;
            _text = text;
            _index = initialIndex;
            OnChangedSelection = onChangedSelection;
        }

        public int GetCurrentIndex()
        {
            return _index;
        }

        public StringDropdown SetOptions(string[] options)
        {
            _options = options;
            return this;
        }

        public void Draw()
        {
            var prevIndex = _index;

            _index = EditorGUILayout.Popup(_text, _index, _options);

            if (!Equals(prevIndex, _index))
            {
                OnChangedSelection.InvokeSafe(_index);
            }
        }
    }
}