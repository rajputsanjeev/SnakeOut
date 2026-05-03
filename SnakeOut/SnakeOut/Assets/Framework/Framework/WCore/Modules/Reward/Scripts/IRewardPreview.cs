using UnityEngine;

namespace Framework.Core
{
	public interface IRewardPreview
	{
		public RewardType Type { get; }
		public Sprite Icon { get; }
		public string Text { get; }

		public int SortingOrder { get; }

		GameObject GetCustomUIPrefab();
	}
}
