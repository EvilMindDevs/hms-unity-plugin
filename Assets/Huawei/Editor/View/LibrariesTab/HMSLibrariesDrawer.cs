namespace HmsPlugin
{
    public class HMSLibrariesDrawer : VerticalSequenceDrawer
    {
        private TabBar _tabBar;
        private TabView _tabView;
        private Toggle.Toggle _appSupportToggle;
        private Toggle.Toggle _hmsCoreInstallToggle;

        public const string AppCompatEnabled = "AppCompat";
        public const string GooglePlayReleaseEnabled = "GooglePlayRelease";

        public HMSLibrariesDrawer(TabBar tabBar)
        {
            _tabBar = tabBar;
            if (!HMSMainEditorSettings.Instance.Settings.HasKey(AppCompatEnabled)) 
                HMSMainEditorSettings.Instance.Settings.SetBool(AppCompatEnabled, true);
            if (!HMSMainEditorSettings.Instance.Settings.HasKey(GooglePlayReleaseEnabled)) 
                HMSMainEditorSettings.Instance.Settings.SetBool(GooglePlayReleaseEnabled, true);
            _appSupportToggle = new Toggle.Toggle("App Compat (com.android.support:appcompat-v7:21.0.0)", HMSMainEditorSettings.Instance.Settings.GetBool(AppCompatEnabled, true), OnAppSupportToggleChanged, false).SetLabelWidth(350);
            _hmsCoreInstallToggle = new Toggle.Toggle("HMS Core Installer (com.huawei.hms:hmscoreinstaller:6.6.0.300)", HMSMainEditorSettings.Instance.Settings.GetBool(GooglePlayReleaseEnabled, true), OnGooglePlayReleaseEnabledChanged, false).SetLabelWidth(375);
        }
        

        private void OnAppSupportToggleChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(AppCompatEnabled, value);
        }

        
        private void OnGooglePlayReleaseEnabledChanged(bool value)
        {
            HMSMainEditorSettings.Instance.Settings.SetBool(GooglePlayReleaseEnabled, value);
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
            AddDrawer(new Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Google Play Release").SetBold(true), new HorizontalLine()));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), _hmsCoreInstallToggle, new Spacer()));
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(5));



        }
    }
}
