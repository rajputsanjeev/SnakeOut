using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "DailyBundle", menuName = "Shop/Daily Bundle/Create Bundle")]
	public class DailyBundleScriptableObject : ScriptableObject
	{
		public Sprite CoinIcon;
		public DailyBundleUIController UIPrefab;
		public List<DailyBundleData> DailyBundleData;
	}

	[System.Serializable]
	public class DailyBundleData
	{
		public string ID;
		public RewardItem Reward;
		public int Price;
	}
}