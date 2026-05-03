using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
	//=======================
	// General
	//=======================
	public static bool ContainsAll<Type>(this IEnumerable<Type> current, params Type[] values)
	{
		for (var index = 0; index < values.Length; ++index)
		{
			if (!current.Contains(values[index]))
			{
				return false;
			}
		}

		return true;
	}

	public static bool ContainsAny<Type>(this IEnumerable<Type> current, IEnumerable<Type> values,
		Func<Type, Type, bool> method)
	{
		return values.Any(x => current.Any(y => method(y, x)));
	}

	public static bool ContainsAll<Type>(this IEnumerable<Type> current, IEnumerable<Type> values,
		Func<Type, Type, bool> method)
	{
		return values.All(x => current.Any(y => method(y, x)));
	}

	public static bool ContainsAll<Type>(this IEnumerable<Type> current, IEnumerable<Type> other)
	{
		return !other.Except(current).Any();
	}

	public static bool ContainsAny<Type>(this IEnumerable<Type> current, params Type[] values)
	{
		for (var index = 0; index < values.Length; ++index)
		{
			if (current.Contains(values[index]))
			{
				return true;
			}
		}

		return false;
	}

	public static bool ContainsAmount<Type>(this IEnumerable<Type> current, int amount, params Type[] values)
	{
		var count = 0;
		for (var index = 0; index < values.Length; ++index)
		{
			if (current.Contains(values[index]))
			{
				count += 1;
			}
		}

		return count >= amount;
	}

	public static bool HasAny<Type>(this IEnumerable<Type> current, params Type[] values)
	{
		return current.ContainsAny(values);
	}

	public static bool HasAll<Type>(this IEnumerable<Type> current, params Type[] values)
	{
		return current.ContainsAll(values);
	}

	public static string Display<Type>(this IEnumerable<Type> current)
	{
		var output = "[";
		foreach (var value in current)
		{
			output += value + ",";
		}

		return output.Trim(',') + "]";
	}

	public static void Print<Type>(this IEnumerable<Type> current, char marker = '•')
	{
		foreach (var value in current)
		{
			System.Console.WriteLine(marker + " " + value);
		}
	}

	public static Type[] Unique<Type>(this IEnumerable<Type> current, IEnumerable<Type> other)
	{
		return current.Except(other).Union(other.Except(current)).ToArray();
	}

	public static Type[] Unique<Type>(this IEnumerable<Type> current)
	{
		var unique = new List<Type>();
		foreach (var value in current)
		{
			if (!unique.Contains(value))
			{
				unique.Add(value);
			}
		}

		return unique.ToArray();
	}

	//=======================
	// General
	//=======================
	public static IEnumerable<Type> Diff<Type>(this IEnumerable<Type> current, IEnumerable<Type> other)
	{
		return current.Except(other).Concat(other.Except(current));
	}

	public static IEnumerable<Type> Unshift<Type>(this IEnumerable<Type> current, Type item)
	{
		var result = current.ToList();
		result.Insert(0, item);
		return result;
	}

	public static IEnumerable<Type> ReverseOrder<Type>(this IEnumerable<Type> current)
	{
		current.Reverse();
		return current;
	}

	public static IEnumerable<Type> Order<Type>(this IEnumerable<Type> current)
	{
		var copy = current.ToList();
		copy.Sort();
		return copy.ToArray();
	}

	public static Type GetElementType(this IEnumerable current)
	{
		var collection = current.GetType();
		if (collection.IsArray())
		{
			return collection.GetElementType();
		}

		if (collection.IsDictionary())
		{
			var pair = typeof(KeyValuePair<,>);
			var types = collection.GetGenericArguments();
			return pair.MakeGenericType(types);
		}

		return collection.GetGenericArguments().FirstOrDefault();
	}

	//=======================
	// LINQ-ish
	//=======================
	public static IEnumerable<int> Indexes<Type>(this IEnumerable<Type> collection, Func<Type, bool> method)
	{
		var index = 0;
		var matches = new List<int>();
		foreach (var item in collection)
		{
			if (method(item))
			{
				matches.Add(index);
			}

			index += 1;
		}

		return matches;
	}

	public static void Map<From, To>(this IEnumerable<From> current, IList<To> other, Action<To, From> method)
	{
		var index = 0;
		foreach (var value in current)
		{
			if (index >= other.Count())
			{
				break;
			}

			method(other[index], value);
			index += 1;
		}
	}

	public static List<Type> If<Type>(this IEnumerable<Type> current, Func<Type, bool> comparer)
	{
		var results = new List<Type>();
		foreach (var item in current)
		{
			if (comparer(item))
			{
				results.Add(item);
			}
		}

		return results;
	}

	public static void For<Type>(this IEnumerable<Type> current, Action<int> executor)
	{
		var amount = current.Count();
		for (var index = 0; index < amount; ++index)
		{
			executor(index);
		}
	}

	public static void ForEach(this IEnumerable current, Action<object> executor)
	{
		foreach (var item in current)
		{
			executor(item);
		}
	}

	public static void Assign<Type, AssignType>(this IList<Type> current, IList<AssignType> assigner,
		Action<Type, AssignType> executor)
	{
		var amount = current.Count();
		for (var index = 0; index < amount; ++index)
		{
			executor(current[index], assigner[index]);
		}
	}

	public static Type[] SelectMultiple<Type>(this IEnumerable<Type> current, params int[] indices)
	{
		var list = new List<Type>();
		var count = current.Count();
		if (count < 1)
		{
			return new Type[0];
		}

		foreach (var index in indices)
		{
			if (index < 0 || index >= count)
			{
				continue;
			}

			list.Add((Type)current.GetByKey(index));
		}

		return list.ToArray();
	}

	public static List<ReturnType> SelectFor<Type, ReturnType>(this IEnumerable<Type> current,
		Func<int, ReturnType> executor)
	{
		var amount = current.Count();
		var results = new List<ReturnType>();
		for (var index = 0; index < amount; ++index)
		{
			results.Add(executor(index));
		}

		return results;
	}

	public static IEnumerable<Type> SkipLast<Type>(this IEnumerable<Type> current)
	{
		return current.SkipRight(1);
	}

	public static IEnumerable<Type> SkipRight<Type>(this IEnumerable<Type> current, int amount)
	{
		return current.Take(current.Count() - amount);
	}

	public static IEnumerable<Type> TakeRight<Type>(this IEnumerable<Type> current, int amount)
	{
		return current.Skip(current.Count() - amount).Take(amount);
	}

	//=======================
	// Numeric
	//=======================
	public static float FirstOrNaN(this IEnumerable<float> current)
	{
		if (current.Count() > 0)
		{
			return current.First();
		}

		return float.NaN;
	}

	public static double FirstOrNaN(this IEnumerable<double> current)
	{
		if (current.Count() > 0)
		{
			return current.First();
		}

		return double.NaN;
	}

	public static short Average(this IEnumerable<short> current)
	{
		short value = 0;
		var items = 0;
		foreach (var item in current)
		{
			value += item;
			items += 1;
		}

		return (short)(value / items);
	}

	//=======================
	// String
	//=======================
	public static bool Contains(this IEnumerable<string> current, string key, bool ignoreCase)
	{
		key = key.ToLower();
		foreach (var item in current)
		{
			if (item.ToLower() == key)
			{
				return true;
			}
		}

		return false;
	}

	public static bool ContainsAny(this IEnumerable<string> current, params string[] keys)
	{
		return current.ContainsAny(true, keys);
	}

	public static bool ContainsAny(this IEnumerable<string> current, bool ignoreCase, params string[] keys)
	{
		foreach (var key in keys)
		{
			var term = ignoreCase ? key.ToLower() : key;
			foreach (var item in current)
			{
				var existing = ignoreCase ? item.ToLower() : item;
				if (existing == term)
				{
					return true;
				}
			}
		}

		return false;
	}

	public static string Join(this IEnumerable<string> current, string separator = " ")
	{
		return string.Join(separator, current.ToArray());
	}

	public static List<string> Filter(this IEnumerable<string> current, string text)
	{
		var newList = new List<string>();
		var wildcard = text.Contains("*");
		text = text.Replace("*", "");
		foreach (var item in current)
		{
			if (wildcard && item.Contains(text))
			{
				newList.Add(item);
			}
			else if (item == text)
			{
				newList.Add(item);
			}
		}

		return newList;
	}

	public static List<string> Replace(this IEnumerable<string> current, string replace, string with,
		bool ignoreCase = true)
	{
		var results = new List<string>();
		foreach (var item in current)
		{
			results.Add(item.Replace(replace, with, ignoreCase));
		}

		return results;
	}

	public static List<string> AddSuffix(this IEnumerable<string> current, string suffix)
	{
		var results = new List<string>();
		foreach (var item in current)
		{
			results.Add(item + suffix);
		}

		return results;
	}

	public static string[] Trim(this IEnumerable<string> current, string values)
	{
		return current.Select(x => x.Trim(values)).ToArray();
	}

	public static string[] ToTitleCase(this IEnumerable<string> current)
	{
		return current.Select(x => x.ToTitleCase()).ToArray();
	}

	public static string[] ToCamelCase(this IEnumerable<string> current)
	{
		return current.Select(x => x.ToCamelCase()).ToArray();
	}

	public static string[] ToPascalCase(this IEnumerable<string> current)
	{
		return current.Select(x => x.ToPascalCase()).ToArray();
	}
}