using HmsPlugin;
using HmsPlugin.Window;
using UnityEditor;

public class HMSPublishingAPIWindow : HMSEditorWindow
{
    [MenuItem("Huawei/Publishing API/Querying App Information")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HMSPublishingAPIWindow), false, "HMS Publishing API").minSize = new UnityEngine.Vector2(400,500);
    }

    public override IDrawer CreateDrawer()
    {
        var tabBar = new TabBar();
        HMSPublishingAPITabFactory.CreatePublishingAPITab(tabBar);
        return tabBar;
    }
}
