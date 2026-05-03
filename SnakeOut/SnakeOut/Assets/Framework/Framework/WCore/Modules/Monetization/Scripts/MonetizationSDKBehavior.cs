using UnityEngine;

namespace Framework.Core
{
    public sealed class MonetizationSDKBehavior : SDKBehavior
    {
        [SerializeField] MonetizationSettings settings;

        public override void OnUserConsentReceived()
        {
            Monetization.Init(settings);

            AdsManager.Init(settings);
            IAPManager.Init(settings);
        }
    }
}
