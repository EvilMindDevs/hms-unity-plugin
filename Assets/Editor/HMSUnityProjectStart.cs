using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class HMSUnityProjectStart
{
    static HMSUnityProjectStart()
    {
        EditorApplication.update += RunOnce;
    }

    static void RunOnce()
    {
        HMSConfigureGitStandards.Start();

        EditorApplication.update -= RunOnce;
    }

}