namespace Framework.Core
{
    public class AnalyticsIAPFailData : IAnalyticsEventData
    {
        public IAPItem Item;
		public string Source;
		public PurchaseFailureReason FailureReason;
    }
}
