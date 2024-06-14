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
        private const string TAG = "MLTextToSpeechListenerManager";
        public Action<string> OnAudioAvailableAction;
        public Action<string, MLTtsAudioFragment, int, Tuple<int, int>, Bundle> OnAudioAvailableBundleAction;
        public Action<string, MLTtsError> OnErrorAction;
        public Action<string, int, AndroidJavaObject> OnEventAction;
        public Action<string> OnRangeStartAction;
        public Action<string, MLTtsWarn> OnWarnAction;
        public MLTextToSpeechListenerManager()
        {
            Debug.Log($"{TAG} created");
        }
        public void OnAudioAvailable(string taskId, MLTtsAudioFragment audioFragment, int offset, Tuple<int, int> keyValuePairs, Bundle bundle)
        {
            Debug.Log($"{TAG} OnAudioAvailable: {taskId}");
            Debug.Log($"{TAG} AudioFragment AudioFormat: {audioFragment.GetAudioFormat()}");
            Debug.Log($"{TAG} AudioFragment ChannelInfo: {audioFragment.GetChannelInfo()}");
            Debug.Log($"{TAG} AudioFragment SampleRateInHz: {audioFragment.GetSampleRateInHz()}");
            Debug.Log($"{TAG} AudioFragment AudioDatalenght: {audioFragment.GetAudioData().Length}");
            Debug.Log($"{TAG} AudioFragment SAMPLE_RATE_16K: {MLTtsAudioFragment.SAMPLE_RATE_16K}");
            Debug.Log($"{TAG} Bundle: {bundle.Size()}");
            Debug.Log($"{TAG} Offset: {offset}");
            Debug.Log($"{TAG} Key Value Pairs: {keyValuePairs?.Item1} - {keyValuePairs?.Item2}");
            foreach (var item in audioFragment.GetAudioData())
            {
                Debug.Log($"{TAG} Audio Data: {item}");
            }
            OnAudioAvailableAction?.Invoke(taskId);
        }

        public void OnError(string taskId, MLTtsError err)
        {
            Debug.Log($"{TAG} OnError: {taskId}");
            OnErrorAction?.Invoke(taskId, err);
        }

        public void OnEvent(string taskId, int eventId, AndroidJavaObject bundle)
        {
            Debug.Log($"{TAG} OnEvent: {taskId}");
            Debug.Log($"{TAG} EventId: {eventId}");
            // Callback method of a TTS event. eventId indicates the event name.
            bool isInterrupted = false;
            switch (eventId)
            {
                //EVENT_PLAY_START: Called when playback starts.
                case 1:
                    Debug.Log($"{TAG} EVENT_PLAY_START");
                    break;
                //EVENT_PLAY_STOP: Called when playback stops.
                case 4:
                    Debug.Log($"{TAG} EVENT_PLAY_STOP");
                    isInterrupted = bundle.Get<bool>(MLTtsConstants.EVENT_PLAY_STOP_INTERRUPTED);
                    break;
                //EVENT_PLAY_RESUME: Called when playback resumes.
                case 2:
                    Debug.Log($"{TAG} EVENT_PLAY_RESUME");
                    // Called when playback resumes.
                    break;
                //EVENT_PLAY_PAUSE: Called when playback pauses.
                case 3:
                    Debug.Log($"{TAG} EVENT_PLAY_PAUSE");
                    break;
                //EVENT_SYNTHESIS_START: Called when TTS starts.
                // Pay attention to the following callback events when you focus on only synthesized audio data but do not use the internal player for playback:
                case 5:
                    Debug.Log($"{TAG} EVENT_SYNTHESIS_START");
                    // Called when TTS starts.
                    break;
                //EVENT_SYNTHESIS_END: Called when TTS ends.
                case 6:
                    Debug.Log($"{TAG} EVENT_SYNTHESIS_END");
                    // Called when TTS ends.
                    break;
                //EVENT_SYNTHESIS_COMPLETE: Called when TTS is complete. All synthesized audio streams are passed to the app.
                case 7:
                    Debug.Log($"{TAG} EVENT_SYNTHESIS_COMPLETE");
                    // TTS is complete. All synthesized audio streams are passed to the app.
                    isInterrupted = bundle.Get<bool>(MLTtsConstants.EVENT_SYNTHESIS_INTERRUPTED);
                    break;
                default:
                    break;
            }
            Debug.Log($"{TAG} isInterrupted: {isInterrupted}");
            OnEventAction?.Invoke(taskId, eventId, bundle);
        }

        public void OnRangeStart(string taskId, int start, int end)
        {
            Debug.Log($"{TAG} OnRangeStart: {taskId}");
            OnRangeStartAction?.Invoke(taskId);
        }

        public void OnWarn(string taskId, MLTtsWarn warn)
        {
            Debug.Log($"{TAG} OnWarn: {taskId}");
            OnWarnAction?.Invoke(taskId, warn);
        }
    }
}
