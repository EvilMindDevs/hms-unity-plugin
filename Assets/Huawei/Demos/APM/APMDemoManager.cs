using HuaweiMobileServices.APM;

using UnityEngine;

public class APMDemoManager : MonoBehaviour
{
    private readonly string TAG = "[HMS] APMDemoManager ";

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {

    }

    #region Singleton

    public static APMDemoManager Instance { get; private set; }
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

    public void APMSCollectionOff()
    {
        Debug.Log(TAG + "APMSCollectionOff");

        HMSAPMManager.Instance.EnableCollection(false);
    }

    public void APMSCollectionOn()
    {
        Debug.Log(TAG + "APMSCollectionOn");

        HMSAPMManager.Instance.EnableCollection(true);
    }

    public void APMSAnrMonitorOff()
    {
        Debug.Log(TAG + "APMSAnrMonitorOff");

        HMSAPMManager.Instance.EnableAnrMonitor(false);
    }

    public void APMSAnrMonitorOn()
    {
        Debug.Log(TAG + "APMSAnrMonitorOn");

        HMSAPMManager.Instance.EnableAnrMonitor(true);
    }

    #region SendCustomEvent

    public void SendCustomEvent()
    {
        Debug.Log(TAG + "SendCustomEvent");

        var customTrace = APMS.GetInstance().CreateCustomTrace("CustomEvent1");
        customTrace.Start();

        //code you want to trace

        BusinessLogicStart(customTrace);
        BusinessLogicEnd(customTrace);
        customTrace.Stop();

    }

    private void BusinessLogicStart(CustomTrace customTrace)
    {
        customTrace.PutMeasure("ProcessingTimes", 0);

        for (int i = 0; i < 5; i++)
        {
            customTrace.IncrementMeasure("ProcessingTimes", 1);
        }
    }

    private void BusinessLogicEnd(CustomTrace customTrace)
    {
        customTrace.PutProperty("ProcessingResult", "Success");
        customTrace.PutProperty("Status", "Normal");
    }

    #endregion

    public void SendCustomEventByAnnotation()
    {
        Debug.Log(TAG + "SendCustomEventByAnnotation");
    }

}
