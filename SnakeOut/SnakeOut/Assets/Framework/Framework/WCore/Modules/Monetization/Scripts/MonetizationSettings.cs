using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
	public class MonetizationSettings : ScriptableObject
	{
		[SerializeField, Hide] IAPSettings iapSettings;
		public IAPSettings IAPSettings => iapSettings;

		[SerializeField, Hide] AdsSettings adsSettings;
		public AdsSettings AdsSettings => adsSettings;

		[SerializeField, Hide] bool isModuleActive = true;
		public bool IsModuleActive => isModuleActive;

		[SerializeField] bool verboseLogging = false;
		public bool VerboseLogging => verboseLogging;

		[SerializeField] bool debugMode = false;
		public bool DebugMode => debugMode;

		[ShowIf("debugMode")]
		[SerializeField] List<string> testDevices;
		public List<string> TestDevices => testDevices;

		[Space]
		[SerializeField] string privacyLink = "";
		public string PrivacyLink => privacyLink;

		[SerializeField] string termsOfUseLink = "";
		public string TermsOfUseLink => termsOfUseLink;


		[SerializeField] bool isShowAdsOnLevelComplete;
		public bool IsShowOnLevelComplete => isShowAdsOnLevelComplete;

		[Tooltip("Level require to show ads")]
		[SerializeField] int levelRequired;
		public int LevelRequire => levelRequired;


		[SerializeField] string rateUsPlayStoreUrl = "";
		public string RateUsPlayStoreUrl => rateUsPlayStoreUrl;

		[SerializeField] string rateUsWebUrl = "";
		public string RateUsWebUrl => rateUsWebUrl;
	}
}
