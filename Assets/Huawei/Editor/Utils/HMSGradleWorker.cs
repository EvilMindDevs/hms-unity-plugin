using System;
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
        private readonly Dictionary<string, string[]> gradleSettings;
        public int callbackOrder => 0;

        private string gradleTemplatesPath = EditorApplication.applicationContentsPath + @"/PlaybackEngines/AndroidPlayer/Tools/GradleTemplates";

        public HMSGradleWorker()
        {
            gradleSettings = new Dictionary<string, string[]>()
            {
                { AccountToggleEditor.AccountKitEnabled,        new [] { "com.huawei.hms:hwid:6.12.0.300" } },
                { AdsToggleEditor.AdsKitEnabled,                new []
                    {
                        "com.huawei.hms:ads-lite:13.4.69.300",
                        "com.huawei.hms:ads-consent:3.4.69.300",
                        "com.huawei.hms:ads-identifier:3.4.62.300",
                        "com.huawei.hms:ads-installreferrer:3.4.62.300"
                    }
                },
                { AnalyticsToggleEditor.AnalyticsKitEnabled,    new [] { "com.huawei.hms:hianalytics:6.12.0.301" } },
                { CrashToggleEditor.CrashKitEnabled,            new [] { "com.huawei.agconnect:agconnect-crash:1.9.1.301" } },
                { GameServiceToggleEditor.GameServiceEnabled,   new [] { "com.huawei.hms:game:6.10.0.300" } },
                { IAPToggleEditor.IAPKitEnabled,                new [] { "com.huawei.hms:iap:6.13.0.300" } },
                { PushToggleEditor.PushKitEnabled,              new [] { "com.huawei.hms:push:6.11.0.300" } },
                { RemoteConfigToggleEditor.RemoteConfigEnabled, new [] { "com.huawei.agconnect:agconnect-remoteconfig:1.6.3.300" } },
                { CloudDBToggleEditor.CloudDBEnabled,           new [] { "com.huawei.agconnect:agconnect-cloud-database:1.5.5.300" } },
                { AuthToggleEditor.AuthEnabled,                 new [] { "com.huawei.agconnect:agconnect-auth:1.9.1.301" } },
                { NearbyServiceToggleEditor.NearbyServiceEnabled, new [] { "com.huawei.hms:nearby:6.2.0.301" } },
                { AppMessagingToggleEditor.AppMessagingEnabled, new [] { "com.huawei.agconnect:agconnect-appmessaging:1.6.3.300" } },
                { HMSLibrariesDrawer.AppCompatEnabled,          new []{ "com.android.support:appcompat-v7:21.0.0" } },
                { LocationToggleEditor.LocationEnabled,          new [] { "com.huawei.hms:location:6.4.0.300" } },
                { ScanKitToggleEditor.ScanKitEnabled,            new [] { "com.huawei.hms:scan:2.6.0.301" } },
                { AppLinkingToggleEditor.AppLinkingEnabled,      new [] { "com.huawei.agconnect:agconnect-applinking:1.9.1.301" } },
                { DriveKitToggleEditor.DriveKitEnabled,          new [] { "com.huawei.hms:drive:5.2.0.300" } },
                { CloudStorageToggleEditor.CloudStorageEnabled,  new [] { "com.huawei.agconnect:agconnect-storage:1.9.1.301" } },
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
                },
                {MLKitToggleEditor.MLKitEnabled, GetMLKitModulesGradlePackages()},
                {HMSLibrariesDrawer.HMSCoreInstalledEnabled, new [] {"com.huawei.hms:hmscoreinstaller:6.6.0.300"}}
            };
        }
        private void CreateGradleFiles(string[] gradleConfigs)
        {
#if UNITY_2022_3_OR_NEWER
            CreateMainGradleFile(gradleConfigs);
            CreateLauncherGradleFile(gradleConfigs);
            BaseProjectGradleFile("1.9.1.301");
#elif UNITY_2019_3_OR_NEWER
            CreateMainGradleFile(gradleConfigs);
            CreateLauncherGradleFile(gradleConfigs);
            BaseProjectGradleFile("1.6.1.300");

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
            //Gradle 7+ and classpath 'com.huawei.agconnect:agcp:1.9.1.301'
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
            stringBuilder.AppendLine("\tpackagingOptions {");
            stringBuilder.AppendLine("\t\tpickFirst \"okhttp3/internal/publicsuffix/publicsuffixes.gz\"");
            stringBuilder.AppendLine("\t\tpickFirst \"**/*.so\"");
            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");

            File.WriteAllText(path, stringBuilder.ToString());
        }
        private void BaseProjectGradleFile(string agcversion)
        {
            // Combine paths to get the full file path
            string filePath = Path.Combine(Application.dataPath + "/Huawei/Plugins/Android/hmsBaseProjectTemplate.gradle");

            // Initiate a new StringBuilder object for efficient string concatenations
            StringBuilder sb = new StringBuilder();

            // Construct the contents of the gradle file using AppendLine and AppendFormat methods
            sb.AppendLine("allprojects {");
            sb.AppendLine("\tbuildscript {");
            sb.AppendLine("\t\trepositories {");

            // Use placeholder '{}' and RepoUrl constant
            // Template for adding a Maven repository
            sb.AppendFormat("\t\t\tmaven {{ url '{0}' }}\n", "https://developer.huawei.com/repo/");

            sb.AppendLine("\t\t}");  // End of repositories
            sb.AppendLine("\t\tdependencies {");

            // Add the classpath dependency for Huawei's AGC
            sb.AppendFormat("\t\t\t{0}\n", AddClasspath("com.huawei.agconnect:agcp:" + agcversion));

            sb.AppendLine("\t\t}");  // End of dependencies
            sb.AppendLine("\t}");    // End of buildscript
            sb.AppendLine("\trepositories {");

            // Repeat repository definition for allprojects
            sb.AppendFormat("\t\tmaven {{ url '{0}' }}\n", "https://developer.huawei.com/repo/");

            sb.AppendLine("\t}");    // End of repositories
            sb.AppendLine("}");      // End of allprojects

            // Now we write the constructed string content to the gradle file
            using (var file = File.CreateText(filePath))
            {
                file.Write(sb.ToString());
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

            foreach (var key in gradleSettings.Keys)
            {
                if (settings.GetBool(key))
                {
                    gradle.AddRange(gradleSettings[key]);
                }
            }

            CreateGradleFiles(gradle.ToArray());
        }
        private string[] CoreGradles()
        {
            return new string[] { "com.huawei.hms:base:6.6.0.300", "com.huawei.agconnect:agconnect-core:1.6.5.300" };
        }
        #region ML Kit
        private string[] GetMLKitModulesGradlePackages()
        {
            var IsMLKitEnabled = HMSMainEditorSettings.Instance.Settings.GetBool(MLKitToggleEditor.MLKitEnabled);

            if (!IsMLKitEnabled)
            {
                return Array.Empty<string>();
            }

            var mainPackages = new List<string> { "com.huawei.hms:ml-computer-agc-inner:3.11.2.300" };


            AddConditionalPackages(mainPackages);


            return mainPackages.ToArray();
        }
        private void AddConditionalPackages(List<string> packages)
        {
            var settings = HMSMLKitSettings.Instance.Settings;

            if (settings.GetBool(HMSMLKitSettings.EnableTranslateModule))
            {
                packages.AddRange(new[]
                {
                    "com.huawei.hms:ml-computer-translate:3.11.0.302",
                    "com.huawei.hms:ml-computer-translate-model:3.11.0.302"
                });
            }

            if (settings.GetBool(HMSMLKitSettings.EnableTextToSpeechModule))
            {
                packages.AddRange(new[]
                {
                    "com.huawei.hms:ml-computer-voice-tts:3.12.0.301",
                    "com.huawei.hms:ml-computer-voice-tts-model-bee:3.6.0.300",
                    "com.huawei.hms:ml-computer-voice-tts-model-eagle:3.6.0.300"
                });
            }

            if (settings.GetBool(HMSMLKitSettings.EnableLanguageDetectionModule))
            {
                packages.AddRange(new[]
                {
                    "com.huawei.hms:ml-computer-language-detection:3.11.0.302",
                    "com.huawei.hms:ml-computer-language-detection-model:3.11.0.302"
                });
            }

            if (settings.GetBool(HMSMLKitSettings.EnableTextRecognitionModule))
            {
                packages.AddRange(new[]
                {
                    "com.huawei.hms:ml-computer-vision-ocr:3.11.0.301",
                    "com.huawei.hms:ml-computer-vision-ocr-cn-model:3.11.0.301",
                    "com.huawei.hms:ml-computer-vision-ocr-jk-model:3.11.0.301",
                    "com.huawei.hms:ml-computer-vision-ocr-latin-model:3.11.0.301"
                });
            }
        }
        #endregion
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

            HMSEditorUtils.UpdateAssemblyDefinitions(pluginEnabled);
        }
        private void OnBuildError(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                Application.logMessageReceived -= OnBuildError;
                HMSEditorUtils.UpdateAssemblyDefinitions(false, false);
            }
        }
    }
}
