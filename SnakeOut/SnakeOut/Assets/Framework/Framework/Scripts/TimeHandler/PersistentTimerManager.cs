using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Framework
{
	public class PersistentTimerManager : MonoBehaviour
	{
		private static PersistentTimerManager instance;
		private const string PREFIX = "PERSISTENT_TIMER_";

		private readonly Dictionary<string, TimerHandle> activeTimers = new();

		public static PersistentTimerManager Instance
		{
			get
			{
				if (instance == null)
				{
					var go = new GameObject("PersistentTimerManager");
					instance = go.AddComponent<PersistentTimerManager>();
					DontDestroyOnLoad(go);
				}
				return instance;
			}
		}

		/* -------------------------------------------------- */
		/* PUBLIC API                                         */
		/* -------------------------------------------------- */
		public void StartPersistentTimer(string key,TimeSpan duration,	List<TextMeshProUGUI> texts = null,	Action onStart = null,	Action onComplete = null)
		{
			string fullKey = PREFIX + key;

			if (activeTimers.ContainsKey(fullKey))
				return;

			DateTime endUtc;

			if (PlayerPrefs.HasKey(fullKey))
			{
				endUtc = DateTime.Parse(
					PlayerPrefs.GetString(fullKey),
					null,
					System.Globalization.DateTimeStyles.RoundtripKind);
			}
			else
			{
				endUtc = DateTime.UtcNow.Add(duration);
				PlayerPrefs.SetString(fullKey, endUtc.ToString("O"));
				PlayerPrefs.Save();
			}

			if (endUtc <= DateTime.UtcNow)
			{
				onComplete?.Invoke();
				PlayerPrefs.DeleteKey(fullKey);
				return;
			}

			CreateTimer(fullKey, endUtc, true, texts, onStart, onComplete);
		}

		public void StartRuntimeTimer(TimeSpan duration,List<TextMeshProUGUI> texts,Action onStart = null,Action onComplete = null)
		{
			DateTime endUtc = DateTime.UtcNow.Add(duration);
			CreateTimer(null, endUtc, false, texts, onStart, onComplete);
		}

		/* -------------------------------------------------- */
		/* INTERNAL                                           */
		/* -------------------------------------------------- */
		private void CreateTimer(string key, DateTime endUtc, bool persistent, List<TextMeshProUGUI> texts = null, Action onStart = null, Action onComplete = null)
		{
			GameObject go = new GameObject($"Timer_{key ?? "Runtime"}");
			go.transform.SetParent(transform);

			TimerHandle handle = go.AddComponent<TimerHandle>();
			activeTimers[key] = handle;

			handle.Initialize(endUtc, key, persistent, texts, onStart,
				() =>
				{
					activeTimers.Remove(key);
					onComplete?.Invoke();
				});
		}
		public bool IsTimerRunning(string key)
		{
			string fullKey = PREFIX + key;

			if (!PlayerPrefs.HasKey(fullKey))
				return false;

			DateTime endUtc = DateTime.Parse(
				PlayerPrefs.GetString(fullKey),
				null,
				System.Globalization.DateTimeStyles.RoundtripKind);

			return endUtc > DateTime.UtcNow;
		}

		public bool IsTimerExpired(string key)
		{
			return !IsTimerRunning(key);
		}

		public TimeSpan GetRemainingTime(string key)
		{
			string fullKey = PREFIX + key;

			if (!PlayerPrefs.HasKey(fullKey))
				return TimeSpan.Zero;

			DateTime endUtc = DateTime.Parse(
				PlayerPrefs.GetString(fullKey),
				null,
				System.Globalization.DateTimeStyles.RoundtripKind);

			TimeSpan remaining = endUtc - DateTime.UtcNow;
			return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
		}

	}
}
