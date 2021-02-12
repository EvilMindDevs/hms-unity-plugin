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
        string destPath = Path.Combine(Directory.GetParent(path).FullName + "//launcher", fileName);
        if (File.Exists(destPath))
            File.Delete(destPath);
        File.Copy(filePath, destPath);
    }
}
