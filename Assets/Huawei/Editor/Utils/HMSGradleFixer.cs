using HmsPlugin;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

public class HMSGradleFixer : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 1;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string fileName = "agconnect-services.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string destPath = "";
#if UNITY_2019_3_OR_NEWER || UNITY_2020
        destPath = Path.Combine(Directory.GetParent(path).FullName + "//launcher", fileName);

        string hmsMainTemplatePath = Application.dataPath + "/Plugins/Android/hmsMainTemplate.gradle";
        FileUtil.CopyFileOrDirectory(hmsMainTemplatePath, Path.GetFullPath(path) + @"/hmsMainTemplate.gradle");
        using (var writer = File.AppendText(Path.GetFullPath(path) + "/build.gradle"))
            writer.WriteLine("apply from: 'hmsMainTemplate.gradle'");

        string launcherTemplatePath = Application.dataPath + "/Plugins/Android/hmsLauncherTemplate.gradle";
        FileUtil.CopyFileOrDirectory(launcherTemplatePath, Directory.GetParent(path).FullName + @"/launcher/hmsLauncherTemplate.gradle");
        using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/launcher/build.gradle"))
            writer.WriteLine("apply from: 'hmsLauncherTemplate.gradle'");

        string baseProjectTemplatePath = Application.dataPath + "/Plugins/Android/hmsBaseProjectTemplate.gradle";
        FileUtil.CopyFileOrDirectory(baseProjectTemplatePath, Directory.GetParent(path).FullName + @"/hmsBaseProjectTemplate.gradle");
        using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/build.gradle"))
            writer.WriteLine("apply from: 'hmsBaseProjectTemplate.gradle'");

        if (HMSMainEditorSettings.Instance.Settings.GetBool(PushToggleEditor.PushKitEnabled))
        {
            string unityPlayerActivityJavaPath = path + @"\src\main\java\com\unity3d\player\UnityPlayerActivity.java";

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
        string hmsMainTemplatePath = Application.dataPath + @"/Plugins/Android/hmsMainTemplate.gradle";
        var lines = File.ReadAllLines(hmsMainTemplatePath);

        File.AppendAllLines(path + "/build.gradle", lines);
        destPath = Path.Combine(path, fileName);
#endif
        if (File.Exists(destPath))
            FileUtil.DeleteFileOrDirectory(destPath);
        FileUtil.CopyFileOrDirectory(filePath, destPath);
    }
}
