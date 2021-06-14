
using HmsPlugin;
using HmsPlugin.Window;
using UnityEditor;

public class HMSMainWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Kit Settings")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HMSMainWindow), false, "HMS Settings");
    }

    [MenuItem("Huawei/Check for Updates")]
    public static void CheckForUpdates()
    {
        HMSPluginUpdater.Request(true);
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();

        HMSMainKitsTabFactory.CreateTab(tabBar);

        return tabBar;

    }
}
