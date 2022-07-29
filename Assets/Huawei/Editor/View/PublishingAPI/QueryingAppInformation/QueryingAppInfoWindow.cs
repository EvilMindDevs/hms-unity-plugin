using HmsPlugin;
using HmsPlugin.Window;

using UnityEditor;

using UnityEngine;

public class QueryingAppInformationWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/Publishing API/Querying App Information")]
    public static void ShowQueryingAppInformationWindow()
    {
        GetWindow(typeof(QueryingAppInformationWindow), false, "Querying App Information").minSize = new UnityEngine.Vector2(400, 700);
    }

    // Validate the menu item defined by the function above.
    // The menu item will be disabled if this function returns false.
    [MenuItem("Huawei/Connect API/Publishing API/Querying App Information", true)]
    static bool ValidateShowQueryingAppInformationWindow()
    {
        Debug.Log("ValidateShowQueryingAppInformationWindow");
        return !string.IsNullOrEmpty(HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.AccessToken, ""));
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSPublishingAPITabFactory.CreateQueryingAppInformationTab(tabBar);
        return tabBar;
    }
}
