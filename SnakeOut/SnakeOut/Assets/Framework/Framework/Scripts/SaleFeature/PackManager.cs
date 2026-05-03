using System;
using System.Collections.Generic;
using Base.UI.Components;
using Framework.Core;
using Framework.UI.Controllers;
using UnityEngine;

namespace Framework
{
	public class PackManager : MonoBehaviour
	{
		public enum Mode
		{
			MonthWise,      // let SOs decide via months
			CategoryTag,    // show packs filtered by tag (categoryTag)
			ForceSpecific,  // show one specific pack
		}

		public bool UseUtc = true;
		public bool UseOnline;
		public MainMenuButtons MainMenuButton;

		[Header("Packs")]
		public List<SeasonPack> allPacks = new List<SeasonPack>();

		[Header("Display")]
		public Mode displayMode = Mode.MonthWise;
		[Tooltip("When using CategoryTag filter this tag will be used.")]
		public string categoryTagFilter;
		[Tooltip("When forcing a specific pack, assign here.")]
		public SeasonPack forcedPack;

		[Header("Testing (Editor only)")]
		[Tooltip("Use to simulate a date in editor and preview which pack will show.")]
		public bool overrideSystemDate = false;
		public DateTimeStruct overrideDate = new DateTimeStruct { year = 0 };
		public SeasonPack ActivePack { get; private set; }
		public event Action<SeasonPack> OnActivePackChanged;
		public PackUIBinder packUIBinder;
		private IAPRewardsHolder _iapRewardsHolder;
		private UISaleController _panelComponentl;

		private DateTime Now =>
		UseUtc
			? (UseOnline ? UtcTimeService.Instance.GetCurrentDate() : DateTime.UtcNow)
			: DateTime.Now;

		private async void Start()
		{
			EvaluateActivePack();

			_panelComponentl = GetComponent<UISaleController>();
			_iapRewardsHolder = GetComponent<IAPRewardsHolder>();
			_iapRewardsHolder.RewardReceived.AddListener(OnPurchaseComplete);

			await SaleManager.Initialize(this);

			if (ActivePack == null)
			{
				return;
			}

			// Hide the pack if purchased
			if (SaleManager.IsPackPurchased(ActivePack.packId))
			{
				Debug.Log("User already purchased this month's pack → Hide it");
				ActivePack = null;
				MainMenuButton.EnableButton(false);
				return;
			}
			packUIBinder.BuildUI();
		}

		private void OnPurchaseComplete()
		{
			_panelComponentl.CloseCurrentPanel();
		}

		/// <summary>
		/// Call this to reevaluate which pack should be active (also called from editor).
		/// </summary>
		public void EvaluateActivePack()
		{
			SeasonPack chosen = null;
			var now = Now;

			if (displayMode == Mode.ForceSpecific && forcedPack != null)
			{
				chosen = forcedPack;
			}
			else if (displayMode == Mode.CategoryTag && !string.IsNullOrEmpty(categoryTagFilter))
			{
				// find matching packs by tag, that are active now, order by priority
				foreach (var p in allPacks)
				{
					if (p == null) continue;
					if (!string.Equals(p.categoryTag, categoryTagFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
					if (!p.IsActiveAt(now)) continue;
					if (chosen == null || p.priority > chosen.priority) chosen = p;
				}
			}
			else if (displayMode == Mode.MonthWise)
			{
				foreach (var p in allPacks)
				{
					if (p == null) continue;
					if (!p.IsActiveAt(now)) continue;
					if (chosen == null || p.priority > chosen.priority) chosen = p;
				}
			}

			if (chosen != ActivePack)
			{
				ActivePack = chosen;
				OnActivePackChanged?.Invoke(ActivePack);
			}

			if (ActivePack == null)
			{
				MainMenuButton.EnableButton(false);
			}
		}

		// Helper struct to allow serializing a date (editor).
		[Serializable]
		public struct DateTimeStruct
		{
			public int year; // 0 means use system year
			[Range(1, 12)] public int month;
			[Range(1, 31)] public int day;

			public DateTime ToDateTimeOrNow()
			{
				try
				{
					var now = DateTime.UtcNow;
					int y = year == 0 ? now.Year : year;
					int m = month == 0 ? now.Month : month;
					int d = day == 0 ? now.Day : Mathf.Clamp(day, 1, DateTime.DaysInMonth(y, m));
					return new DateTime(y, m, d, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
				}
				catch
				{
					return DateTime.UtcNow;
				}
			}
		}
	}

	[Serializable]
	public class SaleSaveData
	{
		public string lastPurchasedPackId = "";
		public int lastActiveMonth = -1;

		public void Reset()
		{
			lastPurchasedPackId = "";
			lastActiveMonth = -1;
		}
	}
}