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
using HmsPlugin.PublishingAPI;

internal class HMSPublishingAPITabFactory
{
    private static string versionInfo = "";

    static HMSPublishingAPITabFactory()
    {
        versionInfo = File.ReadAllText(Application.dataPath + "/Huawei/VERSION");
    }

    public static TabView CreateQueryingAppInformationTab(TabBar tabBar)
    {
        var tab = new TabView("Query App Information");
        tabBar.AddTab(tab);
        tab.AddDrawer(new QueryingAppInfoEditor());
        return tab;
    }
}
