//using System;
//using System.Collections.Generic;
//using Framework.Core;
//using SDK;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace Framework
//{
//	public sealed class TikTokAnalyticsModule : BaseAnalyticsModule
//	{
//		public override Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>> GetHandlers()
//		{
//			return new Dictionary<AnalyticsEventType, Action<IAnalyticsEventData>>
//			{
//				{ AnalyticsEventType.CurrencySource, OnCurrencySource },
//				{ AnalyticsEventType.CurrencySink, OnCurrencySource },
//				{ AnalyticsEventType.IAPClicked, OnIAP },
//				{ AnalyticsEventType.IAPPurchased, OnIAP },
//				{ AnalyticsEventType.IAPFailed, OnIAP },
//				{ AnalyticsEventType.IAPFirstPurchase, OnIAP },
//				{ AnalyticsEventType.RVClicked, OnRVClicked },
//				{ AnalyticsEventType.RVShow, OnRVShow },
//				{ AnalyticsEventType.RVComplete, OnRVComplete },
//				{ AnalyticsEventType.RVFailed, OnRVFailed },
//				{ AnalyticsEventType.LevelComplete, OnLevelStatus },
//				{ AnalyticsEventType.LevelFailed, OnLevelStatus },
//				{ AnalyticsEventType.LevelReplay, OnLevelStatus }
//			};
//		}

//		public override void OnInitialized()
//		{
//#if MODULE_TIKTOK
//			// App id              // IOS_APPID   APP_ id                      Android_APP_ID
//			TikTokBusinessSDK.InitializeSdk(new TikTokConfig("1324456775", "7614108812693323793", "com.NextGame.BlockFlowColourJam", "7614108812693323793"));

//			// Replace with actual TikTok SDK call
//			TikTokBusinessSDK.TrackTTEvent(new TikTokBaseEvent("app-open", new Dictionary<string, object>()
//			{

//			}, "1"));

//			Debug.Log("[TikTokAnalytics] Initialized");
//#endif
//		}

//		private void SendEvent(string eventName, Dictionary<string, object> parameters = null)
//		{
//			Debug.Log("[TikTokAnalytics] Event: " + eventName);

//#if MODULE_TIKTOK
//			// Replace with actual TikTok SDK call
//			TikTokBusinessSDK.TrackTTEvent(new TikTokBaseEvent(eventName, parameters, "1"));
//#endif
//		}

//		private void OnCurrencySource(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsCurrencyData currency)
//			{
//				var parameters = new Dictionary<string, object>();

//				foreach (var kvp in currency.CurrenciesDelta)
//				{
//					parameters[kvp.Key.ToString()] = kvp.Value;
//				}

//				SendEvent(currency.Source, parameters);
//			}
//		}

//		private void OnIAP(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsIAPData iap)
//			{
//				var parameters = new Dictionary<string, object>
//				{
//					{ "product_id", iap.Item.ID },
//					{ "product_type", iap.Item.ProductType.ToString() },
//					{ "price", iap.LocalizedPrice }
//				};

//				SendEvent("purchase", parameters);
//			}
//		}

//		private void OnRVClicked(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsRVData ads)
//			{
//				SendEvent("rv_clicked",
//					new Dictionary<string, object>
//					{
//						{ "placement", ads.Source }
//					});
//			}
//		}

//		private void OnRVShow(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsRVData ads)
//			{
//				SendEvent("rv_show",
//					new Dictionary<string, object>
//					{
//						{ "placement", ads.Source }
//					});
//			}
//		}

//		private void OnRVComplete(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsRVData ads)
//			{
//				SendEvent("rv_complete",
//					new Dictionary<string, object>
//					{
//						{ "placement", ads.Source }
//					});
//			}
//		}

//		private void OnRVFailed(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsRVData ads)
//			{
//				SendEvent("rv_failed",
//					new Dictionary<string, object>
//					{
//						{ "placement", ads.Source }
//					});
//			}
//		}

//		private void OnLevelStatus(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsLevelData level)
//			{
//				Dictionary<string, object> parameters = new Dictionary<string, object>();

//				parameters.Add("level", 3);
//				parameters.Add("score", 1000);

//#if MODULE_TIKTOK
//				TikTokBusinessSDK.TrackTTEvent(
//					new TikTokBaseEvent("LevelComplete", parameters, "TEST62326")
//				);
//#endif
//			}
//		}
//	}
//}