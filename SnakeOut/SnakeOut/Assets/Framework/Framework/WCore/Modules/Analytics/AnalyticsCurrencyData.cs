using System.Collections.Generic;

namespace Framework.Core
{
    public class AnalyticsCurrencyData : IAnalyticsEventData
    {
        public string Source;
        public Dictionary<CurrencyType, int> CurrenciesDelta;
    }
}
