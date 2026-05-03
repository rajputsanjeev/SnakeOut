using System.Collections.Generic;

namespace Framework.Core
{
    public class AnalyticsLevelData : IAnalyticsEventData
    {
        public string Status;
		public Dictionary<string, object> LevelDelta;
	}
}
