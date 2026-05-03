using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[DefaultExecutionOrder(-999999)]
	internal sealed class TimerRunner : MonoBehaviour
	{
		internal readonly Dictionary<string, Timer> timers = new();
		private readonly List<string> cachedKeys = new(256);

		private void Update()
		{
			if (timers.Count == 0)
				return;

			// 1️⃣ CACHE KEYS (SAFE SNAPSHOT)
			cachedKeys.Clear();
			foreach (var id in timers.Keys)
				cachedKeys.Add(id);

			DateTime now = DateTime.UtcNow;

			for (int i = 0; i < cachedKeys.Count; i++)
			{
				string id = cachedKeys[i];

				if (!timers.TryGetValue(id, out var timer))
					continue;

				if (timer.Update(now))
				{
					// 🔒 Prevent killing restarted timer
					if (timers.TryGetValue(id, out var current) &&
						ReferenceEquals(current, timer))
					{
						timers.Remove(id);
					}
				}
			}
		}
	}
}
