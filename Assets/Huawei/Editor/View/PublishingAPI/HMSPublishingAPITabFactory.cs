using HmsPlugin;
using HmsPlugin.PublishingAPI;

internal class HMSPublishingAPITabFactory
{
    public static TabView CreateQueryingAppInformationTab(TabBar tabBar)
    {
        var tab = new TabView("Query App Information");
        tabBar.AddTab(tab);
        tab.AddDrawer(new QueryingAppInfoEditor());
        return tab;
    }
}
