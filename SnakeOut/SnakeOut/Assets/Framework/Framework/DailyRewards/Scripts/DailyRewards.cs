/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Framework;

namespace NiobiumStudios
{
	/**
    * Daily Rewards keeps track of the player daily rewards based on the time he last selected a reward
    **/
	public class DailyRewards : DailyRewardsCore<DailyRewards>
	{
		// Needed Constants
		private const string LAST_REWARD_TIME = "LastRewardTime";
		private const string LAST_REWARD = "LastReward";
		private const string DEBUG_TIME = "DebugTime";
		private const string FMT = "O";

		public List<Reward> rewards;        // Rewards list 
		public List<RewardStepData> CurrentRewardSet = new List<RewardStepData>();
		public List<DailyRewardSet> RewardSets = new List<DailyRewardSet>();
		public DateTime lastRewardTime;     // The last time the user clicked in a reward
		public int availableReward;         // The available reward position the player claim
		public int lastReward;              // the last reward the player claimed
		public bool keepOpen = true;        // Keep open even when there are no Rewards available?
		public bool resetPrize = true;      // Reset the prize on next day?

		// Delegates
		public delegate void OnClaimPrize(int day);                 // When the player claims the prize
		public OnClaimPrize onClaimPrize;
		public TimeSpan debugTime;         // For debug purposes only

		void Start()
		{
			// Initializes the timer with the current time
			StartCoroutine(InitializeTimer());
		}

		private IEnumerator InitializeTimer()
		{
			yield return StartCoroutine(base.InitializeDate());

			if (base.isErrorConnect)
			{
				if (onInitialize != null)
					onInitialize(true, base.errorMessage);
			}
			else
			{
				LoadDebugTime();
				// We don't count seconds on Daily Rewards
				//now = now.AddSeconds(-now.Second);
				CheckRewards();

				if (onInitialize != null)
					onInitialize();
			}
		}

		protected override void OnApplicationPause(bool pauseStatus)
		{
			base.OnApplicationPause(pauseStatus);
			CheckRewards();
		}

		public TimeSpan GetTimeDifference()
		{
			TimeSpan difference = (lastRewardTime - now);
			difference = difference.Subtract(debugTime);
			return difference.Add(new TimeSpan(0, 24, 0, 0));
		}

		private void LoadDebugTime()
		{
			int debugHours = PlayerPrefs.GetInt(GetDebugTimeKey(), 0);
			debugTime = new TimeSpan(debugHours, 0, 0);
		}

		// Check if the player have unclaimed prizes
		public void CheckRewards()
		{
			string lastClaimedTimeStr = PlayerPrefs.GetString(GetLastRewardTimeKey());
			lastReward = PlayerPrefs.GetInt(GetLastRewardKey());

			// First time ever
			if (string.IsNullOrEmpty(lastClaimedTimeStr))
			{
				availableReward = 1;
				return;
			}

			lastRewardTime = DateTime.ParseExact(lastClaimedTimeStr, FMT, CultureInfo.InvariantCulture);

			// Apply debug time
			DateTime advancedTime = now.AddHours(debugTime.TotalHours);
			TimeSpan diff = advancedTime - lastRewardTime;

			// How many hours have passed?
			double hours = diff.TotalHours;

			// Not enough time passed → no reward available
			if (hours < 24)
			{
				availableReward = 0;
				return;
			}

			// At least 24 hours passed → user can claim next reward
			// NO reset if days > 2
			if (lastReward >= rewards.Count)
			{
				// Reset after completing all 7 days
				availableReward = 1;
				lastReward = 0;
			}
			else
			{
				availableReward = lastReward + 1;
			}
		}

		// Checks if the player claim the prize and claims it by calling the delegate. Avoids duplicate call
		public void ClaimPrize()
		{
			if (availableReward > 0)
			{
				// Delegate
				if (onClaimPrize != null)
					onClaimPrize(availableReward);

				Debug.Log("ID " + instanceId + " Reward [" + rewards[availableReward - 1] + "] Claimed!");
				PlayerPrefs.SetInt(GetLastRewardKey(), availableReward);

				// Remove seconds
				//var timerNoSeconds = now.AddSeconds(-now.Second);
				// If debug time was added then we store it
				//timerNoSeconds = timerNoSeconds.AddHours(debugTime.TotalHours);

				string lastClaimedStr = now.AddHours(debugTime.TotalHours).ToString(FMT);
				PlayerPrefs.SetString(GetLastRewardTimeKey(), lastClaimedStr);
				PlayerPrefs.SetInt(GetDebugTimeKey(), (int)debugTime.TotalHours);
			}
			else if (availableReward == 0)
			{
				Debug.LogError("Error! The player is trying to claim the same reward twice.");
			}

			CheckRewards();
		}

		//Returns the lastReward playerPrefs key depending on instanceId
		private string GetLastRewardKey()
		{
			if (instanceId == 0)
				return LAST_REWARD;

			return string.Format("{0}_{1}", LAST_REWARD, instanceId);
		}

		//Returns the lastRewardTime playerPrefs key depending on instanceId
		private string GetLastRewardTimeKey()
		{
			if (instanceId == 0)
				return LAST_REWARD_TIME;

			return string.Format("{0}_{1}", LAST_REWARD_TIME, instanceId);
		}

		//Returns the advanced debug time playerPrefs key depending on instanceId
		private string GetDebugTimeKey()
		{
			if (instanceId == 0)
				return DEBUG_TIME;

			return string.Format("{0}_{1}", DEBUG_TIME, instanceId);
		}

		// Returns the daily Reward of the day
		public Reward GetReward(int day)
		{
			return rewards[day - 1];
		}

		public List<RewardItem> GetRewardList(int day)
		{
			if (CurrentRewardSet == null)
			{
				CurrentRewardSet = RewardSets[0].steps;
			}
			return CurrentRewardSet[day].items;
		}

		public void SetCurrentRewardSet()
		{
			CurrentRewardSet = RewardSets[0].steps;
		}

		public RewardItem GetFinalReward()
		{
			return RewardSets[0].FinalRewardSet;
		}

		public int GetTotalDays()
		{
			return RewardSets[0].steps.Count;
		}

		public int GetRemainingDays()
		{
			return (PlayerPrefs.GetInt(GetLastRewardKey()));
		}

		// Resets the Daily Reward for testing purposes
		public void Reset()
		{
			PlayerPrefs.DeleteKey(GetLastRewardKey());
			PlayerPrefs.DeleteKey(GetLastRewardTimeKey());
			PlayerPrefs.DeleteKey(GetDebugTimeKey());
		}
	}
}