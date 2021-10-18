using System;
using System.Collections.Generic;
using HmsPlugin.Collections;
using HmsPlugin.Extensions;

namespace HmsPlugin
{
    public class EditableMenu : IDrawerWithHeight
    {
        private List<string> _options = new List<string>();
        private List<Action> _callbacks = new List<Action>();
        private Menu _menu;

        public EditableMenu()
        {
            _menu = new Menu(_options.ToReadonlyList());
            _menu.OptionSelected += OnOptionSelected;
        }

        private void OnOptionSelected(int index)
        {
            _callbacks[index].InvokeSafe();
        }

        public void Draw()
        {
            _menu.Draw();
        }

        public float GetHeight()
        {
            return _menu.GetHeight();
        }

        public void ClearOptions()
        {
            _options.Clear();
            _callbacks.Clear();
        }

        public void AddOption(string text, Action callback)
        {
            _options.Add(text);
            _callbacks.Add(callback);
        }
    }
}