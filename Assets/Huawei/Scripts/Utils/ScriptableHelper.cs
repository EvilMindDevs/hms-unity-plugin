using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableHelper
{
    public static void Save(UnityEngine.Object scriptableObject)
    {

#if UNITY_EDITOR
        EditorUtility.SetDirty(scriptableObject);

        // Note: we do not call AssetDatabase.SaveAssets() because it takes too long on bigger projects 
        // And SetDirty should be enough to live between play mode changes & reopening Unity
#endif
    }

    // path should start with "Assets"
    // filename should not contain file extension
    public static T Load<T>(string filename, string path) where T : ScriptableObject
    {
        var asset = Resources.Load<T>(filename);

        if (asset == null)
        {
            asset = Create<T>(filename, path);
        }

        return asset;
    }

    // path should start with "Assets"
    // filename should not contain file extension
    public static T Create<T>(string filename, string path) where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR

        Directory.CreateDirectory(path);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, filename + ".asset"));

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        // AssetDatabase.SaveAssets ();
        // AssetDatabase.Refresh();

        return asset;
#else
        Debug.LogError("Creating ScriptableObjects during runtime is not allowed!");
        return (T)null;
#endif
    }
}