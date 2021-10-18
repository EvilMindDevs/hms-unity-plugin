using UnityEngine;

namespace HmsPlugin
{
    public class ScrollView : VerticalSequenceDrawer
    {
        private Vector2 _position = Vector2.zero;

        public override void Draw()
        {
            _position = GUILayout.BeginScrollView(_position, false, false);
            base.Draw();
            GUILayout.EndScrollView();
        }
    }
}