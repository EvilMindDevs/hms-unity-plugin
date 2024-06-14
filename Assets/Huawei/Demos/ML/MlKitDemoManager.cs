using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MlKitDemoManager : MonoBehaviour
{
    [SerializeField] private GameObject m_mlKitDemoMenu;
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenTranslateDemo(GameObject translateMenu)
    {
        m_mlKitDemoMenu.SetActive(false);
        translateMenu.SetActive(true);
        Debug.Log($"[{TranslateDemoManager.Instance.enabled}] OpenTranslateDemo");
    }

    public void OpenTextToSpeechDemo(GameObject translateMenu)
    {
        m_mlKitDemoMenu.SetActive(false);
        translateMenu.SetActive(true);
        Debug.Log($"[{TextToSpeechDemoManager.Instance.enabled}] OpenTranslateDemo");

    }

    #endregion
}
