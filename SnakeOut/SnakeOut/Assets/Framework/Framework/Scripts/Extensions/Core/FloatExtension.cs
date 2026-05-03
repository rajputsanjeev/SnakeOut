using System;
using System.Collections.Generic;
using System.Linq;

public static class FloatExtension
{
	public static float[] Unpack(this float current, int amount)
	{
		current *= 10000000;
		var bitPrecision = 24 / amount;
		var intPrecision = 1 << bitPrecision;
		var floatPrecision = (float)intPrecision;
		var unpacked = new List<float>();
		for (var index = amount; index > 0; --index)
		{
			var slot = (float)(current / Math.Pow(intPrecision, index - 1) % floatPrecision) / floatPrecision;
			unpacked.Add(slot);
		}

		return unpacked.ToArray();
	}

	public static float Square(this float current)
	{
		return current * current;
	}

	public static float InverseSquare(this float x)
	{
		var xhalf = 0.5f * x;
		var i = BitConverter.ToInt32(BitConverter.GetBytes(x), 0);
		i = 0x5f3759df - (i >> 1);
		x = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
		x = x * (1.5f - xhalf * x * x);
		return x;
	}

	public static float MoveTowards(this float current, float end, float speed)
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

	public static float Distance(this float current, float end)
	{
		return Math.Abs(current - end);
	}

	public static bool HasValue(this float current)
	{
		return !float.IsNaN(current) && !float.IsInfinity(current);
	}

	public static bool Between(this float current, float start, float end)
	{
		return current >= start && current <= end;
	}

	public static bool Is(this float current, params float[] include)
	{
		foreach (var item in include)
		{
			if (current == item)
			{
				return true;
			}
		}

		return false;
	}

	public static bool Not(this float current, params float[] disclude)
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

	public static bool InRange(this float current, float start, float end)
	{
		return current.Between(start, end);
	}

	public static float Closest(this float current, params float[] values)
	{
		var match = float.MaxValue;
		foreach (var value in values)
		{
			if (current.Distance(value) < match)
			{
				match = value;
			}
		}

		return match;
	}

	public static float RoundClosestDown(this float current, params float[] values)
	{
		float highest = -1;
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

	public static float RoundClosestUp(this float current, params float[] values)
	{
		float lowest = -1;
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

	public static float Mean(this IEnumerable<float> current)
	{
		return (float)current.Average();
	}

	public static float Median(this IEnumerable<float> current)
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

	public static float Mode(this IEnumerable<float> current)
	{
		return current.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault();
	}

	public static float Saturate(this float current)
	{
		return current.Clamp(0, 1);
	}

	public static float Clamp(this float current, float min, float max)
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

	public static float Step(this float current, float stepSize)
	{
		return (current / stepSize).Round() * stepSize;
	}

	public static float ClampStep(this float current, float stepSize, float min, float max)
	{
		return current.Step(stepSize).Clamp(min, max);
	}

	public static float Lerp(this float current, float start, float end)
	{
		return ((end - start) * current) + start;
	}

	public static float Remap(this float current, float start, float end, float goalStart = 0, float goalEnd = 1)
	{
		if (end - start == 0)
		{
			return goalEnd;
		}

		return ((goalEnd - goalStart) / (end - start) * (current - end) + goalEnd).Clamp(goalStart, goalEnd);
	}

	public static float Min(this float current, float value)
	{
		return Math.Min(current, value);
	}

	public static float Max(this float current, float value)
	{
		return Math.Max(current, value);
	}

	public static float Abs(this float current)
	{
		return Math.Abs(current);
	}

	public static float Ceil(this float current)
	{
		return (float)Math.Ceiling(current);
	}

	public static float Floor(this float current)
	{
		return (float)Math.Floor(current);
	}

	public static float Round(this float current)
	{
		return (float)Math.Round(current);
	}

	public static float[] Subtract(this IList<float> current, IList<float> other)
	{
		var result = new float[current.Count];
		for (var index = 0; index < current.Count; ++index)
		{
			result[index] = current[index];
			if (index >= other.Count)
			{
				continue;
			}

			result[index] -= other[index];
		}

		return result;
	}

	public static float[] Add(this IList<float> current, IList<float> other)
	{
		var result = new float[current.Count];
		for (var index = 0; index < current.Count; ++index)
		{
			result[index] = current[index];
			if (index >= other.Count)
			{
				continue;
			}

			result[index] += other[index];
		}

		return result;
	}

	public static float[] Multiply(this IList<float> current, IList<float> other)
	{
		var result = new float[current.Count];
		for (var index = 0; index < current.Count; ++index)
		{
			result[index] = current[index];
			if (index >= other.Count)
			{
				continue;
			}

			result[index] *= other[index];
		}

		return result;
	}

	public static float[] Divide(this IList<float> current, IList<float> other)
	{
		var result = new float[current.Count];
		for (var index = 0; index < current.Count; ++index)
		{
			result[index] = current[index];
			if (index >= other.Count)
			{
				continue;
			}

			result[index] /= other[index];
		}

		return result;
	}
}