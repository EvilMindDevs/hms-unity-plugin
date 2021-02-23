using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin.Window
{
    public abstract class HMSEditorWindow : EditorWindow
    {
        [NonSerialized] private List<IDrawer> _drawers = new List<IDrawer>(); // [NonSerialized] is not required, but added to guarantee the behaviour doesn't change
        [NonSerialized] private bool _triedCreating; // [NonSerialized] is a hack to avoid having _drawers not serialized and _triedCreating which leads to having 0 drawers
        // maybe later we find a way to not lose a reference to _drawers instead. (may not be possible when script recompilation happens)

        private void Create()
        {
            if (_triedCreating) return; // need to fix reference losing when play mode is pressed (tab index is reset)

            try
            {
                var createdDrawer = CreateDrawer();
                _drawers.Add(createdDrawer);
                //EditorUtility.SetDirty(_drawers);

            }
            finally
            {
                _triedCreating = true;
            }
        }

        public abstract IDrawer CreateDrawer();

        private void OnEnable()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        void OnGUI()
        {
            // Create inside OnGUI to have access to all GUIStyles, etc
            if (!_triedCreating)
            {
                Create();
            }

            _drawers.ForEach(d => d.Draw());
        }

        void OnInspectorUpdate()
        {
            // call here to make it update at 10fps (OnGUI is called 60 or more times)
            Repaint();
            // Alternative way to call only on button presses something like
            // GUI.changed = true;
            // GUI.FocusControl(""); // force update
        }
    }
}