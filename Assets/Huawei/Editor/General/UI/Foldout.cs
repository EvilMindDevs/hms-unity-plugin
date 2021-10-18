using System;
using HmsPlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public interface IFoldout : IDrawer, ISequenceDrawer
    {
        event Action<bool> Toggled; // true if was expanded, false if was collapsed
        IFoldout SetOpened(bool opened);
    }

    public class Foldout : VerticalSequenceDrawer, IFoldout
    {
        public event Action<bool> Toggled;

        private string _title;
        private bool _opened = true;

        public Foldout(string title, bool opened = true, params IDrawer[] drawers) : base(drawers)
        {
            _title = title;
            _opened = opened;
        }

        public IFoldout SetOpened(bool opened)
        {
            bool fireEvent = _opened != opened;

            _opened = opened;

            if (fireEvent)
            {
                Toggled.InvokeSafe(_opened);
            }

            return this;
        }

        public override void Draw()
        {
            var state = _opened;

            _opened = EditorGUILayout.Foldout(_opened, _title);

            if (state != _opened)
            {
                Toggled.InvokeSafe(_opened);
            }

            if (!_opened) return;

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(20);
            base.Draw();

            EditorGUILayout.EndHorizontal();
        }
    }
}