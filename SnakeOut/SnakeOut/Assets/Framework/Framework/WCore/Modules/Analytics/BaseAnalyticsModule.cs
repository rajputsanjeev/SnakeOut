using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
	[StaticUnload]
	[Define("MODULE_FIREBASE_ANALYTICS", "Firebase.Analytics.FirebaseAnalytics", "Firebase.Analytics.dll")]
	[Define("MODULE_GAME_ANALYTICS", "GameAnalyticsSDK.GameAnalytics", "GameAnalytics.dll")]
	[Define("MODULE_FACEBOOK", "Facebook.Unity.FB", "Facebook.Unity.dll")]
	[Define("MODULE_TIKTOK", "SDK.TikTokBusinessSDK", "tiktok-business-android-sdk.dll")]
	public abstract class BaseAnalyticsModule
	{
		private List<SimpleCallback> eventsBeforeInitialization;
		private bool isInitialized = false;

		private Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>> handlers;

		public BaseAnalyticsModule()
		{
			handlers = GetHandlers();

			isInitialized = false;
			eventsBeforeInitialization = new List<SimpleCallback>();

			AnalyticsController.EventFired += OnEventFired;

			Debug.Log($"[Analytics]: {GetType()} created.");
		}

		public void Init()
		{
			if (isInitialized) return;

			isInitialized = true;

			foreach (SimpleCallback analyticEvent in eventsBeforeInitialization)
			{
				analyticEvent?.Invoke();
			}

			eventsBeforeInitialization = null;

			Debug.Log($"[Analytics]: {GetType()} initialized.");

			OnInitialized();
		}

		private void OnEventFired(AnalyticsEventType type, IAnalyticsEventData analyticsEventData)
		{
			Action<IAnalyticsEventData> handler = null;
			handlers.TryGetValue(type, out handler);

			void InvokeHandler()
			{
				if (handler != null)
				{
					handler.Invoke(analyticsEventData);
				}
			}

			if (isInitialized)
			{
				InvokeHandler();
			}
			else
			{
				eventsBeforeInitialization.Add(() =>
				{
					InvokeHandler();
				});
			}
		}

		public abstract Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>> GetHandlers();
		public abstract void OnInitialized();
	}
}
