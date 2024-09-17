using HmsPlugin;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.ML.SkeletonDetection;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;


public class HMSMLSkeletonDetectionManager : HMSManagerSingleton<HMSMLSkeletonDetectionManager>
{

    private const string TAG = "[HMS] HMSMLSkeletonDetectionManager";
    public Action<AndroidIntent, AndroidBitmap> OnImageSelectSuccess;
    private MLSkeletonAnalyzerSetting mLSkeletonAnalyzerSetting;
    private MLSkeletonAnalyzer mLSkeletonAnalyzer;
    private readonly string apiKey;


    private bool IsTREnabled => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableSkeletonDetectionModule);


    public HMSMLSkeletonDetectionManager()
    {
        if (!IsTREnabled)
        {
            Debug.LogError($"{TAG}Text Recognition is not enabled");
            return;
        }
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

        Debug.Log($"{TAG} API KEY-> {apiKey}");

        instance.SetApiKey(apiKey);
      
        Debug.Log($"{TAG}-> Init");
    }


    public void InitConfiguration(int analyzerType) {

        Debug.Log($"{TAG} InitConfiguration");

        mLSkeletonAnalyzerSetting = new MLSkeletonAnalyzerSetting.Factory().SetAnalyzerType(analyzerType).Create();

        mLSkeletonAnalyzer = MLSkeletonAnalyzerFactory.GetInstance().GetSkeletonAnalyzer(mLSkeletonAnalyzerSetting);
        Debug.Log($"{TAG} InitConfiguration {(mLSkeletonAnalyzer == null ? "null" : "not null")}");
    }


    public IList<MLSkeleton> AnalyzeFrameSync(MLFrame mLFrame) {

        var result = mLSkeletonAnalyzer.AnalyseFrame(mLFrame);
        return result;

    }


    public void AnalyzeFrameAsync(MLFrame mLFrame, Action<IList<MLSkeleton>> success, Action<HMSException> failure)
    {
        Debug.Log($"{TAG} ANALYZE FRAME ASYNC");

        Debug.Log($"{TAG} AnalyzeFrameAsync {(mLSkeletonAnalyzer == null ? "null" : "not null")}");

        var task = mLSkeletonAnalyzer.AsyncAnalyseFrame(mLFrame);
        Debug.Log($"{TAG} AnalyzeFrameAsync {(task == null ? "null" : "not null")}");
        task.AddOnSuccessListener((result) =>
        {
            Debug.Log($"{TAG} ANALYZE FRAME ASYNC -> {result}");
            success?.Invoke(result);

        }).AddOnFailureListener((error) =>
        {
            Debug.LogError($"{TAG} ANALYZE FRAME ASYNC -> Error: {error.WrappedCauseMessage}");
            failure?.Invoke(error);
        });
    }


    public void StopSkeletonDetection()
    {
        if (mLSkeletonAnalyzer != null)
        {
            mLSkeletonAnalyzer.Stop();
        }
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
}

//TODO: Camera stream detection will add next sprint for Skeleton and other image detection kits
