using System.Collections.Generic;
using System.Linq;
using Base.UI.Manager;
using Frameork;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Framework.Core.CurrencyRewardPreviewSettings;

namespace Framework.Core
{
	public class UIRewardsConfirmation : UIPage, IPopupWindow
	{
		[SerializeField] Image backgroundImage;
		[SerializeField] RectTransform rewardsContainerTransform;
		[SerializeField] GridLayoutGroup rewardsGridLayoutGroup;
		[SerializeField] GameObject rewardUIPrefab;
		[SerializeField] TMP_Text tapToClaimText;

		[Space]

		[Space]
		[SerializeField] Sprite defaultRewardSprite;

		[Space]
		[SerializeField] AudioClip displayAudioClip;

		public bool IsOpened => canvas.enabled;

		private SimpleCallback closeCallback;
		private RewardUIData[] rewards;
		private List<RewardItem> reardItems = new List<RewardItem>();

		private TweenCase fadeTweenCase;

		private float rewardItemScale = 1f;

		public override void Init()
		{
			backgroundImage.AddEvent(UnityEngine.EventSystems.EventTriggerType.PointerUp, (data) => OnCloseButtonClicked());
		}

		public override void PlayShowAnimation()
		{
			float appearanceDelay = 0.1f;

			if (!rewards.IsNullOrEmpty())
			{
				UpdateDynamicSize();

				for (int i = 0; i < rewards.Length; i++)
				{
					RewardUIData reward = rewards[i];

					UIRewardPreviewBehavior behavior = reward.Behavior;
					behavior.transform.localScale = Vector3.one * rewardItemScale;

					behavior.CanvasGroup.alpha = 0.0f;

					reward.FadeTweenCase = behavior.CanvasGroup.DOFade(1.0f, 0.45f, appearanceDelay * (i + 1), unscaledTime: true);

					reward.ScaleTweenCase = behavior.Image.DOScale(1.1f, 0.15f, appearanceDelay * (i + 1), unscaledTime: true).SetEasing(Ease.Type.SineOut).OnComplete(() =>
					{
						reward.ScaleTweenCase = behavior.Image.DOScale(0.95f, 0.2f, unscaledTime: true).OnComplete(() =>
						{
							reward.ScaleTweenCase = behavior.Image.DOScale(1f, 0.1f, unscaledTime: true).SetEasing(Ease.Type.SineOut);
						});
					});
				}
			}

			float tapTextDelay = rewards.IsNullOrEmpty() ? 0f : appearanceDelay * rewards.Length + 0.5f;

			tapToClaimText.color = tapToClaimText.color.SetAlpha(0f);
			fadeTweenCase = tapToClaimText.DOFade(1f, 0.2f, tapTextDelay, unscaledTime: true).SetEasing(Ease.Type.CubicInOut);

			if (displayAudioClip != null)
				AudioController.PlaySound(displayAudioClip);

			UIController.OnPageOpened(this);
		}

		public void UpdateDynamicSize()
		{
			if (rewards.Length <= 6)
			{
				rewardsGridLayoutGroup.cellSize = new Vector2(300f, 300f);
				rewardItemScale = 1f;
			}
			else
			{
				rewardsGridLayoutGroup.cellSize = new Vector2(200f, 200f);
				rewardItemScale = 0.8f;
			}
		}

		public override void PlayHideAnimation()
		{
			float animationDuration = 0f;

			if (!rewards.IsNullOrEmpty())
			{
				animationDuration = 0.3f;

				for (int i = 0; i < rewards.Length; i++)
				{
					RewardUIData reward = rewards[i];

					UIRewardPreviewBehavior behavior = reward.Behavior;
					behavior.CanvasGroup.alpha = 1f;

					reward.FadeTweenCase = behavior.CanvasGroup.DOFade(0f, 0.28f, unscaledTime: true);

					reward.ScaleTweenCase = behavior.Image.DOScaleY(1.1f, 0.05f, 0.1f, unscaledTime: true).SetEasing(Ease.Type.SineIn).OnComplete(() =>
					{
						reward.ScaleTweenCase = behavior.Image.DOScaleY(0f, 0.15f, unscaledTime: true).SetEasing(Ease.Type.SineOut);
					});
				}
			}

			Tween.DelayedCall(animationDuration, () =>
			{
				closeCallback?.Invoke();
				UIController.OnPageClosed(this);
			}, unscaledTime: true);
		}

		private void OnDestroy()
		{
			fadeTweenCase.KillActive();

			if (!rewards.IsNullOrEmpty())
			{
				foreach (RewardUIData reward in rewards)
				{
					reward?.Destroy();
				}
			}
		}

		public static bool Display(List<IRewardPreview> previews, SimpleCallback closeCallback = null)
		{
			if (previews.IsNullOrEmpty())
				return false;

			UIRewardsConfirmation rewardsConfirmation = UIController.GetPage<UIRewardsConfirmation>();
			rewardsConfirmation.rewards = new RewardUIData[0];

			if (rewardsConfirmation != null)
			{
				rewardsConfirmation.closeCallback = closeCallback;

				RewardUIData[] oldRewards = rewardsConfirmation.rewards;
				if (!oldRewards.IsNullOrEmpty())
				{
					foreach (RewardUIData reward in oldRewards)
					{
						reward?.Destroy();
					}
				}
				rewardsConfirmation.rewardsContainerTransform.DestroyChild();

				previews = previews.OrderBy(x => x.SortingOrder).ToList();

				RewardUIData[] newRewards = new RewardUIData[previews.Count];
				for (int i = 0; i < newRewards.Length; i++)
				{
					IRewardPreview preview = previews[i];

					GameObject uiPrefab = preview.GetCustomUIPrefab();
					if (uiPrefab == null)
						uiPrefab = rewardsConfirmation.rewardUIPrefab;

					GameObject uiObject = Instantiate(uiPrefab, rewardsConfirmation.rewardsContainerTransform);
					uiObject.transform.ResetLocal();

					UIRewardPreviewBehavior previewBehavior = uiObject.GetComponent<UIRewardPreviewBehavior>();
					previewBehavior.Init(preview, rewardsConfirmation.defaultRewardSprite);

					newRewards[i] = new RewardUIData(previewBehavior, preview);
				}

				rewardsConfirmation.rewards = newRewards;

				UIController.ShowPage<UIRewardsConfirmation>();
				AudioController.PlaySound(AudioController.AudioClips.ClaimSound);

				RewardBaseHandle.FlowRewards.Clear();
				foreach (var item in previews)
				{
					RewardBaseHandle.FlowRewards.Add(new FlowReward(item.Type));
				}

				return true;
			}

			return false;
		}

		public static bool Display(List<RewardItem> reardItem, SimpleCallback closeCallback = null)
		{
			if (reardItem.IsNullOrEmpty())
				return false;

			UIRewardsConfirmation rewardsConfirmation = UIController.GetPage<UIRewardsConfirmation>();
			rewardsConfirmation.rewards = new RewardUIData[0];

			if (rewardsConfirmation != null)
			{
				rewardsConfirmation.closeCallback = closeCallback;
				rewardsConfirmation.reardItems.Clear();
				rewardsConfirmation.rewardsContainerTransform.DestroyChild();

				List<RewardItem> newRewards = new List<RewardItem>();
				for (int i = 0; i < reardItem.Count; i++)
				{
					RewardItem preview = reardItem[i];

					GameObject uiPrefab = rewardsConfirmation.rewardUIPrefab;

					GameObject uiObject = Instantiate(uiPrefab, rewardsConfirmation.rewardsContainerTransform);
					uiObject.transform.ResetLocal();

					UIRewardPreviewBehavior previewBehavior = uiObject.GetComponent<UIRewardPreviewBehavior>();
					previewBehavior.Init(preview, reardItem[i].icon);
					newRewards.Add(preview);
				}

				rewardsConfirmation.reardItems = newRewards;

				UIController.ShowPage<UIRewardsConfirmation>();
				AudioController.PlaySound(AudioController.AudioClips.ClaimSound);
				return true;
			}

			foreach (var item in reardItem)
			{
				RewardBaseHandle.FlowRewards.Add(new FlowReward(item.type));
			}

			return false;
		}

		private void OnCloseButtonClicked()
		{
			AudioController.PlaySound(AudioController.AudioClips.buttonSound);

			UIController.HidePage(this);

			UIPanelManager.HasRewardCollection = true;
			if (UIPanelManager.Instance.m_LastActivePanel != null) UIPanelManager.Instance.m_LastActivePanel.Hide();
			UIController.HidePage<UIStore>();
			UIController.HidePage<UINoAdsPopUp>();

			RewardFlowController.Instance.PlayPurchaseAnimation(RewardBaseHandle.FlowRewards, () =>
			{
				UIPanelManager.HasRewardCollection = false;
				UIPanelManager.Instance.m_LastActivePanel?.Show();
				RewardBaseHandle.FlowRewards.Clear();
			});
		}

		public class RewardUIData
		{
			public readonly UIRewardPreviewBehavior Behavior;
			public readonly IRewardPreview Preview;

			public TweenCase FadeTweenCase;
			public TweenCase ScaleTweenCase;

			public RewardUIData(UIRewardPreviewBehavior behavior, IRewardPreview preview)
			{
				Behavior = behavior;
				Preview = preview;
			}

			public void Destroy()
			{
				if (Behavior != null && Behavior.gameObject != null)
					GameObject.Destroy(Behavior.gameObject);

				FadeTweenCase.KillActive();
				ScaleTweenCase.KillActive();
			}
		}
	}
}
