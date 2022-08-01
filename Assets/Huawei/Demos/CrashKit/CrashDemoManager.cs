
using UnityEngine;

public class CrashDemoManager : MonoBehaviour
{
    private readonly string TAG = "[HMS] CrashDemoManager ";
    #region Singleton

    public static CrashDemoManager Instance { get; private set; }
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

    public void TestCrash()
    {
        Debug.Log(TAG + " TestCrash");

        HMSCrashManager.Instance.TestCrash();
    }

    public void EnableCrashCollection(bool value)
    {
        Debug.Log(TAG + " EnableCrashCollection");

        HMSCrashManager.Instance.EnableCrashCollection(value);
    }

    public void CustomReport()
    {
        Debug.Log(TAG + " CustomReport");

        HMSCrashManager.Instance.CustomReport();
    }
}
