using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CrashDemoView : MonoBehaviour
{

    private Button Btn_EnableCrash;
    private Button Btn_DisableCrash;
    private Button Btn_MakeCrash;
    private Button Btn_Report;

    #region Monobehaviour

    private void Awake()
    {
        Btn_EnableCrash = GameObject.Find("EnablecrashButton").GetComponent<Button>();
        Btn_DisableCrash = GameObject.Find("DisablecrashButton").GetComponent<Button>();
        Btn_MakeCrash = GameObject.Find("MakecrashButton").GetComponent<Button>();
        Btn_Report = GameObject.Find("CustomreportButton").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_EnableCrash.onClick.AddListener(ButtonClick_EnableCrash);
        Btn_DisableCrash.onClick.AddListener(ButtonClick_DisableCrash);
        Btn_MakeCrash.onClick.AddListener(ButtonClick_MakeCrash);
        Btn_Report.onClick.AddListener(ButtonClick_Report);
    }

    private void OnDisable()
    {
        Btn_EnableCrash.onClick.RemoveListener(ButtonClick_EnableCrash);
        Btn_DisableCrash.onClick.RemoveListener(ButtonClick_DisableCrash);
        Btn_MakeCrash.onClick.RemoveListener(ButtonClick_MakeCrash);
        Btn_Report.onClick.RemoveListener(ButtonClick_Report);
    }

    #endregion

    #region Button Events

    private void ButtonClick_EnableCrash()
    {
        CrashDemoManager.Instance.EnableCrashCollection(true);
    }

    private void ButtonClick_DisableCrash()
    {
        CrashDemoManager.Instance.EnableCrashCollection(false);
    }

    private void ButtonClick_MakeCrash()
    {
        CrashDemoManager.Instance.TestCrash();
    }

    private void ButtonClick_Report()
    {
        CrashDemoManager.Instance.CustomReport();
    }

    #endregion

}
