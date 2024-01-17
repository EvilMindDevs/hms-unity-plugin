namespace HmsPlugin
{
    internal class HMSAdsTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSAdsSettingsDrawer());


            return tabView;
        }
    }
}
