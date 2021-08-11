using HmsPlugin;
using HmsPlugin.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class HMSPMSAPIWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/PMS API")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HMSPMSAPIWindow), false, "HMS PMS API");
    }


    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
       // HMSConnectAPITabFactory.CreateConnectAPITab(tabBar);
        return tabBar;
    }
}
