using System;
using UnityEngine;

namespace Framework
{
	[Serializable]
	public class PrivilegeSaveData : FeatureUnlocked
	{
		public int SelectedCycleIndex = 0;     // index of the chosen PrivilegeCycleSO asset
		public string CycleStartUtc = "";      // iso date when the 28-day cycle began
		public int CurrentStep = 0;
		public int TotalAvaliableKeys = 0;   
		public int TotalAvaliableKeysRemain = 0;
		public bool[] FreeCollectedFlag = new bool[28];// Free collected flags for 28 steps
		public bool[] PaidCollectedFlag = new bool[28];// Paid collected flags for 28 steps
		public bool PaidActive = false;        // whether paid privilege is active in this cycle
		public bool[] IsRewardUnlock = new bool[28];// Save is reward unlock for this cycle
		public int FreeReward;
		public int PaidReward;
	}

	public class FeatureUnlocked
	{
		public bool isUnlocked;
	}
}
