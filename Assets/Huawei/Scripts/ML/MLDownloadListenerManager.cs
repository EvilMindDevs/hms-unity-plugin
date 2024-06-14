using System;
using UnityEngine;
using static HuaweiMobileServices.ML.DownloadModel.MLModelDownloadListener;

public class MLDownloadListenerManager : IMLModelDownloadListener
{
    public Action<long, long> OnProcessAction;
    public MLDownloadListenerManager()
    {
        Debug.Log($"{this.GetType().Name} was created");
    }
    public void OnProcess(long total, long current)
    {
        OnProcessAction?.Invoke(total, current);
    }

}
