using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class TabBar : IDrawer
    {
        private List<TabView> _tabs = new List<TabView>();
        private int selectedTab = 0;
        private string[] tabNames; // used for caching as tabs count doesn't change after creation

        public void AddTab(TabView tab)
        {
            if (_tabs != null && _tabs.Find(c => c.Title == tab.Title) != null)
                return;
            
            _tabs.Add(tab);
            RefreshTabNames();
        }

        public void RemoveTab(TabView tab)
        {
            if (_tabs.Remove(tab))
                RefreshTabNames();
        }

        private void RefreshTabNames()
        {
            tabNames = new string[_tabs.Count];
            for (int i = 0; i < tabNames.Length; ++i)
            {
                tabNames[i] = _tabs[i].Title;
            }
        }

        public void Draw()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            _tabs[selectedTab].Draw();
        }
    }
}