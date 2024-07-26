using System;
using System.Collections.Generic;
using System.Linq;
using HmsPlugin;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.ML.TextRecognition;
using HuaweiMobileServices.Utils;
using UnityEngine;

public class HMSMLTextRecognitionKitManager : HMSManagerSingleton<HMSMLTextRecognitionKitManager>
{
    private const string TAG = "[HMS] MLTextRecognitionKitManager ";
    private readonly string apiKey;
    private readonly int defaultUserRegion = MLApplication.REGION_DR_GERMAN;
    private readonly int offlineDefaultDetectMode;
    private readonly LanguageCodes offlineDefaultLanguageCode;
    private readonly int onlineDefaultDetectMode;
    private readonly LanguageCodes onlineDefaultLanguageCode;
    private MLTextAnalyzer mlTextAnalyzer;
    private bool IsTREnabled => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTextRecognitionModule);
    public Dictionary<LanguageCodes, string> LanguageDict { get; } = new Dictionary<LanguageCodes, string>
    {
        { LanguageCodes.English, "en" },
        { LanguageCodes.Chinese, "zh" },
        { LanguageCodes.French, "fr" },
        { LanguageCodes.German, "de" },
        { LanguageCodes.Italian, "it" },
        { LanguageCodes.Japanese, "ja" },
        { LanguageCodes.Korean, "ko" },
        { LanguageCodes.Portuguese, "pt" },
        { LanguageCodes.Russian, "ru" },
        { LanguageCodes.Spanish, "es" },
        { LanguageCodes.Polish, "pl" },
        { LanguageCodes.Norwegian, "no" },
        { LanguageCodes.Swedish, "sv" },
        { LanguageCodes.Danish, "da" },
        { LanguageCodes.Turkish, "tr" },
        { LanguageCodes.Finnish, "fi" },
        { LanguageCodes.Thai, "th" },
        { LanguageCodes.Arabic, "ar" },
        { LanguageCodes.Hindi, "hi" }
    };

    public Action<AndroidIntent, AndroidBitmap> OnImageSelectSuccess;
    public Action<MLText> OnAnalyseFrameSuccess;
    public Action<HMSException> OnAnalyseFrameFailure;

    public HMSMLTextRecognitionKitManager()
    {
        if (!IsTREnabled)
        {
            Debug.LogError($"{TAG}Text Recognition is not enabled");
            return;
        }

        offlineDefaultDetectMode = MLLocalTextSetting.OCR_DETECT_MODE;
        offlineDefaultLanguageCode = LanguageCodes.English;
        onlineDefaultDetectMode = MLRemoteTextSetting.OCR_LOOSE_SCENE;
        onlineDefaultLanguageCode = LanguageCodes.English;
        apiKey = HMSMLKitSettings.Instance.Settings.Get(HMSMLKitSettings.MLKeyAPI);

        HMSManagerStart.Start(OnAwake, TAG);
    }

    private void OnAwake()
    {
        Init();
    }

    public void Init()
    {
        MLApplication instance = MLApplication.Initialize();
        instance.SetApiKey(apiKey);
        instance.SetUserRegion(defaultUserRegion);
        Debug.Log($"{TAG}-> Init");
    }

    public void OnDeviceConfiguration(LanguageCodes? language = null, int? detectMode = null)
    {
        var setting = new MLLocalTextSetting.Factory()
            .SetOCRMode(detectMode ?? offlineDefaultDetectMode)
            .SetLanguage(LanguageDict[language ?? offlineDefaultLanguageCode])
            .Create();

        mlTextAnalyzer = MLAnalyzerFactory.Instance.GetLocalTextAnalyzer(setting);
    }

    public void OnCloudConfiguration(List<LanguageCodes> languages = null, string borderType = null)
    {
        languages ??= new List<LanguageCodes> { onlineDefaultLanguageCode };
        var setting = new MLRemoteTextSetting.Factory()
            .SetBorderType(borderType ?? MLRemoteTextSetting.ARC)
            .SetLanguageList(languages.Select(lang => LanguageDict.GetValueOrDefault(lang)).ToList())
            .SetTextDensityScene(onlineDefaultDetectMode)
            .Create();

        mlTextAnalyzer = MLAnalyzerFactory.Instance.GetRemoteTextAnalyzer(setting);
    }

    public void AnalyzeFrameAsync(MLFrame frame)
    {
        Debug.Log($"{TAG}Analyzing Frame {(frame == null ? "Frame is null" : "Frame is not null")}");
        var task = mlTextAnalyzer.AnalyseFrameAsync(frame);

        task.AddOnSuccessListener(result =>
        {
            Debug.Log($"{TAG}Text Recognition Success & Result: {result}");
            OnAnalyseFrameSuccess?.Invoke(result);
        }).AddOnFailureListener(exception =>
        {
            Debug.Log($"{TAG}Text Recognition Failed: {exception.WrappedExceptionMessage}");
            OnAnalyseFrameFailure?.Invoke(exception);
        });
    }

    public IList<MLText.Block> AnalyzeFrame(MLFrame frame)
    {
        Debug.Log($"{TAG}Analyzing Frame {(frame == null ? "Frame is null" : "Frame is not null")}");
        var result = mlTextAnalyzer.AnalyseFrame(frame);
        Debug.Log($"{TAG}Text Recognition Success & Result: {result}");
        return result;
    }

    public void SelectImage()
    {
        AndroidToast.MakeText("Please select an image file").Show();
        AndroidFilePicker.mOnSuccessListener = OnSuccessFilePicker;
        AndroidFilePicker.OpenFilePicker("image/*");
    }

    private void OnSuccessFilePicker(AndroidIntent androidIntent, AndroidBitmap bitmap)
    {
        Debug.Log($"{TAG}FilePicker: Success: {androidIntent.GetData()?.GetPath}");
        Debug.Log($"{TAG}FilePickerBitmap: Success: {(bitmap == null ? "null" : "not null")}");
        OnImageSelectSuccess?.Invoke(androidIntent, bitmap);
    }

    public enum LanguageCodes
    {
        English,
        Chinese,
        French,
        German,
        Italian,
        Japanese,
        Korean,
        Portuguese,
        Russian,
        Spanish,
        Polish,
        Norwegian,
        Swedish,
        Danish,
        Turkish,
        Finnish,
        Thai,
        Arabic,
        Hindi
    }

}

