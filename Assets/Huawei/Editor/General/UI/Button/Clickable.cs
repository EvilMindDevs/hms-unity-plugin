using System;
using HmsPlugin.Extensions;
using UnityEngine;

namespace HmsPlugin.Button
{
    // Pass in any element that should be clickable and onClick will be invoked when it is
    // i.e. works with HelpBox, Label
    public class Clickable : IDrawer
    {
        private IDrawer _drawer;
        private Action _OnClick;

        public Clickable(IDrawer drawer, Action onClick)
        {
            _drawer = drawer;
            _OnClick = onClick;
        }


        public void Draw()
        {
            _drawer.Draw();

            if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _OnClick.InvokeSafe();
            }
        }
    }
}