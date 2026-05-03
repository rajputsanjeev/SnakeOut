using Firebase.Analytics;
using Framework.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public static class AnalyticsLevelFirebaseConverter
	{
		public static Parameter[] ToFirebaseParams(AnalyticsLevelData data)
		{
			List<Parameter> parameters = new List<Parameter>();

			// Status (required)
			if (!string.IsNullOrEmpty(data.Status))
			{
				parameters.Add(new Parameter("status", data.Status));
			}

			// Level delta (dynamic)
			if (data.LevelDelta != null)
			{
				foreach (var kv in data.LevelDelta)
				{
					AddSafeParameter(parameters, kv.Key, kv.Value);
				}
			}

			return parameters.ToArray();
		}

		private static void AddSafeParameter(
			List<Parameter> parameters,
			string key,
			object value)
		{
			if (value == null) return;

			switch (value)
			{
				case int i:
					parameters.Add(new Parameter(key, i));
					break;

				case long l:
					parameters.Add(new Parameter(key, l));
					break;

				case float f:
					parameters.Add(new Parameter(key, (double)f));
					break;

				case double d:
					parameters.Add(new Parameter(key, d));
					break;

				case string s:
					parameters.Add(new Parameter(key, s));
					break;

				case bool b:
					parameters.Add(new Parameter(key, b ? 1 : 0)); // Firebase-safe
					break;

				case System.Enum e:
					parameters.Add(new Parameter(key, e.ToString()));
					break;

				default:
					Debug.LogWarning(
						$"[Analytics] Unsupported LevelDelta type: {key} = {value.GetType().Name}"
					);
					break;
			}
		}
	}
}
