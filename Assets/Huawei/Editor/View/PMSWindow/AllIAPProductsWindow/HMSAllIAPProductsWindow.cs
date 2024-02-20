using HmsPlugin.Window;

using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSAllIAPProductsWindow : HMSEditorWindow
    {
        [MenuItem("Huawei/Connect API/PMS API/Query IAP Products")]
        public static void ShowProductWindow()
        {
            GetWindow(typeof(HMSAllIAPProductsWindow), false, "Query Products").minSize = new UnityEngine.Vector2(400, 700);
        }


        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("Huawei/Connect API/PMS API/Query IAP Products", true)]
        static bool ValidatePMSAPIWindow()
        {
            Debug.Log("ValidatePMSAPIWindow");
            return !string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken, ""));
        }

        public override IDrawer CreateDrawer()
        {
            var tabBar = new TabBar();
            HMSPMSAPITabFactory.AllProductsTab(tabBar);
            return tabBar;
        }
    }

}
