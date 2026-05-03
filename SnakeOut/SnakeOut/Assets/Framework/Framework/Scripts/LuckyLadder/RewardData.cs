using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[Serializable]
	public enum RewardType { Coins, Power1, Power2, Power3, Power4, Power5, Power6, Power7, Power8, Piggy, Question }

	[Serializable]
	public class RewardItem
	{
		public RewardType type;
		public int quantity;
		public Sprite icon;
	}

	[Serializable]
	public class RewardStepData
	{
		public List<RewardItem> items = new List<RewardItem>();
	}
}