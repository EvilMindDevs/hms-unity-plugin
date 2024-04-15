using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace HmsPlugin
{
    public class HMSGradleFixer : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 1;
        private const string MINGRADLEVERSION = "3.5.4";
        //Unity 2022.3 and higher
        private const string agconnect_agcp_7_2 = "classpath 'com.huawei.agconnect:agcp:1.9.1.301'";
        //Unity 2019 and lower
        private const string build_gradle = "classpath 'com.android.tools.build:gradle:" + MINGRADLEVERSION + "'";

        private void GradleVersionFixer(string gradleFileAsString, string path)
        {
#if UNITY_2022_3_OR_NEWER
        // Grade 7.2
        string currentBuildgradle = File.ReadAllText(Directory.GetParent(path).FullName + "/build.gradle").Replace("apply from: 'hmsBaseProjectTemplate.gradle'", "");

        if (currentBuildgradle.Contains("dependencies")){
            if(!currentBuildgradle.Contains("com.huawei.agconnect:agcp:")){

                string buildscriptdependencies = "buildscript {\n\tdependencies {\n\t\t" + agconnect_agcp_7_2 + "\n\t}\n}\n";
                currentBuildgradle = buildscriptdependencies + currentBuildgradle;
                File.WriteAllText(Directory.GetParent(path).FullName + "/build.gradle", currentBuildgradle);
            }
            if (!currentBuildgradle.Contains("com.android.tools.build:gradle:")){//TODO: check for gradle version is bigger then minGradle Version

                string buildscriptdependencies = "buildscript {\n\tdependencies {\n\t\t" + build_gradle + "\n\t}\n}\n";
                currentBuildgradle = buildscriptdependencies + currentBuildgradle;
                File.WriteAllText(Directory.GetParent(path).FullName + "/build.gradle", currentBuildgradle);
            }
        }else{
            string buildscriptdependencies = "buildscript {\n\tdependencies {\n\t\t" + build_gradle + "\n\t\t" + agconnect_agcp_7_2 + "\n\t}\n}\n";
            currentBuildgradle = buildscriptdependencies + currentBuildgradle;
            File.WriteAllText(Directory.GetParent(path).FullName + "/build.gradle", currentBuildgradle);
        }

        string currentSettingsgradle = File.ReadAllText(Directory.GetParent(path).FullName + "/settings.gradle");
        if(!currentSettingsgradle.Contains("https://developer.huawei.com/repo/"))
            File.WriteAllText(Directory.GetParent(path).FullName + "/settings.gradle", currentSettingsgradle.Replace("mavenCentral()", "mavenCentral()\n\t\tmaven { url 'https://developer.huawei.com/repo/' }"));

#else
            string gradleRowPattern = @".*gradle:(\d\.?)+";
            string gradleVersionPattern = @"(\d\.?)+";

            if (!Version.TryParse(MINGRADLEVERSION, out Version gradleMinVersion))
            {
                gradleMinVersion = new Version(3, 5, 4);
            }

            Match gradleRowMatch = Regex.Match(gradleFileAsString, gradleRowPattern);
            Match gradleVersionMatch = Regex.Match(gradleRowMatch.Value, gradleVersionPattern);

            if (!Version.TryParse(gradleVersionMatch.Value, out Version gradleVersion))
            {
                gradleVersion = new Version(3, 5, 4);
            }
            // if users gradle version is lesser than our minimum version.
            if (gradleVersion.CompareTo(gradleMinVersion) < 0)
            {
                gradleFileAsString = gradleFileAsString.Replace(gradleVersion.ToString(), gradleMinVersion.ToString());

#if UNITY_2019_3_OR_NEWER
                File.WriteAllText(Directory.GetParent(path).FullName + "/build.gradle", gradleFileAsString);
#elif UNITY_2018_1_OR_NEWER
                File.WriteAllText(path + "/build.gradle", gradleFileAsString);
#endif
            }
#endif
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
            {
                HMSEditorUtils.UpdateAssemblyDefinitions(false);
                return;
            }

            string fileName = "agconnect-services.json";
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            string destPath = "";
#if UNITY_2019_3_OR_NEWER
            destPath = Path.Combine(Directory.GetParent(path).FullName + Path.DirectorySeparatorChar + "launcher", fileName);

            string hmsMainTemplatePath = Application.dataPath + "/Huawei/Plugins/Android/hmsMainTemplate.gradle";
            FileUtil.ReplaceFile(hmsMainTemplatePath, Path.GetFullPath(path) + @"/hmsMainTemplate.gradle");
            using (var writer = File.AppendText(Path.GetFullPath(path) + "/build.gradle"))
                writer.WriteLine("\napply from: 'hmsMainTemplate.gradle'");

            string launcherTemplatePath = Application.dataPath + "/Huawei/Plugins/Android/hmsLauncherTemplate.gradle";
            FileUtil.ReplaceFile(launcherTemplatePath, Directory.GetParent(path).FullName + @"/launcher/hmsLauncherTemplate.gradle");
            using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/launcher/build.gradle"))
                writer.WriteLine("\napply from: 'hmsLauncherTemplate.gradle'");

            string baseProjectTemplatePath = Application.dataPath + "/Huawei/Plugins/Android/hmsBaseProjectTemplate.gradle";
            FileUtil.ReplaceFile(baseProjectTemplatePath, Directory.GetParent(path).FullName + @"/hmsBaseProjectTemplate.gradle");

            //TODO: HMSMainKitsTabFactory.GetEnabledEditors() counts zero sometimes
            // Get enabled Kits and check if they are one of the below, because only them needs to be updated to the latest version.
            /*foreach (var toggle in HMSMainKitsTabFactory.GetEnabledEditors())
            {
                if (toggle.GetType() == typeof(AccountToggleEditor)
                    || toggle.GetType() == typeof(PushToggleEditor)
                    || toggle.GetType() == typeof(IAPToggleEditor)
                    || toggle.GetType() == typeof(NearbyServiceToggleEditor)
                    || toggle.GetType() == typeof(AnalyticsToggleEditor))
                {
                    GradleVersionFixer(File.ReadAllText(Directory.GetParent(path).FullName + "/build.gradle"), path);
                }
            }*/


#if UNITY_2022_2_OR_NEWER

        GradleVersionFixer(File.ReadAllText(Directory.GetParent(path).FullName + "/build.gradle"), path);
#else
            GradleVersionFixer(File.ReadAllText(Directory.GetParent(path).FullName + "/build.gradle"), path);
            using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/build.gradle"))
                writer.WriteLine("\napply from: 'hmsBaseProjectTemplate.gradle'");
#endif

            if (HMSMainEditorSettings.Instance.Settings.GetBool(PushToggleEditor.PushKitEnabled))
            {
                string unityPlayerActivityJavaPath = path + @"/src/main/java/com/unity3d/player/UnityPlayerActivity.java";

                var sb = new StringBuilder();
                FileStream fs = new FileStream(unityPlayerActivityJavaPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line;
                    do
                    {
                        line = sr.ReadLine();
                        sb.AppendLine(line);
                    }
                    while (line.Trim().TrimStart().TrimEnd() != "import android.os.Process;");
                    sb.AppendLine("import org.m0skit0.android.hms.unity.push.PushBridge;");

                    do
                    {
                        line = sr.ReadLine();
                        sb.AppendLine(line);
                    }
                    while (line.Trim().TrimStart().TrimEnd() != "mUnityPlayer.newIntent(intent);");
                    sb.AppendLine("PushBridge.OnNotificationMessage(intent);");
                    sb.Append(sr.ReadToEnd());
                }

                using (var sw = new StreamWriter(unityPlayerActivityJavaPath))
                {
                    sw.Write(sb.ToString());
                }
            }

#elif UNITY_2018_1_OR_NEWER
        string hmsMainTemplatePath = Application.dataPath + @"/Huawei/Plugins/Android/hmsMainTemplate.gradle";
        var lines = File.ReadAllLines(hmsMainTemplatePath);

        File.AppendAllLines(path + "/build.gradle", lines);
        GradleVersionFixer(File.ReadAllText(path + "/build.gradle"), path);
        destPath = Path.Combine(path, fileName);
#endif
            if (File.Exists(destPath))
                FileUtil.DeleteFileOrDirectory(destPath);
            File.Copy(filePath, destPath);
        }
    }
}
