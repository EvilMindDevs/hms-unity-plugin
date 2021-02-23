using System;
using HmsPlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.TextField
{
    public class ReturnChecker : IDrawer
    {
        public event Action ReturnPressed;

        private readonly IDrawer _drawer;
        private readonly string _uniqueControlName;

        public ReturnChecker(IDrawer drawer, string uniqueControlName = null)
        {
            _drawer = drawer;
            if (uniqueControlName == null)
            {
                uniqueControlName = "Control" + GetHashCode();
            }

            _uniqueControlName = uniqueControlName;
        }

        public void Draw()
        {
            GUI.SetNextControlName(_uniqueControlName);

            _drawer.Draw();

            if (GUI.GetNameOfFocusedControl().Equals(_uniqueControlName))
            {
                if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
                {
                    EditorApplication.delayCall += () => ReturnPressed.InvokeSafe();
                }
            }
        }
    }
}