using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "RewardSet", menuName = "Data/LuckyLadder/RewardSet")]
	public class LuckyLadderRewardSet : RewardListBase
	{
		public string SetName;
		public int FreeNumber;
		public RewardStepData FinalRewardSet;

		public RewardStepData GeteRewardStep(int idx)
		{
			if (steps == null || idx < 0 || idx >= steps.Count) return null;
			return steps[idx];
		}
	}
}