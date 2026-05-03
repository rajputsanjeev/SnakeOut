using System.Collections;
using Framework.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Framework
{
	[RequireComponent(typeof(Button))]
	public class InternetRequiredButton : MonoBehaviour
	{
		[Header("Is this button watch ad button or purchase item button")]
		public bool IsInternetButton = true;
		public float DelayCall = 2f;

		[Header("Watch Ads Button")]
		public bool IsAdsButton;

		[Header("Optional Custom Sprites")]
		public bool UseCustomSprites;
		public Sprite InternetOnSprite;
		public Sprite InternetOffSprite;

		[Header("Toast Message")]
		public string NoInternetMessage = "No Internet Connection";

		private Button button;
		private Image buttonImage;
		private GameObject blocker;

		private void Awake()
		{
			if (IsInternetButton)
			{
				button = GetComponent<Button>();
				buttonImage = GetComponent<Image>();

				CreateBlocker();
				StartCoroutine(CheckInternet());

				InternetCheckerService.OnInternetStateChanged += UpdateState;
			}
		}

		private IEnumerator CheckInternet()
		{
			yield return new WaitForSeconds(DelayCall);
			UpdateState(InternetCheckerService.IsInternetAvailable);
		}

		private void OnDestroy()
		{
			InternetCheckerService.OnInternetStateChanged -= UpdateState;
		}

		// ---------------- BLOCKER ----------------
		private void CreateBlocker()
		{
			blocker = new GameObject("InternetBlocker");
			blocker.transform.SetParent(transform, false);

			RectTransform rt = blocker.AddComponent<RectTransform>();
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = rt.offsetMax = Vector2.zero;

			Image img = blocker.AddComponent<Image>();
			img.color = new Color(0, 0, 0, 0); // invisible

			Button blockBtn = blocker.AddComponent<Button>();
			blockBtn.onClick.AddListener(OnBlockedClick);

			blocker.SetActive(false);
		}

		private void OnBlockedClick()
		{
			ToastMessage.Instance.Show(NoInternetMessage);

#if MODULE_HAPTIC
			Haptic.Play(Haptic.HAPTIC_HARD);
#endif
			AudioController.PlaySound(AudioController.AudioClips.buttonSound);
		}

		// ---------------- STATE ----------------

		private void UpdateState(bool hasInternet)
		{
			blocker.SetActive(!hasInternet);
			UpdateSprite(hasInternet);
		}

		public void UpdateSprite(bool hasInternet)
		{
			if (buttonImage == null) return;

			if (UseCustomSprites)
			{
				buttonImage.sprite =
					hasInternet ? InternetOnSprite : InternetOffSprite;
			}
			else if (InternetButtonSpriteController.Instance != null)
			{
				//If no intenet then change to grey. If internet enable then check if it is ad button then change the sprite according with ad sprite.If its button sprite
				//change at runtime then get its orginal sprite otherwise change with InternetOnSprite sprite.

				if (button.interactable)
				{
					buttonImage.sprite =
						hasInternet
						? IsAdsButton ? InternetButtonSpriteController.Instance.InternetWatchAdsSprite :
						  InternetButtonSpriteController.Instance.InternetOnSprite
						: InternetButtonSpriteController.Instance.InternetOffSprite;
				}
			}
		}
	}
}
