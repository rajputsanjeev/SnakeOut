using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
	public class LadderStepView : MonoBehaviour
	{
		public enum StepState
		{
			UnCollect,
			Collect,
			Current
		}
		public StepState CurrentState;
		public UnityEvent OnCollected;
		public UnityEvent OnCurrentInvoke;
		public UnityEvent NotCollected;
		public Sprite CurrentSprite;
		public Sprite UnCollectSprite;
		public Sprite CollectSprite;
		public List<GameObject> LeftRightSprite = new List<GameObject>();

		private Image _backgroundImage;
		private List<LuckyLadderRewardIcon> RewardIcon = new();

		// Render RewardStepData (shows up to 4 items)
		public void Render(RewardStepData stepData)
		{
			_backgroundImage = GetComponent<Image>();
			RewardIcon = GetComponentsInChildren<LuckyLadderRewardIcon>(true).ToList();

			// clear first
			for (int i = 0; i < RewardIcon.Count; i++)
			{
				if (RewardIcon[i].itemQtyTexts != null) RewardIcon[i].itemQtyTexts.text = "";
			}

			if (stepData == null) return;

			int count = Mathf.Min(stepData.items.Count, 4);
			for (int i = 0; i < count; i++)
			{
				var rw = stepData.items[i];
				if (RewardIcon[i].itemIcons != null && RewardIcon[i].itemQtyTexts != null && rw.icon != null)
				{
					RewardIcon[i].gameObject.SetActive(true);
					RewardIcon[i].SetData(rw);
				}
			}
		}

		public void OnChangeRender(bool isCollected)
		{
			if (isCollected)
			{
				OnCollected?.Invoke();
				CurrentState = StepState.Collect;
				ChangeSprite(CurrentState);
			}
			else
			{
				NotCollected?.Invoke();
				CurrentState = StepState.UnCollect;
				ChangeSprite(CurrentState);
			}
		}

		public void ChangeSprite(StepState stepState)
		{
			if (_backgroundImage == null)
			{
				_backgroundImage = GetComponent<Image>();
			}
			switch (stepState)
			{
				case StepState.Collect:
					_backgroundImage.sprite = CollectSprite;
					break;
				case StepState.UnCollect:
					_backgroundImage.sprite = UnCollectSprite;
					break;
				case StepState.Current:
					_backgroundImage.sprite = CurrentSprite;
					break;
			}

			if (LeftRightSprite.Count > 0)
			{
				foreach (var item in LeftRightSprite)
				{
					item.gameObject.SetActive(stepState == StepState.Current);
				}
			}
		}

		public void OnCurrentReward()
		{
			OnCurrentInvoke?.Invoke();
			CurrentState = StepState.Current;
			ChangeSprite(CurrentState);

		}
	}
}