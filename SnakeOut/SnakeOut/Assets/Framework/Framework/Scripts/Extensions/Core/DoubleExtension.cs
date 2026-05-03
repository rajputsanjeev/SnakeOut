using System;
using System.Collections.Generic;
using System.Linq;

public static class DoubleExtension
{
	public static double MoveTowards(this double current, double end, double speed)
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

	public static double Distance(this double current, double end)
	{
		return Math.Abs(current - end);
	}

	public static bool HasValue(this double current)
	{
		return !double.IsNaN(current) && !double.IsInfinity(current);
	}

	public static bool Between(this double current, double start, double end)
	{
		return current >= start && current <= end;
	}

	public static bool Is(this double current, params double[] disclude)
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

	public static bool Not(this double current, params double[] disclude)
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

	public static bool InRange(this double current, double start, double end)
	{
		return current.Between(start, end);
	}

	public static double Closest(this double current, params double[] values)
	{
		var match = double.MaxValue;
		foreach (var value in values)
		{
			if (current.Distance(value) < match)
			{
				match = value;
			}
		}

		return match;
	}

	public static double RoundClosestDown(this double current, params double[] values)
	{
		double highest = -1;
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

	public static double RoundClosestUp(this double current, params double[] values)
	{
		double lowest = -1;
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

	public static double Mean(this IEnumerable<double> current)
	{
		return current.Average();
	}

	public static double Median(this ICollection<double> current)
	{
		var count = current.Count;
		var sorted = current.OrderBy(n => n);
		var midValue = sorted.ElementAt(count / 2);
		var median = midValue;
		if (count % 2 == 0)
		{
			median = (midValue + sorted.ElementAt((count / 2) - 1)) / 2;
		}

		return median;
	}

	public static double Mode(this IEnumerable<double> current)
	{
		return current.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
	}

	public static double Min(this double current, double value)
	{
		return Math.Min(current, value);
	}

	public static double Max(this double current, double value)
	{
		return Math.Max(current, value);
	}

	public static double Abs(this double current)
	{
		return Math.Abs(current);
	}

	public static double Ciel(this double current)
	{
		return Math.Ceiling(current);
	}

	public static double Floor(this double current)
	{
		return Math.Floor(current);
	}

	public static double ClampStep(this double current, double stepSize)
	{
		return (Math.Round(current / stepSize) * stepSize);
	}

	public static double Step(this double value, double step)
	{
		if (step <= 0)
		{
			return value;
		}

		value = Math.Floor(value * step) / (step - 0.5f);
		return Math.Max(value, 0.01f);
	}
}