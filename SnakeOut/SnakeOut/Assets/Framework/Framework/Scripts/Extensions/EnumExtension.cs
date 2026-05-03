using UnityEngine;

public static class EnumExtension
{
	public static T GetRandom<T>() where T : System.Enum
	{
		var values = (T[])System.Enum.GetValues(typeof(T));
		return values[Random.Range(0, values.Length)];
	}
}
