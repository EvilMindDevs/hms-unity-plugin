using System;
using UnityEditor;

namespace HmsPlugin
{
    public class PersistentFoldout : IFoldout
    {
        public event Action<bool> Toggled
        {
            add { _foldout.Toggled += value; }
            remove { _foldout.Toggled -= value; }
        }

        private readonly IFoldout _foldout;
        private readonly string _uniquePrefsKey;
        private readonly bool _openedByDefault;

        public PersistentFoldout(IFoldout foldout, string uniquePrefsKey, bool openedByDefault = true)
        {
            _foldout = foldout;
            _uniquePrefsKey = uniquePrefsKey;
            _openedByDefault = openedByDefault;

            var shouldBeOpened = Load(_openedByDefault);
            _foldout.SetOpened(shouldBeOpened);

            _foldout.Toggled += OnToggled; // subscribe after SetOpened to avoid getting my own event
        }

        private void OnToggled(bool opened)
        {
            Save(opened);
        }

        public IFoldout SetOpened(bool opened)
        {
            _foldout.SetOpened(opened);
            Save(opened);
            return this;
        }

        public void Draw()
        {
            _foldout.Draw();
        }

        private void Save(bool state)
        {
            EditorPrefs.SetBool(_uniquePrefsKey, state);
        }

        private bool Load(bool defaultValue)
        {
            return EditorPrefs.GetBool(_uniquePrefsKey, defaultValue);
        }

        public void AddDrawer(IDrawer drawer)
        {
            _foldout.AddDrawer(drawer);
        }

        public void RemoveDrawer(IDrawer drawer)
        {
            _foldout.RemoveDrawer(drawer);
        }

        public int Count()
        {
            return _foldout.Count();
        }
    }
}