namespace HmsPlugin
{
    public class Modeling3dKitToggleEditor : ToggleEditor, IDrawer
    {
        public const string Modeling3dkitEnabled = "Modeling3dkit";
        private TabBar _tabBar;
        private TabView _tabView;

        public Modeling3dKitToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(Modeling3dkitEnabled);
            _tabView = ModelingTabFactory.CreateTab("3D Modeling Kit");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("3D Modeling Kit (beta)", enabled, OnStateChanged, true);
            Enabled = enabled;
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                EnableToggle();
            }
            else
            {
                DisableToggle();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(Modeling3dkitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            if (_tabBar != null && _tabView != null)
                _tabBar.AddTab(_tabView);
            Enabled = true;
        }

        public override void DisableToggle()
        {
            if (_tabBar != null && _tabView != null)
                _tabBar.RemoveTab(_tabView);
            Enabled = false;
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(Modeling3dkitEnabled));
            }
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            if (removeTabs)
            {
                if (_tabBar != null && _tabView != null)
                    _tabBar.RemoveTab(_tabView);
            }
            else
            {
                if (_tabBar != null && _tabView != null)
                    _tabBar.AddTab(_tabView);
            }
        }
    }
}
