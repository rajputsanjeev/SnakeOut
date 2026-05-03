using System;
using System.Collections.Generic;
using System.Linq;

public static class IntExtensions
{
	#region General

	public static int Modulus(this int current, int max)
	{
		return (current % max + max) % max;
	}

	#endregion

	#region Bitwise

	public static bool Contains(this int current, Enum mask)
	{
		return (current & System.Convert.ToInt32(mask)) != 0;
	}

	public static bool Contains(this int current, int mask)
	{
		return (current & mask) != 0;
	}

	public static int SetFlag(this int current, int bit, bool state)
	{
		if (state)
		{
			current |= 1 << bit;
			return current;
		}

		current &= ~(1 << bit);
		return current;
	}

	public static bool[] UnpackBools(this int current, int amount)
	{
		var unpacked = new List<bool>();
		for (var index = amount - 1; index >= 0; --index)
		{
			var mask = 1 << index;
			var isActive = (current & mask) == mask;
			unpacked.Add(isActive);
		}

		return unpacked.ToArray();
	}

	#endregion


	#region Numeric

	public static int Square(this int current)
	{
		return current * current;
	}

	public static float InverseSquare(this int current)
	{
		var x = (float)current;
		var xhalf = 0.5f * x;
		var i = BitConverter.ToInt32(BitConverter.GetBytes(x), 0);
		i = 0x5f3759df - (i >> 1);
		x = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
		x = x * (1.5f - xhalf * x * x);
		return x;
	}

	public static int Clamp(this int current, int min, int max)
	{
		if (current < min)
		{
			return min;
		}

		if (current > max)
		{
			return max;
		}

		return current;
	}

	public static int Step(this int current, float stepSize)
	{
		return (int)((current / stepSize).Round() * stepSize);
	}

	public static int ClampStep(this int current, int stepSize, int min, int max)
	{
		return current.Step(stepSize).Clamp(min, max);
	}

	public static int Lerp(this int current, int start, int end)
	{
		return ((end - start) * current) + start;
	}

	public static int Remap(this int current, int start, int end, int goalStart = 0, int goalEnd = 1)
	{
		if (end - start == 0)
		{
			return goalEnd;
		}

		return ((goalEnd - goalStart) / (end - start) * (current - end) + goalEnd).Clamp(goalStart, goalEnd);
	}

	public static int MoveTowards(this int current, int end, int speed)
	{
		if (current > end)
		{
			speed *= -1;
		}

		current += speed;
		current = end < current ? Math.Max(current, end) : Math.Min(current, end);
		if ((speed > 0 && current > end) || (speed < 0 && current < end))
		{
			current = end;
		}

		return current;
	}

	public static int Distance(this int current, int end)
	{
		return Math.Abs(current - end);
	}

	public static bool Between(this int current, int start, int end)
	{
		return current >= start && current <= end;
	}

	public static bool Is(this int current, params int[] disclude)
	{
		foreach (var item in disclude)
		{
			if (current == item)
			{
				return true;
			}
		}

		return false;
	}

	public static bool Not(this int current, params int[] disclude)
	{
		foreach (var item in disclude)
		{
			if (current == item)
			{
				return false;
			}
		}

		return true;
	}

	public static bool InRange(this int current, int start, int end)
	{
		return current.Between(start, end);
	}

	public static int Closest(this int current, params int[] values)
	{
		var match = int.MaxValue;
		foreach (var value in values)
		{
			if (current.Distance(value) < match)
			{
				match = value;
			}
		}

		return match;
	}

	public static int RoundClosestDown(this int current, params int[] values)
	{
		var highest = -1;
		foreach (var value in values)
		{
			if (current >= value)
			{
				highest = value;
				break;
			}
		}

		foreach (var value in values)
		{
			if (current >= value && value > highest)
			{
				highest = value;
			}
		}

		return highest;
	}

	public static int RoundClosestUp(this int current, params int[] values)
	{
		var lowest = -1;
		foreach (var value in values)
		{
			if (current >= value)
			{
				lowest = value;
				break;
			}
		}

		foreach (var value in values)
		{
			if (current <= value && value < lowest)
			{
				lowest = value;
			}
		}

		return lowest;
	}

	public static int Mean(this IEnumerable<int> current)
	{
		return (int)current.Average();
	}

	public static int Median(this IEnumerable<int> current)
	{
		var count = current.Count();
		var sorted = current.OrderBy(n => n);
		var midValue = sorted.ElementAt(count / 2);
		var median = midValue;
		if (count % 2 == 0)
		{
			median = (midValue + sorted.ElementAt((count / 2) - 1)) / 2;
		}

		return median;
	}

	public static int Mode(this IEnumerable<int> current)
	{
		return current.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault();
	}

	public static int Min(this int current, int value)
	{
		return Math.Min(current, value);
	}

	public static int Max(this int current, int value)
	{
		return Math.Max(current, value);
	}

	public static int Abs(this int current)
	{
		return Math.Abs(current);
	}

	public static bool MatchesAny(this int current, params int[] values)
	{
		foreach (var value in values)
		{
			if (current == value)
			{
				return true;
			}
		}

		return false;
	}

	#endregion
}