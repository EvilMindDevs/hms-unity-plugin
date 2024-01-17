namespace HmsPlugin
{
    internal class HMSIAPTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSIAPSettingsDrawer());


            return tabView;
        }
    }
}
