using System;
using System.Collections.Generic;
using System.Linq;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static HMSMLTranslateKitManager;

public class TranslateDemoManager : MonoBehaviour
{
    private const string Tag = "[HMS] TranslateDemoManager ";
    [SerializeField] private Button m_onlineButton;
    [SerializeField] private Button m_offlineButton;
    [SerializeField] private Dropdown m_sourceLanguageDropdown;
    [SerializeField] private Dropdown m_targetLanguageDropdown;
    [SerializeField] private GameObject m_translateMenu;
    [SerializeField] private GameObject m_translatePanel;
    [SerializeField] private InputField m_targetInputField;
    [SerializeField] private InputField m_sourceInputField;
    [SerializeField] private Button m_translateButton;
    [SerializeField] private Button m_backButton;
    [SerializeField] private Button m_changeButton;
    [SerializeField] private Text m_subTitle;
    [SerializeField] private GameObject m_translateModule;
    [SerializeField] private GameObject m_baseModule;
    [SerializeField] private Text m_progressText;

    private HashSet<string> m_languages = new HashSet<string>();
    private int m_sourceLanguageIndex;
    private int m_targetLanguageIndex;

    #region Singleton

    public static TranslateDemoManager Instance { get; private set; }

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

    #region Unity Methods

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        SetupDropdownListeners();
        HMSMLTranslateKitManager.Instance.OnProcessAction += OnProcessAction;
    }

    #endregion

    #region Initialization Methods

    private void InitializeUI()
    {
        m_translateButton.gameObject.SetActive(false);

    }

    private void SetupButtonListeners()
    {
        m_offlineButton.onClick.AddListener(OnOfflineButtonClicked);
        m_onlineButton.onClick.AddListener(OnOnlineButtonClicked);
        m_backButton.onClick.AddListener(BackToMenu);
        m_changeButton.onClick.AddListener(ChangeSourceToTarget);
    }

    private void SetupDropdownListeners()
    {
        m_sourceLanguageDropdown.onValueChanged.AddListener(SelectSourceLanguage);
        m_targetLanguageDropdown.onValueChanged.AddListener(SelectTargetLanguage);
    }


    #endregion

    #region Button Handlers

    private void OnTranslateButtonClicked(TranslateMode mode)
    {
        Debug.Log($"{Tag} -> OnTranslateButtonClicked: {mode}");
        m_subTitle.text = Enum.GetName(typeof(TranslateMode), mode);
        // Set up the translate button and get languages based on the mode
        if (mode == TranslateMode.Offline)
        {
            m_translateButton.onClick.AddListener(OfflineTranslate);
            HMSMLTranslateKitManager.Instance.OnDownloadModelSuccess += OnDownloadModelSuccess;
            HMSMLTranslateKitManager.Instance.OnGetLocalAllLanguagesSuccess += OnGetLocalAllLanguagesSuccess;
            HMSMLTranslateKitManager.Instance.GetLocalAllLanguages();
        }
        else
        {
            m_translateButton.onClick.AddListener(Translate);
            HMSMLTranslateKitManager.Instance.OnGetCloudAllLanguagesSuccess += OnGetCloudAllLanguagesSuccess;
            HMSMLTranslateKitManager.Instance.GetCloudAllLanguages();
        }

        // Configure the TranslateKitManager for the specified mode
        HMSMLTranslateKitManager.Instance.Configure(mode);

        // Hide the translate menu and show the translate panel
        m_translateMenu.SetActive(false);
        m_translatePanel.SetActive(true);
    }

    private void OnOfflineButtonClicked()
    {
        OnTranslateButtonClicked(TranslateMode.Offline);
    }

    private void OnOnlineButtonClicked()
    {
        OnTranslateButtonClicked(TranslateMode.Online);
    }

    private void BackToMenu()
    {
        m_translateMenu.SetActive(true);
        m_translatePanel.SetActive(false);
        m_translateButton.gameObject.SetActive(false);
        m_sourceInputField.text = string.Empty;
        m_targetInputField.text = string.Empty;
        m_sourceLanguageDropdown.ClearOptions();
        m_targetLanguageDropdown.ClearOptions();
        HMSMLTranslateKitManager.Instance.OnDownloadModelSuccess = null;
        HMSMLTranslateKitManager.Instance.OnGetLocalAllLanguagesSuccess = null;
        HMSMLTranslateKitManager.Instance.OnGetCloudAllLanguagesSuccess = null;
        HMSMLTranslateKitManager.Instance.OnProcessAction = null;
        m_translateButton.onClick.RemoveAllListeners();
    }

    private void ChangeSourceToTarget()
    {
        // Swap languages
        int temp = m_sourceLanguageIndex;
        m_sourceLanguageIndex = m_targetLanguageIndex;
        m_targetLanguageIndex = temp;
        m_sourceLanguageDropdown.value = m_sourceLanguageIndex;
        m_targetLanguageDropdown.value = m_targetLanguageIndex;

        // Swap text
        m_sourceInputField.text = string.Empty;
        m_targetInputField.text = string.Empty;
    }
    #endregion

    #region Translation Methods

    private void OnGetCloudAllLanguagesSuccess(ISet<string> availableLanguages)
    {
        m_languages = new HashSet<string>(availableLanguages);
        Debug.Log($"{Tag} -> OnGetCloudAllLanguagesSuccess");
        PopulateDropdowns();
    }

    private void OnGetLocalAllLanguagesSuccess(ISet<string> availableLanguages)
    {
        m_languages = new HashSet<string>(availableLanguages.OrderBy(lang => lang).ToList());
        Debug.Log($"{Tag} -> OnGetLocalAllLanguagesSuccess");
        PopulateDropdowns();
    }

    private void OnDownloadModelSuccess()
    {
        Debug.Log($"{Tag} -> OnDownloadModelSuccess");
    }

    private void PopulateDropdowns()
    {
        m_sourceLanguageDropdown.ClearOptions();
        m_targetLanguageDropdown.ClearOptions();
        var options = m_languages.Select(lang => new Dropdown.OptionData(lang)).ToList();
        m_sourceLanguageDropdown.AddOptions(options);
        m_targetLanguageDropdown.AddOptions(options);
    }

    public void SelectSourceLanguage(int index)
    {
        m_sourceLanguageIndex = index;
        string selectedLanguage = m_languages.ElementAt(index);
        Debug.Log($"{Tag} -> SelectSourceLanguage: {selectedLanguage}");
        HMSMLTranslateKitManager.Instance.SetSourceLanguage(selectedLanguage);
        CheckIfTranslationPossible();
    }

    public void SelectTargetLanguage(int index)
    {
        m_targetLanguageIndex = index;
        string selectedLanguage = m_languages.ElementAt(index);
        Debug.Log($"{Tag} -> SelectTargetLanguage: {selectedLanguage}");
        HMSMLTranslateKitManager.Instance.SetTargetLanguage(selectedLanguage);
        CheckIfTranslationPossible();
    }

    private void CheckIfTranslationPossible()
    {
        bool isTranslationPossible = m_sourceLanguageIndex != 0 && m_targetLanguageIndex != 0 &&
                                     m_sourceLanguageIndex != m_targetLanguageIndex;

        Debug.Log($"{Tag} -> CheckIfTranslationPossible {isTranslationPossible}");

        if (isTranslationPossible)
        {
            m_translateButton?.gameObject?.SetActive(true);
        }
        else
        {
            m_translateButton?.gameObject?.SetActive(false);
        }

    }

    private void Translate()
    {
        if (m_sourceInputField.text == string.Empty)
        {
            AndroidToast.MakeText("Source Input Field is empty. Please enter a text to translate.").Show();
            return;
        }
        Debug.Log($"{Tag} -> Online Translate Start");
        HMSMLTranslateKitManager.Instance.Translate(m_sourceInputField.text, OnTranslateSuccess, OnTranslateFailure);
    }

    private void OfflineTranslate()
    {
        if (m_sourceInputField.text == string.Empty)
        {
            AndroidToast.MakeText("Source Input Field is empty. Please enter a text to translate.").Show();
            return;
        }
        Debug.Log($"{Tag} -> Offline Translate Start");
        HMSMLTranslateKitManager.Instance.TranslateOffline(m_sourceInputField.text, OnTranslateSuccess, OnTranslateFailure);

    }

    private void OnTranslateSuccess(string result)
    {
        Debug.Log($"{Tag} -> OnTranslateSuccess: {result}");
        m_targetInputField.text = result;
    }

    private void OnTranslateFailure(Exception error)
    {
        Debug.Log($"{Tag} -> OnTranslateFailure: {error.Message}");
    }

    private void OnProcessAction(long total, long current)
    {
        int progress = (int)((current * 100) / total);
        Debug.Log($"{Tag} -> OnProcessAction: {progress}%");
        // m_progressText.text = $"{progress}%";
    }

    public void ResetComponents()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
