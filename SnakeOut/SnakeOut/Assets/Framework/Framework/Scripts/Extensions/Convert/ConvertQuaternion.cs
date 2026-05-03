using UnityEngine;

public static class ConvertQuaternion
{
	public static (float x, float y, float z, float w) ToTuple(this Quaternion current)
	{
		return (current.x, current.y, current.z, current.w);
	}

	public static float[] ToFloatArray(this Quaternion current)
	{
		return new float[4] { current.x, current.y, current.z, current.w };
	}

	public static Color ToColor(this Quaternion current)
	{
		return new Color(current.x, current.y, current.z, current.w);
	}

	public static Quaternion ToQuaternion(this Vector4 current)
	{
		return new Quaternion(current.x, current.y, current.z, current.w);
	}

	public static Quaternion ToVector4(this Vector4 current, string value)
	{
		return value.ToQuaternion();
	}

	public static Quaternion ToQuaternion(this float[] current)
	{
		var x = current.Length >= 1 ? current[0] : 0;
		var y = current.Length >= 2 ? current[1] : 0;
		var z = current.Length >= 3 ? current[2] : 0;
		var w = current.Length >= 4 ? current[3] : 0;
		return new Quaternion(x, y, z, w);
	}

	public static Quaternion ToQuaternion(this string current, string separator = ",")
	{
		if (!current.Contains(separator))
		{
			return Quaternion.identity;
		}

		// var values = current.Trim("(", ")").Split(separator).ConvertAll<float>();
		// return new Quaternion(values[0], values[1], values[2], values[3]);
		return Quaternion.identity;
	}
}