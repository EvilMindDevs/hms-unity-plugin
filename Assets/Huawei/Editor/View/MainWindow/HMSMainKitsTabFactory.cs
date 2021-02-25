using HmsPlugin;
using HmsPlugin.HelpBox;
using HmsPlugin.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

internal class HMSMainKitsTabFactory
{
    public static TabView CreateTab(TabBar tabBar)
    {
        var tab = new TabView("Kits");

        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new AdsToggleEditor(tabBar));
        tab.AddDrawer(new Toggle("IAP"));
        tab.AddDrawer(new Toggle("Game Services"));
        tab.AddDrawer(new Toggle("Remote Config"));
        tab.AddDrawer(new Toggle("Analytics"));
        tab.AddDrawer(new Toggle("Push"));
        tab.AddDrawer(new Toggle("Account"));
        tab.AddDrawer(new Toggle("Crash"));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new HelpBox("Please import your agconnect-services.json file to StreamingAssets folder", MessageType.Error));

        return tab;
    }
}
