#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class HMSPluginUpdaterInit : AssetPostprocessor
{
    static HMSPluginUpdaterInit()
    {
        HMSPluginUpdater.Request();
    }
}

#endif