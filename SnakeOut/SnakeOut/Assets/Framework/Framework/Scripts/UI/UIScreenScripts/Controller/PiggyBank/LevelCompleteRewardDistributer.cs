using System;
using Frameork;
using Framework.Core;
using UnityEngine;
using Watermelon;

namespace Framework
{
	public class LevelCompleteRewardDistributer : MonoBehaviour
	{
		public PiggyBankConfig PiggyScriptable;

		private PiggyBankSaveData _piggyBankSaveData;
		private PrivilegeSaveData _privilegeSaveData;
		private TicTacSaveData _ticTacSaveData;
		private IcelandAdvantacher _icelandSaveData;

		private bool _isPrivilegeActive;
		private bool _isTicTacActive;
		private bool _isIceLandActive;
		private bool _isPiggyBankActive;

		public void Awake()
		{
			_piggyBankSaveData = PiggyBankPersistence.Load();
			_privilegeSaveData = PrivilegePersistence.Load();

			PrivilegeManager.OnPanelShowing += OnPrivilegeActive;
			TicTacCycleManager.OnPanelShowing += OnTicTacActive;
			IceAdvantureCycleManager.OnPanelShowing += OnIceLandActive;
			UIPiggyBankController.OnPanelShowing += OnPiggyBankActive;

			MyEventArgs.GameControllerEvents.OnLevelComplete.AddListener(AddRewardOnLevelComplete);
			MyEventArgs.GameControllerEvents.OnLevelFailed.AddListener(OnLevelFailed);
		}

		private void OnPrivilegeActive(bool active)
		{
			_isPrivilegeActive = active;
		}

		private void OnTicTacActive(bool active)
		{
			_isTicTacActive = active;
		}

		private void OnIceLandActive(bool active)
		{
			_isIceLandActive = active;
		}

		private void OnPiggyBankActive(bool active)
		{
			_isPiggyBankActive = active;
		}

		private void AddRewardOnLevelComplete(int level, int amount)
		{
			PiggyBankPersistence.Save(_piggyBankSaveData);

			var maxCoins =
				PiggyScriptable.baseMaxCoins +
				(_piggyBankSaveData.purchasesCount * PiggyScriptable.maxCoinsIncreasePerPurchase);

			//Piggy bank
			if (_piggyBankSaveData.coinsStored <= maxCoins && _isPiggyBankActive )
			{
				var rw = new FlowReward(RewardType.Piggy);
				RewardFlowQueue.AddReward(rw);

				_piggyBankSaveData.coinsStored += PiggyScriptable.levelWinCoin;
			}

			if (Monetization.Settings.LevelRequire < GetCurrentLevelAbstract.Instance.GetLevel())
			{
				var rw = new FlowReward(RewardType.Coins);
				RewardFlowQueue.AddReward(rw);
			}

			//Privilege
			if (_isTicTacActive) OnLevelCompletedQuestionMarkKey(1);

			//Privilege
			if (_isPrivilegeActive) OnLevelCompletedGiveKey(1);

			//On Level Complete
			if (_isIceLandActive) OnIncreaseCurrentStep();
		}

		public void OnLevelCompletedGiveKey(int count = 1)
		{
			var rw = new FlowReward(RewardType.Power6);
			RewardFlowQueue.AddReward(rw);

			_privilegeSaveData = PrivilegePersistence.Load();

			if (count <= 0) return;
			_privilegeSaveData.TotalAvaliableKeys += count;
			PrivilegePersistence.Save(_privilegeSaveData);
		}

		private void OnLevelCompletedQuestionMarkKey(int v)
		{
			var rw = new FlowReward(RewardType.Question);
			RewardFlowQueue.AddReward(rw);

			_ticTacSaveData = TicTacPersistence.Load();

			_ticTacSaveData.SavedQuestionMark++;
			TicTacPersistence.Save(_ticTacSaveData);
		}

		private void OnIncreaseCurrentStep()
		{
			_icelandSaveData = IceLandPersistence.Load();

			if (_icelandSaveData.CurrentStep < 4 && _icelandSaveData.IsStarted)
			{
				_icelandSaveData.CurrentStep++;
				IceLandPersistence.Save(_icelandSaveData);
			}
		}

		public void OnLevelFailed()
		{
			_icelandSaveData = IceLandPersistence.Load();

			if (_icelandSaveData.CurrentStep < 4 && _icelandSaveData.IsStarted)
			{
				_icelandSaveData.CurrentStep = 0;
				IceLandPersistence.Save(_icelandSaveData);
			}
		}

		private void OnDestroy()
		{
			MyEventArgs.GameControllerEvents.OnLevelComplete.RemoveListener(AddRewardOnLevelComplete);
			MyEventArgs.GameControllerEvents.OnLevelFailed.RemoveListener(OnLevelFailed);

		}
	}
}
