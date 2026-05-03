using System.Linq;
using Framework;
using UnityEngine;
using UnityEngine.UI;
using Framework.Core;
using BlockGame;

namespace Framework
{
	public class UIStore : UIPage
	{
		private const int OVERLAY_SORTING_LAYER = 110;
		private const float DEFAULT_STORE_HEIGHT_OFFSET = 300;

		[SerializeField]
		public EnumUtilsContainer.Scenes CurrentScene = EnumUtilsContainer.Scenes.MAIN_SCENE;

		[BoxGroup("References", "References")]
		[SerializeField] RectTransform safeAreaTransform;
		[BoxGroup("References")]
		[SerializeField] CurrencyUIPanelSimple coinsUI;

		[BoxGroup("Scroll View", "Scroll View")]
		[SerializeField] VerticalLayoutGroup layout;
		[BoxGroup("Scroll View")]
		[SerializeField] RectTransform content;

		[BoxGroup("Buttons", "Buttons")]
		[SerializeField] Button closeButton;

		private IStoreElement[] offersElements;

		private void Awake()
		{
			offersElements = new IStoreElement[content.childCount];
			for (int i = 0; i < offersElements.Length; i++)
			{
				Transform child = content.GetChild(i);

				IStoreElement storeElement = child.GetComponent<IStoreElement>();
				if (storeElement == null)
				{
					storeElement = new DefaultStoreElement((RectTransform)child);
				}

				storeElement.Init();

				offersElements[i] = storeElement;
			}

			closeButton.onClick.AddListener(OnCloseButtonClicked);
		}

		public override void Init()
		{
			NotchSaveArea.RegisterRectTransform(safeAreaTransform);

			coinsUI.Init();
		}

		public override void PlayHideAnimation()
		{
			base.PlayHideAnimation();
			UIController.OnPageClosed(this);
		}

		public override void PlayShowAnimation()
		{
			base.PlayShowAnimation();
			float height = layout.padding.top + layout.padding.bottom + DEFAULT_STORE_HEIGHT_OFFSET;

			IStoreElement[] activeOffers = offersElements.Where(x => x.IsActive).ToArray();
			for (int i = 0; i < activeOffers.Length; i++)
			{
				IStoreElement offer = activeOffers[i];

				offer.KillTweenCases();
				offer.PlayAnimation(i);

				height += offer.Height;
			}

			height += activeOffers.Length * layout.spacing;

			closeButton.transform.localScale = Vector3.zero;
			closeButton.transform.DOScale(1.0f, 0.3f, 0.2f, unscaledTime: true).SetEasing(Ease.Type.BackOut);

			content.sizeDelta = new Vector2(0, height);
			content.anchoredPosition = Vector2.zero;

			UIController.OnPageOpened(this);
		}

		public void Hide()
		{
			foreach (IStoreElement offer in offersElements)
			{
				offer.KillTweenCases();
			}

			UIController.HidePage<UIStore>();
		}

		private void OnCloseButtonClicked()
		{
			if (CurrentScene == EnumUtilsContainer.Scenes.MAIN_SCENE)
			{
				MenuManager.HomeButtonAction?.Invoke();
			}
			else
			{
				UIController.HidePage<UIStore>();
			}
		}

		public void SpawnCurrencyCloud(RectTransform spawnRectTransform, CurrencyType currencyType, int amount, SimpleCallback completeCallback = null)
		{
			FloatingCloud.SpawnCurrency(currencyType.ToString(), spawnRectTransform, coinsUI.RectTransform, amount, null, completeCallback);
		}
	}
}
