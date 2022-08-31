using HuaweiMobileServices.Scan;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;

public class HMSScanKitManager : HMSManagerSingleton<HMSScanKitManager>
{
    private readonly string TAG = "[HMS] HMSScanKitManager";

    public Action<string, HmsScan> ScanSuccess { get; set; }
    public Action<HMSException> ScanFailure { get; set; }

    private Dictionary<ScanKitPermission, bool> permissionStates = new Dictionary<ScanKitPermission, bool>();

    private int _scanType;

    private enum ScanKitPermission
    {
        READ_EXTERNAL_STORAGE = 1,
        CAMERA = 2,

        
    }

    public HMSScanKitManager()
    {
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(OnAwake);
    }

    private void OnAwake()
    {
        Debug.Log($"[{TAG}]: OnAwake() ");
    }

    #region Scan

    public void Scan(int scanType)
    {
        _scanType = scanType;
        StartScan();
    }

    public void StartScan()
    {
        Debug.Log($"[{TAG}]: StartScan ");

        var creator = new HmsScanAnalyzerOptions.Creator();
        var processedCreator = HmsScanAnalyzerOptions.Creator.SetHmsScanTypes(_scanType, creator);
        var hmsScanAnalyzerOptions = processedCreator.Create();

        ScanUtil.StartScan(ScanSuccess, ScanFailure, hmsScanAnalyzerOptions);
    }

    #endregion

}
