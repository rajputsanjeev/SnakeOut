using System;
using System.Collections;
using Framework.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class WatchAdsPower : MonoBehaviour
	{
		public delegate void ClaimReward();
		public ClaimReward ClaimRewardEvent;

		[Header("UTC Setting")]
		public bool UseUtc = true;
		public bool UseOnline;

		[Header("Settings")]
		public string powerUpName = "ExtraTime";
		public int maxUses = 2;
		public Button RewardBtn;

		[Header("UI")]
		public bool showCooldownText = false;
		public TextMeshProUGUI cooldownText;
		public bool showUsedCount = false;
		public TextMeshProUGUI usedCount;

		[Header("Time Expire Condition")]
		public bool IsShowGameobjectWhenMaxReach = false;

		private PowerUpData data;

		protected DateTime Now =>
		UseUtc
			? (UseOnline ? UtcTimeService.Instance.GetCurrentDate() : DateTime.UtcNow)
			: DateTime.Now;

		private DateTime CooldownEndUtc =>
			string.IsNullOrEmpty(data.cooldownEndUtc)
				? DateTime.MinValue
				: DateTime.Parse(data.cooldownEndUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);

		private void Start()
		{
			if (RewardBtn == null) RewardBtn = GetComponent<Button>();
			WatchAdsManager.Load();

			data = WatchAdsManager.GetPowerUp(powerUpName);
			if (data == null)
			{
				data = new PowerUpData
				{
					powerUpName = powerUpName,
					maxUses = maxUses,
					currentUses = 0,
					lastResetUtc = Now.ToString("o")
				};

				WatchAdsManager.RegisterPowerUp(data);
			}

			ClaimRewardEvent += OnAdsComplete;

			// Resume cooldown if app was closed
			CheckDailyReset();
			StartTimer();
			UpdateUI();
			UpdateButtonState();
		}

		public void CheckDailyReset()
		{
			var now = Now;
			var lastReset = string.IsNullOrEmpty(data.lastResetUtc)
				? DateTime.MinValue
				: DateTime.Parse(data.lastResetUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);

			if (lastReset.Date < now.Date)
			{
				ResetData();
			}
		}

		private void StartTimer()
		{
			if (showCooldownText && cooldownText != null)
			{
				cooldownText.gameObject.SetActive(showCooldownText);
			}

			if (data.currentUses >= data.maxUses)
			{
				TimerManager.CreateTimer(GetTimeUntilNextReset(), OnTimerCompleted,
				x =>
				{
					if (showCooldownText && cooldownText != null)
					{
						cooldownText.text = TimeUtils.FormatSpan(x);
					}
				},
				"powerUpName");
			}
		}

		private void OnTimerCompleted()
		{
			ResetData();
		}

		public void ResetData()
		{
			data.cooldownEndUtc = "";
			data.currentUses = 0;
			data.lastResetUtc = Now.ToString("o");

			WatchAdsManager.Save();

			UpdateUI();
			UpdateButtonState();
		}

		private void OnAdsComplete()
		{
			if (data.currentUses >= data.maxUses)
				return;

			data.currentUses++;

			if (data.currentUses >= data.maxUses)
			{
				data.lastResetUtc = Now.ToString("o");
				data.cooldownEndUtc = Now.AddHours(24).ToString("o");
				StartTimer();
			}
			WatchAdsManager.SetPowerUp(data);

			UpdateUI();
			UpdateButtonState();
		}

		private void UpdateUI()
		{
			if (showUsedCount && usedCount)
			{
				usedCount.text = (data.maxUses - data.currentUses).ToString();
			}

			if (showCooldownText && cooldownText)
				cooldownText.text = "";
		}

		private void UpdateButtonState()
		{
			bool reachedMaxUses = data.currentUses >= data.maxUses;
			RewardBtn.interactable = !reachedMaxUses;
			RewardBtn.gameObject.SetActive(!reachedMaxUses);
			if (reachedMaxUses)
			{
				RewardBtn.gameObject.SetActive(IsShowGameobjectWhenMaxReach);
			}
			else
			{
				RewardBtn.gameObject.SetActive(true);
			}
		}

		public TimeSpan GetTimeUntilNextReset()
		{
			var now = Now;
			return CooldownEndUtc - now;
		}
	}
}
