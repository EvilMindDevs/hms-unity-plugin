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

        private string gradleTemplatesPath = EditorApplication.applicationContentsPath + @"\PlaybackEngines\AndroidPlayer\Tools\GradleTemplates";

        public HMSGradleWorker()
        {
            gradleSettings = new Dictionary<string, string[]>()
            {
                { AccountToggleEditor.AccountKitEnabled, new string[]{ "com.huawei.hms:hwid:5.2.0.300" } },
                { AdsToggleEditor.AdsKitEnabled, new string[]{ "com.huawei.hms:ads-lite:13.4.39.302", "com.huawei.hms:ads-consent:3.4.39.302", "com.huawei.hms:ads-identifier:3.4.39.302" } },
                { AnalyticsToggleEditor.AnalyticsKitEnabled, new string[] { "com.huawei.hms:hianalytics:5.2.0.301" } },
                { CrashToggleEditor.CrashKitEnabled, new string[] { "com.huawei.agconnect:agconnect-crash:1.4.2.301" } },
                { GameServiceToggleEditor.GameServiceEnabled, new string[] { "com.huawei.hms:game:5.0.4.302" } },
                { IAPToggleEditor.IAPKitEnabled, new string[] { "com.huawei.hms:iap:5.1.0.300" } },
                { PushToggleEditor.PushKitEnabled, new string[] { "com.huawei.hms:push:5.1.1.301" } },
                { RemoteConfigToggleEditor.RemoteConfigEnabled, new string[] { "com.huawei.agconnect:agconnect-remoteconfig:1.5.0.300" } },
                { CloudDBToggleEditor.CloudDBEnabled, new string[] { "com.huawei.agconnect:agconnect-cloud-database:1.4.5.300" } },
                { AuthToggleEditor.AuthEnabled, new string[] { "com.huawei.agconnect:agconnect-auth:1.4.2.301" } },
            };
        }

        private void CreateGradleFiles(string[] gradleConfigs)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Plugins"))
            {
                AssetDatabase.CreateFolder("Assets", "Plugins");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Plugins/Android"))
            {
                AssetDatabase.CreateFolder("Assets/Plugins", "Android");
            }
#if UNITY_2019 || UNITY_2020
            CreateMainGradleFile(gradleConfigs);
            CreateLauncherGradleFile(gradleConfigs);
            BaseProjectGradleFile();

#elif UNITY_2018
            CreateMainGradleFile(gradleConfigs);
#endif
            AssetDatabase.Refresh();
        }

        private void CreateMainGradleFile(string[] gradleConfigs)
        {
#if UNITY_2019 || UNITY_2020
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsMainTemplate.gradle"))
            {
                file.Write("dependencies {\n\t");
                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }
                file.Write("}\n");
            }

#elif UNITY_2018
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/mainTemplate.gradle"))
            {
                file.Write("buildscript {\n\t");
                file.Write("repositories {**ARTIFACTORYREPOSITORY**\n\t\t");
                file.Write("google()\n\t\t");
                file.Write("jcenter()\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n\n\t");
                file.Write("dependencies {\n\t\t");
                file.Write(AddClasspath("com.android.tools.build:gradle:3.4.0"));
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.4.2.300"));
                file.Write("**BUILD_SCRIPT_DEPS**\n\t}\n}\n\n");
                file.Write("allprojects {\n\t");
                file.Write("repositories {**ARTIFACTORYREPOSITORY**\n\t\t");
                file.Write("google()\n\t\t");
                file.Write("jcenter()\n\t\t");
                file.Write("flatDir {\n\t\t\t");
                file.Write("dirs 'libs'\n\t\t}\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n}\n\n");

                file.WriteLine("apply plugin: 'com.android.application'\n**APPLY_PLUGINS**\n");
                file.WriteLine("apply plugin: 'com.huawei.agconnect'\n");

            #region Dependencies
                file.Write("dependencies {\n\t");
                file.Write("implementation fileTree(dir: 'libs', include: ['*.jar'])\n\t");
                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }
                file.Write("**DEPS**}\n\n");
            #endregion

            #region Android Settings
                file.Write("android {\n\t");
                file.Write("compileSdkVersion **APIVERSION**\n\t");
                file.Write("buildToolsVersion '**BUILDTOOLS**'\n\n\t");
                file.Write("compileOptions {\n\t\t");
                file.Write("sourceCompatibility JavaVersion.VERSION_1_8\n\t\t");
                file.Write("targetCompatibility JavaVersion.VERSION_1_8\n\t}\n\n\t");
                file.Write("defaultConfig {\n\t\t");
                file.Write("minSdkVersion **MINSDKVERSION**\n\t\t");
                file.Write("targetSdkVersion **TARGETSDKVERSION**\n\t\t");
                file.Write("applicationId '**APPLICATIONID**'\n\t\t");
                file.Write("ndk {\n\t\t\t");
                file.Write("abiFilters **ABIFILTERS**\n\t\t}\n\t\t");
                file.Write("versionCode **VERSIONCODE**\n\t\t");
                file.Write("versionName '**VERSIONNAME**'\n\t\t");
                file.Write("consumerProguardFiles 'proguard-unity.txt'**USER_PROGUARD**\n\t}\n\n\t");
                file.Write("lintOptions {\n\t\t");
                file.Write("abortOnError false\n\t}\n\n\t");
                file.Write("aaptOptions {\n\t\t");
                file.Write("noCompress = ['.unity3d', '.ress', '.resource', '.obb'**STREAMING_ASSETS**]\n\t}**SIGN**\n\n\t");
                file.Write("buildTypes {\n\t\t");
                file.Write("debug {\n\t\t\t");
                file.Write("minifyEnabled **MINIFY_DEBUG**\n\t\t\t");
                file.Write("useProguard **PROGUARD_DEBUG**\n\t\t\t");
                file.Write("proguardFiles getDefaultProguardFile('proguard-android.txt'), 'proguard-unity.txt'**USER_PROGUARD**\n\t\t\t");
                file.Write("jniDebuggable true\n\t\t}\n\t\t");
                file.Write("release {\n\t\t\t");
                file.Write("minifyEnabled **MINIFY_RELEASE**\n\t\t\t");
                file.Write("useProguard **PROGUARD_RELEASE**\n\t\t\t");
                file.Write("proguardFiles getDefaultProguardFile('proguard-android.txt'), 'proguard-unity.txt'**USER_PROGUARD****SIGNCONFIG**\n\t\t}\n\t}");
                file.Write("**PACKAGING_OPTIONS****SPLITS**\n");
                file.Write("**BUILT_APK_LOCATION**\n\t");
                file.Write("bundle {\n\t\t");
                file.Write("language {\n\t\t\t");
                file.Write("enableSplit = false\n\t\t}\n\t\t");
                file.Write("density {\n\t\t\t");
                file.Write("enableSplit = false\n\t\t}\n\t\t");
                file.Write("abi {\n\t\t\t");
                file.Write("enableSplit = true\n\t\t}\n\t}\n}");
                file.Write("**SPLITS_VERSION_CODE****REPOSITORIES****SOURCE_BUILD_SETUP**");
            #endregion
            }
#endif
        }

        private void CreateLauncherGradleFile(string[] gradleConfigs)
        {
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsLauncherTemplate.gradle"))
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
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsBaseProjectTemplate.gradle"))
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
            Settings settings = HMSMainEditorSettings.Instance.Settings;
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
            PrepareGradleFile();
            var bookInfo = AssetImporter.GetAtPath("Assets/Plugins/Android/BookInfo.java") as PluginImporter;
            var objectTypeInfoHelper = AssetImporter.GetAtPath("Assets/Plugins/Android/ObjectTypeInfoHelper.java") as PluginImporter;

            if (bookInfo != null)
                bookInfo.SetCompatibleWithPlatform(BuildTarget.Android, HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBToggleEditor.CloudDBEnabled));
            if (objectTypeInfoHelper != null)
                objectTypeInfoHelper.SetCompatibleWithPlatform(BuildTarget.Android, HMSMainEditorSettings.Instance.Settings.GetBool(CloudDBToggleEditor.CloudDBEnabled));
        }
    }
}
