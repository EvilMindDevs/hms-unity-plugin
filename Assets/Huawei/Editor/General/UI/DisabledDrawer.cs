using UnityEditor;

namespace HmsPlugin
{
    public class DisabledDrawer : IDrawer
    {
        private bool _enabled = true;
        private readonly IDrawer _drawer;
        public DisabledDrawer(IDrawer drawer)
        {
            _drawer = drawer;
        }

        public DisabledDrawer SetEnabled(bool enabled)
        {
            _enabled = enabled;
            return this;
        }

        public void Draw()
        {
            EditorGUI.BeginDisabledGroup(_enabled);
            _drawer.Draw();
            EditorGUI.EndDisabledGroup();
        }
    }

    public class DisabledDrawers : IDrawer
    {
        private bool _enabled = true;
        private readonly IDrawer[] _drawers;
        public DisabledDrawers(params IDrawer[] drawers)
        {
            _drawers = drawers;
        }

        public DisabledDrawers SetEnabled(bool enabled)
        {
            _enabled = enabled;
            return this;
        }

        public void Draw()
        {
            EditorGUI.BeginDisabledGroup(_enabled);
            for (int i = 0; i < _drawers.Length; i++)
            {
                _drawers[i].Draw();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}