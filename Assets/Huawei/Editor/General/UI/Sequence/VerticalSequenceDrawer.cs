using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class VerticalSequenceDrawer : SequenceDrawer
    {
        private GUIStyle _style = GUIStyle.none;

        public VerticalSequenceDrawer(params IDrawer[] drawers) : base(drawers)
        {
        }

        public VerticalSequenceDrawer SetStyle(GUIStyle style)
        {
            _style = style;
            return this;
        }


        public override void Draw()
        {
            EditorGUILayout.BeginVertical(_style);
            base.Draw();
            EditorGUILayout.EndVertical();
        }
    }
}