using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
	[StaticUnload]
	public static class AnalyticsController
	{
		public static event AnalyticsEventCallback EventFired;

		public static void OnLevelComplete(string source, Dictionary<string, object> levelStatus)
		{
			AnalyticsLevelData analyticsLevelData = new AnalyticsLevelData();
			analyticsLevelData.Status = source;
			analyticsLevelData.LevelDelta = levelStatus;

			TrackEvent(AnalyticsEventType.LevelComplete, analyticsLevelData);
		}

		public static void OnLevelFailed(string source, Dictionary<string, object> levelStatus)
		{
			AnalyticsLevelData analyticsLevelData = new AnalyticsLevelData();
			analyticsLevelData.Status = source;
			analyticsLevelData.LevelDelta = levelStatus;

			TrackEvent(AnalyticsEventType.LevelFailed, analyticsLevelData);
		}

		public static void OnLevelReplay(string source, Dictionary<string, object> levelStatus)
		{
			AnalyticsLevelData analyticsLevelData = new AnalyticsLevelData();
			analyticsLevelData.Status = source;
			analyticsLevelData.LevelDelta = levelStatus;

			TrackEvent(AnalyticsEventType.LevelReplay, analyticsLevelData);
		}

		public static void OnCurrencySource(string source, Dictionary<CurrencyType, int> currenciesDelta)
		{
			AnalyticsCurrencyData analyticsCurrencyData = new AnalyticsCurrencyData();
			analyticsCurrencyData.Source = source;
			analyticsCurrencyData.CurrenciesDelta = currenciesDelta;

			TrackEvent(AnalyticsEventType.CurrencySource, analyticsCurrencyData);
		}

		public static void OnCurrencySink(string sink, Dictionary<CurrencyType, int> currenciesDelta)
		{
			AnalyticsCurrencyData analyticsCurrencyData = new AnalyticsCurrencyData();
			analyticsCurrencyData.Source = sink;
			analyticsCurrencyData.CurrenciesDelta = currenciesDelta;

			TrackEvent(AnalyticsEventType.CurrencySink, analyticsCurrencyData);
		}

		public static void OnIAPClicked(IAPItem item)
		{
			AnalyticsIAPData analyticsData = new AnalyticsIAPData();
			analyticsData.Item = item;
			analyticsData.Source = AnalyticsEventType.IAPClicked.ToString();

			TrackEvent(AnalyticsEventType.IAPClicked, analyticsData);
		}

		public static void OnRVClicked(string source)
		{
			AnalyticsRVData analyticsData = new AnalyticsRVData();
			analyticsData.Source = source;
			analyticsData.AdProv = AdProvider.AdMob;

			TrackEvent(AnalyticsEventType.RVClicked, analyticsData);
		}

		public static void OnRVComplete(string source)
		{
			AnalyticsRVData analyticsData = new AnalyticsRVData();
			analyticsData.Source = source;
			analyticsData.AdProv = AdProvider.AdMob;

			TrackEvent(AnalyticsEventType.RVComplete, analyticsData);
		}

		public static void OnRVShow(string source)
		{
			AnalyticsRVData analyticsData = new AnalyticsRVData();
			analyticsData.Source = source;
			analyticsData.AdProv = AdProvider.AdMob;

			TrackEvent(AnalyticsEventType.RVShow, analyticsData);
		}

		public static void OnRVFailed(string source)
		{
			AnalyticsRVData analyticsData = new AnalyticsRVData();
			analyticsData.Source = source;
			analyticsData.AdProv = AdProvider.AdMob;

			TrackEvent(AnalyticsEventType.RVFailed, analyticsData);
		}

		public static void OnInterstitialDisplayed(string source)
		{
			AnalyticsIntData analyticsData = new AnalyticsIntData();
			analyticsData.Source = source;

			TrackEvent(AnalyticsEventType.InterstitialDisplayed, analyticsData);
		}

#if MODULE_IAP
		public static void OnIAPPurchased(AnalyticsIAPData analyticsIAPData)
		{
			analyticsIAPData.Source = AnalyticsEventType.IAPPurchased.ToString();
			TrackEvent(AnalyticsEventType.IAPPurchased, analyticsIAPData);
		}

		public static void OnIAPFailed(IAPItem item, PurchaseFailureReason failureReason)
		{
			AnalyticsIAPFailData analyticsData = new AnalyticsIAPFailData();
			analyticsData.Item = item;
			analyticsData.FailureReason = failureReason;
			analyticsData.Source = AnalyticsEventType.IAPFailed.ToString();

			TrackEvent(AnalyticsEventType.IAPFailed, analyticsData);
		}
#endif

		public static void TrackEvent(AnalyticsEventType analyticsEventType, IAnalyticsEventData eventData = null)
		{
#if DEBUG_LOGS
            Debug.Log(string.Format("[Analytics]: Event <b>\"{0}\"</b> fired.", analyticsEventType));

#endif

			EventFired?.Invoke(analyticsEventType, eventData);
		}

		private static void UnloadStatic()
		{
			EventFired = null;
		}

		public delegate void AnalyticsEventCallback(AnalyticsEventType type, IAnalyticsEventData analyticsEventData);
	}
}
