using HmsPlugin;
using HmsPlugin.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class HMSAllIAPProductsWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/PMS API/Query IAP Products")]
    public static void ShowProductWindow()
    {
        GetWindow(typeof(HMSAllIAPProductsWindow), false, "Query Products").minSize = new UnityEngine.Vector2(400, 700);
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSPMSAPITabFactory.AllProductsTab(tabBar);
        return tabBar;
    }
}

