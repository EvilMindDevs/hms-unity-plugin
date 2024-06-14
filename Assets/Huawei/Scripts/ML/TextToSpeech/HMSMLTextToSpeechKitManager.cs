using HmsPlugin;
using HuaweiMobileServices.ML.TextToSpeech;
using HuaweiMobileServices.ML.Common;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using HuaweiMobileServices.ML.DownloadModel;
using HuaweiMobileServices.Utils;
public class HMSMLTextToSpeechKitManager : HMSManagerSingleton<HMSMLTextToSpeechKitManager>
{
    private const string TAG = "[HMS] MLTextToSpeechKitManager ";
    public Action<long, long> OnProcessAction;
    private readonly string API_KEY;
    private readonly string OnlineDefaultLanguageCode;
    private readonly string OnlineDefaultSpeakerCode;
    private readonly string OfflineDefaultLanguageCode;
    private readonly string OfflineDefaultSpeakerCode;

    private string SelectedOfflineLanguageCode;
    private string SelectedOfflineSpeakerCode;

    private Dictionary<string, string> LanguageCodes = new Dictionary<string, string>
    {
        { MLTtsConstants.TTS_SPEAKER_FEMALE_EN, MLTtsConstants.TTS_EN_US },     // English
        { MLTtsConstants.TTS_SPEAKER_FEMALE_EN_2, MLTtsConstants.TTS_EN_US },   // English
        { MLTtsConstants.TTS_SPEAKER_MALE_EN, MLTtsConstants.TTS_EN_US },       // English
        { MLTtsConstants.TTS_SPEAKER_MALE_EN_2, MLTtsConstants.TTS_EN_US },     // English
        { MLTtsConstants.TTS_SPEAKER_FEMALE_TR, MLTtsConstants.TTS_LAN_TR_TR }, // Turkish
        { MLTtsConstants.TTS_SPEAKER_FEMALE_AR, MLTtsConstants.TTS_LAN_AR_AR }, // Arabic
        { MLTtsConstants.TTS_SPEAKER_MALE_ZH, MLTtsConstants.TTS_ZH_HANS },     // Chinese
        { MLTtsConstants.TTS_SPEAKER_MALE_ZH_2, MLTtsConstants.TTS_ZH_HANS },   // Chinese
        { MLTtsConstants.TTS_SPEAKER_FEMALE_ZH, MLTtsConstants.TTS_ZH_HANS },   // Chinese
        { MLTtsConstants.TTS_SPEAKER_FEMALE_ZH_2, MLTtsConstants.TTS_ZH_HANS }, // Chinese
        { MLTtsConstants.TTS_SPEAKER_FEMALE_DE, MLTtsConstants.TTS_LAN_DE_DE }, // German
        { MLTtsConstants.TTS_SPEAKER_FEMALE_ES, MLTtsConstants.TTS_LAN_ES_ES }, // Spanish
        { MLTtsConstants.TTS_SPEAKER_FEMALE_FR, MLTtsConstants.TTS_LAN_FR_FR }, // French
        { MLTtsConstants.TTS_SPEAKER_FEMALE_IT, MLTtsConstants.TTS_LAN_IT_IT }, // Italian
        { MLTtsConstants.TTS_SPEAKER_FEMALE_MS, MLTtsConstants.TTS_LAN_MS_MS }, // Malay
        { MLTtsConstants.TTS_SPEAKER_FEMALE_PL, MLTtsConstants.TTS_LAN_PL_PL }, // Polish
        { MLTtsConstants.TTS_SPEAKER_FEMALE_RU, MLTtsConstants.TTS_LAN_RU_RU }, // Russian
        { MLTtsConstants.TTS_SPEAKER_FEMALE_TH, MLTtsConstants.TTS_LAN_TH_TH }, // Thai
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_MALE_EAGLE, MLTtsConstants.TTS_EN_US },      // English
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_FEMALE_BEE, MLTtsConstants.TTS_LAN_DE_DE },  // English
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_FEMALE_EAGLE, MLTtsConstants.TTS_EN_US },    // English
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_FEMALE_BOLT, MLTtsConstants.TTS_EN_US },     // English
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_FEMALE_FLY, MLTtsConstants.TTS_EN_US },      // English
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_MALE_BOLT, MLTtsConstants.TTS_EN_US },       // English
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_DE_DE_FEMALE_FLY, MLTtsConstants.TTS_LAN_DE_DE },  // German
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_DE_DE_FEMALE_BEE, MLTtsConstants.TTS_LAN_DE_DE },  // German
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_ES_ES_FEMALE_BEE, MLTtsConstants.TTS_LAN_ES_ES },  // Spanish
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_ES_ES_FEMALE_FLY, MLTtsConstants.TTS_LAN_ES_ES },  // Spanish
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_IT_IT_FEMALE_FLY, MLTtsConstants.TTS_LAN_IT_IT },  // Italian
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_IT_IT_FEMALE_BEE, MLTtsConstants.TTS_LAN_IT_IT },  // Italian
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_FR_FR_FEMALE_BEE, MLTtsConstants.TTS_LAN_FR_FR },  // French
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_FR_FR_FEMALE_FLY, MLTtsConstants.TTS_LAN_FR_FR },  // French
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_RU_RU_FEMALE_FLY, MLTtsConstants.TTS_LAN_RU_RU },  // Russian
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_AR_AR_FEMALE_FLY, MLTtsConstants.TTS_LAN_AR_AR },  // Arabic
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_TH_TH_FEMALE_FLY, MLTtsConstants.TTS_LAN_TH_TH },  // Thai
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_ZH_HANS_FEMALE_BOLT, MLTtsConstants.TTS_ZH_HANS }, // Chinese
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_ZH_HANS_MALE_BOLT, MLTtsConstants.TTS_ZH_HANS },   // Chinese
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_ZH_HANS_FEMALE_EAGLE, MLTtsConstants.TTS_ZH_HANS }, // Chinese
        { MLTtsConstants.TTS_SPEAKER_OFFLINE_ZH_HANS_MALE_EAGLE, MLTtsConstants.TTS_ZH_HANS }, // Chinese
    };
    private MLTtsConfig mlConfigs;
    private MLTtsEngine mlTtsEngine;
    private MLLocalModelManager mLLocalModelManager;
    private int DefaultUserRegion = MLApplication.REGION_DR_GERMAN;
    private string m_ttsMode;
    public HMSMLTextToSpeechKitManager()
    {
        OnlineDefaultLanguageCode = MLTtsConstants.TTS_EN_US;
        OnlineDefaultSpeakerCode = MLTtsConstants.TTS_SPEAKER_FEMALE_EN;

        OfflineDefaultLanguageCode = MLTtsConstants.TTS_EN_US;
        OfflineDefaultSpeakerCode = MLTtsConstants.TTS_SPEAKER_OFFLINE_EN_US_FEMALE_BEE;

        API_KEY = HMSMLKitSettings.Instance.Settings.Get(HMSMLKitSettings.MLKeyAPI);

        HMSManagerStart.Start(OnAwake, TAG);
    }
    private void OnAwake()
    {
        Init();
    }

    public void Init()
    {
        MLApplication instance = MLApplication.Initialize();
        instance.SetApiKey(API_KEY);
        instance.SetUserRegion(DefaultUserRegion);
        Debug.Log(TAG + "-> Init");
    }

    public void Configurations(string ttsMode)
    {
        Debug.Log(TAG + "-> Configurations {ttsMode}");
        m_ttsMode = ttsMode;
        mlConfigs = new MLTtsConfig()
            .SetLanguage(ttsMode == MLTtsConstants.TTS_ONLINE_MODE ? OnlineDefaultLanguageCode : OfflineDefaultLanguageCode)
            .SetPerson(ttsMode == MLTtsConstants.TTS_ONLINE_MODE ? OnlineDefaultSpeakerCode : OfflineDefaultSpeakerCode)
            .SetSynthesizeMode(ttsMode);

        if (ttsMode == MLTtsConstants.TTS_ONLINE_MODE)
        {
            mlConfigs.SetSpeed(1.0f).SetVolume(1.0f);
        }

        mlTtsEngine = new MLTtsEngine(mlConfigs);

        var listener = new MLTtsCallback(new MLTextToSpeechListenerManager());

        mlTtsEngine.SetTtsCallback(listener);

        if (m_ttsMode == MLTtsConstants.TTS_ONLINE_MODE)
        {
            mlTtsEngine.SetPlayerVolume(20);
        }

        mlTtsEngine.UpdateConfig(mlConfigs);

        if (m_ttsMode == MLTtsConstants.TTS_OFFLINE_MODE)
        {
            OnDeviceModelManager();
        }
    }

    public void SetLanguage(string languageCode)
    {
        Debug.Log(TAG + "-> SetLanguage: " + languageCode);
        mlConfigs.SetLanguage(languageCode);
        mlTtsEngine.UpdateConfig(mlConfigs);
    }

    public void SetSpeaker(string speakerCode)
    {
        mlConfigs.SetPerson(speakerCode);
        Debug.Log(TAG + "-> SetSpeaker: " + speakerCode);
        var languageCode = LanguageCodes.GetValueOrDefault(speakerCode);
        SetLanguage(languageCode);
        mlTtsEngine.UpdateConfig(mlConfigs);

        if (m_ttsMode == MLTtsConstants.TTS_OFFLINE_MODE)
        {
            SelectedOfflineSpeakerCode = speakerCode;
            OnDeviceModelManager();
        }
    }

    public void SetSpeed(float speed)
    {
        mlConfigs.SetSpeed(speed);
        mlTtsEngine.UpdateConfig(mlConfigs);
    }

    public void SetVolume(float volume)
    {
        mlConfigs.SetVolume(volume);
        mlTtsEngine.UpdateConfig(mlConfigs);
    }

    public void Stop()
    {
        mlTtsEngine.Stop();
    }

    public void Shutdown()
    {
        mlTtsEngine.Shutdown();
    }

    public string Speak(string text, int playMode)
    {
        try
        {
            return mlTtsEngine.Speak(text, playMode);
        }
        catch (Exception e)
        {
            Debug.LogError(TAG + "-> Speak Error: " + e.Message);
            return null;
        }
    }

    public void Pause()
    {
        mlTtsEngine.Pause();
    }

    public void Resume()
    {
        mlTtsEngine.Resume();
    }

    public List<MLTtsSpeaker> GetSpeakers()
    {
        return mlTtsEngine.GetSpeakers().ToList();
    }

    public void OnDeviceModelManager()
    {
        var offlineSpeaker = SelectedOfflineSpeakerCode ?? OfflineDefaultSpeakerCode;
        // Create an on-device TTS model manager.
        mLLocalModelManager = MLLocalModelManager.Instance;
        // Create an MLTtsLocalModel instance to set the speaker so that the language model corresponding to the speaker can be downloaded through the model manager.
        MLTtsLocalModel model = new MLTtsLocalModel.Factory(offlineSpeaker).Create();

        // Check whether the model corresponding to the speaker has been downloaded. If not, download the model.
        mLLocalModelManager.IsModelExist(model).AddOnSuccessListener((result) =>
        {
            bool isExist = result;
            if (!isExist)
            {
                Debug.Log($"{TAG} -> Model does not exist, downloading {offlineSpeaker}");
                DownloadModel(offlineSpeaker);
            }
            else
            {
                Debug.Log($"{TAG} -> Model exists, {offlineSpeaker}");
            }
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log($"{TAG} -> IsModelExist Failed: {exception.Message}");
        });
    }

    private void DownloadModel(string person)
    {
        // Create an on-device TTS model manager.
        mLLocalModelManager = MLLocalModelManager.Instance;
        // Create an MLTtsLocalModel instance and pass the speaker (indicating by person) to download the language model corresponding to the speaker.
        MLTtsLocalModel model = new MLTtsLocalModel.Factory(person).Create();
        // Create a download policy configurator. You can set that when any of the following conditions is met, the model can be downloaded: 1. The device is charging; 2. Wi-Fi is connected; 3. The device is idle.
        MLModelDownloadStrategy request = new MLModelDownloadStrategy.Factory().NeedWifi().Create();
        //configuring the download listener
        var listenerInstance = new MLDownloadListenerManager
        {
            OnProcessAction = OnProcessAction
        };
        var listener = new MLModelDownloadListener(listenerInstance);

        // Register the download listener.
        mLLocalModelManager.DownloadModel(model, request, listener).AddOnSuccessListener((result) =>
        {
            Debug.Log("Model downloaded successfully");
            mlTtsEngine.UpdateConfig(mlConfigs);
        }).AddOnFailureListener((exception) =>
        {
            Debug.Log("Model downloaded failed");
        });
    }
}
