using System;
using UnityEngine;

namespace Framework
{
	public static class DebugExtension
	{
		// Change this to false if you ever want force-disable logs
		private const bool FORCE_ENABLE = true;

		// ---------- PUBLIC API ----------

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		[System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
		public static void Log(string message, string color = null)
		{
			if (!IsEnabled()) return;
			Debug.Log(Format(message, color));
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		[System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
		public static void LogWarning(string message, string color = null)
		{
			if (!IsEnabled()) return;
			Debug.LogWarning(Format(message, color));
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		[System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
		public static void LogError(string message, string color = null)
		{
			if (!IsEnabled()) return;
			Debug.LogError(Format(message, color));
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		[System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
		public static void Log(string message, Color color)
		{
			if (!IsEnabled()) return;
			Debug.LogError(Format(message, ColorToHex(color)));
		}

		private static string ColorToHex(Color color)
		{
			Color32 c = color;
			return $"{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
		}

		// ---------- INTERNAL ----------

		private static bool IsEnabled()
		{
			return FORCE_ENABLE;
		}

		private static string Format(string message, string color)
		{
			if (string.IsNullOrEmpty(color))
				return message;

			color = NormalizeColor(color);
			return $"<color={color}>{message}</color>";
		}

		private static string NormalizeColor(string color)
		{
			if (color.StartsWith("#"))
				return color;

			if (IsHex(color))
				return $"#{color}";

			return color.ToLower();
		}

		private static bool IsHex(string value)
		{
			if (value.Length != 6 && value.Length != 8)
				return false;

			foreach (char c in value)
				if (!Uri.IsHexDigit(c))
					return false;

			return true;
		}
	}
}