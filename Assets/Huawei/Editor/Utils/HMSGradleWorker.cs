using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSGradleWorker : HMSEditorSingleton<HMSGradleWorker>, IPreprocessBuildWithReport
    {
        private Dictionary<string, string[]> gradleSettings;
        public int callbackOrder => 0;

        private string gradleTemplatesPath = EditorApplication.applicationContentsPath + @"/PlaybackEngines/AndroidPlayer/Tools/GradleTemplates";

        public HMSGradleWorker()
        {
            gradleSettings = new Dictionary<string, string[]>()
            {
                { AccountToggleEditor.AccountKitEnabled, new string[]{ "com.huawei.hms:hwid:5.3.0.302" } },
                { AdsToggleEditor.AdsKitEnabled, new string[]{ "com.huawei.hms:ads-lite:13.4.39.302", "com.huawei.hms:ads-consent:3.4.39.302", "com.huawei.hms:ads-identifier:3.4.39.302" } },
                { AnalyticsToggleEditor.AnalyticsKitEnabled, new string[] { "com.huawei.hms:hianalytics:5.3.1.300" } },
                { CrashToggleEditor.CrashKitEnabled, new string[] { "com.huawei.agconnect:agconnect-crash:1.4.2.301" } },
                { GameServiceToggleEditor.GameServiceEnabled, new string[] { "com.huawei.hms:game:5.0.4.303" } },
                { IAPToggleEditor.IAPKitEnabled, new string[] { "com.huawei.hms:iap:5.3.0.300" } },
                { PushToggleEditor.PushKitEnabled, new string[] { "com.huawei.hms:push:5.3.0.301" } },
                { RemoteConfigToggleEditor.RemoteConfigEnabled, new string[] { "com.huawei.agconnect:agconnect-remoteconfig:1.5.0.300" } },
                { CloudDBToggleEditor.CloudDBEnabled, new string[] { "com.huawei.agconnect:agconnect-cloud-database:1.4.5.300" } },
                { AuthToggleEditor.AuthEnabled, new string[] { "com.huawei.agconnect:agconnect-auth:1.4.2.301" } },
                { NearbyServiceToggleEditor.NearbyServiceEnabled, new string[] { "com.huawei.hms:nearby:5.3.0.300" } },
                { AppMessagingToggleEditor.AppMessagingEnabled, new string[] { "com.huawei.agconnect:agconnect-appmessaging:1.4.2.301" } }
            };
        }

        private void CreateGradleFiles(string[] gradleConfigs)
        {
#if UNITY_2019_3_OR_NEWER
            CreateMainGradleFile(gradleConfigs);
            CreateLauncherGradleFile(gradleConfigs);
            BaseProjectGradleFile();

#elif UNITY_2018_1_OR_NEWER
            CreateMainGradleFile(gradleConfigs);
#endif
            AssetDatabase.Refresh();
        }

        private void CreateMainGradleFile(string[] gradleConfigs)
        {
#if UNITY_2019_3_OR_NEWER
            using (var file = File.CreateText(Application.dataPath + "/Huawei/Plugins/Android/hmsMainTemplate.gradle"))
            {
                file.Write("dependencies {\n\t");
                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }
                file.Write("}\n");
            }

#elif UNITY_2018_1_OR_NEWER
            using (var file = File.CreateText(Application.dataPath + "/Huawei/Plugins/Android/hmsMainTemplate.gradle"))
            {
                file.Write("buildscript {\n\t");
                file.Write("repositories {\n\t\t");
                file.Write("google()\n\t\t");
                file.Write("jcenter()\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n\n\t");
                file.Write("dependencies {\n\t\t");
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.4.2.300"));
                file.Write("\t}\n}\n\n");
                file.Write("allprojects {\n\t");
                file.Write("repositories {\n\t\t");
                file.Write("google()\n\t\t");
                file.Write("jcenter()\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n}\n\n");

                file.WriteLine("apply plugin: 'com.huawei.agconnect'\n");

                file.Write("dependencies {\n\t");
                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }
                file.Write("}\n\n");
            }
#endif
        }

        private void CreateLauncherGradleFile(string[] gradleConfigs)
        {
            using (var file = File.CreateText(Application.dataPath + "/Huawei/Plugins/Android/hmsLauncherTemplate.gradle"))
            {
                file.Write("apply plugin: 'com.huawei.agconnect'\n\n");
                file.Write("dependencies {\n\t");

                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }

                file.Write("\n}\n");
            }
        }

        private void BaseProjectGradleFile()
        {
            using (var file = File.CreateText(Application.dataPath + "/Huawei/Plugins/Android/hmsBaseProjectTemplate.gradle"))
            {
                file.Write("allprojects {\n\t");
                file.Write("buildscript {\n\t\t");
                file.Write("repositories {\n\t\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t\t}\n\n\t\t");
                file.Write("dependencies {\n\t\t\t");
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.4.2.300"));
                file.Write("\n\t\t}\n\t}\n\n\t");
                file.Write("repositories {\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n}\n\n");
            }
        }

        private string AddDependency(string name)
        {
            return $"implementation '{name}'\n\t";
        }

        private string AddClasspath(string name)
        {
            return $"classpath '{name}'\n\t\t\t";
        }

        public void PrepareGradleFile()
        {
            HMSSettings settings = HMSMainEditorSettings.Instance.Settings;
            List<string> gradle = new List<string>(CoreGradles());
            for (int i = 0; i < settings.Keys.Count(); i++)
            {
                if (settings.Values.ElementAt(i) == true.ToString())
                {
                    gradle.AddRange(gradleSettings[settings.Keys.ElementAt(i)]);
                }
            }
            CreateGradleFiles(gradle.ToArray());
        }

        private string[] CoreGradles()
        {
            return new string[] { "com.huawei.hms:base:5.2.0.300", "com.android.support:appcompat-v7:28.0.0", "com.huawei.agconnect:agconnect-core:1.4.1.300" };
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Application.logMessageReceived += OnBuildError;
            bool pluginEnabled = HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true);
            if (report.summary.platform != BuildTarget.Android && pluginEnabled)
            {
                //TODO: Maybe add a button called "Don't Remind" and stop this popup getting shown and auto disable the plugin.
                EditorUtility.DisplayDialog("HMS Unity Plugin Warning!", "HMS Unity Plugin only works on Android platforms. Plugin is getting disabled for this build.", "Ok");
                pluginEnabled = false;
            }

            var huaweiMobileServicesDLL = AssetImporter.GetAtPath("Assets/Huawei/Dlls/HuaweiMobileServices.dll") as PluginImporter;
            var appDebugAar = AssetImporter.GetAtPath("Assets/Huawei/Plugins/Android/app-debug.aar") as PluginImporter;
            var bookInfo = AssetImporter.GetAtPath("Assets/Huawei/Plugins//Android/BookInfo.java") as PluginImporter;
            var objectTypeInfoHelper = AssetImporter.GetAtPath("Assets/Huawei/Plugins/Android/ObjectTypeInfoHelper.java") as PluginImporter;
            var pushKitPlugin = AssetImporter.GetAtPath("Assets/Huawei/Plugins/Android/HMSUnityPushKit.plugin") as PluginImporter;

            if (pluginEnabled)
                PrepareGradleFile();

            if (bookInfo != null)
                bookInfo.SetCompatibleWithPlatform(BuildTarget.Android, HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBToggleEditor.CloudDBEnabled) && pluginEnabled);
            if (objectTypeInfoHelper != null)
                objectTypeInfoHelper.SetCompatibleWithPlatform(BuildTarget.Android, HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBToggleEditor.CloudDBEnabled) && pluginEnabled);
            if (huaweiMobileServicesDLL != null)
                huaweiMobileServicesDLL.SetCompatibleWithPlatform(BuildTarget.Android, pluginEnabled);
            if (pushKitPlugin != null)
                pushKitPlugin.SetCompatibleWithPlatform(BuildTarget.Android, HMSMainEditorSettings.Instance.Settings.GetBool(PushToggleEditor.PushKitEnabled) && pluginEnabled);
            if (appDebugAar != null)
                appDebugAar.SetCompatibleWithPlatform(BuildTarget.Android, pluginEnabled);

            HMSEditorUtils.HandleAssemblyDefinitions(pluginEnabled);
        }

        private void OnBuildError(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                Application.logMessageReceived -= OnBuildError;
                HMSEditorUtils.HandleAssemblyDefinitions(false, false);
            }
        }
    }
}

