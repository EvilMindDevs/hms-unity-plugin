using System;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    // Draws the passed class when triggered
    public class TimedDrawer : IDrawer
    {
        private IDrawer _drawer;
        private double _finishTime = Double.MinValue;

        public TimedDrawer(IDrawer drawer)
        {
            _drawer = drawer;
        }

        public void Trigger(float seconds)
        {
            _finishTime = CurrentTime() + seconds;
        }

        double CurrentTime()
        {
            return EditorApplication.timeSinceStartup;
        }

        public void Draw()
        {
            if (CurrentTime() <= _finishTime)
            {
                _drawer.Draw();
            }
        }
    }
}