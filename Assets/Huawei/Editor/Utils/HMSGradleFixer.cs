using HmsPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

public class HMSGradleFixer : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 1;
    private const string MINGRADLEVERSION = "3.5.4";

    private void GradleVersionFixer(string gradleFileAsString, string path)
    {
        string gradleRowPattern = @".*gradle:(\d\.?)+";
        string gradleVersionPattern = @"(\d\.?)+";
        Version gradleMinVersion = Version.Parse(MINGRADLEVERSION);

        Match gradleRowMatch = Regex.Match(gradleFileAsString, gradleRowPattern);
        Match gradleVersionMatch = Regex.Match(gradleRowMatch.Value, gradleVersionPattern);
        Version gradleVersion = Version.Parse(gradleVersionMatch.Value);
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
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
        {
            HMSEditorUtils.HandleAssemblyDefinitions(false);
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
        GradleVersionFixer(File.ReadAllText(Directory.GetParent(path).FullName + "/build.gradle"), path);

        using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/build.gradle"))
            writer.WriteLine("\napply from: 'hmsBaseProjectTemplate.gradle'");

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
