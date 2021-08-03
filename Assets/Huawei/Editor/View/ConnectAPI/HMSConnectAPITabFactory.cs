using HmsPlugin;
using HmsPlugin.Button;
using HmsPlugin.ConnectAPI;
using HmsPlugin.Label;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class HMSConnectAPITabFactory
{
    private static string versionInfo = "";

    static HMSConnectAPITabFactory()
    {
        versionInfo = File.ReadAllText(Application.dataPath + "/Huawei/VERSION");
    }

    public static TabView CreateConnectAPITab(TabBar tabBar)
    {
        var tab = new TabView("Connect API");
        tabBar.AddTab(tab);

        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new TokenObtainerEditor());
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new Clickable(new Label("HMS Unity Plugin v" + versionInfo).SetBold(true), () => { Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/"); }));
        return tab;
    }
}
