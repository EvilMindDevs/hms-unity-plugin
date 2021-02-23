using UnityEngine;
using UnityEditor;

namespace HmsPlugin.List
{
    public class AlteratingListDrawer : SequenceDrawer
    {
        private ListOrientation _orientation;
        private GUIStyle gsAlterStyle;

        public AlteratingListDrawer(ListOrientation orientation)
        {
            _orientation = orientation;

            gsAlterStyle = new GUIStyle();
            gsAlterStyle.normal.background = MakeTex(600, 1, GetAlternativeColor());
        }

        private void BeginLayout(GUIStyle style = null)
        {
            if (_orientation == ListOrientation.Horizontal)
            {
                if (style != null) GUILayout.BeginVertical(style);
                else GUILayout.BeginVertical();
            }
            else
            {
                if (style != null) GUILayout.BeginHorizontal(style);
                else GUILayout.BeginHorizontal();
            }
        }

        private void EndLayout()
        {
            if (_orientation == ListOrientation.Horizontal)
            {
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.EndHorizontal();
            }
        }

        private Color GetAlternativeColor()
        {
            if (EditorGUIUtility.isProSkin)
            {
                return new Color(1.0f, 1.0f, 1.0f, 0.03f);
            }
            else
            {
                return new Color(0f, 0f, 0f, 0.05f);
            }
        }

        public override void Draw()
        {
            for (var i = 0; i < _drawers.Count; ++i)
            {

                if (i % 2 == 0)
                {
                    BeginLayout();

                }
                else
                {
                    BeginLayout(gsAlterStyle);

                }

                _drawers[i].Draw();

                EndLayout();
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}