using HmsPlugin.Collections;
using HmsPlugin.Extensions;
using System;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public interface IDrawerWithHeight : IDrawer
    {
        float GetHeight();
    }

    public class Menu : IDrawerWithHeight
    {
        private readonly IReadOnlyList<string> _options;
        public event Action<int> OptionSelected;

        public Menu(IReadOnlyList<string> options)
        {
            Debug.Assert(options != null);
            _options = options;
        }


        public void Draw()
        {
            var area = EditorGUILayout.GetControlRect(true, 0f); // this is a hack to get a rect, specify 0 height to not add extra padding
            
            for (var index = 0; index < _options.Count; index++)
            {
                var line = new Rect(0, index * SingleHeight(), area.width - 2, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(line, _options[index], EditorStyles.toolbarButton))
                {
                    var selectedIndex = index;
                    EditorApplication.delayCall += () =>
                    {
                        OptionSelected.InvokeSafe(selectedIndex);
                    };
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight * _options.Count + EditorGUIUtility.standardVerticalSpacing * 2);
            
        }

        private static float SingleHeight()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public float GetHeight()
        {
            return _options.Count * SingleHeight();
        }
    }
}