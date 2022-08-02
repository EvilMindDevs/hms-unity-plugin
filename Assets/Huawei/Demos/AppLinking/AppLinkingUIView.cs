
using System;

using UnityEngine;
using UnityEngine.UI;

public class AppLinkingUIView : MonoBehaviour
{

    private Button Btn_CreateAppLinking;
    private Button Btn_ShareLongAppLinking;
    private Button Btn_ShareShortAppLinking;
    private Button Btn_GetDeepLink;

    private Text Txt_LongAppLink;
    private Text Txt_ShortAppLink;
    private Text Txt_DeepLink;

    #region Monobehaviour

    private void Awake()
    {
        Btn_CreateAppLinking = GameObject.Find("CreateAppLinking").GetComponent<Button>();
        Btn_ShareLongAppLinking = GameObject.Find("ShareLongAppLinking").GetComponent<Button>();
        Btn_ShareShortAppLinking = GameObject.Find("ShareShortAppLinking").GetComponent<Button>();
        Btn_GetDeepLink = GameObject.Find("GetDeepLink").GetComponent<Button>();

        Txt_LongAppLink = GameObject.Find("LongAppLinkText").GetComponent<Text>();
        Txt_ShortAppLink = GameObject.Find("ShortAppLinkText").GetComponent<Text>();
        Txt_DeepLink = GameObject.Find("DeepLinkText").GetComponent<Text>();

    }



    private void OnEnable()
    {
        Btn_CreateAppLinking.onClick.AddListener(ButtonClick_CreateAppLinking);
        Btn_ShareLongAppLinking.onClick.AddListener(ButtonClick_ShareLongAppLinking);
        Btn_ShareShortAppLinking.onClick.AddListener(ButtonClick_ShareShortAppLinking);
        Btn_GetDeepLink.onClick.AddListener(ButtonClick_GetDeepLink);

        AppLinkingDemo.longLinkText += OnLongLinkText;
        AppLinkingDemo.shortLinkText += OnShortLinkText;
        AppLinkingDemo.deepLinkText += OnDeepLinkText;
    }

    private void OnDisable()
    {
        Btn_CreateAppLinking.onClick.RemoveListener(ButtonClick_CreateAppLinking);
        Btn_ShareLongAppLinking.onClick.RemoveListener(ButtonClick_ShareLongAppLinking);
        Btn_ShareShortAppLinking.onClick.RemoveListener(ButtonClick_ShareShortAppLinking);
        Btn_GetDeepLink.onClick.RemoveListener(ButtonClick_GetDeepLink);

        AppLinkingDemo.longLinkText -= OnLongLinkText;
        AppLinkingDemo.shortLinkText -= OnShortLinkText;
        AppLinkingDemo.deepLinkText -= OnDeepLinkText;
    }

    #endregion



    #region Callbacks

    private void OnLongLinkText(string log)
    {
        Txt_LongAppLink.text = log;
    }

    private void OnShortLinkText(string log)
    {
        Txt_ShortAppLink.text = log;
    }

    private void OnDeepLinkText(string log)
    {
        Txt_DeepLink.text = log;
    }

    #endregion



    #region Button Events

    private void ButtonClick_CreateAppLinking()
    {
        AppLinkingDemo.Instance.CreateAppLinking();
    }

    private void ButtonClick_ShareLongAppLinking()
    {
        AppLinkingDemo.Instance.ShareLongLink();
    }

    private void ButtonClick_ShareShortAppLinking()
    {
        AppLinkingDemo.Instance.ShareShortLink();
    }

    private void ButtonClick_GetDeepLink()
    {
        AppLinkingDemo.Instance.GetLink();
    }

    #endregion

}


