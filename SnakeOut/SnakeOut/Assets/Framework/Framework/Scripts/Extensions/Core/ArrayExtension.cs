using System;
using System.Collections.Generic;

public static class ArrayExtension
{
	public static int IndexOf<Type>(this Array current, Type value)
	{
		return Array.IndexOf(current, value);
	}

	public static int IndexOf<Type>(this Array current, Enum enumerable)
	{
		var name = enumerable.ToString();
		return current.IndexOf(name);
	}

	public static Type[] Copy<Type>(this Type[] current)
	{
		var result = new Type[current.Length];
		current.CopyTo(result, 0);
		return result;
	}

	public static Type[] Concat<Type>(this Type[] current, Type[] other)
	{
		var result = new Type[current.Length + other.Length];
		current.CopyTo(result, 0);
		other.CopyTo(result, current.Length);
		return result;
	}

	public static bool Has<Type>(this Type[] current, params Type[] other)
	{
		foreach (var itemA in current)
		{
			foreach (var itemB in other)
			{
				if (itemA.Equals(itemB))
				{
					return true;
				}
			}
		}

		return false;
	}

	public static bool Exists<Type>(this Type[] current, Predicate<Type> predicate)
	{
		return Array.Exists(current, predicate);
	}

	public static Type Find<Type>(this Type[] current, Predicate<Type> predicate)
	{
		return Array.Find(current, predicate);
	}

	public static Type[] Clear<Type>(this Type[] current)
	{
		return Array.Empty<Type>();
	}

	public static Type[] Append<Type>(this Type[] current, Type element)
	{
		return current.Add(element);
	}

	public static Type[] Prepend<Type>(this Type[] current, Type element)
	{
		var copy = new List<Type>(current);
		copy.Insert(0, element);
		return copy.ToArray();
	}

	public static Type[] Add<Type>(this Type[] current, Type element)
	{
		var extra = new Type[] { element };
		return current.Concat(extra);
	}

	public static Type[] AddNew<Type>(this Type[] current)
	{
		var extra = new Type[] { Activator.CreateInstance<Type>() };
		return current.Concat(extra);
	}

	public static Type[] AddNew<Type>(this Type[] current, Type element)
	{
		if (current.IndexOf(element) == -1)
		{
			var extra = new Type[] { element };
			return current.Concat(extra);
		}

		return current;
	}

	public static Type[] Remove<Type>(this Type[] current, Type value)
	{
		var copy = new List<Type>(current);
		copy.Remove(value);
		return copy.ToArray();
	}

	public static Type[] RemoveAt<Type>(this Type[] current, int index)
	{
		var copy = new List<Type>(current);
		copy.RemoveAt(index);
		return copy.ToArray();
	}

	public static Type[] RemoveAll<Type>(this Type[] current, Type value)
	{
		var copy = new List<Type>(current);
		copy.RemoveAll(x => x.Equals(value));
		return copy.ToArray();
	}

	public static Type[] Resize<Type>(this Type[] current, int newSize)
	{
		Array.Resize(ref current, newSize);
		return current;
	}

	public static Type[,] Fill<Type>(this Type[,] current) where Type : new()
	{
		for (var indexA = 0; indexA < current.GetLength(0); ++indexA)
		{
			for (var indexB = 0; indexB < current.GetLength(1); ++indexB)
			{
				current[indexA, indexB] = new Type();
			}
		}

		return current;
	}

	public static bool Any<Type>(this Type[,] current, Func<Type, bool> method)
	{
		foreach (var item in current)
		{
			if (method(item))
			{
				return true;
			}
		}

		return false;
	}

	public static Type[] Flatten<Type>(this Type[,] current)
	{
		var size = (current.GetUpperBound(0) + 1) * (current.GetUpperBound(1) + 1);
		var result = new Type[size];
		var item = 0;
		for (var indexB = 0; indexB <= current.GetUpperBound(1); ++indexB)
		{
			for (var indexA = 0; indexA <= current.GetUpperBound(0); ++indexA)
			{
				result[item++] = current[indexA, indexB];
			}
		}

		return result;
	}

	public static Type[] GetRange<Type>(this Type[] array, int startIndex, int length)
	{
		var subset = new Type[length];
		Array.Copy(array, startIndex, subset, 0, length);
		return subset;
	}

	public static Type[] GetRange<Type>(this Type[] array, int startIndex)
	{
		var length = array.Length - startIndex;
		var subset = new Type[length];
		Array.Copy(array, startIndex, subset, 0, length);
		return subset;
	}
}