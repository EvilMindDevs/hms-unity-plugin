using HmsPlugin;
using HmsPlugin.Window;

using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSCreateProductsWindow : HMSEditorWindow
    {
        [MenuItem("Huawei/Connect API/PMS API/Create Products")]
        public static void ShowProductWindow()
        {
            //TODO: set minimum size see example in HMSCreateAProductWindow
            GetWindow(typeof(HMSCreateProductsWindow), false, "Create Products");
        }

        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("Huawei/Connect API/PMS API/Create Products", true)]
        static bool ValidatePMSAPIWindow()
        {
            Debug.Log("ValidatePMSAPIWindow");
            return !string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken, ""));
        }

        public override IDrawer CreateDrawer()
        {
            var tabBar = new TabBar();
            HMSPMSAPITabFactory.CreateProductsTab(tabBar);
            return tabBar;
        }
    }
}
