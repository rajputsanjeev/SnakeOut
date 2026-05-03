using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "AnalyticsEventList", menuName = "Analytics/Analytics Event List")]
	public class AnalyticsEventList : ScriptableObject
	{
		public List<string> eventNames = new List<string>();
	}
}
