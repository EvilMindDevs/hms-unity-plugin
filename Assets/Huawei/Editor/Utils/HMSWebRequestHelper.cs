using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

internal class HMSWebRequestHelper
{
    private static GameObject persistedObj;

    static HMSWebRequestHelper()
    {
        if (persistedObj == null)
        {
            if (GameObject.Find("[HMSWebRequestHelper]") != null)
                persistedObj = GameObject.Find("[HMSWebRequestHelper]");
            else
            {
                persistedObj = new GameObject("[HMSWebRequestHelper]");
                //persistedObj.hideFlags = HideFlags.HideAndDontSave;
                persistedObj.AddComponent<HMSWebRequestBehaviour>();
            }
        }
    }

    internal static void PostRequest(string url, string bodyJsonString, Action<UnityWebRequest> callback)
    {
        persistedObj.GetComponent<HMSWebRequestBehaviour>().Post(url, bodyJsonString, callback);
    }
}

public class HMSWebRequestBehaviour : MonoBehaviour
{
    public void Post(string url, string bodyJsonString, Action<UnityWebRequest> callback)
    {
        StartCoroutine(PostCoroutine(url, bodyJsonString, callback));
    }

    private IEnumerator PostCoroutine(string url, string bodyJsonString, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
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
                    Debug.LogError("HMSWebRequestHelper encountered an unknown error");
                }
                else
                {
                    Debug.LogError("HMSWebRequestHelper encountered an error: " + request.error);
                }
                yield break;
            }

            callback(request);
        }
    }
}