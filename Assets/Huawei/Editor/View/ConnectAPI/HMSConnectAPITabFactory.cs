using HmsPlugin.Button;
using HmsPlugin.ConnectAPI;
using System.IO;
using UnityEngine;

namespace HmsPlugin
{
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
            tab.AddDrawer(new Clickable(new Label.Label("HMS Unity Plugin v" + versionInfo).SetBold(true), () => { Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/"); }));
            return tab;
        }
    }
}
