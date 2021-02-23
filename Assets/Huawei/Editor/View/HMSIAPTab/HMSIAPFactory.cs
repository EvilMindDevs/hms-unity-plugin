using UnityEngine;
using UnityEditor;

namespace Assets.Huawei.Editor.View
{
    public class HMSIAPFactory : ScriptableObject
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}