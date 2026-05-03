using Framework.Core;
using UnityEngine;

namespace Framework
{
	public class RateUs : MonoBehaviour
	{
		// Replace with your game’s Play Store package name
		private string playStoreURL = "";
		private string playStoreWebURL = "";

		public void OnRateUsButtonClick()
		{
			playStoreURL = Monetization.Settings.RateUsPlayStoreUrl;
			playStoreWebURL = Monetization.Settings.RateUsWebUrl;

			try
			{
				// Try opening Play Store app
				Application.OpenURL(playStoreURL);
			}
			catch
			{
				// If Play Store app not available, open in browser
				Application.OpenURL(playStoreWebURL);
			}
		}
	}
}