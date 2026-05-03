using System;
using Framework.Core;

#if MODULE_CONFIG_CAT
using System.Globalization;
using System.Threading.Tasks;
using ConfigCat.Client;
#endif

using UnityEngine;

namespace Framework.Core
{
#if MODULE_CONFIG_CAT
	public static class ConfigCatHandler
	{
		private static IConfigCatClient configCatClient;
		private static bool isInitialized;

		public static IConfigCatClient ConfigCatClient => configCatClient;

		// Call this once (for example at game start)
		public static void Initialize(string apiKey)
		{
			if (isInitialized)
				return;

			configCatClient = ConfigCat.Client.ConfigCatClient.Get(apiKey, options =>
			{
				options.Logger = new ConfigCatToUnityDebugLogAdapter(LogLevel.Warning);
			});

			isInitialized = true;
		}

		public static void Dispose()
		{
			configCatClient?.Dispose();
			configCatClient = null;
			isInitialized = false;
		}

		public static async Task LoadConfigAsync(Action<bool> callback = null, string eventName = null)
		{
			if (!isInitialized || configCatClient == null)
			{
				Debug.LogError("ConfigCatHandler not initialized. Call Initialize() first.");
				callback?.Invoke(false);
				return;
			}

			bool isFeatureEnable;

			if (string.IsNullOrEmpty(eventName))
			{
				isFeatureEnable = true;
			}
			else
			{
				isFeatureEnable = await configCatClient.GetValueAsync(eventName, false);
			}

			callback?.Invoke(isFeatureEnable);
		}

		// ----------------- LOGGER -----------------

		private sealed class ConfigCatToUnityDebugLogAdapter : IConfigCatLogger
		{
			private readonly LogLevel logLevel;

			public ConfigCatToUnityDebugLogAdapter(LogLevel logLevel)
			{
				this.logLevel = logLevel;
			}

			public LogLevel LogLevel
			{
				get => this.logLevel;
				set { throw new NotSupportedException(); }
			}

			public void Log(LogLevel level, LogEventId eventId, ref FormattableLogMessage message, Exception exception = null)
			{
				var eventIdString = eventId.Id.ToString(CultureInfo.InvariantCulture);
				var exceptionString = exception == null ? string.Empty : Environment.NewLine + exception;
				var logMessage = $"ConfigCat[{eventIdString}] {message.InvariantFormattedMessage}{exceptionString}";

				switch (level)
				{
					case LogLevel.Error:
						Debug.LogError("ConfigCat " + logMessage);
						break;
					case LogLevel.Warning:
						Debug.LogWarning("ConfigCat " + logMessage);
						break;
					case LogLevel.Info:
						Debug.Log("ConfigCat " + logMessage);
						break;
					case LogLevel.Debug:
						Debug.Log("ConfigCat " + logMessage);
						break;
				}
			}
		}
	}
#endif
}
