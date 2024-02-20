using HmsPlugin.Window;
using UnityEditor;

namespace HmsPlugin
{
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

}
