using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public static class HMSUnityPackageCreator
    {
        [MenuItem("Huawei/Prepare Packages")]
        private static void PreparePackages()
        {
            string[] hmsFileNames =
            {
                "Assets/Huawei",
                "Assets/Plugins",
                "Assets/Resources"
            };
            AssetDatabase.ExportPackage(hmsFileNames, Application.dataPath + "/HMSUnityPackageV2.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
        }
    }
}