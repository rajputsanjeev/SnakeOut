using System.Collections.Generic;
using Frameork;

namespace Framework
{
	public static class RewardFlowQueue
	{
		private static readonly List<FlowReward> rewards = new();

		public static void AddReward(FlowReward reward)
		{
			if (rewards.Exists(r => r.RwType == reward.RwType))
				return; // already added → ignore

			rewards.Add(reward);
		}

		public static IReadOnlyList<FlowReward> GetRewards()
		{
			return rewards;
		}

		public static bool HasRewards => rewards.Count > 0;

		public static void Clear()
		{
			rewards.Clear();
		}
	}
}
