/*
 * Use UnityPurchasing.Initialize(this, ConfigurationBuilder.Instance(HuaweiPurchasingModule.Instance())); 
 * to bind HmsPlugin as store module for builtin purchasing API
 * */


#if UNITY_PURCHASING
using UnityEngine.Purchasing.Extension;

namespace HmsPlugin
{

    public class HuaweiPurchasingModule : AbstractPurchasingModule
    {
        public override void Configure()
        {
            RegisterStore("AppGallery", HuaweiStore.GetInstance());
        }

        static HuaweiPurchasingModule currentInstance;
        public static HuaweiPurchasingModule Instance()
        {
            if (currentInstance != null) return currentInstance;
            currentInstance      = new HuaweiPurchasingModule();
            return currentInstance;
        }
    }
}
#endif
