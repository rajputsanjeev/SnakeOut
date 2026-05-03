using System.Collections.Generic;
using Framework;
using Framework.Core;
using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "Sale Bundle", menuName = "Shop/Create Bundle")]
	public class ShopBundlesScriptableObject : ScriptableObject
	{
		public List<BundleData> BundlesData = new List<BundleData>();
	}

	[System.Serializable]
	public class BundleData
	{
		public string BundleName;
		public Sprite BundleSprite;
		public ProductKeyType ProductKeyType;
		public GameObject Prefab;
		public RewardItem CoinReward;
		public List<RewardItem> PowerupReward = new List<RewardItem>();
	}
}