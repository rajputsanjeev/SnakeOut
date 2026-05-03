using System;
using UnityEngine;

namespace Framework
{
	[Serializable]
	public class PrivilegeStepData
	{
		[Header("How many keys are required to unlock this slot")]
		public int keysRequired = 5;

		[Header("Free reward (given to all)")]
		public RewardItem[] freeRewards = new RewardItem[0];

		[Header("Paid reward (extra/better rewards when user purchased)")]
		public RewardItem[] paidRewards = new RewardItem[0];

		[TextArea] public string description;
	}
}
