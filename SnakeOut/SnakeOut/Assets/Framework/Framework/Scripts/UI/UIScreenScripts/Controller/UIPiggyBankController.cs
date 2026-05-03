using System;
using System.Threading.Tasks;
using Base.UI.Controller;
using BaseView;
using UnityEngine;
using Framework.Core;

namespace Framework
{
	public class UIPiggyBankController : Behaviour<UIPiggyBankView>
	{
		public static Action<bool> OnPanelShowing;
		public enum PiggyTimerType
		{
			Offer,
			CooldownAfterOffer,
			CooldownAfterPurchase
		}
		public bool ShowDebug;
		public bool useUtc = true;
		public bool UseOnline = true;
		public PiggyBankConfig PiggyScriptable;

		public MainMenuButtons MainMenuButton;
		public PiggyTimerType CurrentPiggyTimerType = PiggyTimerType.Offer;
		public Sprite CoinSprite;

		public bool IsConfigCatEnable;
		public string ConfigCatEvent;

		private UIPiggyBankView m_View;
		private PiggyBankSaveData save;
		private PiggyBankTimer _baseTimeHandle;
		private IAPCustomHolder _customHolder;
		private bool IsShowing => ShouldShow();

		private DateTime Now =>
			useUtc
				? (UseOnline ? UtcTimeService.Instance.GetCurrentDate() : DateTime.UtcNow)
				: DateTime.Now;

		private bool _isConfigCatEnabled;

		#region INIT
		protected override void Init()
		{
			base.Init();
			m_View = (UIPiggyBankView)Prefab;

			save = PiggyBankPersistence.Load();
			_baseTimeHandle = GetComponent<PiggyBankTimer>();
			_customHolder = GetComponent<IAPCustomHolder>();
		}

		private void Start()
		{
			_customHolder.OnPurchaseComplete += OnPurchaseComplete;
			if (IsConfigCatEnable)
			{
#if MODULE_CONFIG_CAT && (UNITY_ANDROID || UNITY_IOS)
				ConfigCatHandler.LoadConfigAsync(OnConfigLoaded, ConfigCatEvent);
#endif
			}
			else
			{
				OnConfigLoaded(true);
			}
		}

		private void OnConfigLoaded(bool isFeature)
		{
			_isConfigCatEnabled = isFeature;
			MainMenuButton.SetRequiredLevel(PiggyScriptable.RequiredLevel.ShowAfterLevel, PiggyScriptable.RequiredLevel.IsShowWhenLevelNotReach, _isConfigCatEnabled, OpenCurrentPanel);
			if (MainMenuButton.IsLevelReach() && _isConfigCatEnabled)
			{
				UpdatePiggyFlow();
				RefreshUI();
			}
		}
		#endregion

		#region DURATIONS
		private TimeSpan OfferDuration =>
			new TimeSpan(
				PiggyScriptable.offerDurationDays,
				PiggyScriptable.offerDurationHours,
				PiggyScriptable.offerDurationHMin,
				PiggyScriptable.offerDurationSecond
			).Add(TimeSpan.FromSeconds(PiggyScriptable.offerDurationSecond));

		private TimeSpan OfferCooldown =>
			new TimeSpan(
				PiggyScriptable.reappearAfterOfferEndDays,
				PiggyScriptable.reappearAfterOfferEndHours,
				PiggyScriptable.reappearAfterOfferEndMin,
				PiggyScriptable.reappearAfterOfferEndSecond
			).Add(TimeSpan.FromSeconds(PiggyScriptable.reappearAfterOfferEndSecond));

		private TimeSpan PurchaseCooldown =>
			new TimeSpan(
				PiggyScriptable.reappearAfterPurchaseDays,
				PiggyScriptable.reappearAfterPurchaseHours,
				PiggyScriptable.reappearAfterOfferEndMin,
				PiggyScriptable.reappearAfterOfferEndSecond
			).Add(TimeSpan.FromSeconds(PiggyScriptable.reappearAfterPurchaseSecond));

		#endregion

		#region CORE FLOW (ONLY PLACE THAT CONTROLS TIMER)
		private void UpdatePiggyFlow()
		{
			DateTime now = Now;

			// ---------------- OFFER RUNNING ----------------
			if (!string.IsNullOrEmpty(save.startDateTime))
			{
				DateTime start = DateTime.Parse(
					save.startDateTime,
					null,
					System.Globalization.DateTimeStyles.RoundtripKind);

				DateTime end = start + OfferDuration;

				if (now < end)
				{
					StartTimer(end - now, true, PiggyTimerType.Offer);
					return;
				}

				// Offer ended
				save.startDateTime = null;
				save.lastOfferEndUtc = now.ToString("o");
				PiggyBankPersistence.Save(save);
			}

			// ---------------- PURCHASE COOLDOWN ----------------
			if (!string.IsNullOrEmpty(save.lastPurchaseUtc))
			{
				DateTime purchase = DateTime.Parse(
					save.lastPurchaseUtc,
					null,
					System.Globalization.DateTimeStyles.RoundtripKind);

				DateTime end = purchase + PurchaseCooldown;

				if (now < end)
				{
					StartTimer(end - now, false, PiggyTimerType.CooldownAfterPurchase);
					return;
				}

				save.lastPurchaseUtc = null;
				PiggyBankPersistence.Save(save);
			}

			// ---------------- OFFER COOLDOWN ----------------
			if (!string.IsNullOrEmpty(save.lastOfferEndUtc))
			{
				DateTime endOffer = DateTime.Parse(
					save.lastOfferEndUtc,
					null,
					System.Globalization.DateTimeStyles.RoundtripKind);

				DateTime end = endOffer + OfferCooldown;

				if (now < end)
				{
					StartTimer(end - now, false, PiggyTimerType.CooldownAfterOffer);
					return;
				}

				save.lastOfferEndUtc = null;
				PiggyBankPersistence.Save(save);
			}

			// ---------------- START NEW OFFER ----------------
			save.startDateTime = now.ToString("o");
			PiggyBankPersistence.Save(save);
			StartTimer(OfferDuration, true, PiggyTimerType.Offer);
		}

		private void StartTimer(TimeSpan duration, bool showOnUI, PiggyTimerType timerType)
		{
			// Cancel ONLY when starting a NEW timer
			TimerManager.Cancel("Piggy");
			CurrentPiggyTimerType = timerType;

			if (!IsShowing)
			{
				return;
			}

			TimerManager.CreateTimer(
				duration,
				OnTimerCompleted,
				x =>
				{
					MainMenuButton.EnableButton(CurrentPiggyTimerType == PiggyTimerType.Offer);
					Log($"[PiggyTimer][{timerType}] {x}");

					if (showOnUI)
					{
						_baseTimeHandle.UpdateTime(
							_baseTimeHandle.FormatSpan(x)
						);
					}
				},
				"Piggy"
			);
		}
		private void OnTimerCompleted()
		{
			Log("[PiggyTimer] Completed → refreshing state");
			if (_currentComponent._isOn)
			{
				CloseCurrentPanel();
			}
			UpdatePiggyFlow();   // evaluate next state
			RefreshUI();
		}
		#endregion

		#region UI
		public bool ShouldShow()
		{
			if (MainMenuButton.PlayerLevel + 1 < PiggyScriptable.RequiredLevel.ShowAfterLevel)
				return false;

			if (PiggyScriptable.showOnlyOnce && save.purchasesCount > 0)
				return false;

			if (CurrentPiggyTimerType != PiggyTimerType.Offer)
				return false;

			// Visible ONLY when offer is running
			return true;
		}

		private void RefreshUI()
		{
			OnPanelShowing?.Invoke(ShouldShow());

			MainMenuButton.EnableButton(CurrentPiggyTimerType == PiggyTimerType.Offer);

			var maxCoins =
				PiggyScriptable.baseMaxCoins +
				(save.purchasesCount * PiggyScriptable.maxCoinsIncreasePerPurchase);

			m_View.PiggyCoinsText.text = $"{save.coinsStored}/{maxCoins}";
			m_View.PiggyBankFill.fillAmount =
				Mathf.Clamp01((float)save.coinsStored / maxCoins);

			m_View.ClaimButton.interactable =
				save.coinsStored >= maxCoins;

			MainMenuButton.RefreshCircle(save.coinsStored >= maxCoins, "");

			m_View.FullIndicator.gameObject.SetActive(save.coinsStored >= maxCoins);
			m_View.claimBtnImage.sprite = save.coinsStored >= maxCoins ? PiggyScriptable.grayBtnIcon : PiggyScriptable.grayBtnIcon;
		}

		#endregion

		#region PURCHASE
		private void OnPurchaseComplete()
		{
			RewardBaseHandle.Instance.SetReward(new RewardItem() { type = RewardType.Coins, quantity = save.coinsStored, icon = CoinSprite });

			save.purchasesCount++;
			save.coinsStored = 0;
			save.lastPurchaseUtc = Now.ToString("o");
			save.startDateTime = null;
			save.lastOfferEndUtc = null;

			PiggyBankPersistence.Save(save);

			MainMenuButton.RefreshCircle(false, "");

			UpdatePiggyFlow();
			RefreshUI();
			CloseCurrentPanel();
		}

		#endregion

		private void OnDestroy()
		{
			TimerManager.Cancel("Piggy");
		}

		public override bool IsShow()
		{
			return IsShowing;
		}
	}
}
