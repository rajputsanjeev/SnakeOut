using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "RewardSet", menuName = "Data/DailyReward/RewardSet")]
	public class DailyRewardSet : RewardListBase
	{
		public RewardItem FinalRewardSet;
	}
}