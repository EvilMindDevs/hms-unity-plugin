using HmsPlugin;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CloudDBDemoUIView : MonoBehaviour
{

    private Text Txt_Log;

    private Button Btn_CreateObjectType;
    private Button Btn_GetCloudDBZoneConfigs;
    private Button Btn_OpenCloudDBZone;
    private Button Btn_OpenCloudDBZone2;
    private Button Btn_EnableNetwork;
    private Button Btn_DisableNetwork;

    private Button Btn_ExecuteUpsertAddBookInfo;
    private Button Btn_ExecuteUpsertUpdateBookInfo;
    private Button Btn_ExecuteDeleteDeleteBookInfo;
    private Button Btn_ExecuteUpsertAddListBookInfo;
    private Button Btn_ExecuteDeleteDeleteListBookInfo;
    private Button Btn_ExecuteQueryGetBookInfo;
    private Button Btn_ExecuteSumQuery;
    private Button Btn_ExecuteCountQuery;


    #region Monobehaviour

    private void Awake()
    {
        Txt_Log = GameObject.Find("LoggedUserText").GetComponent<Text>();

        Btn_CreateObjectType = GameObject.Find("CreateObjectType").GetComponent<Button>();
        Btn_GetCloudDBZoneConfigs = GameObject.Find("GetCloudDBZoneConfigs").GetComponent<Button>();
        Btn_OpenCloudDBZone = GameObject.Find("OpenCloudDBZone").GetComponent<Button>();
        Btn_OpenCloudDBZone2 = GameObject.Find("OpenCloudDBZone2").GetComponent<Button>();
        Btn_EnableNetwork = GameObject.Find("EnableNetwork").GetComponent<Button>();
        Btn_DisableNetwork = GameObject.Find("DisableNetwork").GetComponent<Button>();

        Btn_ExecuteUpsertAddBookInfo = GameObject.Find("ExecuteUpsert (Add BookInfo)").GetComponent<Button>();
        Btn_ExecuteUpsertUpdateBookInfo = GameObject.Find("ExecuteUpsert (Update BookInfo)").GetComponent<Button>();
        Btn_ExecuteDeleteDeleteBookInfo = GameObject.Find("ExecuteDelete (Delete BookInfo)").GetComponent<Button>();
        Btn_ExecuteUpsertAddListBookInfo = GameObject.Find("ExecuteUpsert (Add List BookInfo)").GetComponent<Button>();
        Btn_ExecuteDeleteDeleteListBookInfo = GameObject.Find("ExecuteDelete (Delete List BookInfo)").GetComponent<Button>();
        Btn_ExecuteQueryGetBookInfo = GameObject.Find("ExecuteQuery (Get BookInfo)").GetComponent<Button>();
        Btn_ExecuteSumQuery = GameObject.Find("ExecuteSumQuery").GetComponent<Button>();
        Btn_ExecuteCountQuery = GameObject.Find("ExecuteCountQuery").GetComponent<Button>();
    }

    private void OnEnable()
    {
        Btn_CreateObjectType.onClick.AddListener(ButtonClick_CreateObjectType);
        Btn_GetCloudDBZoneConfigs.onClick.AddListener(ButtonClick_GetCloudDBZoneConfigs);
        Btn_OpenCloudDBZone.onClick.AddListener(ButtonClick_OpenCloudDBZone);
        Btn_OpenCloudDBZone2.onClick.AddListener(ButtonClick_OpenCloudDBZone2);
        Btn_EnableNetwork.onClick.AddListener(ButtonClick_EnableNetwork);
        Btn_DisableNetwork.onClick.AddListener(ButtonClick_DisableNetwork);

        Btn_ExecuteUpsertAddBookInfo.onClick.AddListener(ButtonClick_ExecuteUpsertAddBookInfo);
        Btn_ExecuteUpsertUpdateBookInfo.onClick.AddListener(ButtonClick_ExecuteUpsertUpdateBookInfo);
        Btn_ExecuteDeleteDeleteBookInfo.onClick.AddListener(ButtonClick_ExecuteDeleteDeleteBookInfo);
        Btn_ExecuteUpsertAddListBookInfo.onClick.AddListener(ButtonClick_ExecuteUpsertAddListBookInfo);
        Btn_ExecuteDeleteDeleteListBookInfo.onClick.AddListener(ButtonClick_ExecuteDeleteDeleteListBookInfo);
        Btn_ExecuteQueryGetBookInfo.onClick.AddListener(ButtonClick_ExecuteQueryGetBookInfo);
        Btn_ExecuteSumQuery.onClick.AddListener(ButtonClick_ExecuteSumQuery);
        Btn_ExecuteCountQuery.onClick.AddListener(ButtonClick_ExecuteCountQuery);

        CloudDBDemo.CloudDBDemoLog += OnCloudDBDemoLog;
    }

    private void OnDisable()
    {
        Btn_CreateObjectType.onClick.RemoveListener(ButtonClick_CreateObjectType);
        Btn_GetCloudDBZoneConfigs.onClick.RemoveListener(ButtonClick_GetCloudDBZoneConfigs);
        Btn_OpenCloudDBZone.onClick.RemoveListener(ButtonClick_OpenCloudDBZone);
        Btn_OpenCloudDBZone2.onClick.RemoveListener(ButtonClick_OpenCloudDBZone2);
        Btn_EnableNetwork.onClick.RemoveListener(ButtonClick_EnableNetwork);
        Btn_DisableNetwork.onClick.RemoveListener(ButtonClick_DisableNetwork);

        Btn_ExecuteUpsertAddBookInfo.onClick.RemoveListener(ButtonClick_ExecuteUpsertAddBookInfo);
        Btn_ExecuteUpsertUpdateBookInfo.onClick.RemoveListener(ButtonClick_ExecuteUpsertUpdateBookInfo);
        Btn_ExecuteDeleteDeleteBookInfo.onClick.RemoveListener(ButtonClick_ExecuteDeleteDeleteBookInfo);
        Btn_ExecuteUpsertAddListBookInfo.onClick.RemoveListener(ButtonClick_ExecuteUpsertAddListBookInfo);
        Btn_ExecuteDeleteDeleteListBookInfo.onClick.RemoveListener(ButtonClick_ExecuteDeleteDeleteListBookInfo);
        Btn_ExecuteQueryGetBookInfo.onClick.RemoveListener(ButtonClick_ExecuteQueryGetBookInfo);
        Btn_ExecuteSumQuery.onClick.RemoveListener(ButtonClick_ExecuteSumQuery);
        Btn_ExecuteCountQuery.onClick.RemoveListener(ButtonClick_ExecuteCountQuery);

        CloudDBDemo.CloudDBDemoLog -= OnCloudDBDemoLog;
    }

    #endregion

    #region Callbacks

    private void OnCloudDBDemoLog(string log)
    {
        Txt_Log.text = log;
    }

    #endregion

    #region Button Events



    private void ButtonClick_CreateObjectType()
    {
        CloudDBDemo.Instance.CreateObjectType();
    }

    private void ButtonClick_GetCloudDBZoneConfigs()
    {
        CloudDBDemo.Instance.GetCloudDBZoneConfigs();
    }

    private void ButtonClick_OpenCloudDBZone()
    {
        CloudDBDemo.Instance.OpenCloudDBZone();
    }

    private void ButtonClick_OpenCloudDBZone2()
    {
        CloudDBDemo.Instance.OpenCloudDBZone2();
    }

    private void ButtonClick_EnableNetwork()
    {
        CloudDBDemo.Instance.EnableNetwork();
    }

    private void ButtonClick_DisableNetwork()
    {
        CloudDBDemo.Instance.DisableNetwork();
    }

    private void ButtonClick_ExecuteUpsertAddBookInfo()
    {
        CloudDBDemo.Instance.AddBookInfo();
    }

    private void ButtonClick_ExecuteUpsertUpdateBookInfo()
    {
        CloudDBDemo.Instance.UpdateBookInfo();
    }

    private void ButtonClick_ExecuteDeleteDeleteBookInfo()
    {
        CloudDBDemo.Instance.DeleteBookInfo();
    }

    private void ButtonClick_ExecuteUpsertAddListBookInfo()
    {
        CloudDBDemo.Instance.AddBookInfoList();
    }

    private void ButtonClick_ExecuteDeleteDeleteListBookInfo()
    {
        CloudDBDemo.Instance.DeleteBookInfoList();
    }

    private void ButtonClick_ExecuteQueryGetBookInfo()
    {
        CloudDBDemo.Instance.GetBookInfo();
    }

    private void ButtonClick_ExecuteSumQuery()
    {
        CloudDBDemo.Instance.ExecuteSumQuery();
    }

    private void ButtonClick_ExecuteCountQuery()
    {
        CloudDBDemo.Instance.ExecuteCountQuery();
    }

    #endregion


}

