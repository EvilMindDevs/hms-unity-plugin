using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class NearbyDemoUIView : MonoBehaviour
{

    private Button Btn_SendFile;
    private Button Btn_StartScan;
    private Button Btn_StopB;
    private Button Btn_StopS;
    private Button Btn_DisconnectAll;

    #region Monobehaviour

    private void Awake()
    {
        Btn_SendFile = GameObject.Find("Btn_SendFile").GetComponent<Button>();
        Btn_StartScan = GameObject.Find("Btn_StartScan").GetComponent<Button>();
        Btn_StopB = GameObject.Find("Btn_StopB").GetComponent<Button>();
        Btn_StopS = GameObject.Find("Btn_StopS").GetComponent<Button>();
        Btn_DisconnectAll = GameObject.Find("Btn_DisconnectAll").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_SendFile.onClick.AddListener(ButtonClick_SendFile);
        Btn_StartScan.onClick.AddListener(ButtonClick_StartScan);
        Btn_StopB.onClick.AddListener(ButtonClick_StopB);
        Btn_StopS.onClick.AddListener(ButtonClick_StopS);
        Btn_DisconnectAll.onClick.AddListener(ButtonClick_DisconnectAll);
    }

    private void OnDisable()
    {
        Btn_SendFile.onClick.RemoveListener(ButtonClick_SendFile);
        Btn_StartScan.onClick.RemoveListener(ButtonClick_StartScan);
        Btn_StopB.onClick.RemoveListener(ButtonClick_StopB);
        Btn_StopS.onClick.RemoveListener(ButtonClick_StopS);
        Btn_DisconnectAll.onClick.RemoveListener(ButtonClick_DisconnectAll);
    }

    #endregion

    #region Button Events

    private void ButtonClick_SendFile()
    {
        NearbyDemoManager.Instance.SendFilesInner();
    }

    private void ButtonClick_StartScan()
    {
        NearbyDemoManager.Instance.OnScanResult();
    }

    private void ButtonClick_StopB()
    {
        NearbyDemoManager.Instance.StopBroadCasting();
    }

    private void ButtonClick_StopS()
    {
        NearbyDemoManager.Instance.StopScanning();
    }

    private void ButtonClick_DisconnectAll()
    {
        NearbyDemoManager.Instance.DisconnectAllConnection();
    }

    #endregion

}
