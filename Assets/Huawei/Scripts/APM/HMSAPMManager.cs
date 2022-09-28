using HuaweiMobileServices.APM;
using HuaweiMobileServices.Utils;

using UnityEngine;

public class HMSAPMManager : HMSManagerSingleton<HMSAPMManager>
{
    private readonly string TAG = "[HMS] HMSAPMManager";

    public HMSAPMManager()
    {
        if (!HMSDispatcher.InstanceExists)
            HMSDispatcher.CreateDispatcher();
        HMSDispatcher.InvokeAsync(OnAwake);
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
