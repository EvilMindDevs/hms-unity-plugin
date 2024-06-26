using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HuaweiMobileServices.ML.TextToSpeech;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextToSpeechDemoManager : MonoBehaviour
{
    #region Variables
    private const string TAG = "[HMS] TextToSpeechDemoManager";
    [SerializeField] private InputField m_inputField;
    [SerializeField] private Text m_wordCountText;
    [SerializeField] private Button m_pausedButton;
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Slider m_volumeSlider;
    [SerializeField] private Slider m_speechRateSlider;
    [SerializeField] private Dropdown m_speakerDropdown;
    [SerializeField] private Dropdown m_playModeDropdown;
    [SerializeField] private Button m_onlineButton;
    [SerializeField] private Button m_offlineButton;
    [SerializeField] private GameObject m_body;
    [SerializeField] private Text m_progressText;
    [SerializeField] private GameObject m_offlineModeWarning;
    [SerializeField] private Button m_resetButton;
    [SerializeField] private GameObject m_basePanel;
    [SerializeField] private GameObject m_textToSpeechPanel;
    private enum TTSMode { ONLINE, OFFLINE }
    private TTSMode m_ttsMode;
    private List<MLTtsSpeaker> m_speakers = new List<MLTtsSpeaker>();
    private string[] playModeResources = new string[] { "0-QUEUE_APPEND", "1-QUEUE_FLUSH" };

    #endregion

    #region Singleton

    public static TextToSpeechDemoManager Instance { get; private set; }
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
    private void Awake()
    {
        Singleton();
    }

    void Start()
    {

        m_offlineButton.onClick.AddListener(() =>
        {
            HMSMLTextToSpeechKitManager.Instance.Configurations(MLTtsConstants.TTS_OFFLINE_MODE);
            OnClickHeaderButton();
            m_ttsMode = TTSMode.OFFLINE;
            HMSMLTextToSpeechKitManager.Instance.OnProcessAction += OnDownloadProgress;

            m_offlineModeWarning.gameObject.SetActive(true);
        });

        m_onlineButton.onClick.AddListener(delegate
        {
            HMSMLTextToSpeechKitManager.Instance.Configurations(MLTtsConstants.TTS_ONLINE_MODE);
            OnClickHeaderButton();
            m_ttsMode = TTSMode.ONLINE;
            m_offlineModeWarning.SetActive(false);
        });

        m_playModeDropdown.ClearOptions();
        m_playModeDropdown.AddOptions(playModeResources.Select(x => x).ToList());


        if (PlayerPrefs.HasKey("PlayMode"))
        {
            m_speakerDropdown.value = PlayerPrefs.GetInt("PlayMode");

        }
        else
        {
            m_playModeDropdown.value = 0;
            PlayerPrefs.SetInt("PlayMode", 0);
        }
        PlayerPrefs.Save();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CalculateInputWordCount()
    {
        string inputText = m_inputField.text;
        m_wordCountText.text = $"Entered: {inputText.ToCharArray().Length} / 500";
    }

    public void StartTextToSpeech()
    {
        string inputText = m_inputField.GetComponent<InputField>().text;
        int playMode = Convert.ToInt32(playModeResources[m_playModeDropdown.value].Split('-')[0]);
        var result = HMSMLTextToSpeechKitManager.Instance.Speak(inputText, playMode);
        Debug.Log($"{TAG} -> Speak Result: {result}");
    }

    public void StopTextToSpeech()
    {
        HMSMLTextToSpeechKitManager.Instance.Stop();
    }

    public void PauseTextToSpeech()
    {
        m_resumeButton.gameObject.SetActive(true);
        HMSMLTextToSpeechKitManager.Instance.Pause();
        m_pausedButton.gameObject.SetActive(false);
    }

    public void ResumeTextToSpeech()
    {
        m_pausedButton.gameObject.SetActive(true);
        HMSMLTextToSpeechKitManager.Instance.Resume();
        m_resumeButton.gameObject.SetActive(false);
    }

    public void SetVolume()
    {
        Debug.Log("Volume: " + m_volumeSlider.value);
        HMSMLTextToSpeechKitManager.Instance.SetVolume(m_volumeSlider.value);
    }

    public void SetSpeechRate()
    {
        HMSMLTextToSpeechKitManager.Instance.SetSpeed(m_speechRateSlider.value);
    }

    public List<MLTtsSpeaker> GetAllSpeakers()
    {
        return HMSMLTextToSpeechKitManager.Instance.GetSpeakers();
    }

    public void SetSpeaker()
    {
        HMSMLTextToSpeechKitManager.Instance.SetSpeaker(m_speakers[m_speakerDropdown.value].Name);
    }

    public void SetPlayMode()
    {
        PlayerPrefs.SetInt("PlayMode", m_playModeDropdown.value);
    }

    public void OnClickHeaderButton()
    {
        m_body.SetActive(true);
        m_speakers = GetAllSpeakers();
        Debug.Log("Speakers: " + m_speakers.Count);
        m_speakerDropdown.ClearOptions();
        m_speakerDropdown.AddOptions(m_speakers.Select(x => x.Name).ToList());
    }

    public void OnDownloadProgress(long current, long total)
    {
        int percentage = (int)((current * 100) / total);
        Debug.Log($"Download Progress: {percentage}/100");

        HMSDispatcher.InvokeAsync(() =>
        {
            m_progressText.gameObject.SetActive(true);
            m_progressText.text = $"Downloading: {percentage}/100";


            if (percentage >= 99)
            {
                StartCoroutine(HideProgressAfterDelay(5));
            }
        });

    }

    private IEnumerator HideProgressAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        HMSDispatcher.InvokeAsync(() =>
        {
            m_progressText.gameObject.SetActive(false);
        });
    }

    public void ResetComponents()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

}
