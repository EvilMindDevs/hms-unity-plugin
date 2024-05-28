namespace HmsPlugin
{
    internal class HMSMLKitTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSMLKitSettingsDrawer());


            return tabView;
        }
    }
}

