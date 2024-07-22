using Assets.Huawei.Scripts.ML.LangDetector;
using HuaweiMobileServices.ML.LanguageDetection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDetectionDemoManager : MonoBehaviour
{

    private const string Tag = "[HMS] LanguageDetectDemoManager ";
    public Button onlineButton;
    public Button offlineButton;
    public InputField langInputField;
    public GameObject detectionMenu;
    public GameObject buttonMenu;
    public Button firstBestDetectButton;
    public Button probilityDetectButton;
    public Button backButton;
    public Text resultText;
    public Text titleText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("AZIZ WAS HERE!!");
        onlineButton.onClick.AddListener(OnOnlineButtonClicked);
        offlineButton.onClick.AddListener(OnOfflineButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        langInputField.text = "Merhaba nasilsin, Hello how are you, مرحبا كيف حالك, 你好吗, Hallo hoe is het, Hola, cómo estás";
    }


    private void OnOnlineButtonClicked()
    {
        Debug.Log("Online clicked");
        OnDetectionButtonClicked(DetectionMode.Online);
        // Online butonuna basıldığında yapılacak işlemleri ekleyin
    }

    private void OnOfflineButtonClicked()
    {
        Debug.Log("Offline clicked");
        OnDetectionButtonClicked(DetectionMode.Offline);
        // Offline butonuna basıldığında yapılacak işlemleri ekleyin
    }
    private void OnBackButtonClicked()
    {
        Debug.Log("Back clicked");
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
            Debug.Log("Detection ONLINE");
            titleText.text = "ONLINE DETECTION";
            HMSMLLanguageDetectionManager.Instance.Configure(detectionMode);
            firstBestDetectButton.onClick.AddListener(SyncFirstBestDetectOnline);
            probilityDetectButton.onClick.AddListener(SyncProbilityDetectOnline);

        }
        else
        {
            Debug.Log("Detection OFFLINE");
            titleText.text = "LOCAL DETECTION";
            HMSMLLanguageDetectionManager.Instance.Configure(detectionMode);
            firstBestDetectButton.onClick.AddListener(SyncFirstBestDetectOffline);
            probilityDetectButton.onClick.AddListener(SyncProbilityDetectOffline);
        }
    }


    //Ml Remote Lang Detector Functions (Online) 
    private void FirstBestDetectOnline()
    {
        Debug.Log("FirstBestDetectOnline");
        Debug.Log("FirstBestDetectOnline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.FirstBestDetectRemote(langInputField.text, OnDetectString, OnDetectFailure);
    }
    private void SyncFirstBestDetectOnline()
    {
        Debug.Log("SyncFirstBestDetectOnline");
        Debug.Log("SyncFirstBestDetectOnline" + langInputField.text);
        var detectedText = HMSMLLanguageDetectionManager.Instance.SyncFirstBestDetectRemote(langInputField.text);
        resultText.text = detectedText;
    }
    private void ProbilityDetectOnline()
    {
        Debug.Log("ProbilityDetectOnline");
        Debug.Log("ProbilityDetectOnline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.ProbilityDetectRemote(langInputField.text, OnDetectList, OnDetectFailure);
    }
    private void SyncProbilityDetectOnline()
    {
        Debug.Log("SyncProbilityDetectOnline");
        Debug.Log("SyncProbilityDetectOnline" + langInputField.text);
        var detectedList = HMSMLLanguageDetectionManager.Instance.SyncProbilityDetectRemote(langInputField.text);
        OnDetectList(detectedList);
    }


    //ML Local Lang Detector Functions (Offline)
    private void FirstBestDetectOffline()
    {
        Debug.Log("FirstBestDetectOffline");
        Debug.Log("FirstBestDetectOffline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.FirstBestDetectLocal(langInputField.text, OnDetectString, OnDetectFailure);
    }
    private void SyncFirstBestDetectOffline()
    {
        Debug.Log("SyncFirstBestDetectOffline");
        Debug.Log("SyncFirstBestDetectOffline" + langInputField.text);
        var detectedText = HMSMLLanguageDetectionManager.Instance.SyncFirstBestDetectLocal(langInputField.text);
        resultText.text = detectedText;
    }
    private void ProbilityDetectOffline()
    {
        Debug.Log("ProbilityDetectOffline");
        Debug.Log("ProbilityDetectOffline" + langInputField.text);
        HMSMLLanguageDetectionManager.Instance.ProbilityDetectLocal(langInputField.text, OnDetectList, OnDetectFailure);
    }

    private void SyncProbilityDetectOffline()
    {
        Debug.Log("SyncProbilityDetectOffline");
        Debug.Log("SyncProbilityDetectOffline" + langInputField.text);
        var detectedList = HMSMLLanguageDetectionManager.Instance.SyncProbilityDetectLocal(langInputField.text);
        OnDetectList(detectedList);
    }




    private void StopDetection() {

        HMSMLLanguageDetectionManager.Instance.StopLocalLangDetector();
        HMSMLLanguageDetectionManager.Instance.StopRemoteLangDetector();
    }


    private void OnDetectList(IList<MLDetectedLang> result) {
        Debug.Log($"OnDetectList List Count: {result.Count}");

        resultText.text = "";
        if (result.Count!=0||result!=null)
        {
            foreach (MLDetectedLang item in result)
            {
                resultText.text += $"Language: {item.GetLangCode()}, Probability: {item.GetProbability():F2}\n";
                Debug.Log ($"Language: {item.GetLangCode()}, Probability: {item.GetProbability():F2}");
            }
        }
        else {

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
        Debug.Log($"{Tag} -> OnDetectFailure: {error.Message}");
    }
}
