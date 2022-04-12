using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashDemoManager : MonoBehaviour
{
    public void TestCrash() 
    {
        HMSCrashManager.Instance.TestCrash();
    }

    public void EnableCrashCollection(bool value)
    {
        HMSCrashManager.Instance.EnableCrashCollection(value);
    }

    public void customReport()
    {
        HMSCrashManager.Instance.customReport();
    }
}
