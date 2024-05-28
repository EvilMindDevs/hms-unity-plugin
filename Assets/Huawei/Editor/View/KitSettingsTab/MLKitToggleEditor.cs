namespace HmsPlugin
{
    public class MLKitToggleEditor : ToggleEditor, IDrawer
    {
        public const string MLKitEnabled = "MLKit";
        private TabBar _tabBar;
        private TabView _tabView;

        public MLKitToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(MLKitEnabled);
            _tabView = HMSMLKitTabFactory.CreateTab("ML Kit");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("ML Kit", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(MLKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void EnableToggle()
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;

            AddTabToBar();
            Enabled = true;
        }

        public override void DisableToggle()
        {
            RemoveTabFromBar();
            Enabled = false;
        }

        public override void RefreshToggles()
        {
            if (_toggle != null)
            {
                _toggle.SetChecked(HMSMainEditorSettings.Instance.Settings.GetBool(MLKitEnabled));
            }
        }

        public override void RemoveToggleTabView(bool removeTabs)
        {
            if (removeTabs)
            {
                RemoveTabFromBar();
            }
            else
            {
                AddTabToBar();
            }
        }

        private void AddTabToBar()
        {
            if (_tabBar != null && _tabView != null)
            {
                _tabBar.AddTab(_tabView);
            }
        }

        private void RemoveTabFromBar()
        {
            if (_tabBar != null && _tabView != null)
            {
                _tabBar.RemoveTab(_tabView);
            }
        }
    }
}
