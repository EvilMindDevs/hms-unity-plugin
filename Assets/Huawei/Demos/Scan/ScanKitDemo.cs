using HuaweiMobileServices.Scan;

using UnityEngine;
using UnityEngine.UI;

public class ScanKitDemo : MonoBehaviour
{

    private readonly string TAG = "[HMS] ScanKitDemo";

    private Text Txt_QRCode;

    #region Singleton

    public static ScanKitDemo Instance { get; private set; }
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    #region Monobehaviour

    private void Awake()
    {
        Singleton();
        Txt_QRCode = GameObject.Find("TxtQRCodeText").GetComponent<Text>();
    }

    private void OnEnable()
    {
        HMSScanKitManager.Instance.ScanSuccess += OnScanSuccess;
    }

    private void OnDisable()
    {
        HMSScanKitManager.Instance.ScanSuccess -= OnScanSuccess;
    }

    #endregion

    #region Main

    public void Scan()
    {
        HMSScanKitManager.Instance.Scan(HmsScanBase.ALL_SCAN_TYPE);
    }

    #endregion

    #region Callbacks

    private void OnScanSuccess(string text, HmsScan hmsScan)
    {
        Txt_QRCode.text = text;
    }

    #endregion

}
