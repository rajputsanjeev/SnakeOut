using UnityEngine;

public static class VectorExtensions
{
	public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
	{
		v.x = x ?? v.x;
		v.y = y ?? v.y;
		v.z = z ?? v.z;

		return v;
	}

	public static Vector3Int With(this Vector3Int v, int? x = null, int? y = null, int? z = null)
	{
		v.x = x ?? v.x;
		v.y = y ?? v.y;
		v.z = z ?? v.z;

		return v;
	}

	public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
	{
		v.x = x ?? v.x;
		v.y = y ?? v.y;

		return v;
	}

	public static Vector2Int With(this Vector2Int v, int? x = null, int? y = null)
	{
		v.x = x ?? v.x;
		v.y = y ?? v.y;

		return v;
	}
}
