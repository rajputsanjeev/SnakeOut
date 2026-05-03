
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Framework
{
	public class SaleTimerUI : MonoBehaviour
	{
		public List<TextMeshProUGUI> _timerText = new List<TextMeshProUGUI>();
		private Coroutine countdownRoutine;

		public void StartTimer(SeasonPack pack)
		{
			// start countdown
			if (!pack.alwaysShow)
			{
				StopCountdown();
				countdownRoutine = StartCoroutine(CountdownCoroutine(pack));
			}
			else
			{
				foreach (var txt in _timerText)
				{
					txt.text = $"{pack.displayName}";
				}
			}
		}

		private IEnumerator CountdownCoroutine(SeasonPack pack)
		{
			while (true)
			{
				var now = DateTime.UtcNow;
				var (start, end) = pack.GetEffectiveStartEnd(now);
				if (now < start)
				{
					// sale not started yet
					var diff = start - now;

					foreach (var txt in _timerText)
					{

						txt.text = $"Starts in: {FormatTimeSpan(diff)}";
					}
				}
				else if (now > end)
				{
					foreach (var txt in _timerText)
					{

						txt.text = "Sale ended";
					}
				}
				else
				{
					var remaining = end - now;
					foreach (var txt in _timerText)
					{

						txt.text = $"Ends in: {FormatTimeSpan(remaining)}";
					}
				}
				yield return new WaitForSeconds(1f);
			}
		}

		private string FormatTimeSpan(TimeSpan ts)
		{
			int days = ts.Days;
			int hours = ts.Hours;
			int minutes = ts.Minutes;
			int seconds = ts.Seconds;
			if (days > 0) return $"{days}d {hours:00}h {minutes:00}m {seconds:00}s";
			return $"{hours:00}h {minutes:00}m {seconds:00}s";
		}

		public void StopCountdown()
		{
			if (countdownRoutine != null)
			{
				StopCoroutine(countdownRoutine);
				countdownRoutine = null;
			}
		}
	}
}