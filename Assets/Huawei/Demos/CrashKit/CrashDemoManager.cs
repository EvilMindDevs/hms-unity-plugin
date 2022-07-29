
using UnityEngine;

public class CrashDemoManager : MonoBehaviour
{

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
        Debug.Log("TestCrash");

        HMSCrashManager.Instance.TestCrash();
    }

    public void EnableCrashCollection(bool value)
    {
        Debug.Log("EnableCrashCollection");

        HMSCrashManager.Instance.EnableCrashCollection(value);
    }

    public void CustomReport()
    {
        Debug.Log("CustomReport");

        HMSCrashManager.Instance.CustomReport();
    }
}
