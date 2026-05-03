using System;
using System.Collections.Generic;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core
{
	public class RewardVideo : ISaveObject
	{
		public int MaxFreeAdsReward = 9;
		public int FreeCoinRewardUsed;

		public void Flush()
		{
		}
	}

	public sealed class AdsRewardsHolder : RewardsHolder
	{
		[Group("Settings"), UniqueID]
		[SerializeField] string rewardID;

		[Group("Settings"), Space]
		[SerializeField] Button adsButton;

		[Group("Settings")]
		[SerializeField] bool disableAfterPurchase;

		[Group("Settings"), Space]
		[SerializeField] string analyticsEvent = "Default";
		private WatchAdsPower _watchAdsPower;

		private void Start()
		{
			InitializeComponents();
			adsButton.onClick.AddListener(OnPurchased);

			_watchAdsPower = GetComponent<WatchAdsPower>();
			if (_watchAdsPower == null)
			{
				_watchAdsPower = GetComponentInChildren<WatchAdsPower>();
			}
		}

		private void OnPurchased()
		{
#if MODULE_HAPTIC
			Haptic.Play(Haptic.HAPTIC_HARD);
#endif

			AudioController.PlaySound(AudioController.AudioClips.buttonSound);

			AdsManager.ShowRewardBasedVideo((reward) =>
			{
				if (reward)
				{
					rewardSet.ApplyReward();
					_watchAdsPower.ClaimRewardEvent?.Invoke();
				}
			}, analyticsEvent);
		}
	}
}
