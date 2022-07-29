using HmsPlugin;
using HmsPlugin.Button;
using HmsPlugin.Window;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

public class HMSCreateAProductWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/PMS API/Create a Product")]
    public static void ShowProductWindow()
    {
        GetWindow(typeof(HMSCreateAProductWindow), false, "Create a Product").minSize = new UnityEngine.Vector2(400, 700);
    }

    // Validate the menu item defined by the function above.
    // The menu item will be disabled if this function returns false.
    [MenuItem("Huawei/Connect API/PMS API/Create a Product", true)]
    static bool ValidatePMSAPIWindow()
    {
        Debug.Log("ValidatePMSAPIWindow");
        return !string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken, ""));
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSPMSAPITabFactory.CreateProductTab(tabBar);
        return tabBar;
    }
}

