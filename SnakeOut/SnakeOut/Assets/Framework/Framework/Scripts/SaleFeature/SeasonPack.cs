using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "SeasonPack", menuName = "Sales/Season Pack", order = 0)]
	public class SeasonPack : ScriptableObject
	{
		public enum Theme
		{
			Winter,
			Festival,
			Spring,
			Summer,
			Monsoon,
		}

		[Header("Identity")]
		public string packId = "pack_id";
		public string displayName = "Winter Pack";
		public PackManager.Mode displayMode = PackManager.Mode.MonthWise;
		public Theme theme = Theme.Winter;

		[Tooltip("Optional tag/category to filter by (eg: 'Diwali', 'BlackFriday')")]
		public string categoryTag;

		[Header("UI Sprites")]
		public Sprite backgroundSprite;
		public Sprite upperSprite;     // the banner above
		public Sprite bottomSprite;    // the bottom green button sprite, etc
		public Sprite badgeSprite;     // discount badge sprite

		[Header("Prices & Labels")]
		public int price = 100;        // final price (or original price — choose convention)
		public int offPrice = 200;     // original price if you want to show crossed price
		public string priceLabelFormat = "{0}"; // format string for price display if needed

		[Header("Rewards")]
		public DailyRewardSet CoinReward;
		public DailyRewardSet SmallItemReward;

		[Header("Timing configuration")]
		[Tooltip("Use month range (startMonth..endMonth). If useSpecificDates==true, specific dates take precedence.")]
		public bool useMonthRange = true;
		[Range(1, 12)] public int startMonth = 12;
		[Range(1, 12)] public int endMonth = 1;

		[Tooltip("Use explicit start/end dates (year, month, day)")]
		public bool useSpecificDates = false;
		public int startYear = 0; // 0 = use current year
		[Range(1, 12)] public int startMonthExact = 12;
		[Range(1, 31)] public int startDayExact = 1;
		public int endYear = 0; // 0 = use current year
		[Range(1, 12)] public int endMonthExact = 12;
		[Range(1, 31)] public int endDayExact = 31;

		[Tooltip("If set to > 0, sale runs for this many days from the effective start date (overrides the end date calculation).")]
		public int saleDaysOverride = 0;

		[Header("Visibility")]
		[Tooltip("If true, this pack will always be eligible regardless of date checks.")]
		public bool alwaysShow = false;

		[Tooltip("Use this to prioritize packs when multiple match (higher = earlier selection).")]
		public int priority = 0;

		[Header("Editor only")]
		public string notes;

		/// <summary>
		/// Returns the computed start and end DateTime for the given year (based on the configuration).
		/// This method tries to infer reasonable years when month ranges cross year boundary.
		/// </summary>
		public (DateTime start, DateTime end) GetEffectiveStartEnd(DateTime referenceNow)
		{
			if (alwaysShow)
			{
				// arbitrary wide range
				var s = new DateTime(referenceNow.Year - 5, 1, 1);
				var e = new DateTime(referenceNow.Year + 5, 12, 31);
				return (s, e);
			}

			if (useSpecificDates)
			{
				int sy = startYear == 0 ? referenceNow.Year : startYear;
				int ey = endYear == 0 ? referenceNow.Year : endYear;

				// Create safe DateTimes (clamp day to month length)
				var s = SafeDate(sy, startMonthExact, startDayExact);
				var e = SafeDate(ey, endMonthExact, endDayExact);

				if (saleDaysOverride > 0) e = s.AddDays(saleDaysOverride).AddSeconds(-1);
				return (s, e);
			}

			// month range-based
			// Determine start year and end year relative to referenceNow
			int sMonth = Mathf.Clamp(startMonth, 1, 12);
			int eMonth = Mathf.Clamp(endMonth, 1, 12);
			int sYear = referenceNow.Year;
			int eYear = referenceNow.Year;

			// if range wraps across year boundary (eg start=11, end=2), then if current month >= start then end is next year
			// We choose a start date such that the range that includes referenceNow is selected if possible
			if (sMonth <= eMonth)
			{
				// same year range (e.g., Apr to Jun)
				sYear = referenceNow.Year;
				eYear = referenceNow.Year;
			}
			else
			{
				// wrapped range (e.g., Nov to Feb)
				// Decide if referenceNow falls in the wrapped portion or before it.
				if (referenceNow.Month >= sMonth)
				{
					sYear = referenceNow.Year;
					eYear = referenceNow.Year + 1;
				}
				else if (referenceNow.Month <= eMonth)
				{
					sYear = referenceNow.Year - 1;
					eYear = referenceNow.Year;
				}
				else
				{
					// choose the next occurrence
					sYear = referenceNow.Year;
					eYear = referenceNow.Year + 1;
				}
			}

			var start = SafeDate(sYear, sMonth, 1);
			// end: last day of endMonth
			var end = SafeDate(eYear, eMonth, DateTime.DaysInMonth(eYear, eMonth)).AddDays(1).AddSeconds(-1);

			if (saleDaysOverride > 0)
			{
				end = start.AddDays(saleDaysOverride).AddSeconds(-1);
			}

			return (start, end);
		}

		private DateTime SafeDate(int year, int month, int day)
		{
			year = Mathf.Max(1, year);
			month = Mathf.Clamp(month, 1, 12);
			var maxDay = DateTime.DaysInMonth(year, month);
			day = Mathf.Clamp(day, 1, maxDay);
			return new DateTime(year, month, day, 0, 0, 0);
		}

		/// <summary>
		/// Simple helper to test if a specific DateTime is within this pack's active period (or alwaysShow)
		/// </summary>
		public bool IsActiveAt(DateTime now)
		{
			if (alwaysShow) return true;
			var (s, e) = GetEffectiveStartEnd(now);
			return now >= s && now <= e;
		}
	}

}
