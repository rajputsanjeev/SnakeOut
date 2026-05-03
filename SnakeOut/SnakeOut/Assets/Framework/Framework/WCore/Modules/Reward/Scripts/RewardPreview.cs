using UnityEngine;

namespace Framework.Core
{
	public class RewardPreview : IRewardPreview
	{
		private Sprite icon;
		public Sprite Icon => icon;

		private string text;
		public string Text => text;

		private int sortingOrder;
		public int SortingOrder => sortingOrder;

		private GameObject customPrefab;
		public GameObject CustomPrefab => customPrefab;

		public RewardType Type => type;
		public RewardType type = RewardType.Coins;

		public RewardPreview(Sprite icon, string text, int sortingOrder = 0, GameObject customPrefab = null, RewardType rewardType = RewardType.Coins)
		{
			this.icon = icon;
			this.text = text;
			this.sortingOrder = sortingOrder;
			this.customPrefab = customPrefab;
			this.type = rewardType;
		}

		public GameObject GetCustomUIPrefab() => customPrefab;
	}
}
