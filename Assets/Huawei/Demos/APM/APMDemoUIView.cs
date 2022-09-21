
using UnityEngine;
using UnityEngine.UI;

public class APMDemoUIView : MonoBehaviour
{

    private Button Btn_ApmsCollectionOff;
    private Button Btn_ApmsCollectionOn;
    private Button Btn_ApmsANRMonitorOff;
    private Button Btn_ApmsANRMonitorOn;
    private Button Btn_SendCustomEvent;

    #region Monobehaviour

    private void Awake()
    {
        Btn_ApmsCollectionOff = GameObject.Find("Btn_ApmsCollectionOff").GetComponent<Button>();
        Btn_ApmsCollectionOn = GameObject.Find("Btn_ApmsCollectionOn").GetComponent<Button>();
        Btn_ApmsANRMonitorOff = GameObject.Find("Btn_ApmsANRMonitorOff").GetComponent<Button>();
        Btn_ApmsANRMonitorOn = GameObject.Find("Btn_ApmsANRMonitorOn").GetComponent<Button>();
        Btn_SendCustomEvent = GameObject.Find("Btn_SendCustomEvent").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_ApmsCollectionOff.onClick.AddListener(ButtonClick_ApmsCollectionOff);
        Btn_ApmsCollectionOn.onClick.AddListener(ButtonClick_ApmsCollectionOn);
        Btn_ApmsANRMonitorOff.onClick.AddListener(ButtonClick_ApmsANRMonitorOff);
        Btn_ApmsANRMonitorOn.onClick.AddListener(ButtonClick_ApmsANRMonitorOn);
        Btn_SendCustomEvent.onClick.AddListener(ButtonClick_SendCustomEvent);
    }

    private void OnDisable()
    {
        Btn_ApmsCollectionOff.onClick.RemoveListener(ButtonClick_ApmsCollectionOff);
        Btn_ApmsCollectionOn.onClick.RemoveListener(ButtonClick_ApmsCollectionOn);
        Btn_ApmsANRMonitorOff.onClick.RemoveListener(ButtonClick_ApmsANRMonitorOff);
        Btn_ApmsANRMonitorOn.onClick.RemoveListener(ButtonClick_ApmsANRMonitorOn);
        Btn_SendCustomEvent.onClick.RemoveListener(ButtonClick_SendCustomEvent);
    }

    #endregion

    #region Button Events

    private void ButtonClick_ApmsCollectionOff()
    {
        APMDemoManager.Instance.APMSCollectionOff();
    }

    private void ButtonClick_ApmsCollectionOn()
    {
        APMDemoManager.Instance.APMSCollectionOn();
    }

    private void ButtonClick_ApmsANRMonitorOff()
    {
        APMDemoManager.Instance.APMSAnrMonitorOff();
    }

    private void ButtonClick_ApmsANRMonitorOn()
    {
        APMDemoManager.Instance.APMSAnrMonitorOn();
    }

    private void ButtonClick_SendCustomEvent()
    {
        APMDemoManager.Instance.SendCustomEvent();
    }

    #endregion



}
