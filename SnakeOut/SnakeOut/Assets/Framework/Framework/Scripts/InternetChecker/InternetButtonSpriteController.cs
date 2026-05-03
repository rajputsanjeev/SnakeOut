using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class InternetButtonSpriteController : MonoBehaviour
	{
		public static InternetButtonSpriteController Instance;

		[Header("Default Sprites")]
		public Sprite InternetOnSprite;
		public Sprite InternetWatchAdsSprite;
		public Sprite InternetOffSprite;

		private void Awake()
		{
			Instance = this;
			InternetCheckerService.OnInternetStateChanged += RefreshAll;
		}

		private void OnDestroy()
		{
			InternetCheckerService.OnInternetStateChanged -= RefreshAll;
		}

		private void RefreshAll(bool isConnected)
		{
			var internetButton = FindObjectsByType<InternetRequiredButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);

			foreach (var btn in internetButton)
			{
				btn.UpdateSprite(isConnected);
			}
		}
	}
}