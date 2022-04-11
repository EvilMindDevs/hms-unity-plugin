using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    [InitializeOnLoad]
    public class HMSPackageChecker
    {
        static HMSPackageChecker()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            if (packageName.Contains("HMSUnityPackage"))
            {
                var enabledEditors = HMSMainKitsTabFactory.GetEnabledEditors();
                if (enabledEditors != null && enabledEditors.Count > 0)
                {
                    enabledEditors.ForEach(c => c.DisableToggle());
                    enabledEditors.ForEach(f => f.EnableToggle());
                }
            }
        }
    }
}