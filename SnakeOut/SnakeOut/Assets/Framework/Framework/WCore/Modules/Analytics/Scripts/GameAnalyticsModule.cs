using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;

#if MODULE_GAME_ANALYTICS
using GameAnalyticsSDK;
#endif

namespace Framework.Core
{
	public sealed class GameAnalyticsModule : BaseAnalyticsModule
	{
		public override Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>> GetHandlers()
		{
			return new Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>>
			{
				{ AnalyticsEventType.CurrencySource, OnCurrencySource },
				{ AnalyticsEventType.CurrencySink,   OnCurrencySink },
				{ AnalyticsEventType.IAPClicked,     OnIAPClicked },
				{ AnalyticsEventType.IAPPurchased,   OnIAPPurchased },
				{ AnalyticsEventType.IAPFailed,      OnIAPFailed },
				{ AnalyticsEventType.IAPFirstPurchase, OnIAPFirstPurchase },
				{ AnalyticsEventType.RVClicked,      OnRVClicked },
				{ AnalyticsEventType.RVShow,      OnRVShow },
				{ AnalyticsEventType.RVComplete,      OnRVComplete },
				{ AnalyticsEventType.RVFailed,      OnRVFailed },
				{ AnalyticsEventType.LevelComplete,    OnLevelComplete },
				{ AnalyticsEventType.LevelFailed,    OnLevelFailed },
				{ AnalyticsEventType.LevelReplay,    OnLevelReplay }
			};
		}

		public override void OnInitialized()
		{
			Debug.Log("[GameAnalytics] Initialized");
		}

		public GameAnalyticsModule()
		{
#if MODULE_GAME_ANALYTICS
			GameAnalytics.Initialize();
			Init(); // GA init is synchronous
#endif
		}

		// ---------------------------------------------------
		// CURRENCY
		// ---------------------------------------------------

		private void OnCurrencySource(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsCurrencyData currency)
			{
				foreach (var kv in currency.CurrenciesDelta)
				{
					if (kv.Value <= 0) continue;

					GameAnalytics.NewResourceEvent(
						GAResourceFlowType.Source,
						kv.Key.ToString(),
						kv.Value,
						currency.Source,
						"source"
					);
				}
			}
#endif
		}

		private void OnCurrencySink(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsCurrencyData currency)
			{
				foreach (var kv in currency.CurrenciesDelta)
				{
					if (kv.Value >= 0) continue;

					GameAnalytics.NewResourceEvent(
						GAResourceFlowType.Sink,
						kv.Key.ToString(),
						Math.Abs(kv.Value),
						currency.Source,
						"sink"
					);
				}
			}
#endif
		}

		// ---------------------------------------------------
		// IAP
		// ---------------------------------------------------

		private void OnIAPClicked(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsIAPData iap)
			{
				GameAnalytics.NewDesignEvent(
					$"iap_clicked:{iap.Item.ProductKeyType.ToString()}:{iap.LocalizedPrice}"
				);
			}
#endif
		}

		private void OnIAPPurchased(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsIAPData iap)
			{
				GameAnalytics.NewBusinessEvent(
					"USD",
					(int)iap.LocalizedPrice,
					iap.Item.ProductType.ToString(),
					iap.Item.ID,
					iap.Source
				);
			}
#endif
		}

		private void OnIAPFailed(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsIAPData iap)
			{
				GameAnalytics.NewDesignEvent(
					$"iap_failed:{iap.Item.ProductKeyType.ToString()}:{iap.LocalizedPrice}"
				);
			}
#endif
		}

		private void OnIAPFirstPurchase(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsIAPData iap)
			{
				GameAnalytics.NewDesignEvent(
					$"iap_first_purchase:{iap.Item.ID}"
				);
			}
#endif
		}

		// ---------------------------------------------------
		// ADS
		// ---------------------------------------------------
		private void OnRVClicked(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, adsData.AdProv.ToString(), adsData.Source);
			}
#endif
		}

		private void OnRVShow(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, adsData.AdProv.ToString(), adsData.Source);
			}
#endif
		}

		private void OnRVComplete(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.RewardedVideo, adsData.AdProv.ToString(), adsData.Source);
			}
#endif
		}

		private void OnRVFailed(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsRVData ads)
			{
				var adsData = data as AnalyticsRVData;
				GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, adsData.AdProv.ToString(), adsData.Source);
			}
#endif
		}

		// ---------------------------------------------------
		// LEVEL
		// ---------------------------------------------------
		private void OnLevelComplete(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsLevelData level)
			{
				var levelData = data as AnalyticsLevelData;
				GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelData.Status, levelData.LevelDelta);
			}
#endif
		}

		private void OnLevelFailed(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsLevelData level)
			{
				var levelData = data as AnalyticsLevelData;
				GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelData.Status, levelData.LevelDelta);
			}
#endif
		}

		private void OnLevelReplay(IAnalyticsEventData data)
		{
#if MODULE_GAME_ANALYTICS
			if (data is AnalyticsLevelData level)
			{
				var levelData = data as AnalyticsLevelData;
				GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelData.Status, levelData.LevelDelta);
			}
#endif
		}
	}
}
