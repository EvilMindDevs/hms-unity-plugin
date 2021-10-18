using System;

namespace HmsPlugin
{
    // Draws if condition delegate returns true
    public class ConditionalDrawer : IDrawer
    {
        private readonly IDrawer _drawer;
        private readonly Func<bool> _shouldDraw;

        public ConditionalDrawer(IDrawer drawer, Func<bool> shouldDraw)
        {
            _drawer = drawer;
            _shouldDraw = shouldDraw;
        }

        public void Draw()
        {
            if (_shouldDraw.Invoke())
            {
                _drawer.Draw();
            }
        }
    }
}