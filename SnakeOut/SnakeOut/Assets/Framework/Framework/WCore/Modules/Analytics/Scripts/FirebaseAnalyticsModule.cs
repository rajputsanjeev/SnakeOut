using System;
using System.Collections.Generic;
using UnityEngine;

#if MODULE_FIREBASE_ANALYTICS
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
#endif

namespace Framework.Core
{
	public sealed class FirebaseAnalyticsModule : BaseAnalyticsModule
	{
		private Firebase.FirebaseApp app;

		public override Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>> GetHandlers()
		{
			return new Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>>
			{
				{ AnalyticsEventType.CurrencySource, OnCurrencySource },
				{ AnalyticsEventType.CurrencySink, OnCurrencySource },
				{ AnalyticsEventType.IAPClicked, OnIAP },
				{ AnalyticsEventType.IAPPurchased, OnIAP },
				{ AnalyticsEventType.IAPFailed, OnIAP },
				{ AnalyticsEventType.IAPFirstPurchase, OnIAP },
				{ AnalyticsEventType.RVClicked, OnRVClicked },
				{ AnalyticsEventType.RVShow,      OnRVShow },
				{ AnalyticsEventType.RVComplete,      OnRVComplete },
				{ AnalyticsEventType.InterstitialDisplayed,      OnInterstitialDisplayed },
				{ AnalyticsEventType.RVFailed,      OnRVFailed },
				{ AnalyticsEventType.LevelComplete, OnLevelStatus },
				{ AnalyticsEventType.LevelFailed, OnLevelStatus },
				{ AnalyticsEventType.LevelReplay, OnLevelStatus }
			};
		}

		private void OnInterstitialDisplayed(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				FirebaseAnalytics.LogEvent(AnalyticsEventType.InterstitialDisplayed.ToString(), "InterstitiaAdVideo", adsData.Source);
			}
#endif
		}

		public override void OnInitialized()
		{
			Debug.Log("[FirebaseAnalytics] Initialized");
		}

		public FirebaseAnalyticsModule()
		{
#if MODULE_FIREBASE_ANALYTICS
			Firebase.FirebaseApp.CheckAndFixDependenciesAsync()
				.ContinueWithOnMainThread(task =>
				{
					if (task.Result == Firebase.DependencyStatus.Available)
					{
						app = Firebase.FirebaseApp.DefaultInstance;
						Init();
					}
					else
					{
						Debug.LogError($"[FirebaseAnalytics] Dependency error: {task.Result}");
					}
				});
#endif
		}

		private void OnCurrencySource(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsCurrencyData currency)
			{
				var currencyData = data as AnalyticsCurrencyData;
				var parameters = FirebaseParamUtils.CurrencyDeltaToParameters(currencyData);
				FirebaseAnalytics.LogEvent(currencyData.Source, parameters);
			}
#endif
		}

		private void OnIAP(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsIAPData currency)
			{
				var iapData = data as AnalyticsIAPData;

				Parameter[] parameters = {
						 new Parameter("product_id", iapData.Item.ID),
						 new Parameter("product_key_type", iapData.Item.ProductKeyType.ToString()),
						 new Parameter("product_type", iapData.Item.ProductType.ToString()),
						 new Parameter("product_time", iapData.Item.TimesPurchased.ToString()),
						 new Parameter("price", iapData.LocalizedPrice) };

				FirebaseAnalytics.LogEvent(iapData.Source, parameters);
			}
#endif
		}

		private void OnRVClicked(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsRVData currency)
			{
				var adsData = data as AnalyticsRVData;
				FirebaseAnalytics.LogEvent(adsData.Source);
			}
#endif
		}

		private void OnRVShow(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				FirebaseAnalytics.LogEvent(AnalyticsEventType.RVShow.ToString(), "RewardedVideo", adsData.Source);
			}
#endif
		}

		private void OnRVComplete(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				FirebaseAnalytics.LogEvent(AnalyticsEventType.RVComplete.ToString(), "RewardedVideo", adsData.Source);
			}
#endif
		}

		private void OnRVFailed(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				FirebaseAnalytics.LogEvent(AnalyticsEventType.RVFailed.ToString(), "RewardedVideo", adsData.Source);
			}
#endif
		}

		private void OnLevelStatus(IAnalyticsEventData data)
		{
#if MODULE_FIREBASE_ANALYTICS
			if (data is AnalyticsLevelData currency)
			{
				var levelData = data as AnalyticsLevelData;
				var parameters = AnalyticsLevelFirebaseConverter.ToFirebaseParams(levelData);

				FirebaseAnalytics.LogEvent(
					levelData.Status,
					parameters
				);
			}
#endif
		}
	}
}
