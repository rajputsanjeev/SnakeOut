#define DEBUG_LOGS

namespace Framework.Core
{
	public enum AnalyticsEventType
	{
		//LevelStatus level_start,level_complete,level_failed,Level_replay
		LevelStatus,
		LevelComplete,
		LevelFailed,
		LevelReplay,

		// Core Events
		CurrencySource = 10,
		CurrencySink = 11,

		IAPClicked = 20,
		IAPPurchased = 21,
		IAPFailed = 22,
		IAPFirstPurchase = 23,

		AdFreePeriodExpired = 25,

		RVClicked = 26,
		InterstitialDisplayed = 27,
		RVShow = 28,
		RVComplete = 29,
		RVFailed = 30,
	}
}
