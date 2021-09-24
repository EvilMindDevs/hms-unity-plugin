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

    public static TabView  CreatePublishingAPITab(TabBar tabBar)
    {
        var tab = new TabView("Publishing API");
        tabBar.AddTab(tab);

        tab.AddDrawer(new QueryingAppInfoEditor());

        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new Clickable(new Label("HMS Unity Plugin v" + versionInfo).SetBold(true), () => { Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/"); }));
        // TODO: Geri kalan özellikleri ekle şu an sadece bir horizontal line atıyor.
        // TokenObtanierEditorü kontrol et ve gerekenleri siteden bakarak ekle
        return tab;
    }
}
