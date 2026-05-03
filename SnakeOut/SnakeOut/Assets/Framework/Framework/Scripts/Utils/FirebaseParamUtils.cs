using System.Collections.Generic;
using Firebase.Analytics;
using Framework.Core;

namespace Framework
{
	public static class FirebaseParamUtils
	{

		public static Parameter[] CurrencyDeltaToParameters(AnalyticsCurrencyData data)
		{
			List<Parameter> parameters = new List<Parameter>();

			// Currency deltas
			if (data.CurrenciesDelta != null)
			{
				foreach (var kv in data.CurrenciesDelta)
				{
					string paramName = $"currency_{kv.Key.ToString().ToLower()}";
					parameters.Add(new Parameter(paramName, kv.Value));
				}
			}

			return parameters.ToArray();
		}

	}
}

