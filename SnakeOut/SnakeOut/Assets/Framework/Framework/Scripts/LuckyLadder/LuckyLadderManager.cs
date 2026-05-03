using System;
using UnityEngine;

namespace Framework
{
	public class LuckyLadderManager : DailyCycleManager<LuckyLadderSaveData>, IManager
	{
		private const int TOTAL_STEPS = 6;

		[Header("Config")]
		public LuckyLadderRewardSet[] rewardSets;

		#region Persistence
		public override void Load()
		{
			save ??= LuckyLadderPersistence.Load();

			// Ensure collected array is valid
			if (save.collected == null || save.collected.Length != TOTAL_STEPS)
			{
				save.collected = new bool[TOTAL_STEPS];
			}

			// Ensure active set index is valid
			int setCount = rewardSets?.Length ?? 0;
			save.activeSetIndex = (setCount > 0 && save.activeSetIndex >= 0 && save.activeSetIndex < setCount)
				? save.activeSetIndex
				: 0;

			base.Load();
		}

		public override void Save()
		{
			save ??= LuckyLadderPersistence.Load();
			LuckyLadderPersistence.Save(save);
		}
		#endregion

		#region Public Getters
		public int GetCurrentStep()
		{
			save ??= LuckyLadderPersistence.Load();
			return save.currentStep;
		}

		public bool[] GetCollectedArray()
		{
			save ??= LuckyLadderPersistence.Load();
			return (bool[])save.collected.Clone();
		}
		#endregion

		#region Daily Reset
		protected override void DoDailyReset()
		{
			base.DoDailyReset();
		}

		protected override void OnDailyDataReset()
		{
			if (rewardSets != null && rewardSets.Length > 0)
				save.activeSetIndex = UnityEngine.Random.Range(0, rewardSets.Length);

			ResetData();
		}

		private void ResetData()
		{
			save.currentStep = 0;
			save.collected = new bool[TOTAL_STEPS];
			save.lastResetUtc = UtcTimeService.Instance.GetCurrentDate().ToString("o");
			save.cooldownEndUtc = "";
			save.isFinalRewardCollected = false;
		}

		public void CheckCooldownExpiry()
		{
			if (!IsAllCurrentSetCollected() || string.IsNullOrEmpty(save.cooldownEndUtc))
				return;

			if (Now >= CooldownEndUtc)
			{
				ResetData();
				Save();
			}
		}
		#endregion

		#region Check Condition
		public bool IsAllCurrentSetCollected()
		{
			save ??= LuckyLadderPersistence.Load();

			for (int i = 0; i < TOTAL_STEPS; i++)
				if (!save.collected[i])
					return false;

			return true;
		}

		public bool IsFinalRewCollected()
		{
			save ??= LuckyLadderPersistence.Load();
			return save.isFinalRewardCollected;
		}

		public bool IsBothRewardCollected()
		{
			save ??= LuckyLadderPersistence.Load();
			return save.isFinalRewardCollected && IsAllCurrentSetCollected();
		}
		#endregion

		#region Collect
		public RewardStepData TryCollectCurrentStep()
		{
			if (IsAllCurrentSetCollected())
				return null;

			int step = save.currentStep;
			if (step < 0 || step >= TOTAL_STEPS)
				return null;

			var reward = rewardSets[save.activeSetIndex].steps[step];

			save.collected[step] = true;
			save.currentStep = NextUncollectedIndex();

			if (IsAllCurrentSetCollected())
				save.cooldownEndUtc = Now.AddMinutes(2).ToString("o");

			Save();
			Notify();
			return reward;
		}

		public RewardStepData GetFinalRewardSet()
		{
			save.isFinalRewardCollected = true;
			Save();
			return rewardSets[save.activeSetIndex].FinalRewardSet;
		}

		public int NextUncollectedIndex()
		{
			for (int i = 0; i < TOTAL_STEPS; i++)
				if (!save.collected[i])
					return i;

			return TOTAL_STEPS;
		}

		public int GetRemainReward()
		{
			int count = 0;
			for (int i = 0; i < TOTAL_STEPS; i++)
				if (!save.collected[i])
					count++;
			if (!save.isFinalRewardCollected)
			{
				count++;
			}
			return count;
		}
		#endregion

		#region Misc
		public void ForceReset()
		{
			DoDailyReset();
			Save();
			Notify();
		}

		public LuckyLadderRewardSet GetActiveSet()
		{
			if (rewardSets == null || rewardSets.Length == 0)
				return null;

			save ??= LuckyLadderPersistence.Load();

			if (save.activeSetIndex < 0 || save.activeSetIndex >= rewardSets.Length)
				save.activeSetIndex = 0;

			return rewardSets[save.activeSetIndex];
		}
		#endregion
	}
}
