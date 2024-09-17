
using UnityEngine;
using UnityEngine.UI;
using HuaweiMobileServices.ML.SkeletonDetection;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.Utils;
using System.Collections.Generic;


public class SkeletonDetectionDemoManager : MonoBehaviour
{
    private const string TAG = "[HMS] SkeletonDetectionDemoManager ";

    [SerializeField] private Button m_backButton;
    [SerializeField] private Button m_selectImageButton;
    [SerializeField] private Button m_detectButton;
    [SerializeField] private InputField m_inputField;
    [SerializeField] public GameObject m_mlKitMenu;
    [SerializeField] public GameObject m_skeletonMenu;


    private MLFrame m_frame;


    #region Singleton

    public static SkeletonDetectionDemoManager Instance { get; private set; }
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

    private void Start()
    {
        SetupButtonListeners();
        HMSMLSkeletonDetectionManager.Instance.InitConfiguration(MLSkeletonAnalyzerSetting.TYPE_NORMAL);
        HMSMLTextRecognitionKitManager.Instance.OnImageSelectSuccess += OnImageSelectSuccess;
        Debug.Log(TAG + "Start");
    }

    private void Awake()
    {
        Singleton();
    }

    private void SetupButtonListeners()
    {
       
        m_backButton.onClick.AddListener(BackToMenu);
        m_selectImageButton.onClick.AddListener(OnSelectImageButton);
        m_detectButton.onClick.AddListener(OnDetectionButton);
       
    }



    private void OnDetectionButton()
    {
        SkeletonDetectionSync();
    }
    private void OnSelectImageButton()
    {
        Debug.Log(TAG + "OnSelectImageButton");
        HMSMLTextRecognitionKitManager.Instance.SelectImage();
    }

    public void OnDetectFrameSuccess(IList<MLSkeleton> result)
    {
        Debug.Log(TAG + "OnDetectFrameSuccess");

        m_inputField.text = string.Empty;
        Debug.Log($"{TAG} -> GetJoints: {(result.Count != 0 || result != null)}\n");
        if (result.Count != 0 || result != null)
        {
            foreach (MLSkeleton item in result)
            {
                var task = item.GetJoints();
                Debug.Log($"{TAG} -> TASK: {task:F2}\n");
                foreach (var joint in task)
                {
                    Debug.Log($"{TAG} -> GetPointX: {joint.GetPointX():F2} - GetPointY: {joint.GetPointY():F2}\n");
                    m_inputField.text += $"GetPointX: {joint.GetPointX():F2} - GetPointY: {joint.GetPointY():F2}\n";
                }

            }
        }
        else
        {

            m_inputField.text = "list is empty or null";
        }


        AndroidToast.MakeText("Skeleton Detection Success").Show();
    }


    private void SkeletonDetectionAsync() {

        HMSMLSkeletonDetectionManager.Instance.AnalyzeFrameAsync(m_frame, OnDetectFrameSuccess, OnDetectFrameFailure);

    }

    private void SkeletonDetectionSync() {

       var result = HMSMLSkeletonDetectionManager.Instance.AnalyzeFrameSync(m_frame);

        foreach (MLSkeleton item in result)
        {
            var task = item.GetJoints();
            Debug.Log($"{TAG} -> TASK: {task:F2}\n");
            foreach (var joint in task)
            {
                Debug.Log($"{TAG} -> GetPointX: {joint.GetPointX():F2} - GetPointY: {joint.GetPointY():F2}\n");
                m_inputField.text += $"GetPointX: {joint.GetPointX():F2} - GetPointY: {joint.GetPointY():F2}\n";
            }

        }

    }


    public void OnDetectFrameFailure(HMSException exception)
    {
        m_inputField.text = exception.WrappedExceptionMessage;
        Debug.Log($"{TAG} -> OnDetectFrameFailure: {exception.WrappedExceptionMessage}\n");
    }

    public void OnImageSelectSuccess(AndroidIntent intent, AndroidBitmap bitmap)
    {
        Debug.Log(TAG + "OnImageSelectSuccess");
        m_frame = MLFrame.FromBitmap(bitmap);
        AndroidToast.MakeText("Image Selected").Show();
    }

    private void BackToMenu()
    {
        m_mlKitMenu.gameObject.SetActive(true);
        m_skeletonMenu.gameObject.SetActive(false);
        m_inputField.text = string.Empty;
   
        m_detectButton.onClick.RemoveAllListeners();
        m_selectImageButton.onClick.RemoveAllListeners();
        StopDetection();
    }

    private void StopDetection() {
        HMSMLSkeletonDetectionManager.Instance.StopSkeletonDetection();
    }

}
