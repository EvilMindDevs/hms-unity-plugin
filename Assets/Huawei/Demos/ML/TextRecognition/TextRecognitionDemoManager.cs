using System;
using System.Collections.Generic;
using System.Linq;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.ML.TextRecognition;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextRecognitionDemoManager : MonoBehaviour
{
    #region Variables
    private const string TAG = "[HMS] TextRecognitionDemoManager ";
    [SerializeField] private Button m_onlineButton;
    [SerializeField] private Button m_offlineButton;
    [SerializeField] private Button m_backButton;
    [SerializeField] private Button m_selectImageButton;
    [SerializeField] private Button m_analyseButton;
    [SerializeField] private Button m_copyClipboardInputButton;
    [SerializeField] private Dropdown m_languageDropdown;
    [SerializeField] private Text m_subTitle;
    [SerializeField] private GameObject m_recognitionMenu;
    [SerializeField] private GameObject m_recognitionPanel;
    [SerializeField] private InputField m_inputField;

    private RecognitionMode selectedMode;
    private MLFrame m_frame;

    public enum RecognitionMode
    {
        Online = 0,
        Offline = 1
    }

    #endregion

    #region Singleton

    public static TextRecognitionDemoManager Instance { get; private set; }
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

    #region Methods
    private void SetupDropdownListeners()
    {
        m_languageDropdown.onValueChanged.AddListener(SelectSourceLanguage);
    }

    private void SetupButtonListeners()
    {
        m_offlineButton.onClick.AddListener(() =>
        {
            OnOfflineButton();
        });
        m_onlineButton.onClick.AddListener(() =>
        {
            OnOnlineButton();
        });

        m_backButton.onClick.AddListener(BackToMenu);

        m_selectImageButton.onClick.AddListener(OnSelectImageButton);
        m_analyseButton.onClick.AddListener(OnAnalyseButton);
        m_copyClipboardInputButton.onClick.AddListener(() =>
        {
            GUIUtility.systemCopyBuffer = m_inputField.text;
            AndroidToast.MakeText("Copied to Clipboard").Show();
        });
    }

    private void Awake()
    {
        Singleton();
    }

    void Start()
    {
        SetupButtonListeners();
        SetupDropdownListeners();

        Debug.Log(TAG + "Start");
    }

    private void OnRecognitionButtonClicked(RecognitionMode mode)
    {
        Debug.Log($"{TAG} -> OnTranslateButtonClicked: {mode}");
        HMSMLTextRecognitionKitManager.Instance.OnImageSelectSuccess += OnImageSelectSuccess;
        HMSMLTextRecognitionKitManager.Instance.OnAnalyseFrameSuccess += OnAnalyseFrameSuccess;
        HMSMLTextRecognitionKitManager.Instance.OnAnalyseFrameFailure += OnAnalyseFrameFailure;
        selectedMode = mode;
        m_subTitle.text = Enum.GetName(typeof(RecognitionMode), mode);
        PopulateDropdowns();

        // Hide the translate menu and show the translate panel
        m_recognitionMenu.SetActive(false);
        m_recognitionPanel.SetActive(true);
    }

    public void OnOnlineButton()
    {
        Debug.Log(TAG + "OnOnlineButton");
        OnRecognitionButtonClicked(RecognitionMode.Online);
        HMSMLTextRecognitionKitManager.Instance.OnCloudConfiguration();
    }

    public void OnOfflineButton()
    {
        Debug.Log(TAG + "OnOfflineButton");
        OnRecognitionButtonClicked(RecognitionMode.Offline);
        HMSMLTextRecognitionKitManager.Instance.OnDeviceConfiguration();
    }

    public void OnSelectImageButton()
    {
        Debug.Log(TAG + "OnSelectImageButton");
        HMSMLTextRecognitionKitManager.Instance.SelectImage();
    }

    public void OnAnalyseButton()
    {
        Debug.Log(TAG + "OnAnalyseButton");
        Debug.Log(TAG + "Analyzing Frame" + m_frame == null ? "Frame is null" : "Frame is not null");
        HMSMLTextRecognitionKitManager.Instance.AnalyzeFrameAsync(m_frame);
    }

    public void OnImageSelectSuccess(AndroidIntent intent, AndroidBitmap bitmap)
    {
        Debug.Log(TAG + "OnImageSelectSuccess");
        m_frame = MLFrame.FromBitmap(bitmap);
        m_analyseButton.interactable = true;
        AndroidToast.MakeText("Image Selected").Show();
    }

    public void OnAnalyseFrameSuccess(MLText result)
    {
        Debug.Log(TAG + "OnAnalyseFrameSuccess");
        m_inputField.text = result.StringValue;
        m_copyClipboardInputButton.interactable = true;
        AndroidToast.MakeText("Text Recognition Success").Show();
    }

    public void OnAnalyseFrameFailure(HMSException exception)
    {
        m_inputField.text = exception.WrappedExceptionMessage;
    }

    private void PopulateDropdowns()
    {
        m_languageDropdown.ClearOptions();
        var options = HMSMLTextRecognitionKitManager.Instance.LanguageDict.Keys.Select(lang => new Dropdown.OptionData(Enum.GetName(typeof(HMSMLTextRecognitionKitManager.LanguageCodes), lang))).ToList();
        m_languageDropdown.AddOptions(options);
    }

    private void BackToMenu()
    {
        HMSMLTextRecognitionKitManager.Instance.OnImageSelectSuccess = null;
        HMSMLTextRecognitionKitManager.Instance.OnAnalyseFrameSuccess = null;
        HMSMLTextRecognitionKitManager.Instance.OnAnalyseFrameFailure = null;
        m_languageDropdown.ClearOptions();
        m_recognitionMenu.SetActive(true);
        m_recognitionPanel.SetActive(false);
        m_analyseButton.interactable = false;
        m_copyClipboardInputButton.interactable = false;
    }

    public void SelectSourceLanguage(int index)
    {
        if (HMSMLTextRecognitionKitManager.Instance.LanguageDict.TryGetValue((HMSMLTextRecognitionKitManager.LanguageCodes)index, out string language))
        {
            Debug.Log($"{TAG} -> SelectSourceLanguage: {language}");
            if (selectedMode == RecognitionMode.Online)
            {
                HMSMLTextRecognitionKitManager.Instance.OnCloudConfiguration(new List<HMSMLTextRecognitionKitManager.LanguageCodes> { (HMSMLTextRecognitionKitManager.LanguageCodes)index });
            }
            else
            {
                HMSMLTextRecognitionKitManager.Instance.OnDeviceConfiguration((HMSMLTextRecognitionKitManager.LanguageCodes)index);
            }
        }
    }

    public void ResetComponents()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
