using System;
using UnityEngine;

namespace Framework
{
	public static class TimerManager
	{
		private static TimerRunner runner;

		private static void EnsureRunner()
		{
			if (runner != null) return;

			GameObject go = new GameObject("[TimerManager]");
			UnityEngine.Object.DontDestroyOnLoad(go);
			runner = go.AddComponent<TimerRunner>();
		}

		/* -------------------------------------------------- */
		/* CREATE (AUTO RESTART BY ID)                         */
		/* -------------------------------------------------- */

		public static string CreateTimer(
			TimeSpan duration,
			Action onComplete = null,
			Action<TimeSpan> onTick = null,
			string id = null,
			string group = null,
			TimerMode mode = TimerMode.Countdown)
		{
			EnsureRunner();

			id ??= Guid.NewGuid().ToString();

			// ✅ SAME ID = RESTART
			runner.timers.Remove(id);

			var timer = new Timer(
				id,
				group,
				mode,
				duration,
				onComplete,
				onTick);

			runner.timers[id] = timer;
			return id;
		}

		/* -------------------------------------------------- */
		/* CONTROL                                            */
		/* -------------------------------------------------- */

		public static void Cancel(string id)
		{
			if (runner == null) return;
			runner.timers.Remove(id);
		}

		public static void Pause(string id)
		{
			if (runner.timers.TryGetValue(id, out var t))
				t.Pause();
		}

		public static void Resume(string id)
		{
			if (runner.timers.TryGetValue(id, out var t))
				t.Resume();
		}

		public static bool IsRunning(string id)
		{
			return runner != null && runner.timers.ContainsKey(id);
		}
	}
}
