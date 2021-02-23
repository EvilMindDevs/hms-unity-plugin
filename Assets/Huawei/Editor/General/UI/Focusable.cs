using HmsPlugin.TextField;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class Focusable : IDrawer
    {
        private readonly IDrawer _drawerToFocus;
        private static int _uniqueId = 0;
        private readonly string _controlName;

        private bool _focus;

        public Focusable(IDrawer drawerToFocus, bool focus = true)
        {
            _drawerToFocus = drawerToFocus;
            _focus = focus;

            _controlName = string.Format("Focusable_" + _uniqueId);
            _uniqueId++;
        }

        public Focusable Focus()
        {
            _focus = true;
            return this;
        }

        public void Draw()
        {
            if (!_focus)
            {
                _drawerToFocus.Draw();
                return;
            }

            _focus = false;

            GUI.SetNextControlName(_controlName);
            _drawerToFocus.Draw();

            if (_drawerToFocus is ITextField)
            {
                EditorGUI.FocusTextInControl(_controlName);
            }
            else
            {
                GUI.FocusControl(_controlName);
            }
        }
    }
}