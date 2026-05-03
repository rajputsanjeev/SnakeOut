//using System;
//using System.Collections.Generic;
//using Facebook.Unity;
//using Framework.Core;
//using UnityEngine;

//namespace Framework
//{
//	public sealed class FacebookAnalyticsModule : BaseAnalyticsModule
//	{
//		public override void OnInitialized()
//		{

//		}

//		public FacebookAnalyticsModule()
//		{
//			if (!FB.IsInitialized)
//			{
//				FB.Init(OnInitComplete);
//			}
//			else
//			{
//				FB.ActivateApp();
//			}
//		}

//		private void OnInitComplete()
//		{
//			if (FB.IsInitialized)
//			{
//				FB.ActivateApp();
//				FB.LogAppEvent("AppOpen");
//				Debug.Log("[FacebookAnalytics] Initialized");
//			}
//			else
//			{
//				Debug.LogError("[FacebookAnalytics] Init Failed");
//			}
//		}

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

//		private void OnCurrencySource(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsCurrencyData currency)
//			{
//				var parameters = new Dictionary<string, object>();

//				foreach (var kvp in currency.CurrenciesDelta)
//				{
//					parameters[kvp.Key.ToString()] = kvp.Value;
//				}
//				FB.LogAppEvent(currency.Source, null, parameters);
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
//					{ "times_purchased", iap.Item.TimesPurchased },
//					{ "price", iap.LocalizedPrice }
//				};

//				// Important: Log real purchase
//				FB.LogPurchase(
//					(float)iap.LocalizedPrice,
//					"USD", // Change dynamically if needed
//					parameters
//				);
//			}
//		}

//		private void OnRVClicked(IAnalyticsEventData data)
//		{
//			if (data is AnalyticsRVData ads)
//			{
//				FB.LogAppEvent("rv_clicked", null,
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
//				FB.LogAppEvent("rv_show", null,
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
//				FB.LogAppEvent("rv_complete", null,
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
//				FB.LogAppEvent("rv_failed", null,
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
//				FB.LogAppEvent(level.Status, null, level.LevelDelta);
//			}
//		}
//	}
//}