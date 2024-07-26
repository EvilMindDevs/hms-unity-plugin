using HmsPlugin;
using HuaweiMobileServices.Utils;
using UnityEngine;

public class MlKitDemoManager : MonoBehaviour
{
    [SerializeField] private GameObject m_mlKitDemoMenu;
    private bool IsTranslateModuleEnabled => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTranslateModule);
    private bool IsTTSEnable => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTextToSpeechModule);
    private bool IsLangDetectionEnable => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableLanguageDetectionModule);
    private bool IsTextRecognitionEnable => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTextRecognitionModule);

    #region Singleton
    public static MlKitDemoManager Instance { get; private set; }
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

    public void OpenTranslateDemo(GameObject translateMenu)
    {
        if (!IsTranslateModuleEnabled)
        {
            AndroidToast.MakeText("Translate Module is not enabled").Show();
            Debug.LogWarning("Translate Module is not enabled");
            return;
        }

        m_mlKitDemoMenu.SetActive(false);
        translateMenu.SetActive(true);
        Debug.Log($"[{TranslateDemoManager.Instance.enabled}] OpenTranslateDemo");
    }

    public void OpenTextToSpeechDemo(GameObject ttsMenu)
    {
        if (!IsTTSEnable)
        {
            AndroidToast.MakeText("Text To Speech Module is not enabled").Show();
            Debug.LogWarning("Text To Speech Module is not enabled");
            return;
        }
        m_mlKitDemoMenu.SetActive(false);
        ttsMenu.SetActive(true);
        Debug.Log($"[{TextToSpeechDemoManager.Instance.enabled}] OpenTextToSpeechDemo");
    }

    public void OpenLangDetectionDemo(GameObject ldMenu)
    {
        if (!IsLangDetectionEnable)
        {
            AndroidToast.MakeText("Lang Detection Module is not enabled").Show();
            Debug.LogWarning("Lang Detection Module is not enabled");
            return;
        }
        m_mlKitDemoMenu.SetActive(false);
        ldMenu.SetActive(true);
        Debug.Log($"[{LanguageDetectionDemoManager.Instance.enabled}] OpenLangDetectionDemo");
    }

    public void OpenTextRecognitionDemo(GameObject trMenu)
    {
        if (!IsTextRecognitionEnable)
        {
            AndroidToast.MakeText("Text Recognition Module is not enabled").Show();
            Debug.LogWarning("Text Recognition Module is not enabled");
            return;
        }
        m_mlKitDemoMenu.SetActive(false);
        trMenu.SetActive(true);
        Debug.Log($"[{TextRecognitionDemoManager.Instance.enabled}] OpenTextRecognitionDemo");
    }

    #endregion
}
