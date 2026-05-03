using System;
using Base.UI.Controller;
using UnityEngine;
using Framework.Core;

namespace Framework
{
	public class PrivilegeManager : MonoBehaviour, IManager
	{
		private const int TOTAL_STEPS = 28;
		private const int CYCLE_DAYS = 28;

		public static Action<bool> OnPanelShowing;
		public event Action OnStateChanged;

		public PrivilegeCycleSet[] cycles;
		public bool UseUtc = true;
		public bool UseOnline;

		public MainMenuButtons MainMenuButton;
		public RequiredLevelScriptableObject RequiredLevelScriptableObject;
		public bool IsClaimBtnOn { get => _isClaimBtnOn; }
		public bool IsConfigCatEnable;
		public string ConfigCatEvent;

		#region Public Getters

		public bool IsUnlocked()
		{
			if (saveData == null)
				LoadOrInitialize();

			return saveData.isUnlocked;
		}

		public PrivilegeStepData GetCurrentStep() => GetActiveCycle().GetStep(saveData.CurrentStep);
		public int GetCurrentStepIndex() => saveData.CurrentStep;
		public int GetAvailableKeys() => saveData.TotalAvaliableKeys;
		public int GetRemaingKeys() => saveData.TotalAvaliableKeysRemain;
		public int GetFreeReward() => saveData.FreeReward;
		public int GetPaidReward() => saveData.PaidReward;
		public int GetAvailableRewardes() => _reward;
		public bool[] GetFreeCollectedFlagArray() => (bool[])saveData.FreeCollectedFlag.Clone();
		public bool[] GetPaidCollectedFlagArray() => (bool[])saveData.PaidCollectedFlag.Clone();

		public DateTime GetCycleStart()
		{
			return string.IsNullOrEmpty(saveData.CycleStartUtc)
				? DateTime.MinValue
				: DateTime.Parse(saveData.CycleStartUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);
		}

		public TimeSpan GetTimeRemainingInCycle()
		{
			var end = GetCycleStart().AddDays(CYCLE_DAYS);
			var rem = end - Now;
			return rem < TimeSpan.Zero ? TimeSpan.Zero : rem;
		}

		#endregion

		protected UICheckScreenReady _uiCheckScreenReady;
		protected UIBaseController _uIBaseController;

		private bool _isClaimBtnOn;
		private PrivilegeSaveData saveData;
		private int _reward;

		private DateTime Now =>
		UseUtc
			? (UseOnline ? UtcTimeService.Instance.GetCurrentDate() : DateTime.UtcNow)
			: DateTime.Now;


		MainMenuButtons IManager.MainMenuButton => MainMenuButton;
		private bool _isConfigCatEnabled;

		#region Unity
		public void OnInitialized()
		{
			if (MainMenuButton == null)
				return;

			_uiCheckScreenReady = GetComponent<UICheckScreenReady>();
			_uIBaseController = GetComponent<UIBaseController>();
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

			LoadOrInitialize();
			EnsureSelectedCycle();
			CheckAutomaticReset();
			SetAvaliableReward();
			SetCurrentStep();
			SetRewardUnlock();
			Notify();
			Invoke(nameof(CheckScreenReady), 2);
		}
		#endregion

		#region Load / Save
		private void LoadOrInitialize()
		{
			saveData = PrivilegePersistence.Load();

			if (saveData.FreeCollectedFlag == null || saveData.FreeCollectedFlag.Length != TOTAL_STEPS)
			{
				saveData.FreeCollectedFlag = new bool[TOTAL_STEPS];
				saveData.PaidCollectedFlag = new bool[TOTAL_STEPS];
				saveData.IsRewardUnlock = new bool[TOTAL_STEPS];
			}

			if (cycles == null || cycles.Length == 0)
				saveData.SelectedCycleIndex = 0;
			else
				saveData.SelectedCycleIndex = Mathf.Clamp(saveData.SelectedCycleIndex, 0, cycles.Length - 1);
		}

		private void Save()
		{
			PrivilegePersistence.Save(saveData);
		}

		public void CheckScreenReady()
		{
			if (_uiCheckScreenReady != null) _uiCheckScreenReady.CheckScreenReady();
		}
		#endregion

		#region Cycle
		public void OpenPanelLevelReach(Action action = null)
		{
			if (MainMenuButton.IsLevelReach() && !IsUnlocked())
			{
				action?.Invoke();
				SetUnlocked();
			}
		}

		private void EnsureSelectedCycle()
		{
			if (string.IsNullOrEmpty(saveData.CycleStartUtc))
			{
				StartNewCycle(RandomCycleIndex());
				return;
			}

			var start = DateTime.Parse(
				saveData.CycleStartUtc,
				null,
				System.Globalization.DateTimeStyles.RoundtripKind
			);

			if ((Now - start).TotalDays >= CYCLE_DAYS)
				StartNewCycle(RandomCycleIndex());
		}

		public void CheckAutomaticReset()
		{
			var start = DateTime.Parse(
				saveData.CycleStartUtc,
				null,
				System.Globalization.DateTimeStyles.RoundtripKind
			);

			if ((Now - start).TotalDays >= CYCLE_DAYS)
				StartNewCycle(RandomCycleIndex());
		}

		private int RandomCycleIndex()
		{
			return (cycles == null || cycles.Length == 0)
				? 0
				: UnityEngine.Random.Range(0, cycles.Length);
		}

		private void StartNewCycle(int cycleIndex)
		{
			saveData.SelectedCycleIndex = Mathf.Clamp(cycleIndex, 0, (cycles?.Length ?? 1) - 1);
			saveData.CycleStartUtc = Now.ToString("o");
			saveData.CurrentStep = 0;
			saveData.TotalAvaliableKeys = 0;
			saveData.TotalAvaliableKeysRemain = 0;
			saveData.PaidActive = false;

			Array.Clear(saveData.FreeCollectedFlag, 0, TOTAL_STEPS);
			Array.Clear(saveData.PaidCollectedFlag, 0, TOTAL_STEPS);
			Array.Clear(saveData.IsRewardUnlock, 0, TOTAL_STEPS);

			saveData.FreeReward = 0;
			saveData.PaidReward = 0;

			Save();
			Notify();
		}

		#endregion

		#region Progress / Keys
		public void OnLevelCompletedGiveKey(int count = 1)
		{
			if (count <= 0) return;

			saveData.TotalAvaliableKeys += count;
			SetCurrentStep();
			SetRewardUnlock();
			Save();
			Notify();
		}

		private void SetCurrentStep()
		{
			var keys = saveData.TotalAvaliableKeys;

			for (int i = 0; i < GetActiveCycle().steps.Length; i++)
			{
				saveData.CurrentStep = i;
				saveData.TotalAvaliableKeysRemain = keys;

				keys -= GetActiveCycle().steps[i].keysRequired;
				if (keys < 0)
					break;
			}
		}

		public void SetRewardUnlock()
		{
			var keys = saveData.TotalAvaliableKeys;

			for (int i = 0; i < GetActiveCycle().steps.Length; i++)
			{
				keys -= GetActiveCycle().steps[i].keysRequired;

				if (keys >= 0 && !saveData.IsRewardUnlock[i])
				{
					saveData.IsRewardUnlock[i] = true;
					saveData.FreeReward++;
					saveData.PaidReward++;
				}
			}

			SetAvaliableReward();
		}

		public void SetAvaliableReward()
		{
			int reward = 0;

			for (int i = 0; i < TOTAL_STEPS; i++)
			{
				if (!saveData.IsRewardUnlock[i])
					continue;

				if (!saveData.FreeCollectedFlag[i])
					reward++;

				if (saveData.PaidActive && !saveData.PaidCollectedFlag[i])
					reward++;
			}

			_reward = reward;
		}

		#endregion

		#region Collect
		public bool CanCollectCurrentStep(int stepIndex)
		{
			if (!IsValidIndex(stepIndex)) return false;
			if (saveData.FreeCollectedFlag[stepIndex]) return false;
			return saveData.IsRewardUnlock[stepIndex];
		}

		public RewardItem[] CollectFreeReward(int stepIndex)
		{
			if (!IsValidIndex(stepIndex)) return null;
			if (saveData.FreeCollectedFlag[stepIndex]) return null;
			if (!saveData.IsRewardUnlock[stepIndex]) return null;

			saveData.FreeCollectedFlag[stepIndex] = true;
			SetAvaliableReward();
			Save();
			Notify();

			return GetActiveCycle().GetStep(stepIndex).freeRewards;
		}

		public RewardItem[] CollectPaidReward(int stepIndex)
		{
			if (!saveData.PaidActive) return null;
			if (!IsValidIndex(stepIndex)) return null;
			if (saveData.PaidCollectedFlag[stepIndex]) return null;
			if (!saveData.IsRewardUnlock[stepIndex]) return null;

			saveData.PaidCollectedFlag[stepIndex] = true;
			SetAvaliableReward();
			Save();
			Notify();

			return GetActiveCycle().GetStep(stepIndex).paidRewards;
		}

		public RewardItem[] CollectFreePlusPaidReward(int stepIndex)
		{
			for (int i = 0; i < TOTAL_STEPS; i++)
			{
				saveData.FreeCollectedFlag[i] = true;
				saveData.PaidCollectedFlag[i] = true;
			}

			SetAvaliableReward();

			if (IsCycleComplete())
				StartNewCycle(RandomCycleIndex());

			Save();
			Notify();
			return null;
		}

		#endregion

		#region Paid
		public bool PurchasePaidPrivilege()
		{
			if (saveData.PaidActive) return false;

			saveData.PaidActive = true;
			SetAvaliableReward();
			Save();
			Notify();
			return true;
		}

		public bool IsPaidActive() => saveData.PaidActive;
		#endregion

		#region Misc
		public PrivilegeCycleSet GetActiveCycle()
		{
			if (cycles == null || cycles.Length == 0)
				return null;

			return cycles[saveData.SelectedCycleIndex];
		}

		public void SetUnlocked()
		{
			saveData.isUnlocked = true;
			Save();
		}

		public void SetScreenState()
		{
			_isClaimBtnOn = true;
		}

		public void DeleteSaveAndRestart()
		{
			PrivilegePersistence.DeleteSave();
			LoadOrInitialize();
			EnsureSelectedCycle();
			Save();
			Notify();
		}

		private bool IsValidIndex(int idx) => idx >= 0 && idx < TOTAL_STEPS;

		private bool IsCycleComplete()
		{
			return GetTimeRemainingInCycle() <= TimeSpan.Zero;
		}

		private void Notify()
		{
			OnStateChanged?.Invoke();
		}
		#endregion


		#region VISIBILITY
		public bool IsShow()
		{
			return MainMenuButton.IsLevelReach() && _isConfigCatEnabled;
		}
		#endregion
	}
}
