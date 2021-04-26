using System.Collections;
using System.Collections.Generic;
using System.IO;
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
#if UNITY_2019  || UNITY_2020
        destPath = Path.Combine(Directory.GetParent(path).FullName + "//launcher", fileName);

        string mainTemplatePath = Application.dataPath + "/Plugins/Android/hmsMainTemplate.gradle";
        FileUtil.CopyFileOrDirectory(mainTemplatePath, Path.GetFullPath(path) + @"/hmsMainTemplate.gradle");

        string launcherTemplatePath = Application.dataPath + "/Plugins/Android/hmsLauncherTemplate.gradle";
        FileUtil.CopyFileOrDirectory(launcherTemplatePath, Directory.GetParent(path).FullName + @"/launcher/hmsLauncherTemplate.gradle");

        string baseProjectTemplatePath = Application.dataPath + "/Plugins/Android/hmsBaseProjectTemplate.gradle";
        FileUtil.CopyFileOrDirectory(baseProjectTemplatePath, Directory.GetParent(path).FullName + @"/hmsBaseProjectTemplate.gradle");

#elif UNITY_2018
        destPath = Path.Combine(path, fileName);
#endif
        if (File.Exists(destPath))
            FileUtil.DeleteFileOrDirectory(destPath);
        FileUtil.CopyFileOrDirectory(filePath, destPath);


    }
}
