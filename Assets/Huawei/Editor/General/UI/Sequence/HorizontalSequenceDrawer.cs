using System.Collections.Generic;
using UnityEditor;

namespace HmsPlugin
{
    public class HorizontalSequenceDrawer : SequenceDrawer
    {
        public HorizontalSequenceDrawer(params IDrawer[] drawers) : base(drawers)
        {
        }

        public override void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            base.Draw();
            EditorGUILayout.EndHorizontal();
        }
    }
}