using HmsPlugin;
using HuaweiMobileServices.APM;

using UnityEngine;

public class HMSAPMManager : HMSManagerSingleton<HMSAPMManager>
{
    private readonly string TAG = "[HMS] HMSAPMManager";

    public HMSAPMManager()
    {
        HMSManagerStart.Start(OnAwake, TAG);
    }

    private void OnAwake()
    {
        Debug.Log($"[{TAG}]: OnAwake() ");
    }

    public void EnableCollection(bool state)
    {
        APMS.GetInstance().EnableCollection(state);
    }

    public void EnableAnrMonitor(bool state)
    {
        APMS.GetInstance().EnableAnrMonitor(state);
    }



}
