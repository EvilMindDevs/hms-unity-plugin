using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class RemoteConfigUIView : MonoBehaviour
{
    private Text Txt_countOfVariables;

    private Button Btn_fetch;
    private Button Btn_applyDefaultWDictionary;
    private Button Btn_applyDefaultWXML;
    private Button Btn_clearAll;

    private Button Btn_getMergetAll;
    private Button Btn_LoadLastFetched;
    private Button Btn_getSource;
    private Button Btn_DeveloperMode;

    #region Monobehaviour

    private void Awake()
    {
        Btn_fetch = GameObject.Find("Btn_fetch").GetComponent<Button>();
        Btn_applyDefaultWDictionary = GameObject.Find("Btn_applyDefaultWDictionary").GetComponent<Button>();
        Btn_applyDefaultWXML = GameObject.Find("Btn_applyDefaultWXML").GetComponent<Button>();
        Btn_clearAll = GameObject.Find("Btn_clearAll").GetComponent<Button>();

        Btn_getMergetAll = GameObject.Find("Btn_getMergetAll").GetComponent<Button>();
        Btn_LoadLastFetched = GameObject.Find("Btn_LoadLastFetched").GetComponent<Button>();
        Btn_getSource = GameObject.Find("Btn_getSource").GetComponent<Button>();
        Btn_DeveloperMode = GameObject.Find("Btn_DeveloperMode").GetComponent<Button>();


    }



    private void OnEnable()
    {
        Btn_fetch.onClick.AddListener(ButtonClick_Fetch);
        Btn_applyDefaultWDictionary.onClick.AddListener(ButtonClick_ApplyDefaultWDictionary);
        Btn_applyDefaultWXML.onClick.AddListener(ButtonClick_ApplyDefaultWXML);
        Btn_clearAll.onClick.AddListener(ButtonClick_ClearAll);

        Btn_getMergetAll.onClick.AddListener(ButtonClick_GetMergetAll);
        Btn_LoadLastFetched.onClick.AddListener(ButtonClick_LoadLastFetched);
        Btn_getSource.onClick.AddListener(ButtonClick_GetSource);
        Btn_DeveloperMode.onClick.AddListener(ButtonClick_DeveloperMode);
    }

    private void OnDisable()
    {
        Btn_fetch.onClick.RemoveListener(ButtonClick_Fetch);
        Btn_applyDefaultWDictionary.onClick.RemoveListener(ButtonClick_ApplyDefaultWDictionary);
        Btn_applyDefaultWXML.onClick.RemoveListener(ButtonClick_ApplyDefaultWXML);
        Btn_clearAll.onClick.RemoveListener(ButtonClick_ClearAll);

        Btn_getMergetAll.onClick.RemoveListener(ButtonClick_GetMergetAll);
        Btn_LoadLastFetched.onClick.RemoveListener(ButtonClick_LoadLastFetched);
        Btn_getSource.onClick.RemoveListener(ButtonClick_GetSource);
        Btn_DeveloperMode.onClick.RemoveListener(ButtonClick_DeveloperMode);
    }

    #endregion

    #region Button Events

    private void ButtonClick_Fetch()
    {
        RemoteConfigDemo.Instance.Fetch();
    }

    private void ButtonClick_ApplyDefaultWDictionary()
    {
        RemoteConfigDemo.Instance.ApplyDefault();
    }

    private void ButtonClick_ApplyDefaultWXML()
    {
        RemoteConfigDemo.Instance.ApplyDefaultXml();
    }

    private void ButtonClick_ClearAll()
    {
        RemoteConfigDemo.Instance.ClearAll();
    }



    private void ButtonClick_GetMergetAll()
    {
        RemoteConfigDemo.Instance.GetMergedAll();
    }

    private void ButtonClick_LoadLastFetched()
    {
        RemoteConfigDemo.Instance.LoadLastFetched();
    }

    private void ButtonClick_GetSource()
    {
        RemoteConfigDemo.Instance.GetSource();
    }

    private void ButtonClick_DeveloperMode()
    {
        RemoteConfigDemo.Instance.DeveloperMode(true);
    }

    #endregion

}
