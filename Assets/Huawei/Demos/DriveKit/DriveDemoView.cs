using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class DriveDemoView : MonoBehaviour
{

    private Button Btn_About;
    private Button Btn_CreateDirectory;
    private Button Btn_CreateFile;
    private Button Btn_ListComments;
    private Button Btn_CreateComments;

    private Text Txt_Description;

    #region Monobehaviour

    private void Awake()
    {
        Btn_About = GameObject.Find("About").GetComponent<Button>();
        Btn_CreateDirectory = GameObject.Find("CreateDirectory").GetComponent<Button>();
        Btn_CreateFile = GameObject.Find("CreateFile").GetComponent<Button>();
        Btn_ListComments = GameObject.Find("ListComments").GetComponent<Button>();
        Btn_CreateComments = GameObject.Find("CreateComments").GetComponent<Button>();

        Txt_Description = GameObject.Find("Description").GetComponent<Text>();
    }

    private void OnEnable()
    {
        Btn_About.onClick.AddListener(ButtonClick_About);
        Btn_CreateDirectory.onClick.AddListener(ButtonClick_CreateDirectory);
        Btn_CreateFile.onClick.AddListener(ButtonClick_CreateFile);
        Btn_ListComments.onClick.AddListener(ButtonClick_ListComments);
        Btn_CreateComments.onClick.AddListener(ButtonClick_CreateComments);

        DriveDemoManager.DriveKitLog += OnDriveKitLog;
    }

    private void OnDisable()
    {
        Btn_About.onClick.RemoveListener(ButtonClick_About);
        Btn_CreateDirectory.onClick.RemoveListener(ButtonClick_CreateDirectory);
        Btn_CreateFile.onClick.RemoveListener(ButtonClick_CreateFile);
        Btn_ListComments.onClick.RemoveListener(ButtonClick_ListComments);
        Btn_CreateComments.onClick.RemoveListener(ButtonClick_CreateComments);

        DriveDemoManager.DriveKitLog -= OnDriveKitLog;
    }

    #endregion

    #region Callback

    private void OnDriveKitLog(string log)
    {
        Txt_Description.text = log;
    }

    #endregion

    #region Button Events

    private void ButtonClick_About()
    {
        DriveDemoManager.Instance.GetAboutOnClick();
    }

    private void ButtonClick_CreateDirectory()
    {
        DriveDemoManager.Instance.CreateDirectoryOnClick();
    }

    private void ButtonClick_CreateFile()
    {
        DriveDemoManager.Instance.CreateFileOnClick();
    }

    private void ButtonClick_ListComments()
    {
        DriveDemoManager.Instance.ListCommentsOnClick();
    }

    private void ButtonClick_CreateComments()
    {
        DriveDemoManager.Instance.CreateCommentsOnClick();
    }

    #endregion

}
