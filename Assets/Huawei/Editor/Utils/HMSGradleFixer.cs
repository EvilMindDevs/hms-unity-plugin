using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Android;
using UnityEngine;

public class HMSGradleFixer : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 1;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string fileName = "agconnect-services.json";
        var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string destPath = "";
#if UNITY_2019
        destPath = Path.Combine(Directory.GetParent(path).FullName + "//launcher", fileName);
#elif UNITY_2018
        destPath = Path.Combine(path, fileName);
#endif
        if (File.Exists(destPath))
            File.Delete(destPath);
        File.Copy(filePath, destPath);
    }
}
