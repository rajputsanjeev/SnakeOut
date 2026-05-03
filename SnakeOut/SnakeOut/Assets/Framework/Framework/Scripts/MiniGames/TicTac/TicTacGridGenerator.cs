using System;
using System.Collections.Generic;
using Framework.UI.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class TicTacGridGenerator : MonoBehaviour
	{
		public RectTransform gridRoot;
		public RectTransform RevelFruitParent;
		public GridLayoutGroup gridLayout;
		public TextMeshProUGUI QuestionText;
		public TicTacCell cellPrefab;
		public TicTacReveal TicTacRevealManager;

		private TicTacCell[] cells;
		private TicTacCycleManager _manager;
		private TicTacSaveData _save => _manager.SaveData;
		private TicTacModeSO _currentChest;
		private UITicTacController _uITicTacController;
		private List<TicTacReveal> _ticTacReveals = new List<TicTacReveal>();

		public void SetManager(TicTacCycleManager manager)
		{
			_manager = manager;
		}

		public void GenerateGrid(TicTacModeSO currentChest, UITicTacController uITicTacController)
		{
			_currentChest = currentChest;
			_uITicTacController = uITicTacController;
			gridRoot.DestroyChild();
			gridLayout.cellSize = new Vector2(currentChest.GridX, currentChest.GridY);
			int total = currentChest.width * currentChest.height;
			cells = new TicTacCell[total];

			gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			gridLayout.constraintCount = currentChest.width;

			for (int i = 0; i < total; i++)
			{
				var cell = Instantiate(cellPrefab, gridRoot);
				cell.gameObject.SetActive(true);
				cell.Init(i, this);
				cells[i] = cell;
			}
			GenerateFruitReveal();
			SyncFruitProgressUI(_ticTacReveals, _currentChest);
		}

		public void SyncFruitProgressUI(List<TicTacReveal> reveals, TicTacModeSO mode)
		{
			for (int fruit = 0; fruit < mode.requiredreveals.Count; fruit++)
			{
				var rule = mode.requiredreveals[fruit];
				int current = _save.TicTacFruitRevels[fruit].CurrentRevel;

				reveals[fruit].SetText(
					rule.requiredReveals,
					current
				);
			}
		}

		public void RestoreState()
		{
			for (int i = 0; i < _save.Cells.Length; i++)
			{
				var cellData = _save.Cells[i];

				if (!cellData.revealed)
					continue;

				if (cellData.fruitIndex >= 0 && cellData.hasFruit)
				{
					var fruitSprite =
						_currentChest.requiredreveals[cellData.fruitIndex].possibleFruit;

					cells[i].Reveal(fruitSprite);
				}
				else
				{
					cells[i].RevealEmpty();
				}
			}
		}
		public void GenerateFruitReveal()
		{
			RevelFruitParent.DestroyChild();
			_ticTacReveals.Clear();

			int fruitCount = _currentChest.requiredreveals.Count;

			// Safety check (VERY IMPORTANT)
			if (_save.TicTacFruitRevels == null ||
				_save.TicTacFruitRevels.Count != fruitCount)
			{
				_save.TicTacFruitRevels = new List<TicTacFruitRevelData>(fruitCount);
				for (int i = 0; i < fruitCount; i++)
					_save.TicTacFruitRevels.Add(new TicTacFruitRevelData());
			}

			for (int i = 0; i < fruitCount; i++)
			{
				var rule = _currentChest.requiredreveals[i];

				var reveal = Instantiate(TicTacRevealManager, RevelFruitParent);
				var rect = reveal.GetComponent<RectTransform>();
				rect.anchoredPosition = Vector2.zero;
				rect.localPosition = Vector3.zero;

				// 🔥 ONE FRUIT PER UI ITEM
				reveal.SetFruitSprites(
					rule.WinCount,
					rule.possibleFruit
				);

				reveal.SetText(
					rule.requiredReveals,
					_save.TicTacFruitRevels[i].CurrentRevel
				);

				_ticTacReveals.Add(reveal);
			}

			QuestionText.text = _save.SavedQuestionMark.ToString();
		}

		public void OnCellClicked(int index)
		{
			if (_save.IsComplete)
			{
				ToastMessage.Instance.Show("Tic tac game already completed.");
				return;
			}
			if (_save.SavedQuestionMark == 0)
			{
				ToastMessage.Instance.Show("Please collect question marks");
				return;
			}

			var data = _save.Cells[index];
			if (data.revealed) return;

			data.revealed = true;

			if (data.hasFruit)
				cells[index].Reveal(
					_currentChest.requiredreveals[data.fruitIndex].possibleFruit
				);
			else
				cells[index].RevealEmpty();

			_save.SavedQuestionMark--;
			RefreshUI();


			var (allDone, completedCount, newFruit) = _uITicTacController.CalculateFruitProgress();
			Debug.Log("CheckWin " + allDone);

			// 🔥 Update UI for the newly completed fruit
			if (newFruit >= 0)
			{
				var rule = _currentChest.requiredreveals[newFruit];
				RevealCompletedFruitOnGrid(newFruit);

				_ticTacReveals[newFruit].SetText(
					rule.requiredReveals,
					rule.requiredReveals // fully done
				);

				// Optional: play animation / sound here
			}

			// 🔥 Final win
			if (allDone)
			{
				_uITicTacController.ShowClaimButton(true);
			}

			_uITicTacController.Save();
		}

		public void RevealCompletedFruitOnGrid(int fruitIndex)
		{
			if (fruitIndex < 0)
				return;

			var rule = _currentChest.requiredreveals[fruitIndex];

			for (int i = 0; i < _save.Cells.Length; i++)
			{
				var cell = _save.Cells[i];

				if (!cell.revealed)
					continue;

				if (cell.fruitIndex == fruitIndex)
				{
					cells[i].Reveal(rule.possibleFruit);
				}
			}
		}

		public void RefreshUI()
		{
			QuestionText.text = _save.SavedQuestionMark.ToString();
		}
	}
}
