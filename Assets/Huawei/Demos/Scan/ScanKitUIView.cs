
using UnityEngine;
using UnityEngine.UI;
public class ScanKitUIView : MonoBehaviour
{
    private Text TxtQrCode;
    private Button BtnScan;

    #region Monobehaviour

    private void Awake()
    {
        TxtQrCode = GameObject.Find("TxtQRCodeText").GetComponent<Text>();
        BtnScan = GameObject.Find("BtnScan").GetComponent<Button>();
    }

    private void OnEnable()
    {
        BtnScan.onClick.AddListener(ButtonClick_Scan);
    }

    private void OnDisable()
    {
        BtnScan.onClick.RemoveListener(ButtonClick_Scan);
    }

    #endregion

    #region Button Events

    private void ButtonClick_Scan()
    {
        ScanKitDemo.Instance.Scan();
    }

    #endregion

}
