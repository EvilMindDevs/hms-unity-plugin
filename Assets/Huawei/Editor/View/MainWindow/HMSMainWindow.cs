
using HmsPlugin;
using HmsPlugin.Window;
using UnityEditor;

public class HMSMainWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Kit Settings", priority = 1)]
    public static void ShowWindow()
    {
        GetWindow(typeof(HMSMainWindow), false, "HMS Kit Settings");
    }

    [MenuItem("Huawei/Check for Updates", priority = 2)]
    public static void CheckForUpdates()
    {
        HMSPluginUpdater.Request(true);
    }

    [MenuItem("Huawei/Utils/Enable Plugin")]
    public static void EnablePlugin()
    {
        HMSEditorUtils.SetHMSPlugin(true, true);
    }

    [MenuItem("Huawei/Utils/Enable Plugin Without Managers")]
    public static void EnableWithoutManagers()
    {
        HMSEditorUtils.SetHMSPlugin(true, false);
    }

    [MenuItem("Huawei/Utils/Disable Plugin")]
    public static void DisablePlugin()
    {
        HMSEditorUtils.SetHMSPlugin(false, false);
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();

        HMSMainKitsTabFactory.CreateKitsTab(tabBar);

        return tabBar;

    }
}
