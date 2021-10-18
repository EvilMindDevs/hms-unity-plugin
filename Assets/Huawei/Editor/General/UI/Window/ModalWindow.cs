using System;
using System.Runtime.CompilerServices;
using HmsPlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Window
{
    public enum WindowResult
    {
        None,
        Ok,
        Cancel,
        Invalid,
        LostFocus
    }

    public class ModalWindow : EditorWindow
    {
        public event Action OnFocusLost;

        protected const float TITLEBAR_HEIGHT = 18;
        private string _title = "";

        private IDrawer _contentDrawer;

        public ModalWindow SetTitle(string newTitle)
        {
            this._title = newTitle;
            return this;
        }

        public ModalWindow SetContent(IDrawer contentDrawer)
        {
            this._contentDrawer = contentDrawer;
            return this;
        }

        protected virtual void OnLostFocus()
        {
            OnFocusLost.InvokeSafe();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label(_title);

            GUILayout.EndHorizontal();

            Rect content = new Rect(0, TITLEBAR_HEIGHT, position.width, position.height - TITLEBAR_HEIGHT);
            Draw(content);
        }

        protected virtual void Draw(Rect region)
        {
            if (_contentDrawer != null)
            {
                _contentDrawer.Draw();
            }
        }

        public static ModalWindow CreateWindow(string title, Vector2 size, IDrawer contentDrawer)
        {
            return Create<ModalWindow>(title, size, contentDrawer);
        }

        protected static T Create<T>(string title, Vector2 size, IDrawer contentDrawer) where T : ModalWindow
        {
            T window = CreateInstance<T>();
            window.SetTitle(title);
            window.SetContent(contentDrawer);

            float x = Screen.width / 2;
            float y = Screen.height / 2;

            float width = size.x;
            float height = size.y;

            Rect rect = new Rect(x, y, 0, 0);
            window.position = rect;
            window.ShowAsDropDown(rect, new Vector2(width, height));

            window.CenterOnMainWin();

            return window;
        }
    }
}