using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HmsPlugin
{
    internal class HMSPluginUpdater
    {
        public static string sessionState = "hms_checked_update";

        static HMSPluginUpdateRequest request;
        
        static bool ignorePlatform
        {
            get
            {
#if UNITY_SWITCH || UNITY_GAMECORE || UNITY_PS4 || UNITY_PS5
                return true;
#else
                return false;
#endif
            }
        }

        internal static void Request(bool ignoreSession = false)
        {
            if (ignorePlatform)
            {
                return;
            }
            
            if (!ignoreSession && SessionState.GetBool(sessionState, false))
            {
                return;
            }

            SessionState.SetBool(sessionState, true);
            HMSEditorUtils.UpdateAssemblyDefinitions(HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true));

            Task.Delay(2000).ContinueWith(t => StartUpdateRequest(ignoreSession), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static void StartUpdateRequest(bool ignoreSession)
        {
            var obj = new GameObject("HMSPluginUpdateRequest")
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            request = obj.AddComponent<HMSPluginUpdateRequest>();
            request.StartRequest(ignoreSession);
        }
    }

    public class HMSPluginUpdateRequest : MonoBehaviour
    {
        public static string sessionStatePersisted = "hms_checked_update_persisted";

        private IEnumerator coroutine;

        public void StartRequest(bool ignoreSession = false)
        {
            coroutine = FetchLatestVersion(ignoreSession);
            StartCoroutine(coroutine);
        }

        private IEnumerator FetchLatestVersion(bool ignoreSession)
        {
            var request = UnityWebRequest.Get("https://api.github.com/repos/EvilMindDevs/hms-unity-plugin/tags");
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            var requestError =
               request.result == UnityWebRequest.Result.ProtocolError ||
               request.result == UnityWebRequest.Result.ConnectionError;
#else
            bool requestError =
               request.isNetworkError ||
               request.isHttpError;
#endif

            if (requestError)
            {
                if (request.error == null)
                {
                    Debug.LogError("HMS Plugin Update Checker encountered an unknown error");
                }
                else
                {
                    Debug.LogError("HMS Plugin Update Checker encountered an error: " + request.error);
                }
                DestroyImmediate(gameObject);
                yield break;
            }

            var json = JsonUtility.FromJson<TagList>("{\"tags\":" + request.downloadHandler.text + "}");
            string latestVersionString = FindTheLatestVersion(json);
            string currentVersionString = File.ReadAllText(Application.dataPath + "/Huawei/VERSION");

            long latestVersion = ConvertVersionToLong(latestVersionString);
            long currentVersion = ConvertVersionToLong(currentVersionString);

            if (latestVersion > currentVersion)
            {
                string updateMessage = $"A new version of the HMS Unity Plugin ({latestVersionString}) is available. You are currently using {currentVersionString}";
                Debug.LogWarning($"{updateMessage}\nYou can check releases page of our github. https://github.com/EvilMindDevs/hms-unity-plugin/releases");

                if (!ignoreSession)
                {
                    if (SessionState.GetBool(sessionStatePersisted, false)) yield return null;
                    SessionState.SetBool(sessionStatePersisted, true);
                    if (!EditorPrefs.GetBool($"{sessionStatePersisted}{latestVersionString}", false))
                        DisplayDialog(latestVersionString, updateMessage);
                }
                else
                {
                    DisplayDialog(latestVersionString, updateMessage);
                }
            }
            else if (ignoreSession)
            {
                EditorUtility.DisplayDialog("HMS Unity Plugin", "Your version is up to date", "Ok");
            }


            DestroyImmediate(gameObject);
        }

        private string FindTheLatestVersion(TagList list)
        {
            string latestVersion = "0.0.0";

            foreach (var tag in list.tags)
            {
                string tempVer = tag.name.RemoveAfter('-').Replace("v", "");

                if (ConvertVersionToLong(latestVersion) < ConvertVersionToLong(tempVer))
                {
                    latestVersion = tempVer;
                }
            }

            return latestVersion;
        }

        // Helper method to convert version string to long
        private long ConvertVersionToLong(string versionString)
        {
            if (long.TryParse(versionString.Replace(".", "0").PadRight(8, '0'), out long version))
            {
                return version;
            }
            else
            {
                // Handle the case where the version string could not be parsed to a long
                // This could be setting a default value, logging an error, etc.
                return 0;
            }
        }

        private static void DisplayDialog(string latestVersionString, string updateMessage)
        {
            int option = EditorUtility.DisplayDialogComplex("HMS Unity Plugin Update available", updateMessage + ". Do you want to download the new version ?", "Don't Remind", "No", "Yes");
            switch (option)
            {
                case 0:
                    EditorPrefs.SetBool(sessionStatePersisted + latestVersionString, true);
                    break;
                case 1:
                    // No action needed
                    break;
                case 2:
                    Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/releases");
                    break;
                default:
                    // All cases are handled
                    break;
            }
        }

        [Serializable]
        private class Commit
        {
            public string sha;
            public string url;
        }

        [Serializable]
        private class Tag
        {
            public string name;
            public string zipball_url;
            public string tarball_url;
            public Commit commit;
            public string node_id;
        }

        [Serializable]
        private class TagList
        {
            public Tag[] tags;
        }
    }
}
