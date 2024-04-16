namespace HmsPlugin
{
    public class HMSCloudDBTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSCloudDBSettingsDrawer());

            return tabView;
        }
    }
}
