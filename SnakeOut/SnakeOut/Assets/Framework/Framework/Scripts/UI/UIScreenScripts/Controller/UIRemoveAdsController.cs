using System;
using System.Collections;
using Base.UI.Manager;
using BaseView;
using Framework.UI.Components;
using I2.Loc;
using UnityEngine;

namespace Framework.UI.Controllers
{
	public class UIRemoveAdsController : Behaviour<UIRemoveAdsView>
	{
		private UIRemoveAdsView m_View;
		public string[] years;
		public string[] months;
		public string[] days;
		public string[] hours;
		public string[] minutes;

		protected override void Init()
		{
			base.Init();
			m_View = (UIRemoveAdsView)Prefab;
			InvokeRepeating("CheckAdRemovalStatus", 1f, 60f);
		}

		private void OnAdCompleted()
		{
			SetAdRemovalTime(15); // 15 minutes
		}

		public override void ShowPanel(bool on)
		{
			CheckAdRemovalStatus();
		}

		private IEnumerator UpdateTimer()
		{
			var expiryTime = DateTime.Parse(PlayerPrefs.GetString("AdsExpiryTime"));

			// UI handling while ads are removed
			UtilityManagerScript.isRemoveAds = true;

			ButtonsActivite(false);

			while (DateTime.Now < expiryTime)
			{
				var remainingTime = expiryTime - DateTime.Now;
				m_View.AdsTimerText.text = string.Format("{0:D2}:{1:D2}",
					Mathf.Max(0, (int)remainingTime.TotalMinutes),
					Mathf.Max(0, remainingTime.Seconds));
				yield return new WaitForSeconds(1f); // update every second
			}

			// Timer ended → reset ads state
			m_View.AdsTimerText.text = "00:00";
			UtilityManagerScript.isRemoveAds = false;
		}

		private string FormatTime(TimeSpan time)
		{
			var now = DateTime.Now;
			var expirationDate = now.Add(time);

			// Calculate years, months, days, hours, and minutes
			var yearss = expirationDate.Year - now.Year;
			var monthss = expirationDate.Month - now.Month;
			var dayss = expirationDate.Day - now.Day;

			// Adjust months and days if necessary
			if (dayss < 0)
			{
				monthss--;
				// Calculate days in the previous month
				dayss += DateTime.DaysInMonth(expirationDate.Year, expirationDate.Month - 1);
			}

			// Adjust for negative months
			if (monthss < 0)
			{
				yearss--;
				monthss += 12;
			}
			var currentLanguage = LocalizationManager.CurrentLanguage;
			var languages = LocalizationManager.GetAllLanguages();

			var value = languages.IndexOf(currentLanguage);
			//return string.Format("{0} years, {1} months, {2} days, {3:D2} hours, {4:D2} minutes",
			//    years, months, days, time.Hours, time.Minutes);
			return string.Format("{0} {1}, {2} {3}, {4} {5}, {6:D2} {7}, {8:D2} {9}", yearss, years[value], monthss, months[value], dayss, days[value], time.Hours, hours[value], time.Minutes, minutes[value]);
		}

		private void SetAdRemovalTime(int minutes)
		{
			// Set current time and expiry time in minutes
			var expiryTime = DateTime.Now.AddMinutes(minutes);

			// Save the expiry time in PlayerPrefs (or Firebase or any other backend)
			PlayerPrefs.SetString("AdsExpiryTime", expiryTime.ToString());
			PlayerPrefs.Save();

			// Update the ads removal status
			UtilityManagerScript.isRemoveAds = true;
			UIPanelManager.TurnOffAll();
			m_View.HomeToggle.isOn = true;
		}

		private void CheckAdRemovalStatus()
		{
			if (PlayerPrefs.HasKey("AdsExpiryTime"))
			{
				var expiryTime = DateTime.Parse(PlayerPrefs.GetString("AdsExpiryTime"));

				if (DateTime.Now < expiryTime)
				{
					TimeSpan remainingTime = expiryTime - DateTime.Now;
					m_View.AdsTimerText.text = FormatTime(remainingTime);
					StartCoroutine(UpdateTimer());
					ButtonsActivite(false);
				}
				else
				{
					ButtonsActivite(true);
					// Ads removal period has expired
					UtilityManagerScript.isRemoveAds = false;
					//if (AdsController.adsController.showBanner) AdsController.adsController.LoadBannerAd();
				}
			}
		}

		private void ButtonsActivite(bool isInteractable)
		{
			m_View.AdsTimerText.gameObject.SetActive(!isInteractable);
			m_View.LockImage.gameObject.SetActive(!isInteractable);

			foreach (var btn in m_View.RemoveAdsBtns)
			{
				btn.interactable = isInteractable;
			}
			foreach (var lokImage in m_View.LockImages)
			{
				lokImage.gameObject.SetActive(!isInteractable);
			}
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}