namespace HmsPlugin
{
    internal class HMSRemoteConfigTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSRemoteConfigSettingsDrawer());


            return tabView;
        }
    }
}
