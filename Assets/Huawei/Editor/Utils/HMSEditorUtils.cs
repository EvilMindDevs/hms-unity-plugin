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

        public static void SetHMSPlugin(bool status, bool createManagers, bool refreshAssets = true)
        {
            HMSPluginSettings.Instance.Settings.SetBool(PluginToggleEditor.PluginEnabled, status);
            var enabledEditors = HMSMainKitsTabFactory.GetEnabledEditors();
            if (status)
            {
                if (createManagers)
                {
                    if (enabledEditors != null && enabledEditors.Count > 0)
                    {
                        enabledEditors.ForEach(f => f.CreateManagers());
                    }
                }
                else
                {
                    if (enabledEditors != null && enabledEditors.Count > 0)
                        enabledEditors.ForEach(f => f.DisableManagers(false));
                }
            }
            else
            {
                if (enabledEditors != null && enabledEditors.Count > 0)
                {
                    enabledEditors.ForEach(f => f.DisableManagers(true));
                }
            }
            HMSMainKitsTabFactory.RefreshPluginStatus();
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

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void CheckOldFiles()
        {
            EditorApplication.delayCall += CheckOldFilesNow;
        }

        private static void CheckOldFilesNow()
        {
            string[] fileNames = { "/Plugins/Android/app-debug.aar", "/Plugins/Android/BookInfo.java", "/Plugins/Android/ObjectTypeInfoHelper.java", "/Resources/coins100.png", "/Resources/coins1000.png", "/Resources/no_ads.png", "/Resources/premium.png", "/Resources/xml/remoteConfig.xml", "/Plugins/Android/hmsMainTemplate.gradle", "/Plugins/Android/hmsLauncherTemplate.gradle", "/Plugins/Android/hmsBaseProjectTemplate.gradle" };

            List<string> foundFiles = new List<string>();

            for (int i = 0; i < fileNames.Length; i++)
            {
                string path = fileNames[i];
                if (File.Exists(Application.dataPath + path))
                {
                    foundFiles.Add(path);
                }
            }
            if (foundFiles.Count > 0)
            {
                string allPaths = "";
                foundFiles.ForEach(c => allPaths += c + "\n");
                if (EditorUtility.DisplayDialog("HMS Unity Plugin", "We've found some files that needs to be moved. Do you want to move these files?\n" + allPaths, "Move", "Cancel"))
                {
                    foreach (var path in foundFiles)
                    {
                        if (File.Exists(Application.dataPath + path))
                        {
                            string destPath = Application.dataPath + "/Huawei" + path;
                            if (!File.Exists(destPath))
                                new FileInfo(destPath).Directory.Create();
                            File.Move(Application.dataPath + path, destPath);
                        }
                    }
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
