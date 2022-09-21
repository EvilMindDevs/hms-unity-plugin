using HmsPlugin;
using HmsPlugin.Button;
using HmsPlugin.Label;
using HmsPlugin.TextField;
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
    public static DisabledDrawer _disabledDrawer;
    private static PluginToggleEditor pluginToggleEditor;

    static HMSMainKitsTabFactory()
    {
        versionInfo = File.ReadAllText(Application.dataPath + "/Huawei/VERSION");
        toggleEditors = new List<ToggleEditor>();
    }

    public static TabView CreateKitsTab(TabBar tabBar)
    {
        toggleEditors.Clear();
        var tab = new TabView("Kits");
        tabBar.AddTab(tab);

        pluginToggleEditor = new PluginToggleEditor(tabBar);
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
        var driveToggleEditor = new DriveKitToggleEditor(accountEditor, pushToggleEditor);
        var nearbyServiceToggleEditor = new NearbyServiceToggleEditor();
        var appMessagingToggleEditor = new AppMessagingToggleEditor();
        var appLinkingToggleEditor = new AppLinkingToggleEditor(analyticsEditor);
        var locationToggleEditor = new LocationToggleEditor();
        var scanToogleEditor = new ScanKitToggleEditor();
        var cloudStorageToggleEditor = new CloudStorageToggleEditor();
        var apmToggleEditor = new APMToggleEditor();

        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), pluginToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(_disabledDrawer = new DisabledDrawer
            (
                new VerticalSequenceDrawer
                (
                    new HorizontalSequenceDrawer(new Spacer(), new Label("- HMS Core -").SetBold(true), new Spacer()),
                    new HorizontalSequenceDrawer(new HorizontalLine()),
                    new HorizontalSequenceDrawer(new Spacer(), accountEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), adsToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), analyticsEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), appLinkingToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), driveToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), gameServiceToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), iapToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), locationToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), nearbyServiceToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), pushToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), scanToogleEditor, new Spacer()),
                    new Spacer(),
                    new HorizontalSequenceDrawer(new HorizontalLine()),
                    new HorizontalSequenceDrawer(new Spacer(), new Label("- AppGallery Connect -").SetBold(true), new Spacer()),
                    new HorizontalSequenceDrawer(new HorizontalLine()),
                    new HorizontalSequenceDrawer(new Spacer(), appMessagingToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), apmToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), authEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), cloudDBToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), cloudStorageToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), crashToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), remoteConfigToggleEditor, new Spacer())
                    
                )
            ));
        //tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new HorizontalSequenceDrawer(new Label("HMS Unity Plugin v" + versionInfo).SetBold(true), new Spacer(), new Clickable(new Label(guiContent: new GUIContent(EditorGUIUtility.FindTexture("_Help"))), () => { Application.OpenURL("https://evilminddevs.gitbook.io/hms-unity-plugin/"); })));
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
        toggleEditors.Add(appLinkingToggleEditor);
        toggleEditors.Add(locationToggleEditor);
        toggleEditors.Add(scanToogleEditor);
        toggleEditors.Add(cloudStorageToggleEditor);
        toggleEditors.Add(apmToggleEditor);

        _disabledDrawer.SetEnabled(!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true));

        return tab;
    }

    public static List<ToggleEditor> GetEnabledEditors()
    {
        return toggleEditors.FindAll(c => c.Enabled);
    }

    public static void RefreshPluginStatus()
    {
        if (pluginToggleEditor != null)
        {
            pluginToggleEditor.RefreshDrawer(HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled));
            pluginToggleEditor.RefreshToggle();
        }
        if (toggleEditors != null && toggleEditors.Count > 0)
            toggleEditors.ForEach(c => c.RefreshToggles());
    }
}