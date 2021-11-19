#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class HMSPluginUpdaterInit : AssetPostprocessor
{
    static HMSPluginUpdaterInit()
    {
        EditorApplication.delayCall += () =>
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () => HMSPluginUpdater.Request();
            };
        };
    }
}
#endif
