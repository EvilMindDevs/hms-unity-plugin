using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class HorizontalLine : IDrawer
    {
        public void Draw()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}