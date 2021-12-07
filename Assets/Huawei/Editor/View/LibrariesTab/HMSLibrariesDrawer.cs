using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSLibrariesDrawer : VerticalSequenceDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;
        private Toggle.Toggle _appSupportToggle;

        public const string AppCompatEnabled = "AppCompat";

        public HMSLibrariesDrawer(TabBar tabBar)
        {
            _tabBar = tabBar;
            if (!HMSMainEditorSettings.Instance.Settings.HasKey(AppCompatEnabled)) 
                HMSMainEditorSettings.Instance.Settings.SetBool(AppCompatEnabled, true);
            _appSupportToggle = new Toggle.Toggle("App Compat (com.android.support:appcompat-v7:21.0.0)", HMSMainEditorSettings.Instance.Settings.GetBool(AppCompatEnabled, true), OnAppSupportToggleChanged, false).SetLabelWidth(350);
        }

        private void OnAppSupportToggleChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(AppCompatEnabled, value);
        }

        public void CreateDrawer()
        {
            _tabView = new TabView("Common Libraries");
            _tabView.AddDrawer(this);
            _tabBar.AddTab(_tabView);
            SetupSequence();
        }

        public void DestroyDrawer()
        {
            RemoveAllDrawers();
            _tabBar.RemoveTab(_tabView);
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Common Libraries").SetBold(true), new HorizontalLine()));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), _appSupportToggle, new Spacer()));
            AddDrawer(new HorizontalLine());
        }
    }
}
