using System.Linq;

public static class IntArrayExtension
{
	public static int[] ToIntArray(this (int, int, int)[] current)
	{
		return current.SelectMany(x => new[] { x.Item1, x.Item2, x.Item3 }).ToArray();
	}
}