using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
	public static List<Type> Copy<Type>(this List<Type> current)
	{
		return new List<Type>(current);
	}

	public static List<KeyValuePair<string, T>> Find<T>(this Dictionary<string, T> dictionary, string scenario)
	{
		var matchingElements = dictionary
			.Where(key => key.Key.Contains(scenario))
			.ToList();

		return matchingElements;
	}

	public static T Find<T>(List<T> list, Func<T, string> propertySelector, string value)
	{
		foreach (T item in list)
		{
			if (propertySelector(item) == value)
			{
				return item;
			}
		}
		return default;
	}

	public static void Move<Type>(this List<Type> current, int index, int newIndex) where Type : class
	{
		var item = current[index];
		current.Remove(item);
		current.Insert(newIndex, item);
	}

	public static List<Type> Unshift<Type>(this List<Type> current, Type element)
	{
		current.Insert(0, element);
		return current;
	}

	public static Type Find<Type>(this List<Type> current, Type value)
	{
		return current.Find(x => x.Equals(value));
	}

	public static bool Exists<Type>(this List<Type> current, Type value)
	{
		return current.Exists(x => x.Equals(value));
	}

	public static bool Has<Type>(this List<Type> current, Type value)
	{
		foreach (var item in current)
		{
			if (item.Equals(value))
			{
				return true;
			}
		}

		return false;
	}

	public static Type AddNew<Type>(this List<Type> current) where Type : new()
	{
		var item = new Type();
		current.Add(item);
		return item;
	}

	public static Type AddNew<Type>(this List<Type> current, Type value)
	{
		if (!current.Contains(value))
		{
			current.Add(value);
		}

		return value;
	}

	public static List<Type> Prepend<Type>(this List<Type> current, Type element)
	{
		return current.Unshift(element);
	}

	public static Type AppendNew<Type>(this List<Type> current, Type value)
	{
		if (current.Contains(value))
		{
			current.Remove(value);
		}

		current.Add(value);
		return value;
	}

	public static Type Toggle<Type>(this List<Type> current, Type value)
	{
		if (current.Contains(value))
		{
			current.Remove(value);
			return value;
		}

		current.Add(value);
		return value;
	}

	public static List<Type> Overwrite<Type>(this List<Type> current, params Type[] values) where Type : new()
	{
		current.Clear();
		current.AddRange(values);
		return current;
	}

	public static List<Type> OverwriteToggle<Type>(this List<Type> current, params Type[] values) where Type : new()
	{
		if (!current.ContainsAny(values))
		{
			return current.Overwrite(values);
		}

		foreach (var value in values)
		{
			current.Remove(value);
		}

		return current;
	}

	public static int IndexOf<Type>(this List<Type> current, Type item)
	{
		return current.FindIndex(x => x.Equals(item));
	}

	public static int IndexOf<Type>(this List<Type> current, Enum enumerable)
	{
		var name = enumerable.ToString();
		return current.ToArray().IndexOf(name);
	}

	public static List<Type> Shuffle<Type>(this List<Type> current)
	{
		var copy = current.Copy();
		var random = new Random();
		var total = copy.Count;
		while (total > 1)
		{
			total--;
			var index = random.Next(total + 1);
			var value = copy[index];
			copy[index] = copy[total];
			copy[total] = value;
		}

		return copy;
	}

	public static List<string> ToLower(this List<string> current)
	{
		var newList = new List<string>();
		foreach (var item in current)
		{
			newList.Add(item.ToLower());
		}

		return newList;
	}

	public static List<Type> Order<Type>(this List<Type> current)
	{
		//var copy = current.Copy();
		current.Sort();
		return current;
	}

	public static List<Type> Extend<Type>(this List<Type> current, IEnumerable<Type> values, bool asCopy = false)
	{
		var target = asCopy ? new List<Type>(current) : current;
		target.AddRange(values);
		return target;
	}

	public static List<Type> DeleteAt<Type>(this List<Type> current, int index)
	{
		current.RemoveAt(index);
		return current;
	}

	public static List<Type> Delete<Type>(this List<Type> current, params Type[] values)
	{
		foreach (var value in values)
		{
			current.Remove(value);
		}

		return current;
	}

	public static List<Type> Push<Type>(this List<Type> current, params Type[] values)
	{
		foreach (var value in values)
		{
			current.Append(value);
		}

		return current;
	}

	public static Type TakeRandom<Type>(this List<Type> current)
	{
		return current.Shuffle().First();
	}

	public static List<Type> Resize<Type>(this List<Type> current, int size)
	{
		while (current.Count < size)
		{
			current.Add((Type)Activator.CreateInstance(typeof(Type)));
		}

		while (current.Count > size)
		{
			current.RemoveAt(current.Count - 1);
		}

		return current;
	}

	public static List<Type> Set<Type>(this List<Type> current, int index, Type value)
	{
		if (current.Count <= index)
		{
			current.Resize(index + 1);
		}

		current[index] = value;
		return current;
	}

	public static List<int> DivideSize<Type>(this List<Type> current, int size)
	{
		var data = new int[size].ToList();
		var index = 0;
		while (index < current.Count)
		{
			data[index % size] += 1;
			index += 1;
		}

		return data;
	}

	public static List<List<Type>> DivideEvery<Type>(this List<Type> current, int amount)
	{
		return current.DivideInto(current.Count / amount);
	}

	public static List<List<Type>> DivideInto<Type>(this List<Type> current, int amount)
	{
		var data = new List<List<Type>>();
		var sizes = current.DivideSize(amount);
		var index = 0;
		foreach (var size in sizes)
		{
			var count = 0;
			var entry = data.AddNew();
			while (count < size)
			{
				entry.Add(current[count + index]);
				count += 1;
			}

			index += size;
		}

		return data;
	}

	public static Type FromEnd<Type>(this IList<Type> current, int amount)
	{
		return current[current.Count - amount];
	}

	public static Type Get<Type>(this IList<Type> current, int index)
	{
		return (index < current.Count) && (index >= 0) ? current[index] : default;
	}

	public static List<string> GetContains(this List<string> current, string value)
	{
		var similar = new List<string>();
		foreach (string key in current)
		{
			if (key.Contains(value, true))
			{
				similar.Add(key);
			}
		}

		return similar;
	}

	public static List<string> GetStartsWith(this List<string> current, string value)
	{
		var similar = new List<string>();
		foreach (string key in current)
		{
			if (key.StartsWith(value, true))
			{
				similar.Add(key);
			}
		}

		return similar;
	}

	public static bool Has(this List<string> current, string value)
	{
		var similar = new List<string>();
		foreach (string key in current)
		{
			if (key.Matches(value))
			{
				return true;
			}
		}

		return false;
	}
}