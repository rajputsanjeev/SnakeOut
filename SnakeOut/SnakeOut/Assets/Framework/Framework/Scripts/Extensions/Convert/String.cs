using System;
using System.Security.Cryptography;
using System.Text;

public static class ConvertString
{
	public static string ToMD5(this string current)
	{
		var bytes = Encoding.UTF8.GetBytes(current);
		var hash = MD5.Create().ComputeHash(bytes);
		return BitConverter.ToString(hash).Replace("-", "");
	}

	public static Enum ToEnum(this string current, Type type, string separator)
	{
		if (current.Contains(separator))
		{
			var value = 0;
			foreach (var term in current.Split(separator))
			{
				value += Enum.Parse(type, term, true).ToInt();
			}

			return value.ToEnum(type);
		}

		return (Enum)Enum.Parse(type, current, true);
	}

	public static Type ToEnum<Type>(this string current)
	{
		return (Type)Enum.Parse(typeof(Type), current, true);
	}

	public static Enum ToEnum(this string current, Type type)
	{
		return (Enum)Enum.Parse(type, current, true);
	}

	public static float ToFloat(this string current)
	{
		if (current.IsEmpty())
		{
			return 0;
		}

		return System.Convert.ToSingle(current);
	}

	public static short ToShort(this string current)
	{
		if (current.IsEmpty())
		{
			return 0;
		}

		return System.Convert.ToInt16(current);
	}

	public static int ToInt(this string current)
	{
		if (current.IsEmpty())
		{
			return 0;
		}

		return System.Convert.ToInt32(current);
	}

	public static long ToLong(this string current)
	{
		if (current.IsEmpty())
		{
			return 0;
		}

		return System.Convert.ToInt64(current);
	}

	public static DateTime ToDateTime(this string current)
	{
		if (current.IsEmpty())
		{
			return DateTime.Now;
		}

		return System.Convert.ToDateTime(current);
	}

	public static int ToInt16(this string current)
	{
		return current.ToShort();
	}

	public static int ToInt32(this string current)
	{
		return current.ToInt();
	}

	public static float ToSingle(this string current)
	{
		return current.ToFloat();
	}

	public static double ToDouble(this string current)
	{
		if (current.IsEmpty())
		{
			return 0;
		}

		return System.Convert.ToDouble(current);
	}

	public static bool ToBool(this string current)
	{
		if (current.IsEmpty())
		{
			return false;
		}

		var lower = current.ToLower();
		return lower != "false" && lower != "f" && lower != "0";
	}

	public static byte ToByte(this string current)
	{
		return System.Convert.ToByte(current);
	}

	public static byte[] ToBytes(this string current)
	{
		return Encoding.UTF8.GetBytes(current);
	}

	public static string ToText(this string current, bool ignoreDefault = false, string defaultValue = "")
	{
		return ignoreDefault && current == defaultValue ? null : current;
	}

	public static byte[] ToByteArray(this string value)
	{
		return System.Convert.FromBase64String(value);
	}
}