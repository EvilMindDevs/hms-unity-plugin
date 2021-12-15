using HmsPlugin;
using HmsPlugin.Window;
using UnityEditor;

public class QueryingAppInformationWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Connect API/Publishing API/Querying App Information")]
    public static void ShowQueryingAppInformationWindow()
    {
        GetWindow(typeof(QueryingAppInformationWindow), false, "Querying App Information").minSize = new UnityEngine.Vector2(400, 700);
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSPublishingAPITabFactory.CreateQueryingAppInformationTab(tabBar);
        return tabBar;
    }
}
