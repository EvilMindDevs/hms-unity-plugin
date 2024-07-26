using Assets.Huawei.Scripts.ML.LangDetector;
using HuaweiMobileServices.ML.LanguageDetection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageDetectionDemoManager : MonoBehaviour
{
    private const string TAG = "[HMS] LanguageDetectDemoManager ";
    [SerializeField] public Button onlineButton;
    [SerializeField] public Button offlineButton;
    [SerializeField] public InputField langInputField;
    [SerializeField] public GameObject detectionMenu;
    [SerializeField] public GameObject buttonMenu;
    [SerializeField] public Button firstBestDetectButton;
    [SerializeField] public Button probilityDetectButton;
    [SerializeField] public Button backButton;
    [SerializeField] public Text resultText;
    [SerializeField] public Text titleText;

    #region Singleton

    public static LanguageDetectionDemoManager Instance { get; private set; }
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
    private void Awake()
    {
        Singleton();
    }
    void Start()
    {
        onlineButton.onClick.AddListener(OnOnlineButtonClicked);
        offlineButton.onClick.AddListener(OnOfflineButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        langInputField.text = "Merhaba nasilsin, Hello how are you, مرحبا كيف حالك, 你好吗, Hallo hoe is het, Hola, cómo estás";
    }

    private void OnOnlineButtonClicked()
    {
        OnDetectionButtonClicked(DetectionMode.Online);
    }

    private void OnOfflineButtonClicked()
    {
        OnDetectionButtonClicked(DetectionMode.Offline);
    }

    private void OnBackButtonClicked()
    {
        buttonMenu.gameObject.SetActive(true);
        detectionMenu.gameObject.SetActive(false);
        resultText.text = string.Empty;
        titleText.text = string.Empty;
        firstBestDetectButton.onClick.RemoveAllListeners();
        probilityDetectButton.onClick.RemoveAllListeners();
        StopDetection();
    }

    private void OnDetectionButtonClicked(DetectionMode detectionMode)
    {
        buttonMenu.gameObject.SetActive(false);
        detectionMenu.gameObject.SetActive(true);
        if (detectionMode == DetectionMode.Online)
        {
            Debug.Log($"{TAG} -> Detection ONLINE");
            titleText.text = "ONLINE DETECTION";
            HMSMLLanguageDetectionManager.Instance.Configure(detectionMode);
            firstBestDetectButton.onClick.AddListener(SyncFirstBestDetectOnline);
            probilityDetectButton.onClick.AddListener(SyncProbilityDetectOnline);
        }
        else
        {
            Debug.Log($"{TAG} -> Detection OFFLINE");
            titleText.text = "LOCAL DETECTION";
            HMSMLLanguageDetectionManager.Instance.Configure(detectionMode);
            firstBestDetectButton.onClick.AddListener(SyncFirstBestDetectOffline);
            probilityDetectButton.onClick.AddListener(SyncProbilityDetectOffline);
        }
    }

    //Ml Remote Lang Detector Functions (Online)
    private void FirstBestDetectOnline()
    {
        Debug.Log($"{TAG} -> FirstBestDetectOnline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.FirstBestDetectRemote(langInputField.text, OnDetectString, OnDetectFailure);
    }

    private void SyncFirstBestDetectOnline()
    {
        Debug.Log($"{TAG} -> SyncFirstBestDetectOnline" + langInputField.text);
        var detectedText = HMSMLLanguageDetectionManager.Instance.SyncFirstBestDetectRemote(langInputField.text);
        resultText.text = detectedText;
    }

    private void ProbilityDetectOnline()
    {
        Debug.Log($"{TAG} -> ProbilityDetectOnline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.ProbilityDetectRemote(langInputField.text, OnDetectList, OnDetectFailure);
    }

    private void SyncProbilityDetectOnline()
    {
        Debug.Log($"{TAG} -> SyncProbilityDetectOnline" + langInputField.text);
        var detectedList = HMSMLLanguageDetectionManager.Instance.SyncProbilityDetectRemote(langInputField.text);
        OnDetectList(detectedList);
    }

    //ML Local Lang Detector Functions (Offline)
    private void FirstBestDetectOffline()
    {
        Debug.Log($"{TAG} -> FirstBestDetectOffline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.FirstBestDetectLocal(langInputField.text, OnDetectString, OnDetectFailure);
    }

    private void SyncFirstBestDetectOffline()
    {
        Debug.Log($"{TAG} -> SyncFirstBestDetectOffline" + langInputField.text);
        var detectedText = HMSMLLanguageDetectionManager.Instance.SyncFirstBestDetectLocal(langInputField.text);
        resultText.text = detectedText;
    }

    private void ProbilityDetectOffline()
    {
        Debug.Log($"{TAG} -> ProbilityDetectOffline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.ProbilityDetectLocal(langInputField.text, OnDetectList, OnDetectFailure);
    }

    private void SyncProbilityDetectOffline()
    {
        Debug.Log($"{TAG} -> SyncProbilityDetectOffline" + langInputField.text);
        var detectedList = HMSMLLanguageDetectionManager.Instance.SyncProbilityDetectLocal(langInputField.text);
        OnDetectList(detectedList);
    }

    private void StopDetection()
    {

        HMSMLLanguageDetectionManager.Instance.StopLocalLangDetector();
        HMSMLLanguageDetectionManager.Instance.StopRemoteLangDetector();
    }

    private void OnDetectList(IList<MLDetectedLang> result)
    {


        resultText.text = string.Empty;
        if (result.Count != 0 || result != null)
        {
            foreach (MLDetectedLang item in result)
            {
                resultText.text += $"Language: {item.GetLangCode()}, Probability: {item.GetProbability():F2}\n";
                Debug.Log($"{TAG} -> Language: {item.GetLangCode()}, Probability: {item.GetProbability():F2}");
            }
        }
        else
        {

            resultText.text = "list is empty or null";
        }



    }

    private void StopDetect()
    {
        HMSMLLanguageDetectionManager.Instance.StopRemoteLangDetector();
        HMSMLLanguageDetectionManager.Instance.StopLocalLangDetector();
    }

    private void OnDetectString(string result)
    {
        resultText.text = result;
    }

    private void OnDetectFailure(Exception error)
    {
        Debug.Log($"{TAG} -> OnDetectFailure: {error.Message}");
    }

    public void ResetComponents()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
