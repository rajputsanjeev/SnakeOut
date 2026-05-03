using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public abstract class RewardBaseHandle : GenericSingletonClass<RewardBaseHandle>
	{
		public static List<FlowReward> FlowRewards = new List<FlowReward>();

		public static Action<RewardStepData> UpdateReward;
		public abstract void SetReward(List<RewardItem> reardItem, bool isDouble = false);

		public abstract void SetReward(RewardStepData rewardStepData, bool isDouble = false);

		public abstract void SetReward(RewardItem reardItem, bool isDouble = false);

	}
}
