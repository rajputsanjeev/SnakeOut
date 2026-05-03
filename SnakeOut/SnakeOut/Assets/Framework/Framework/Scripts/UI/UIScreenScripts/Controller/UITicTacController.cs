using System;
using System.Collections.Generic;
using System.Linq;
using BaseView;
using Framework.UI.Components;
using UnityEngine;

namespace Framework.UI.Controllers
{
	public class UITicTacController : Behaviour<UITicTacView>
	{
		public List<ChestConfigSO> ChestConfigs = new List<ChestConfigSO>();
		public ChestConfigSO CurrentChestConfigSO => ChestConfigs[_curentChest];
		public TicTacChestSprite chestPrefab;
		public GameObject ClaimBtns;
		public GameObject PlayBtns;
		public List<TicTacGridGenerator> GridGenerators;
		public DoubleReward DoubleRewards;
		public DoubleReward ClaimQuestionReward;

		private TicTacGridGenerator CurrentGrid;
		private UITicTacView m_View;
		private TicTacCycleManager _manager;
		private BaseTimerHandle _baseTimeHandle;
		private TicTacSaveData _save => _manager.SaveData;
		private int _curentChest => _save.CurrentChest;
		private TicTacModeSO CurrentChest => ChestConfigs[_curentChest].modes[0];
		private List<TicTacChestSprite> _ticTacChestSprites = new List<TicTacChestSprite>();
		private int _totalSize => CurrentChest.width * CurrentChest.height;
		public WatchAdsPower QuestionRewardWatch;
		public WatchAdsPower ClaimRewardWatch;


		protected override void Init()
		{
			base.Init();
			CacheReferences();
		}

		private void CacheReferences()
		{
			GetBaseView();
			m_View = (UITicTacView)Prefab;

			_baseTimeHandle = GetComponent<BaseTimerHandle>();
			_manager = GetComponent<TicTacCycleManager>();
		}

		public override void OnInit()
		{
			base.OnInit();
			CacheReferences();
		}

		public override void Subscribe()
		{
			base.Subscribe();
			CacheReferences();

			m_View.ClaimButton.onClick.AddListener(ClaimReward);
			m_View.PlayButton.onClick.AddListener(CloseCurrentPanel);

			_manager.OnStateChanged += StartTimer;
			if (DoubleRewards != null) DoubleRewards.OnActionAdComplete += DoubleReward;
			if (ClaimQuestionReward != null) ClaimQuestionReward.OnActionAdComplete += ClaimQuestionRewardReward;
		}

		public override bool IsShow()
		{
			return (_manager.IsShow() && IsOfferScreen);
		}

		public void StartTimer()
		{
			InitGame();

			if (_manager.MainMenuButton.IsLevelReach() && !_manager.IsUnlocked())
			{
				OpenCurrentPanel();
				_manager.SetUnlocked();
			}

			TimerManager.CreateTimer(_manager.GetTimeUntilNextReset(), OnTimerCompleted,
				x =>
				{
					var str = !_save.IsComplete ? "Claim before " : "Come Back after \n ";

					_baseTimeHandle.UpdateTime(
					str + _baseTimeHandle.FormatSpan(x));
				},
				"Tic_Tac");
		}

		private void OnTimerCompleted()
		{
			Log("[Tic Tac] Completed → refreshing state");
			if (_currentComponent._isOn)
			{
				CloseCurrentPanel();
			}
			_manager.CheckDailyReset();
			Load();
		}

		private void InitGame()
		{
			Load();
			GridGeneratorNew();
			GenerateTopChest();

			var (allDone, completedCount, newFruit) = CalculateFruitProgress();

			ShowClaimButton(allDone);
		}

		private void Load()
		{
			_manager.Load();

			if (_save == null || _save.Cells == null || _save.Cells.Length != _totalSize)
			{
				_save.Cells = new CellRevealData[_totalSize];
				_save.CurrentChestCollected = new bool[6];
				var requiredreveals = new List<TicTacFruitRevelData>(CurrentChest.requiredreveals.Count);
				_save.TicTacFruitRevels = requiredreveals;

				_manager.Save();
				InitializeCellData();
			}
		}

		private void InitializeCellData()
		{
			for (int i = 0; i < _totalSize; i++)
				_save.Cells[i] = new CellRevealData
				{
					fruitIndex = -1,
					groupIndex = -1
				};

			_save.TicTacFruitRevels.Clear();

			HashSet<int> usedCells = new HashSet<int>();

			for (int fruit = 0; fruit < CurrentChest.requiredreveals.Count; fruit++)
			{
				var rule = CurrentChest.requiredreveals[fruit];

				_save.TicTacFruitRevels.Add(new TicTacFruitRevelData());

				var allGroups = TicTacLineUtility.GetAllGroups(
					CurrentChest.width,
					CurrentChest.height,
					rule.WinCount
				);

				var groups = TicTacLineUtility.PickNonOverlappingLines(
					allGroups,
					rule.requiredReveals,
					usedCells
				);

				for (int g = 0; g < groups.Count; g++)
				{
					foreach (int cellIndex in groups[g])
					{
						_save.Cells[cellIndex].hasFruit = true;
						_save.Cells[cellIndex].fruitIndex = fruit;
						_save.Cells[cellIndex].groupIndex = g;
					}
				}
			}

			Save();
		}

		public void GridGeneratorNew()
		{
			m_View.gridRoot.DestroyChild();
			var ticTacGrid = Instantiate(GridGenerators[CurrentChest.modeId], m_View.gridRoot);
			CurrentGrid = ticTacGrid;
			var rectTransform = ticTacGrid.GetComponent<RectTransform>();
			rectTransform.sizeDelta = new Vector3(CurrentChest.GridX * CurrentChest.width + 50, CurrentChest.GridX * CurrentChest.height + 50);
			rectTransform.anchoredPosition = Vector3.zero;
			ticTacGrid.SetManager(_manager);
			ticTacGrid.GenerateGrid(CurrentChest, this);
			ticTacGrid.RestoreState();
		}

		private void GenerateTopChest()
		{
			m_View.ChestContainer.DestroyChild();
			_ticTacChestSprites = new List<TicTacChestSprite>();
			for (int i = 0; i < ChestConfigs.Count; i++)
			{
				var chest = Instantiate(chestPrefab, m_View.ChestContainer);
				chest.SetSprite(i < _curentChest || _save.CurrentChestCollected[_curentChest] ? ChestConfigs[i].CloseChest : ChestConfigs[i].ChestSprite);
				chest.EnableRing(i <= _curentChest);
				_ticTacChestSprites.Add(chest);
			}
			_ticTacChestSprites[_curentChest].EnableRing(true);
		}

		private void ClaimReward()
		{
			var (allDone, completedCount, newFruit) = CalculateFruitProgress();

			if (allDone)
			{
				RewardBaseHandle.Instance.SetReward(CurrentChestConfigSO.Reward);
				if (_save.CurrentChest < ChestConfigs.Count)
				{
					ResetGenerate();
				}
				else
				{
					ShowClaimButton(false);
					_manager.MarkCompleted();
					_ticTacChestSprites[5].SetSprite(ChestConfigs[5].CloseChest);
					_ticTacChestSprites[5].EnableRing(true);
					DebugExtension.Log("All Chest Complete", "green");
				}
			}
		}

		private void DoubleReward()
		{
			var (allDone, completedCount, newFruit) = CalculateFruitProgress();

			ClaimRewardWatch.ClaimRewardEvent?.Invoke();

			if (allDone)
			{
				RewardBaseHandle.Instance.SetReward(CurrentChestConfigSO.Reward, true);
				if (_save.CurrentChest < ChestConfigs.Count - 1)
				{
					ResetGenerate();
				}
				else
				{
					ShowClaimButton(false);
					_manager.MarkCompleted();
					_ticTacChestSprites[5].SetSprite(ChestConfigs[5].CloseChest);
					_ticTacChestSprites[5].EnableRing(true);
					DebugExtension.Log("All Chest Complete", "green");
				}
			}
		}

		private void ResetGenerate()
		{
			_save.CurrentChestCollected[_save.CurrentChest] = true;
			if (_save.CurrentChest < ChestConfigs.Count - 1)
			{
				_save.CurrentChest++;
				_save.TicTacFruitRevels = new List<TicTacFruitRevelData>();

				var total = CurrentChest.width * CurrentChest.height;
				_save.Cells = new CellRevealData[total];

				Save();

				GridGeneratorNew();
				GenerateTopChest();
				InitializeCellData();
			}
			else
			{
				_manager.MarkCompleted();
				_ticTacChestSprites[5].SetSprite(ChestConfigs[5].CloseChest);
				_ticTacChestSprites[5].EnableRing(true);
				DebugExtension.Log("All Chest Complete", "green");
			}

			ShowClaimButton(false);
		}

		private void ClaimQuestionRewardReward()
		{
			_save.SavedQuestionMark += 3;
			QuestionRewardWatch.ClaimRewardEvent?.Invoke();
			Save();
			CurrentGrid.RefreshUI();
		}

		public (bool allCompleted, int completedCount, int newlyCompletedFruit)
		CalculateFruitProgress()
		{
			int fruitCount = CurrentChest.requiredreveals.Count;
			int completedFruits = 0;
			int newlyCompletedFruit = -1;

			for (int fruit = 0; fruit < fruitCount; fruit++)
			{
				var rule = CurrentChest.requiredreveals[fruit];

				int[] groupCount = new int[rule.requiredReveals];

				foreach (var cell in _save.Cells)
				{
					if (!cell.revealed) continue;
					if (cell.fruitIndex != fruit) continue;

					groupCount[cell.groupIndex]++;
				}

				int completedGroups = 0;
				for (int g = 0; g < groupCount.Length; g++)
				{
					if (groupCount[g] >= rule.WinCount)
						completedGroups++;
				}

				// 🔥 detect NEW completion
				if (_save.TicTacFruitRevels[fruit].CurrentRevel < completedGroups &&
					completedGroups >= rule.requiredReveals)
				{
					newlyCompletedFruit = fruit;
				}

				_save.TicTacFruitRevels[fruit].CurrentRevel = completedGroups;

				if (completedGroups >= rule.requiredReveals)
					completedFruits++;
			}

			bool allCompleted = completedFruits == fruitCount;
			return (allCompleted, completedFruits, newlyCompletedFruit);
		}


		public void Save()
		{
			_manager.Save(_save);
		}

		private void OnDestroy()
		{
			m_View.ClaimButton.onClick.RemoveAllListeners();
			if (DoubleRewards != null) DoubleRewards.OnActionAdComplete -= DoubleReward;
			if (ClaimQuestionReward != null) ClaimQuestionReward.OnActionAdComplete -= ClaimQuestionRewardReward;
		}

		public void ShowClaimButton(bool active)
		{
			PlayBtns.gameObject.SetActive(!_save.IsComplete && !active);
			ClaimBtns.gameObject.SetActive(active && !_save.IsComplete);

			_closeButton.gameObject.SetActive(_save.IsComplete);

			_manager.MainMenuButton.RefreshCircle(active && !_save.IsComplete, "1");
		}
	}
}