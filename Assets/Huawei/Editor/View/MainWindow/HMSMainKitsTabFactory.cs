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

    private static List<ToggleEditor> toggleEditors;

    static HMSMainKitsTabFactory()
    {
        versionInfo = File.ReadAllText(Application.dataPath + "/Huawei/VERSION");
        toggleEditors = new List<ToggleEditor>();
    }

    public static TabView CreateTab(TabBar tabBar)
    {
        toggleEditors.Clear();
        var tab = new TabView("Kits");
        tabBar.AddTab(tab);

        var adsToggleEditor = new AdsToggleEditor(tabBar);
        var accountEditor = new AccountToggleEditor();
        var gameServiceToggleEditor = new GameServiceToggleEditor(tabBar, accountEditor);
        var pushToggleEditor = new PushToggleEditor();
        var analyticsEditor = new AnalyticsToggleEditor();
        var authEditor = new AuthToggleEditor();
        var iapToggleEditor = new IAPToggleEditor(tabBar);
        var remoteConfigToggleEditor = new RemoteConfigToggleEditor(tabBar, analyticsEditor);
        var crashToggleEditor = new CrashToggleEditor(analyticsEditor);
        var cloudDBToggleEditor = new CloudDBToggleEditor(tabBar, authEditor);
        var driveToggleEditor = new DriveKitToggleEditor();
        var nearbyServiceToggleEditor = new NearbyServiceToggleEditor();
        var appMessagingToggleEditor = new AppMessagingToggleEditor();

        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), adsToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), gameServiceToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), pushToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), iapToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), accountEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), analyticsEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), remoteConfigToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), crashToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), authEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), cloudDBToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), driveToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), nearbyServiceToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), appMessagingToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new Clickable(new Label("HMS Unity Plugin v" + versionInfo).SetBold(true), () => { Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/"); }));
        tab.AddDrawer(new HelpboxAGConnectFile());

        toggleEditors.Add(adsToggleEditor);
        toggleEditors.Add(accountEditor);
        toggleEditors.Add(gameServiceToggleEditor);
        toggleEditors.Add(pushToggleEditor);
        toggleEditors.Add(analyticsEditor);
        toggleEditors.Add(authEditor);
        toggleEditors.Add(iapToggleEditor);
        toggleEditors.Add(remoteConfigToggleEditor);
        toggleEditors.Add(crashToggleEditor);
        toggleEditors.Add(cloudDBToggleEditor);
        toggleEditors.Add(driveToggleEditor);
        toggleEditors.Add(nearbyServiceToggleEditor);
        toggleEditors.Add(appMessagingToggleEditor);

        return tab;
    }

    public static List<ToggleEditor> GetEnabledEditors()
    {
        return toggleEditors.FindAll(c => c.Enabled);
    }
}