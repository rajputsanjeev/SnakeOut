using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Framework
{
	public class TimerHandle : MonoBehaviour
	{
		private DateTime endUtc;
		private string persistentKey;
		private bool usePersistence;

		private List<TextMeshProUGUI> texts;
		private Action onStart;
		private Action onComplete;

		public void Initialize(DateTime endTimeUtc,string key,bool persistent,List<TextMeshProUGUI> targetTexts,	Action onStartCallback,	Action onCompleteCallback)
		{
			endUtc = endTimeUtc;
			persistentKey = key;
			usePersistence = persistent;

			texts = targetTexts;
			onStart = onStartCallback;
			onComplete = onCompleteCallback;

			onStart?.Invoke();
			InvokeRepeating(nameof(Tick), 0f, 1f);
		}

		private void Tick()
		{
			TimeSpan remaining = endUtc - DateTime.UtcNow;

			if (remaining <= TimeSpan.Zero)
			{
				UpdateText("Ready");
				Finish();
				return;
			}

			UpdateText(Format(remaining));
		}

		private void Finish()
		{
			CancelInvoke(nameof(Tick));

			if (usePersistence && !string.IsNullOrEmpty(persistentKey))
			{
				PlayerPrefs.DeleteKey(persistentKey);
			}

			onComplete?.Invoke();
			Destroy(gameObject);
		}

		private void UpdateText(string value)
		{
			if (texts == null) return;

			foreach (var txt in texts)
			{
				if (txt != null)
					txt.text = value;
			}
		}

		private string Format(TimeSpan t)
		{
			if (t.TotalDays >= 1)
				return $"{t.Days:D2}:{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

			return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
		}
	}
}
