using System;
using UnityEngine;

namespace Framework
{
	public class PanelOfferSave
	{
		public string lastResetUtc;
		public int todayOpenCount;
		public string lastOpenTimeUtc;
	}

	public static class UIPanelOfferSystem
	{
		private const string KEY = "PANEL_OFFER_SAVE";
		public static PanelOfferSave Save;

		public static DateTime Now => DateTime.UtcNow;

		public static void Load()
		{
			if (PlayerPrefs.HasKey(KEY))
				Save = JsonUtility.FromJson<PanelOfferSave>(PlayerPrefs.GetString(KEY));
			else
				Save = new PanelOfferSave();

			CheckDailyReset();
		}

		static void CheckDailyReset()
		{
			if (string.IsNullOrEmpty(Save.lastResetUtc))
			{
				ResetDay();
				return;
			}

			var last = DateTime.Parse(Save.lastResetUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);
			if (last.Date < Now.Date)
				ResetDay();
		}

		static void ResetDay()
		{
			Save.lastResetUtc = Now.ToString("o");
			Save.todayOpenCount = 0;
			SaveData();
		}

		public static bool CanOpenToday(int max)
		{
			return Save.todayOpenCount < max;
		}

		public static bool CanOpenByTime(int minMinutes)
		{
			if (string.IsNullOrEmpty(Save.lastOpenTimeUtc))
				return true;

			var last = DateTime.Parse(Save.lastOpenTimeUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);
			return (Now - last).TotalMinutes >= minMinutes;
		}

		public static void MarkOpened()
		{
			Save.todayOpenCount++;
			Save.lastOpenTimeUtc = Now.ToString("o");
			SaveData();
		}

		static void SaveData()
		{
			PlayerPrefs.SetString(KEY, JsonUtility.ToJson(Save));
			PlayerPrefs.Save();
		}
	}


}

