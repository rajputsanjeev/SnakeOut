using System;
using System.Linq;
using BaseView;
using Framework.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI.Controllers
{
	public class UIPrivilegeController : Behaviour<UIPrivilegeView>, IPointerDownHandler
	{
		public delegate void ClaimReward(int currentSlot);
		public static ClaimReward ClaimFreeRewardEvent;
		public static ClaimReward ClaimPaidRewardEvent;
		public static ClaimReward ClaimButtonActive;

		[Header("UI")]
		public GameObject InfoScreen;
		public RectTransform listParent;
		public GameObject stepPrefab;
		public ScrollRect scrollRect;
		public GameObject ProgressBar;

		[Header("Sprites")]
		public Sprite LockSprite;
		public Sprite UnLockSprite;

		[Header("Settings")]
		public bool IsGlobalClaimButton;

		private UIPrivilegeView m_View;
		private PrivilegeManager _manager;
		private ProgressBarView _progressBarView;
		private BaseTimerHandle _baseTimeHandle;

		private PrivilegeStepView[] stepViews;
		private const int TOTAL_STEPS = 28;

		private int SelectedIndex => _manager.GetCurrentStepIndex();

		#region Init
		protected override void Init()
		{
			base.Init();
			m_View = (UIPrivilegeView)Prefab;
		}

		private void CacheReferences()
		{
			GetBaseView();
			m_View = (UIPrivilegeView)Prefab;

			if (_manager != null) return;

			_manager = GetComponent<PrivilegeManager>();
			_progressBarView = GetComponent<ProgressBarView>();
			_baseTimeHandle = GetComponent<BaseTimerHandle>();
		}

		public override void Subscribe()
		{
			base.Subscribe();
			CacheReferences();

			m_View.PurchasePrivilegeScreen.onClick.AddListener(OpenPurchasePrivilegeScreen);
			m_View.CollectButton.onClick.AddListener(OnCollectAllButtonPressed);
			m_View.AddKey.onClick.AddListener(() => _manager.OnLevelCompletedGiveKey(5));

			BuildListIfNeeded();

			_manager.OnStateChanged += StartTimer;
		}

		public override bool IsShow()
		{
			return (_manager.MainMenuButton.IsLevelReach() && IsOfferScreen);
		}
		#endregion

		#region Unity Lifecycle
		public override void OnInit()
		{
			base.OnInit();
			CacheReferences();

			if (_manager.MainMenuButton.IsLevelReach() && !_manager.IsUnlocked())
			{
				InfoScreen.SetActive(true);
				OpenCurrentPanel();
				_manager.SetUnlocked();
			}
		}

		private void OnEnable()
		{
			UIPrivilegePurchaseScreenController.OnPrivilegePurchase += OnBuyPaidPressed;
			ClaimFreeRewardEvent += OnCollectFreeButtonPressed;
			ClaimPaidRewardEvent += OnCollectPaidButtonPressed;
			ClaimButtonActive += OnClaimButtonActive;
		}

		private void OnDisable()
		{
			_manager.OnStateChanged -= Refresh;
			UIPrivilegePurchaseScreenController.OnPrivilegePurchase -= OnBuyPaidPressed;
			ClaimFreeRewardEvent -= OnCollectFreeButtonPressed;
			ClaimPaidRewardEvent -= OnCollectPaidButtonPressed;
			ClaimButtonActive -= OnClaimButtonActive;
		}
		#endregion

		#region Timer
		private void StartTimer()
		{
			Refresh();

			TimerManager.CreateTimer(
			_manager.GetTimeRemainingInCycle(),
			OnTimerCompleted,
			x =>
			{
				if (IsShowDebug) DebugExtension.Log($"[Privilege] {x}", "green");

				_baseTimeHandle.UpdateTime(
					_baseTimeHandle.FormatSpan(x)
				);
			},
			"Privilege"
		);
		}

		private void OnTimerCompleted()
		{
			if (IsShowDebug) DebugExtension.Log("[Privilege] Completed → refreshing state", "green");
			if (_currentComponent._isOn)
			{
				CloseCurrentPanel();
			}
			_manager.CheckAutomaticReset();
		}
		#endregion

		#region UI Logic
		private void BuildListIfNeeded()
		{
			if (stepViews != null && stepViews.Length == TOTAL_STEPS)
				return;

			stepViews = new PrivilegeStepView[TOTAL_STEPS];

			for (int i = listParent.childCount; i < TOTAL_STEPS; i++)
				Instantiate(stepPrefab, listParent);

			for (int i = 0; i < TOTAL_STEPS; i++)
				stepViews[i] = listParent.GetChild(i).GetComponent<PrivilegeStepView>();
		}

		public void Refresh()
		{
			var cycle = _manager.GetActiveCycle();
			if (cycle == null) return;

			UpdateKeyProgressUI();

			var freeFlags = _manager.GetFreeCollectedFlagArray();
			var paidFlags = _manager.GetPaidCollectedFlagArray();

			bool canCollect = _manager.CanCollectCurrentStep(SelectedIndex);
			bool paidActive = _manager.IsPaidActive();

			int freeRewardCount = _manager.GetFreeReward();
			int paidRewardCount = _manager.GetPaidReward();

			for (int i = 0; i < TOTAL_STEPS; i++)
			{
				stepViews[i].Render(
					cycle.GetStep(i),
					i,
					freeFlags[i],
					paidFlags[i],
					paidActive,
					canCollect,
					freeRewardCount-- > 0,
					paidRewardCount-- > 0
				);
			}

			UpdateKeysUI(canCollect);
			UpdatePaidUI();
			ScrollRectHelper.ScrollToIndex(scrollRect, SelectedIndex, false, 0.5f);
		}

		private void UpdateKeysUI(bool canCollect)
		{
			int remainingKeys = _manager.GetCurrentStep().keysRequired - _manager.GetAvailableKeys();
			m_View.KeysText.text = Mathf.Max(_manager.GetCurrentStepIndex() + 1, 0).ToString();

			m_View.CollectButton.gameObject.SetActive(IsGlobalClaimButton);

			RefreshClaimText();

			if (!IsGlobalClaimButton) return;

			m_View.CollectButton.interactable = canCollect;
			m_View.CollectButtonText.text = canCollect ? "CLAIM" : "LOCKED";
			m_View.CollectButtonImage.sprite = canCollect ? UnLockSprite : LockSprite;

		}

		private void RefreshClaimText()
		{
			if (_manager.MainMenuButton != null)
			{
				bool hasRewards = _manager.GetAvailableRewardes() > 0;
				_manager.MainMenuButton.RefreshCircle(hasRewards, _manager.GetAvailableRewardes().ToString());
			}
		}

		private void UpdatePaidUI()
		{
			bool paidActive = _manager.IsPaidActive();
			m_View.PurchasePrivilegeScreen.interactable = !paidActive;
			m_View.PaidStatusText.text = paidActive ? "ACTIVE" : "ACTIVATE";
		}

		private void UpdateKeyProgressUI()
		{
			var nextStep = _manager.GetCurrentStep();
			if (nextStep == null)
			{
				_progressBarView.Value(1);
				_progressBarView.SetText("Completed");
				return;
			}

			int current = _manager.GetAvailableKeys();
			int remainingKeys = _manager.GetRemaingKeys();
			int required = nextStep.keysRequired;

			_progressBarView.Init(required);
			_progressBarView.Value(remainingKeys);
			_progressBarView.SetText($"{remainingKeys}/{required}");
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
			scrollRect.enabled = false;
		}
		#endregion

		#region Actions
		public void OnCollectAllButtonPressed()
		{
			var rewards = _manager.CollectFreePlusPaidReward(SelectedIndex);
			if (rewards != null)
				RewardBaseHandle.Instance.SetReward(rewards.ToList());
		}

		private void OnCollectFreeButtonPressed(int slot)
		{
			scrollRect.enabled = false;
			var rewards = _manager.CollectFreeReward(slot);
			if (rewards != null)
				RewardBaseHandle.Instance.SetReward(rewards.ToList());
		}

		private void OnCollectPaidButtonPressed(int slot)
		{
			scrollRect.enabled = false;
			var rewards = _manager.CollectPaidReward(slot);
			if (rewards != null)
				RewardBaseHandle.Instance.SetReward(rewards.ToList());
		}

		private void OnBuyPaidPressed()
		{
			if (_manager.PurchasePaidPrivilege())
				Debug.Log("Paid privilege activated");

			Refresh();
			listParent.anchoredPosition = Vector2.zero;
			listParent.sizeDelta = Vector2.zero;
		}

		private void OnClaimButtonActive(int currentSlot)
		{
			_manager.SetScreenState();
		}
		#endregion

		#region Navigation
		private void OpenPurchasePrivilegeScreen()
		{
			scrollRect.enabled = false;
			CloseCurrentPanel();
			UIPanelManager.Show(Base.UI.Manager.Panel.PRIVILEGE_PURCHASE_SCREEN, true);
		}

		private void OpenPrivilegeScreen()
		{
			UIPanelManager.Show(Base.UI.Manager.Panel.PRIVILAGE, true);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			scrollRect.enabled = true;
		}
		#endregion
	}
}
