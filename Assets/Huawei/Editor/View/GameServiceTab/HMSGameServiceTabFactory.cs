namespace HmsPlugin
{
    public class HMSGameServiceTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSGameServiceSettingsDrawer());

            return tabView;
        }
    }
}
