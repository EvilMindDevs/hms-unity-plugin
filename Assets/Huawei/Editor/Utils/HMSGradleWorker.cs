using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                { AccountToggleEditor.AccountKitEnabled,        new [] { "com.huawei.hms:hwid:6.9.0.301" } },
                { AdsToggleEditor.AdsKitEnabled,                new []
                    {
                        "com.huawei.hms:ads-lite:13.4.61.300",
                        "com.huawei.hms:ads-consent:3.4.61.300",
                        "com.huawei.hms:ads-identifier:3.4.58.301",
                        "com.huawei.hms:ads-installreferrer:3.4.58.301"
                    }
                },
                { AnalyticsToggleEditor.AnalyticsKitEnabled,    new [] { "com.huawei.hms:hianalytics:6.4.1.302" } },
                { CrashToggleEditor.CrashKitEnabled,            new [] { "com.huawei.agconnect:agconnect-crash:1.8.0.300" } },
                { GameServiceToggleEditor.GameServiceEnabled,   new [] { "com.huawei.hms:game:6.2.0.301" } },
                { IAPToggleEditor.IAPKitEnabled,                new [] { "com.huawei.hms:iap:6.4.0.301" } },
                { PushToggleEditor.PushKitEnabled,              new [] { "com.huawei.hms:push:6.7.0.300" } },
                { RemoteConfigToggleEditor.RemoteConfigEnabled, new [] { "com.huawei.agconnect:agconnect-remoteconfig:1.6.3.300" } },
                { CloudDBToggleEditor.CloudDBEnabled,           new [] { "com.huawei.agconnect:agconnect-cloud-database:1.5.5.300" } },
                { AuthToggleEditor.AuthEnabled,                 new [] { "com.huawei.agconnect:agconnect-auth:1.8.0.300" } },
                { NearbyServiceToggleEditor.NearbyServiceEnabled, new [] { "com.huawei.hms:nearby:6.2.0.301" } },
                { AppMessagingToggleEditor.AppMessagingEnabled, new [] { "com.huawei.agconnect:agconnect-appmessaging:1.6.3.300" } },
                { HMSLibrariesDrawer.AppCompatEnabled,          new []{ "com.android.support:appcompat-v7:21.0.0" } },
                { LocationToggleEditor.LocationEnabled,          new [] { "com.huawei.hms:location:6.4.0.300" } },
                { ScanKitToggleEditor.ScanKitEnabled,            new [] { "com.huawei.hms:scan:2.6.0.301" } },
                { AppLinkingToggleEditor.AppLinkingEnabled,      new [] { "com.huawei.agconnect:agconnect-applinking:1.8.0.300" } },
                { DriveKitToggleEditor.DriveKitEnabled,          new [] { "com.huawei.hms:drive:5.0.0.307" } },
                { CloudStorageToggleEditor.CloudStorageEnabled,  new [] { "com.huawei.agconnect:agconnect-storage:1.5.0.100" } },
                { APMToggleEditor.APMEnabled,                    new [] { "com.huawei.agconnect:agconnect-apms:1.6.1.300" } },
                { Modeling3dKitToggleEditor.Modeling3dkitEnabled,new []
                    {
                        "com.huawei.hms:modeling3d-object-reconstruct:1.8.0.300",
                        "com.huawei.hms:modeling3d-motion-capture-model:1.7.0.301",
                        "com.huawei.hms:modeling3d-material-generate:1.7.0.301",
                        //"com.huawei.hms:modeling3d-motion-capture:1.7.0.301",
                        "com.huawei.hms:modeling3d-objectreconstruct-slam:1.7.0.301",
                        "com.huawei.hms:xrkitsdk:1.4.0.0",
                        "com.huawei.hms:arenginesdk:3.7.0.3",
                        "com.google.ar:core:1.30.0"
                    }
                }
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
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.6.1.300"));
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
/* TODO:
#elif UNITY_2022_1_OR_NEWER
//Gradle 7+ and classpath 'com.huawei.agconnect:agcp:1.8.0.300' 
*/
        }

        private void CreateLauncherGradleFile(string[] gradleConfigs)
        {
            var path = Path.Combine(Application.dataPath, "Huawei/Plugins/Android/hmsLauncherTemplate.gradle");

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("apply plugin: 'com.huawei.agconnect'\n");

            #region Dependencies

            stringBuilder.AppendLine("dependencies {");
            for (int i = 0; i < gradleConfigs.Length; i++)
            {
                stringBuilder.AppendLine($"\t{AddDependency(gradleConfigs[i])}");
            }
            stringBuilder.AppendLine("}");
            #endregion
            stringBuilder.AppendLine("android {");
            stringBuilder.AppendLine("packagingOptions {");
            stringBuilder.AppendLine("pickFirst \"okhttp3/internal/publicsuffix/publicsuffixes.gz\"");
            stringBuilder.AppendLine("pickFirst \"**/*.so\"");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");

            File.WriteAllText(path, stringBuilder.ToString());
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
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.6.1.300"));
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
            return new string[] { "com.huawei.hms:base:6.6.0.300", "com.huawei.agconnect:agconnect-core:1.6.5.300", "com.huawei.hms:hmscoreinstaller:6.6.0.300" };
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
            var modeling3dPlugin = AssetImporter.GetAtPath("Assets/Huawei/Plugins/Android/HMSUnityModelingKit.plugin") as PluginImporter;

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
            if (modeling3dPlugin != null)
                modeling3dPlugin.SetCompatibleWithPlatform(BuildTarget.Android, HMSMainEditorSettings.Instance.Settings.GetBool(Modeling3dKitToggleEditor.Modeling3dkitEnabled) && pluginEnabled);
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

