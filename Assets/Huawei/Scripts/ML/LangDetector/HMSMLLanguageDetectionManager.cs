using HmsPlugin;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.ML.LanguageDetection;
using HuaweiMobileServices.ML.LanguageDetection.Cloud;
using HuaweiMobileServices.ML.LanguageDetection.Local;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Huawei.Scripts.ML.LangDetector
{
    public enum DetectionMode
    {
        Online = 0,
        Offline = 1
    }
    public class HMSMLLanguageDetectionManager : HMSManagerSingleton<HMSMLLanguageDetectionManager>
    {

        private const string TAG = "[HMS] HMSMLLanguageDetectionManager ";
        private readonly string apiKey;
        private readonly int DefaultUserRegion = MLApplication.REGION_DR_GERMAN;
        private DetectionMode currentDetectionMode;

        private MLRemoteLangDetectorSetting remoteSetting;
        private MLRemoteLangDetector remoteDetector;
        private MLLocalLangDetectorSetting localSetting;
        private MLLocalLangDetector localDetector;

        private bool IsLangDetectionModuleEnabled => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableLanguageDetectionModule);

        public HMSMLLanguageDetectionManager()
        {
            if (!IsLangDetectionModuleEnabled)
            {
                Debug.Log($"{TAG} -> Lang Detection Module is not enabled");
                return;
            }
            apiKey = HMSMLKitSettings.Instance.Settings.Get(HMSMLKitSettings.MLKeyAPI);
            HMSManagerStart.Start(OnAwake, TAG);
        }
        private void OnAwake()
        {
            Initialize();
        }
        private void Initialize()
        {
            var instance = MLApplication.Initialize();
            instance.SetApiKey(apiKey);
            instance.SetUserRegion(DefaultUserRegion);
            Debug.Log($"{TAG} -> Initialized");
        }

        public void Configure(DetectionMode detectionMode)
        {
            currentDetectionMode = detectionMode;
            switch (detectionMode)
            {
                case DetectionMode.Online:
                    SetOnlineDetection();
                    break;
                case DetectionMode.Offline:
                    SetOfflineDetection();
                    break;
            }
        }


        private void SetOnlineDetection(float f = 0.01f)
        {
            
            remoteSetting = new MLRemoteLangDetectorSetting.Factory().SetTrustedThreshold(f).Create();

            remoteDetector = MLLangDetectorFactory.GetInstance().GetRemoteLangDetector(remoteSetting); 
        }
        private void SetOfflineDetection(float f = 0.01f)
        {
          
            localSetting = new MLLocalLangDetectorSetting.Factory().SetTrustedThreshold(f).Create();

            localDetector = MLLangDetectorFactory.GetInstance().GetLocalLangDetector(localSetting); 
        }


        public void ProbilityDetectLocal(string text, Action<IList<MLDetectedLang>> success, Action<HMSException> failure)
        {
            localDetector.ProbabilityDetect(text).AddOnSuccessListener((result) =>
            {
                Debug.Log($"{TAG} PROBILITY DETECT LOCAL -> {result}");
                success?.Invoke(result);

            }).AddOnFailureListener((error) =>
            {
                Debug.LogError($"{TAG} PROBILITY DETECT LOCAL -> Error: {error.WrappedCauseMessage}");
                failure?.Invoke(error);
            });
        }
        public void FirstBestDetectLocal(string text, Action<string> success, Action<HMSException> failure)
        {
           
            localDetector.FirstBestDetect(text).AddOnSuccessListener((result) =>
            {
                Debug.Log($"{TAG} FIRST BEST DETECT LOCAL -> {result}");
                success?.Invoke(result);
            }).AddOnFailureListener((error) =>
            {
                Debug.LogError($"{TAG} FIRST BEST DETECT LOCAL -> Error: {error.WrappedCauseMessage}");
                failure?.Invoke(error);
            });
        }

        public void ProbilityDetectRemote(string text, Action<IList<MLDetectedLang>> success, Action<HMSException> failure)
        {
            remoteDetector.ProbabilityDetect(text).AddOnSuccessListener((result) =>
            {
                Debug.Log($"{TAG} PROBILITY DETECT REMOTE -> {result}");
                success?.Invoke(result);

            }).AddOnFailureListener((error) =>
            {
                Debug.LogError($"{TAG}  PROBILITY DETECT REMOTE -> Error: {error.WrappedCauseMessage}");
                failure?.Invoke(error);
            });
        }
        public void FirstBestDetectRemote(string text, Action<string> success, Action<HMSException> failure)

        {
            
            remoteDetector.FirstBestDetect(text).AddOnSuccessListener((result) =>
            {
                Debug.Log($"{TAG} FIRST BEST DETECT REMOTE -> {result}");
                success?.Invoke(result);
            }).AddOnFailureListener((error) =>
            {
                Debug.LogError($"{TAG}  FIRST BEST DETECT REMOTE -> Error: {error.WrappedCauseMessage}");
                failure?.Invoke(error);

            });
        }
        public IList<MLDetectedLang> SyncProbilityDetectLocal(string text)
        {
            return localDetector.SyncProbabilityDetect(text); 
        }

        public string SyncFirstBestDetectLocal(string text)
        {
            return localDetector.SyncFirstBestDetect(text); 
        }

        public void StopLocalLangDetector()
        {
            if (localDetector != null)
            {
                localDetector.Stop();
            }
        }


        public IList<MLDetectedLang> SyncProbilityDetectRemote(string text)
        {
            return remoteDetector.SyncProbabilityDetect(text); 
        }

        public string SyncFirstBestDetectRemote(string text)
        {
            return remoteDetector.SyncFirstBestDetect(text); 
        }

        public void StopRemoteLangDetector()
        {
            if (remoteDetector != null)
            {
                remoteDetector.Stop();
            }
        }

    }
}
