using HuaweiMobileServices.Scan;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Android;

public class HMSScanKitManager : HMSManagerSingleton<HMSScanKitManager>
{
    private readonly string TAG = "[HMS] HMSScanKitManager";

    public Action<string, HmsScan> ScanSuccess { get; set; }
    public Action<HMSException> ScanFailure { get; set; }

    private Dictionary<ScanKitPermission, bool> permissionStates = new Dictionary<ScanKitPermission, bool>();

    private PermissionCallbacks callbacks;

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

        callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += OnPermissionGranted;
    }

    private void OnAwake()
    {
        Debug.Log($"[{TAG}]: OnAwake() ");
    }

    private bool PermissionControlForTriggerScan()
    {
        foreach (var state in permissionStates)
        {
            if (!state.Value)
                return false;
        }

        return true;
    }

    #region Scan

    public void Scan(int scanType)
    {
        _scanType = scanType;

        bool cameraHasPermission = Permission.HasUserAuthorizedPermission(Permission.Camera);
        bool externalStorageReadHasPermission = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);

        Debug.Log($"[{TAG}]: cameraHasPermission {cameraHasPermission} ");
        Debug.Log($"[{TAG}]: externalStorageReadHasPermission {externalStorageReadHasPermission} ");

        permissionStates[ScanKitPermission.CAMERA] = cameraHasPermission;
        permissionStates[ScanKitPermission.READ_EXTERNAL_STORAGE] = externalStorageReadHasPermission;

        if (PermissionControlForTriggerScan())
        {
            StartScan();
            return;
        }

        RequestPermission();
    }

    private void RequestPermission()
    {
        Debug.Log($"[{TAG}]: TriggerPermission ");

        Permission.RequestUserPermissions(new string[] { Permission.Camera, Permission.ExternalStorageRead }, callbacks);
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

    #region Callbacks

    private void OnPermissionGranted(string permission)
    {
        Debug.Log($"[{TAG}]: OnPermissionGranted ");

        StartScan();
    }

    #endregion

}
