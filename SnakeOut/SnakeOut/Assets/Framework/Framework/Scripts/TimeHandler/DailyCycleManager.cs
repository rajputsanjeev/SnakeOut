using System;
using System.Threading.Tasks;
using Base.UI.Controller;
using UnityEngine;
using UnityEngine.Events;
using Framework.Core;

namespace Framework
{
	public abstract class DailyCycleManager<TSave> : MonoBehaviour, IManager where TSave : MiniGameCoolDown
	{
		public static Action<bool> OnPanelShowing;
		public event Action OnStateChanged;

		public RequiredLevelScriptableObject RequiredLevelScriptableObject;
		public MainMenuButtons MainMenuButton;

		public bool IsConfigCatEnable;
		public string ConfigCatEvent;
		public bool UseUtc = true;
		public bool UseOnline;
		public bool IsClaimBtnOn => _isClaimBtnOn;

		protected TSave save;
		protected UIBaseController _uIBaseController;
		protected UICheckScreenReady _uiCheckScreenReady;
		protected bool _isClaimBtnOn;
		protected DateTime CooldownEndUtc =>
			string.IsNullOrEmpty(save.cooldownEndUtc)
				? DateTime.MinValue
				: DateTime.Parse(save.cooldownEndUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);

		protected DateTime Now =>
			UseUtc
				? (UseOnline ? UtcTimeService.Instance.GetCurrentDate() : DateTime.UtcNow)
				: DateTime.Now;

		MainMenuButtons IManager.MainMenuButton => MainMenuButton;
		private bool _isConfigCatEnabled;

		#region Initialized
		public void OnInitialized()
		{
			if (MainMenuButton == null)
				return;

			_uIBaseController = GetComponent<UIBaseController>();
			_uiCheckScreenReady = GetComponent<UICheckScreenReady>();

			if (_uIBaseController != null)
			{
				_uIBaseController.Subscribe();
			}

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
			OnPanelShowing?.Invoke(_isConfigCatEnabled);

			MainMenuButton.SetRequiredLevel(RequiredLevelScriptableObject.ShowAfterLevel, RequiredLevelScriptableObject.IsShowWhenLevelNotReach, _isConfigCatEnabled, _uIBaseController.OpenCurrentPanel);

			if (!IsShow())
				return;

			Load();
			CheckDailyReset();
			Notify();
			Invoke(nameof(CheckScreenReady), 1);
		}

		#endregion

		#region UNITY_CYCLE
		protected virtual void Awake()
		{
		}
		#endregion

		#region TIME
		public TimeSpan GetTimeUntilNextReset()
		{
			var now = Now;
			var nextReset = now.Date.AddDays(1);
			return nextReset - now;
		}
		#endregion

		#region VISIBILITY
		public bool IsShow()
		{
			return MainMenuButton.IsLevelReach() && _isConfigCatEnabled;
		}
		#endregion

		#region Load Config
		private async Task LoadConfigAsync(Action<bool> callback = null, string eventName = null)
		{
			var isFeatureEnable = false;
			if (string.IsNullOrEmpty(ConfigCatEvent))
			{
				isFeatureEnable = true;
			}

#if MODULE_CONFIG_CAT && (UNITY_ANDROID || UNITY_IOS)
			isFeatureEnable = await ConfigCatHandler.ConfigCatClient.GetValueAsync(eventName, false);
#endif
			callback?.Invoke(isFeatureEnable);
			callback?.Invoke(isFeatureEnable);
		}
		#endregion

		#region LOAD & SAVE
		public virtual void Load()
		{
			if (save == null)
			{
				Debug.LogError($"{GetType().Name} save data is NULL.");
				return;
			}

			if (string.IsNullOrEmpty(save.lastResetUtc))
			{
				DoDailyReset();
				Save();
			}
		}

		public abstract void Save();

		#endregion

		#region DAILY RESET
		protected virtual void DoDailyReset()
		{
			save.lastResetUtc = Now.ToString("o");

			// very important: child class resets its own daily data here
			OnDailyDataReset();
		}

		protected abstract void OnDailyDataReset();

		public void CheckDailyReset()
		{
			var now = Now;
			var lastReset = string.IsNullOrEmpty(save.lastResetUtc)
				? DateTime.MinValue
				: DateTime.Parse(save.lastResetUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);

			if (lastReset.Date < now.Date)
			{
				DoDailyReset();
				Save();
				Notify();
			}
		}

		#endregion

		#region CheckLock and Unlock
		public void OpenPanelLevelReach(Action action = null)
		{
			if (MainMenuButton.IsLevelReach() && !IsUnlocked())
			{
				action?.Invoke();
				SetUnlocked();
			}
		}
		#endregion

		#region SetLock and Unlock
		public bool IsUnlocked()
		{
			if (save == null)
				Load();

			return save.isUnlocked;
		}

		public void SetUnlocked()
		{
			save.isUnlocked = true;
			Save();
		}
		#endregion

		protected void Notify()
		{
			OnStateChanged?.Invoke();
		}

		public void CheckScreenReady()
		{
			if (_uiCheckScreenReady != null) _uiCheckScreenReady.CheckScreenReady();
		}

		public void SetClaimBtnState()
		{
			_isClaimBtnOn = true;
		}
	}
}
