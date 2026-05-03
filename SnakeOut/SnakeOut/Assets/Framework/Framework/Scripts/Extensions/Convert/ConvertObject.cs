using System;
using System.Collections.Generic;

public static class ConvertObject
{
	#region Conversion

	public static Type As<Type>(this object current)
	{
		if (current == null)
		{
			return default;
		}

		return (Type)current;
	}

	public static Type TryAs<Type>(this object current)
	{
		try
		{
			return (Type)current;
		}
		catch
		{
			return default;
		}
	}

	public static Type Convert<Type>(this object current)
	{
		return System.Convert.ChangeType(current, typeof(Type)).As<Type>();
	}

	public static float ToFloat(this object current)
	{
		return System.Convert.ChangeType(current, typeof(float)).As<float>();
	}

	public static int ToInt(this object current)
	{
		return System.Convert.ChangeType(current, typeof(int)).As<int>();
	}

	public static double ToDouble(this object current)
	{
		return System.Convert.ChangeType(current, typeof(double)).As<double>();
	}

	public static long ToLong(this object current)
	{
		return System.Convert.ChangeType(current, typeof(long)).As<long>();
	}

	public static DateTime ToDateTime(this object current)
	{
		return System.Convert.ChangeType(current, typeof(DateTime)).As<DateTime>();
	}

	public static string ToText(this object current)
	{
		return System.Convert.ChangeType(current, typeof(string)).As<string>();
	}

	public static bool ToBool(this object current)
	{
		return System.Convert.ChangeType(current, typeof(bool)).As<bool>();
	}

	public static object Box<Type>(this Type current)
	{
		return current;
	}

	public static object AsBox<Type>(this Type current)
	{
		return current;
	}

	public static Type[] AsArray<Type>(this Type current)
	{
		return current == null ? Array.Empty<Type>() : new Type[] { current };
	}

	public static Type[] AsArray<Type>(this Type current, int amount)
	{
		return current.AsList(amount).ToArray();
	}

	public static List<Type> AsList<Type>(this Type current)
	{
		return current == null ? new List<Type>() : new List<Type> { current };
	}

	public static List<Type> AsList<Type>(this Type current, int amount)
	{
		if (current == null)
		{
			return new List<Type>();
		}

		var collection = new List<Type>();
		while (amount > 0)
		{
			collection.Add(current);
			amount -= 1;
		}

		return collection;
	}

	#endregion
}