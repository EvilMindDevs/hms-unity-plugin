using HmsPlugin;
using HmsPlugin.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class HMSConnectAPIWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/Token Obtainer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HMSConnectAPIWindow), false, "HMS Connect API");
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSConnectAPITabFactory.CreateConnectAPITab(tabBar);
        return tabBar;
    }
}

