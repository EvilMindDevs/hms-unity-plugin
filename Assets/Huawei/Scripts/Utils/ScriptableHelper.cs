using System.IO;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public static class ScriptableHelper
    {
        public static void Save(UnityEngine.Object scriptableObject)
        {
#if UNITY_EDITOR
            if (scriptableObject != null)
            {
                EditorUtility.SetDirty(scriptableObject);
            }
            else
            {
                Debug.LogWarning("ScriptableObject is null. Cannot set dirty.");
            }
            // Note: we do not call AssetDatabase.SaveAssets() because it takes too long on bigger projects
            // And SetDirty should be enough to live between play mode changes & reopening Unity
#endif
        }

        // path should start with "Assets"
        // filename should not contain file extension
        public static T Load<T>(string filename, string path) where T : ScriptableObject
        {
            var asset = Resources.Load<T>(filename);
            return asset != null ? asset : Create<T>(filename, path);
        }

        // path should start with "Assets"
        // filename should not contain file extension
        public static T Create<T>(string filename, string path) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path);
                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, filename + ".asset"));
                AssetDatabase.CreateAsset(asset, assetPathAndName);
                AssetDatabase.SaveAssetIfDirty(asset);
            }
            else
            {
                Debug.LogError("Invalid path. Cannot create asset.");

            }
            return asset;
#else
            Debug.LogError("Creating ScriptableObjects during runtime is not allowed!");
            return null;
#endif
        }
    }
}
