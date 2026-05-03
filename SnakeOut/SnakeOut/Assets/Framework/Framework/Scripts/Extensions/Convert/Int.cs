using System;
using System.Collections.Generic;

public static class ConvertInt
{
	public static string ToHex(this int current)
	{
		return current.ToString("X6");
	}

	public static Enum ToEnum(this int current, Type enumType)
	{
		return (Enum)Enum.ToObject(enumType, current);
	}

	public static Type ToEnum<Type>(this int current)
	{
		return (Type)Enum.ToObject(typeof(Type), current);
	}

	public static bool ToBool(this int current)
	{
		return current != 0;
	}

	public static byte ToByte(this int current)
	{
		return (byte)current;
	}

	public static Int16 ToInt16(this int current)
	{
		return (Int16)current;
	}

	public static short ToShort(this int current)
	{
		return (short)current;
	}

	public static byte[] ToBytes(this int current)
	{
		return BitConverter.GetBytes(current);
	}

	public static string ToText(this int current, bool ignoreDefault = false, int defaultValue = 0)
	{
		return ignoreDefault && current == defaultValue ? null : current.ToString();
	}

	public static bool[] ToFlags(this int current)
	{
		var value = 1;
		var result = new List<bool>();
		while (value <= current)
		{
			result.Add((value & current) != 0);
			value *= 2;
		}

		return result.ToArray();
	}

	public static bool[] ToFlags(this int current, int size)
	{
		var value = 1;
		var result = new bool[size];
		for (var index = 0; index < size; ++index)
		{
			result[index] = (value & current) != 0;
			value *= 2;
		}

		return result;
	}

	public static int ToInt(this ushort current)
	{
		return current;
	}

	public static int ToInt(this uint current)
	{
		return (int)current;
	}

	public static sbyte ToSignedByte(this int current)
	{
		return (sbyte)current;
	}

	public static ushort ToUnsignedShort(this int current)
	{
		return (ushort)current;
	}

	public static uint ToUnsignedInt(this int current)
	{
		return (uint)current;
	}
}