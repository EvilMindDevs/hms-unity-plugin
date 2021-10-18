using UnityEngine;

namespace HmsPlugin
{
    // Change the depth while drawing the passed in drawer
    // Pass in negative to be on top
    public class Depth : IDrawer
    {
        private readonly int _delta;
        private readonly IDrawer _drawer;

        public Depth(int delta, IDrawer drawer)
        {
            _delta = delta;
            _drawer = drawer;
        }

        public void Draw()
        {
            GUI.depth += _delta;

            _drawer.Draw();

            GUI.depth -= _delta;
        }
    }
}