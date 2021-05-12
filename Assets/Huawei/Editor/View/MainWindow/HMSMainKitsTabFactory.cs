using HmsPlugin;
using HmsPlugin.Button;
using HmsPlugin.HelpBox;
using HmsPlugin.Label;
using HmsPlugin.Toggle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

internal class HMSMainKitsTabFactory
{
    private static string versionInfo = "";
    static HMSMainKitsTabFactory()
    {
        versionInfo = File.ReadAllText(Application.dataPath + "/Huawei/VERSION");
    }

    public static TabView CreateTab(TabBar tabBar)
    {
        var tab = new TabView("Kits");
        tabBar.AddTab(tab);

        var analyticsEditor = new AnalyticsToggleEditor();
        var authEditor = new AuthToggleEditor();
        var accountEditor = new AccountToggleEditor();

        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new AdsToggleEditor(tabBar), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new GameServiceToggleEditor(tabBar, accountEditor), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new PushToggleEditor(), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new IAPToggleEditor(tabBar), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), accountEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), analyticsEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new RemoteConfigToggleEditor(tabBar, analyticsEditor), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new CrashToggleEditor(analyticsEditor), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), authEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new CloudDBToggleEditor(authEditor), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new DriveKitToggleEditor(), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new NearbyServiceToggleEditor(), new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new AppMessagingToggleEditor(), new Spacer()));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new Clickable(new Label("HMS Unity Plugin v" + versionInfo).SetBold(true), () => { Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/tree/2.0"); }));
        tab.AddDrawer(new HelpboxAGConnectFile());

        return tab;
    }
}