using System;
using UnityEngine;
using static HuaweiMobileServices.ML.DownloadModel.MLModelDownloadListener;

public class MLTextToSpeechDownloadListenerManager : IMLModelDownloadListener
{
    public Action<long, long> OnProcessAction;
    public MLTextToSpeechDownloadListenerManager()
    {
        Debug.Log("MLTextToSpeechDownloadListenerManager created");
    }
    public void OnProcess(long total, long current)
    {
        //Debug.Log("MLTextToSpeechDownloadListenerManager -> OnProcess: " + total + " -> " + current);
        OnProcessAction?.Invoke(total, current);
    }

}
