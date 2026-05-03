using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Frameork;
using Framework.Core;

#if MODULE_GOOGLE_PLAY_REVIEW_ANDROID
using Google.Play.Review;
#endif

namespace Framework
{
	[StaticUnload]
	[Define("MODULE_GOOGLE_PLAY_REVIEW_ANDROID", "Google.Play.Review.ReviewManager", "Google.Play.Review.dll")]
	public class InAppReview : MonoBehaviour
	{
		// Only exists when plugin is in project
#if MODULE_GOOGLE_PLAY_REVIEW_ANDROID
		private ReviewManager _reviewManager;
		private PlayReviewInfo _playReviewInfo;
#endif

		public bool IsOpenLink;
		public bool UseNativeAndroidReviewPopUp = true;
		public bool UseNativeIosReviewPopUp = true;

		public string LinkToTheGame = "";

		[System.Serializable]
		public enum Options
		{
			UseAppOpenCounts,
			UseSessionCounts,
		}
		public Options LaunchChecks;

		public int LaunchCountsBeforeShowingPopup = 5;
		public int OpenAfterLevel;
		public GameObject CustomReviewPopup;

		public Button CustomRateUIButton;
		public Button CustomLaterUIButton;

		public int StarRatingsByDefault = 5;
		public Color StarColor = Color.yellow;
		public GameObject[] CustomStars;
		public Sprite StarSprite;

		private int openCount = 0;
		private Color[] originalStarColors;
		private Sprite[] originalStarSprites;
		private bool[] starActiveStates;

		private void Awake()
		{
			if (CustomStars.Length >= 1)
			{
				originalStarColors = new Color[CustomStars.Length];
				originalStarSprites = new Sprite[CustomStars.Length];
				starActiveStates = new bool[CustomStars.Length];

				for (int i = 0; i < CustomStars.Length; i++)
				{
					Image img = CustomStars[i].GetComponent<Image>();
					originalStarColors[i] = img.color;
					originalStarSprites[i] = img.sprite;
					starActiveStates[i] = false;
				}

				OnStarClick(StarRatingsByDefault - 1);
			}
		}

		void Start()
		{
			if (PlayerPrefs.GetInt("DoNotShowRatePopUp") == 0)
			{
				HandleLaunchCountLogic();

				if (openCount >= LaunchCountsBeforeShowingPopup)
				{
					HandleAndroidReview();
					HandleIOSReview();
				}

				if (CustomRateUIButton != null)
					CustomRateUIButton.onClick.AddListener(OpenReviewLink);

				if (CustomLaterUIButton != null)
					CustomLaterUIButton.onClick.AddListener(CloseReviewLink);

				for (int i = 0; i < CustomStars.Length; i++)
				{
					int index = i;
					CustomStars[i].GetComponent<Button>().onClick.AddListener(() => OnStarClick(index));
				}
			}
		}

		private void HandleLaunchCountLogic()
		{
			if (LaunchChecks == Options.UseAppOpenCounts)
			{
				if (PlayerPrefs.GetInt("IsAppOpened") == 0)
				{
					openCount = PlayerPrefs.GetInt("OpenCount", 0);
					openCount++;
					PlayerPrefs.SetInt("OpenCount", openCount);
					PlayerPrefs.Save();
					PlayerPrefs.SetInt("IsAppOpened", 1);
				}
				else
				{
					openCount = PlayerPrefs.GetInt("OpenCount", 0);
					openCount++;
					PlayerPrefs.SetInt("OpenCount", openCount);
					PlayerPrefs.Save();
				}
			}
			else if (OpenAfterLevel <= GetCurrentLevelAbstract.Instance.GetLevel())
			{
				if (MyEventArgs.GameControllerEvents.LevelCompleteCount > 3)
				{
					ShowReviewPopup();
				}
			}
		}

		private void HandleAndroidReview()
		{
#if MODULE_GOOGLE_PLAY_REVIEW_ANDROID
			if (UseNativeAndroidReviewPopUp)
				StartCoroutine(RequestReviewForAndroid());
			else
				ShowReviewPopup();
#else
			// Plugin missing → fallback
			ShowReviewPopup();
#endif
		}

		private void HandleIOSReview()
		{
#if MODULE_GOOGLE_PLAY_REVIEW_IOS
            if (UseNativeIosReviewPopUp)
            {
                PlayerPrefs.SetInt("OpenCount", 0);
                PlayerPrefs.SetInt("IsAppOpened", 0);
                UnityEngine.iOS.Device.RequestStoreReview();
            }
            else
            {
                ShowReviewPopup();
            }
#endif
		}

		// ANDROID GOOGLE PLAY REVIEW — ONLY COMPILES IF PLUGIN EXISTS
#if MODULE_GOOGLE_PLAY_REVIEW_ANDROID
		IEnumerator RequestReviewForAndroid()
		{
			_reviewManager = new ReviewManager();

			var requestFlowOperation = _reviewManager.RequestReviewFlow();
			yield return requestFlowOperation;

			if (requestFlowOperation.Error != ReviewErrorCode.NoError)
			{
				ShowReviewPopup();
				yield break;
			}

			_playReviewInfo = requestFlowOperation.GetResult();

			PlayerPrefs.SetInt("OpenCount", 0);
			PlayerPrefs.SetInt("IsAppOpened", 0);

			var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
			yield return launchFlowOperation;

			_playReviewInfo = null;

			if (launchFlowOperation.Error != ReviewErrorCode.NoError)
			{
				ShowReviewPopup();
				yield break;
			}
		}
#else
		IEnumerator RequestReviewForAndroid()
		{
			ShowReviewPopup();
			yield break;
		}
#endif

		void ShowReviewPopup()
		{
			if (CustomReviewPopup != null)
				CustomReviewPopup.SetActive(true);
		}

		void OpenReviewLink()
		{
			CustomReviewPopup.gameObject.SetActive(false);

			if (IsOpenLink)
			{
				Application.OpenURL(LinkToTheGame);
				CloseReviewLink();
				PlayerPrefs.SetInt("DoNotShowRatePopUp", 1);
			}
			else
			{
				HandleAndroidReview();
				HandleIOSReview();
			}
		}

		void CloseReviewLink()
		{
			if (CustomReviewPopup != null)
				CustomReviewPopup.SetActive(false);

			PlayerPrefs.SetInt("OpenCount", 0);
			PlayerPrefs.SetInt("IsAppOpened", 0);
			PlayerPrefs.SetInt("DoNotShowRatePopUp", 1);
		}

		void OnStarClick(int starIndex)
		{
			for (int i = 0; i <= starIndex; i++)
			{
				Image img = CustomStars[i].GetComponent<Image>();
				img.color = StarColor;

				if (StarSprite != null)
					img.sprite = StarSprite;

				img.enabled = true;
				starActiveStates[i] = true;
			}

			for (int i = starIndex + 1; i < CustomStars.Length; i++)
			{
				Image img = CustomStars[i].GetComponent<Image>();
				img.color = originalStarColors[i];
				img.sprite = originalStarSprites[i];
			}
		}
	}
}