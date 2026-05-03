using System;

public static class ConvertDateTime
{
	public static string ToQuickDate(this DateTime current)
	{
		return current.ToShortDateString().Replace("/", "-");
	}
}