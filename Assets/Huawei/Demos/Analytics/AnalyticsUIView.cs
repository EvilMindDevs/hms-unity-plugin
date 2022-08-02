using HmsPlugin;

using UnityEngine;
using UnityEngine.UI;

public class AnalyticsUIView : MonoBehaviour
{

    private InputField eventIdField;
    private InputField keyField;
    private InputField valueField;

    private Button Btn_SendEvent;

    #region Monobehaviour

    private void Awake()
    {
        eventIdField = GameObject.Find("InputField - EventId").GetComponent<InputField>();
        keyField = GameObject.Find("InputField - Key").GetComponent<InputField>();
        valueField = GameObject.Find("InputField - Value").GetComponent<InputField>();

        Btn_SendEvent = GameObject.Find("Button - SendEvent").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_SendEvent.onClick.AddListener(ButtonClick_SendEvent);
    }

    private void OnDisable()
    {
        Btn_SendEvent.onClick.RemoveListener(ButtonClick_SendEvent);
    }

    #endregion

    #region Button Events

    private void ButtonClick_SendEvent()
    {
        AnalyticsDemoManager.Instance.SendEvent(eventIdField.text, keyField.text, valueField.text);
    }

    #endregion

}


