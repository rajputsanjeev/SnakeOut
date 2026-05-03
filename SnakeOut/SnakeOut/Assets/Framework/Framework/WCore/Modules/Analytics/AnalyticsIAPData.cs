namespace Framework.Core
{
    public class AnalyticsIAPData : IAnalyticsEventData
    {
        public IAPItem Item;
        public string Source;
        public string Receipt;
        public float LocalizedPrice;
        public string StoreSpecificId;
        public string IsoCurrencyCode;
    }
}
