using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public static class HMSEditorUtils
    {
        public static void HandleAssemblyDefinitions(bool enable, bool refreshAssets = true)
        {
            string huaweiMobileServicesCorePath = Application.dataPath + "/Huawei/HuaweiMobileServices.Core.asmdef";
            var huaweiMobileServicesCore = JsonUtility.FromJson<AssemblyDefinitionInfo>(File.ReadAllText(huaweiMobileServicesCorePath));

            if (huaweiMobileServicesCore != null)
            {
                huaweiMobileServicesCore.includePlatforms = enable ? new List<string> { "Editor", "Android" } : new List<string> { "Editor" };
                File.WriteAllText(huaweiMobileServicesCorePath, JsonUtility.ToJson(huaweiMobileServicesCore, true));
            }
            if (refreshAssets)
                AssetDatabase.Refresh();
        }

        [Serializable]
        private class AssemblyDefinitionInfo
        {
            public string name;
            public List<string> references;
            public List<string> includePlatforms;
            public List<string> excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public List<string> precompiledReferences;
            public bool autoReferenced;
            public List<string> defineConstraints;
            public List<string> versionDefines;
            public bool noEngineReferences;
        }
    }
}
