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
                "Assets/Huawei/Demos",
                "Assets/Huawei/Dlls",
                "Assets/Huawei/Editor",
                "Assets/Huawei/Scripts",
                "Assets/Huawei/VERSION",
                "Assets/Huawei/HuaweiMobileServices.Core.asmdef",
                "Assets/Huawei/Plugins",
                "Assets/Huawei/Resources",
                "Assets/StreamingAssets"
            };
            AssetDatabase.ExportPackage(hmsFileNames, Application.dataPath + "/HMSUnityPackageV2.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
        }
    }
}