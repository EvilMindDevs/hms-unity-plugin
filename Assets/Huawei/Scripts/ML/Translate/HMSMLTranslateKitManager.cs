using System;
using System.Collections.Generic;
using HmsPlugin;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.ML.DownloadModel;
using HuaweiMobileServices.ML.Translate;
using HuaweiMobileServices.ML.Translate.Cloud;
using HuaweiMobileServices.ML.Translate.Local;
using HuaweiMobileServices.Utils;
using UnityEngine;

public class HMSMLTranslateKitManager : HMSManagerSingleton<HMSMLTranslateKitManager>
{
    private const string Tag = "[HMS] HMS MLTranslateKitManager";
    private readonly int DefaultUserRegion = MLApplication.REGION_DR_GERMAN;

    public Action<ISet<string>> OnGetCloudAllLanguagesSuccess;
    public Action<ISet<string>> OnGetLocalAllLanguagesSuccess;
    public Action OnDownloadModelSuccess;
    public Action<long, long> OnProcessAction;

    private readonly string apiKey;
    private MLRemoteTranslateSetting remoteSetting;
    private MLRemoteTranslator remoteTranslator;
    private MLLocalTranslateSetting localSetting;
    private MLLocalTranslator localTranslator;
    private string defaultSourceLanguage = "en";
    private string defaultTargetLanguage = "tr";
    public string SelectedSourceLanguage { get; private set; }
    public string SelectedTargetLanguage { get; private set; }
    private TranslateMode currentTranslateMode;
    public enum TranslateMode
    {
        Online = 0,
        Offline = 1
    }
    private bool IsTranslateModuleEnabled => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTranslateModule);

    public HMSMLTranslateKitManager()
    {
        if (!IsTranslateModuleEnabled)
        {
            Debug.Log($"{Tag} -> Translate Module is not enabled");
            return;
        }
        apiKey = HMSMLKitSettings.Instance.Settings.Get(HMSMLKitSettings.MLKeyAPI);

        HMSManagerStart.Start(OnAwake, Tag);
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
        Debug.Log($"{Tag} -> Initialized");
    }

    public void Configure(TranslateMode translateMode)
    {
        currentTranslateMode = translateMode;
        switch (translateMode)
        {
            case TranslateMode.Online:
                SetOnlineTranslate();
                break;
            case TranslateMode.Offline:
                SetOfflineTranslate();
                break;
        }
    }

    public void SetSourceLanguage(string language)
    {
        if (string.IsNullOrEmpty(language))
            return;

        SelectedSourceLanguage = language;
        ConfigureTranslation();
    }

    public void SetTargetLanguage(string language)
    {
        if (string.IsNullOrEmpty(language))
            return;

        SelectedTargetLanguage = language;
        ConfigureTranslation();
    }

    private void ConfigureTranslation()
    {
        Debug.Log($"{Tag} ConfigureTranslation -> {SelectedSourceLanguage} -> {SelectedTargetLanguage}");
        if (currentTranslateMode == TranslateMode.Offline && SelectedSourceLanguage != SelectedTargetLanguage)
        {
            SetOfflineTranslate();
            DownloadModel(SelectedSourceLanguage);
            DownloadModel(SelectedTargetLanguage);
        }
        else
        {
            SetOnlineTranslate();
        }
    }

    private void SetOnlineTranslate()
    {
        remoteSetting = new MLRemoteTranslateSetting.Factory()
            .SetSourceLangCode(SelectedSourceLanguage ?? defaultSourceLanguage)
            .SetTargetLangCode(SelectedTargetLanguage ?? defaultTargetLanguage)
            .Create();

        remoteTranslator = MLTranslatorFactory.Instance.GetRemoteTranslator(remoteSetting);
    }

    private void SetOfflineTranslate()
    {
        localSetting = new MLLocalTranslateSetting.Factory()
            .SetSourceLangCode(SelectedSourceLanguage ?? defaultSourceLanguage)
            .SetTargetLangCode(SelectedTargetLanguage ?? defaultTargetLanguage)
            .Create();

        localTranslator = MLTranslatorFactory.Instance.GetLocalTranslator(localSetting);
    }

    public void GetLocalAllLanguages()
    {
        MLTranslateLanguage.GetLocalAllLanguagesAsync().AddOnSuccessListener((result) =>
        {
            OnGetLocalAllLanguagesSuccess?.Invoke(result);
            Debug.Log($"{Tag} GetLocalAllLanguages count -> {result.Count}");
        }).AddOnFailureListener((error) =>
        {
            Debug.LogError($"{Tag} GetLocalAllLanguages -> Error: {error.WrappedCauseMessage}");
        });
    }

    public void GetCloudAllLanguages()
    {
        MLTranslateLanguage.GetCloudAllLanguagesAsync().AddOnSuccessListener((result) =>
        {
            OnGetCloudAllLanguagesSuccess?.Invoke(result);
            Debug.Log($"{Tag} GetCloudAllLanguages -> {result.Count}");
        }).AddOnFailureListener((error) =>
        {
            Debug.LogError($"{Tag} GetCloudAllLanguages -> Error: {error.WrappedCauseMessage}");
        });
    }

    public void Translate(string text, Action<string> success, Action<HMSException> failure)
    {
        remoteTranslator.TranslateAsync(text).AddOnSuccessListener((result) =>
        {
            Debug.Log($"{Tag} Translate Online -> {result}");
            success?.Invoke(result);
        }).AddOnFailureListener((error) =>
        {
            Debug.LogError($"{Tag} Translate Online -> Error: {error.WrappedCauseMessage}");
            failure?.Invoke(error);
        });
    }

    public void TranslateOffline(string text, Action<string> success, Action<HMSException> failure)
    {
        localTranslator.TranslateAsync(text).AddOnSuccessListener((result) =>
        {
            Debug.Log($"{Tag} Translate Offline -> {result}");
            success?.Invoke(result);
        }).AddOnFailureListener((error) =>
        {
            Debug.LogError($"{Tag} Translate Offline -> Error: {error.WrappedCauseMessage}");
            failure?.Invoke(error);
        });
    }

    public void DownloadModel(string languageCode)
    {
        Debug.Log($"{Tag} DownloadModel -> {languageCode}");

        var manager = MLLocalModelManager.Instance;
        var model = new MLLocalTranslatorModel.Factory(languageCode.ToUpper()).Create();
        var downloadStrategy = new MLModelDownloadStrategy.Factory()
            .NeedWifi()
            .SetRegion(DefaultUserRegion)
            .Create();

        var listener = new MLModelDownloadListener(new MLDownloadListenerManager { OnProcessAction = OnProcessAction });

        manager.DownloadModel(model, downloadStrategy, listener).AddOnSuccessListener((result) =>
        {
            OnDownloadModelSuccess?.Invoke();
            Debug.Log($"{Tag} DownloadModel -> {result}");
        }).AddOnFailureListener((error) =>
        {
            Debug.LogError($"{Tag} DownloadModel -> Error: {error.WrappedCauseMessage}");
        });
    }
}
