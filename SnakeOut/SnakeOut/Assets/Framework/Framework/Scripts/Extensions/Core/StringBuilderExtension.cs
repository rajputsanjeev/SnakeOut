using System.Text;

public static class StringBuilderExtension
{
	public static string Get(this StringBuilder current, bool clear = true)
	{
		if (current.Length < 1)
		{
			return "";
		}

		var result = current.ToString();
		if (clear)
		{
			current.Clear();
		}

		return result;
	}

	public static string Cut(this StringBuilder current, int start, int size)
	{
		var value = current.ToString(start, size);
		current.Remove(start, size);
		return value;
	}

	public static void Clear(this StringBuilder current)
	{
		current.Length = 0;
		current.Capacity = 0;
	}

	public static StringBuilder Append(this StringBuilder current, params string[] values)
	{
		foreach (var value in values)
		{
			current.Append(value);
		}

		return current;
	}

	public static string Pull(this StringBuilder current)
	{
		var value = current.ToString();
		current.Clear();
		return value;
	}

	public static StringBuilder Prepend(this StringBuilder current, params string[] values)
	{
		var index = 0;
		foreach (var value in values)
		{
			current.Insert(index, value);
			index += value.Length;
		}

		return current;
	}

	public static StringBuilder Shorten(this StringBuilder current, int amount)
	{
		current.Length -= amount;
		return current;
	}

	public static StringBuilder Shorten(this StringBuilder current, string value)
	{
		current.Length -= value.Length;
		return current;
	}

	public static StringBuilder TrimRight(this StringBuilder current, params string[] values)
	{
		foreach (var value in values)
		{
			if (value.Length < 1 || value.Length > current.Length)
			{
				continue;
			}

			while (current.EndsWith(value))
			{
				current.Length -= value.Length;
			}
		}

		return current;
	}

	public static bool EndsWith(this StringBuilder current, params char[] values)
	{
		if (current.Length < 1)
		{
			return false;
		}

		foreach (var value in values)
		{
			if (current[current.Length - 1] == value)
			{
				return true;
			}
		}

		return false;
	}

	public static bool EndsWith(this StringBuilder current, params string[] values)
	{
		foreach (var value in values)
		{
			if (value.Length > current.Length)
			{
				continue;
			}

			var match = true;
			for (var index = 0; index < value.Length; ++index)
			{
				if (current[current.Length - value.Length + index] != value[index])
				{
					match = false;
					break;
				}
			}

			if (match)
			{
				return true;
			}
		}

		return false;
	}

	public static bool StartsWith(this StringBuilder current, params string[] values)
	{
		foreach (var value in values)
		{
			if (value.Length > current.Length)
			{
				return false;
			}

			var match = true;
			for (var index = 0; index < value.Length; ++index)
			{
				if (current[index] != value[index])
				{
					match = false;
					break;
				}
			}

			if (match)
			{
				return true;
			}
		}

		return false;
	}
}