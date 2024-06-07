using System.Collections.Generic;
using HuaweiMobileServices.ML.TextToSpeech;
using static HuaweiMobileServices.ML.TextToSpeech.MLTtsCallback;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class MLTextToSpeechListenerManager : IMLTtsCallback
    {
        public Action<string> OnAudioAvailableAction;
        public Action<string, MLTtsAudioFragment, int, Tuple<int, int>, Bundle> OnAudioAvailableBundleAction;
        public Action<string, MLTtsError> OnErrorAction;
        public Action<string, int, AndroidJavaObject> OnEventAction;
        public Action<string> OnRangeStartAction;
        public Action<string, MLTtsWarn> OnWarnAction;
        public MLTextToSpeechListenerManager()
        {
            Debug.Log("MLTextToSpeechListenerManager created");
        }
        public void OnAudioAvailable(string taskId, MLTtsAudioFragment audioFragment, int offset, Tuple<int, int> keyValuePairs, Bundle bundle)
        {
            Debug.Log($"OnAudioAvailable: {taskId}");
            Debug.Log($"AudioFragment AudioFormat: {audioFragment.GetAudioFormat()}");
            Debug.Log($"AudioFragment SAMPLE_RATE_16K: {MLTtsAudioFragment.SAMPLE_RATE_16K}");
            Debug.Log($"Bundle: {bundle}");
            Debug.Log($"Offset: {offset}");
            Debug.Log($"Key Value Pairs: {keyValuePairs?.Item1} - {keyValuePairs?.Item2}");
        }

        public void OnError(string taskId, MLTtsError err)
        {
            Debug.Log($"OnError: {taskId}");
            OnErrorAction?.Invoke(taskId, err);
        }

        public void OnEvent(string taskId, int eventId, AndroidJavaObject bundle)
        {
            Debug.Log($"OnEvent: {taskId}");
            OnEventAction?.Invoke(taskId, eventId, bundle);
        }

        public void OnRangeStart(string taskId, int start, int end)
        {
            Debug.Log($"OnRangeStart: {taskId}");
            OnRangeStartAction?.Invoke(taskId);
        }

        public void OnWarn(string taskId, MLTtsWarn warn)
        {
            Debug.Log($"OnWarn: {taskId}");
            OnWarnAction?.Invoke(taskId, warn);
        }
    }
}
