using System;
using Framework;

namespace Framework
{
	[Serializable]
	public class LuckyLadderSaveData : MiniGameCoolDown
	{
		public int activeSetIndex = 0;
		public int currentStep = 0;            // next unclaimed step (0..6)
		public bool[] collected = new bool[6]; // which steps have been claimed
		public bool isFinalRewardCollected;
	}
}
