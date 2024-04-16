using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public static class HMSUnityPluginFAQ
    {
        [MenuItem("Huawei/FAQ")]
        private static void FAQ()
        {
            Application.OpenURL("https://evilminddevs.gitbook.io/hms-unity-plugin/support/faq");
        }
    }
}