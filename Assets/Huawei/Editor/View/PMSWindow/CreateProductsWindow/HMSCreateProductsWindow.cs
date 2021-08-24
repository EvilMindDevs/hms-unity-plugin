using HmsPlugin;
using HmsPlugin.Button;
using HmsPlugin.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class HMSCreateProductsWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/PMS API/Create Products")]
    public static void ShowProductWindow()
    {
        //TODO: set minimum size see example in HMSCreateAProductWindow
        GetWindow(typeof(HMSCreateProductsWindow), false, "Create Products");
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSPMSAPITabFactory.CreateProductsTab(tabBar);
        return tabBar;
    }
}
