using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Framework;
using Framework.Core;

namespace Framework
{
	public class UIIAPOffer : UIPage, IPopupWindow
	{
		[SerializeField] Image backgroundImage;
		[SerializeField] UIScaleAnimation panelScalable;
		[SerializeField] Button smallCloseButton;

		public bool IsOpened => canvas.enabled;

		private UIFadeAnimation backFade;

		private void OnEnable()
		{
			IAPManager.PurchaseCompleted += OnPurchaseCompleted;
		}

		private void OnDisable()
		{
			IAPManager.PurchaseCompleted -= OnPurchaseCompleted;
		}

		private void OnPurchaseCompleted(ProductKeyType productKeyType)
		{
			if (IsOpened)
				UIController.HidePage(this);
		}

		public override void Init()
		{
			backFade = new UIFadeAnimation(gameObject);

			backgroundImage.AddEvent(EventTriggerType.PointerClick, OnBackgroundClicked);

			smallCloseButton.onClick.AddListener(OnCloseButtonClicked);

			backFade.Hide(immediately: true);
			panelScalable.Hide(immediately: true);
		}

		public override void PlayShowAnimation()
		{
			base.PlayShowAnimation();

			backFade.Show(0.2f, onCompleted: () =>
			{
				panelScalable.Show(immediately: false, duration: 0.3f);
			});

			UIController.OnPageOpened(this);

			AdsManager.HideBanner();
		}

		public override void PlayHideAnimation()
		{
			base.PlayHideAnimation();

			backFade.Hide(0.2f);
			panelScalable.Hide(immediately: false, duration: 0.4f, onCompleted: () =>
			{
				UIController.OnPageClosed(this);
			});

			AdsManager.ShowBanner();
		}

		private void OnCloseButtonClicked()
		{
			AudioController.PlaySound(AudioController.AudioClips.closeButtonSound);

			UIController.HidePage(this);
		}

		private void OnBackgroundClicked(PointerEventData data)
		{
			UIController.HidePage(this);
		}

		[Button]
		private void Show()
		{
			UIController.ShowPage(this);
		}

		[Button]
		private void Hide()
		{
			UIController.HidePage(this);
		}
	}
}
