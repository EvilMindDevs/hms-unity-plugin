
using HmsPlugin;
using HmsPlugin.HelpBox;
using HmsPlugin.Label;
using HmsPlugin.TextField;
using HmsPlugin.Window;
using UnityEditor;

public class HMSMainWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Settings")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HMSMainWindow), false, "HMS Settings");
    }


    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();

        tabBar.AddTab(HMSMainKitsTabFactory.CreateTab(tabBar));

        return tabBar;

    }
}
