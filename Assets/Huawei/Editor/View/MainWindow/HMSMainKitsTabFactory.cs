using HmsPlugin;
using HmsPlugin.HelpBox;
using HmsPlugin.Label;
using HmsPlugin.Toggle;
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
        tabBar.AddTab(tab);

        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new AdsToggleEditor(tabBar), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new RemoteConfigToggleEditor(tabBar), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new IAPToggleEditor(tabBar), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new GameServiceToggleEditor(tabBar), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Toggle("Analytics"), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Toggle("Push"), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Toggle("Account"), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Toggle("Crash"), new Spacer()));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new HelpBox("Please import your agconnect-services.json file to StreamingAssets folder", MessageType.Error));

        return tab;
    }
}
