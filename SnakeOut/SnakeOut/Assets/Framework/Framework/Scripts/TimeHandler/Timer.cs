using System;
using UnityEngine;

namespace Framework
{
	public enum TimerMode
	{
		Countdown,
		CountUp
	}

	public sealed class Timer
	{
		public string Id { get; }
		public string Group { get; }
		public bool Paused { get; private set; }

		private readonly Action onComplete;
		private readonly Action<TimeSpan> onTick;

		private readonly TimerMode mode;
		private readonly TimeSpan target;

		private DateTime startUtc;
		private DateTime endUtc;

		public Timer(
			string id,
			string group,
			TimerMode mode,
			TimeSpan duration,
			Action onComplete,
			Action<TimeSpan> onTick)
		{
			Id = id;
			Group = group;
			this.mode = mode;
			target = duration;
			this.onComplete = onComplete;
			this.onTick = onTick;

			startUtc = UtcTimeService.Instance.GetCurrentDate();
			endUtc = startUtc.Add(duration);
		}

		public bool Update(DateTime now)
		{
			if (Paused)
				return false;

			TimeSpan value =
				mode == TimerMode.Countdown
					? endUtc - now
					: now - startUtc;

			// Running
			if (mode == TimerMode.Countdown && value > TimeSpan.Zero)
			{
				onTick?.Invoke(value);
				return false;
			}

			if (mode == TimerMode.CountUp && value < target)
			{
				onTick?.Invoke(value);
				return false;
			}

			// Complete
			onTick?.Invoke(
				mode == TimerMode.Countdown ? TimeSpan.Zero : target);

			onComplete?.Invoke();
			return true; // ✅ REMOVE ME
		}

		public void Pause() => Paused = true;
		public void Resume() => Paused = false;
	}
}
