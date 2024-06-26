using HmsPlugin;
using HuaweiMobileServices.Utils;
using UnityEngine;

public class MlKitDemoManager : MonoBehaviour
{
    [SerializeField] private GameObject m_mlKitDemoMenu;
    private bool IsTranslateModuleEnabled => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTranslateModule);
    private bool IsTTSEnable => HMSMLKitSettings.Instance.Settings.GetBool(HMSMLKitSettings.EnableTextToSpeechModule);

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
            Debug.Log("Translate Module is not enabled");
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
            Debug.Log("Text To Speech Module is not enabled");
            return;
        }
        m_mlKitDemoMenu.SetActive(false);
        ttsMenu.SetActive(true);
        Debug.Log($"[{TextToSpeechDemoManager.Instance.enabled}] OpenTextToSpeechDemo");

    }

    #endregion
}
