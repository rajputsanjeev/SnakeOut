using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Framework
{
	public abstract class BaseTimerHandle : MonoBehaviour
	{
		public List<TextMeshProUGUI> timerTexts;
		public abstract void StartTimer(bool isStartTimer = false);
		public string FormatSpan(TimeSpan span)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
				span.Days,
				span.Hours,
				span.Minutes,
				span.Seconds);
		}

		public void UpdateTime(string str)
		{
			if (timerTexts.Count == 0)
				return;

			foreach (var txt in timerTexts)
			{
				txt.text = str;
			}
		}
	}
}
