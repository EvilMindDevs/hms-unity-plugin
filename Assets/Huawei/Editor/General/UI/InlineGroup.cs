using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class InlineGroup : IDrawer
    {
        public enum Padding
        {
            MinusHeight,
            None
        }

        private readonly IDrawerWithHeight _drawer;
        private readonly Padding _padding;

        public InlineGroup(IDrawerWithHeight drawer, Padding padding = Padding.None)
        {
            _drawer = drawer;
            _padding = padding;
        }

        public void Draw()
        {
            var area = EditorGUILayout.GetControlRect(true, 0f); // this is a hack to get a rect, specify 0 height to not add extra padding

            float y = area.y;
            if (_padding == Padding.MinusHeight)
            {
                y = area.y - _drawer.GetHeight();
                if (y < 0) y = 0;
            }

            area = new Rect(area.x, y, area.width, _drawer.GetHeight());

            GUI.BeginGroup(area);
            _drawer.Draw();
            GUI.EndGroup();
        }
    }
}