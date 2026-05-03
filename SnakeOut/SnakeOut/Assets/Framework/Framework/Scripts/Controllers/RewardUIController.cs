using System.Collections.Generic;
using NiobiumStudios;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class RewardUIController : MonoBehaviour
	{
		public bool showRewardName;

		[Header("UI Elements")]
		public TextMeshProUGUI TextDay;                // Text containing the Day text eg. Day 12
		public Image CheckMarkImage; // The Reward Image Background
		public List<LuckyLadderRewardIcon> LuckyLadderRewardIconList = new();
		public RectTransform Lock;

		[Header("Internal")]
		public int Day;

		[HideInInspector]
		public Reward Reward;

		public DailyRewardState State;

		// The States a reward can have
		public enum DailyRewardState
		{
			UNCLAIMED_AVAILABLE,
			UNCLAIMED_UNAVAILABLE,
			CLAIMED
		}

		public void Initialize()
		{
			TextDay.text = string.Format("Day {0}", Day.ToString());
			Lock = transform.Find("Lock").GetComponent<RectTransform>();
		}

		public void SetImages(List<RewardItem> rewardItems)
		{
			if (LuckyLadderRewardIconList.Count < rewardItems.Count)
			{
				Debug.LogError("Reward item high " + LuckyLadderRewardIconList.Count + " UI count rewardItems count " + rewardItems.Count + " day" + Day);
			}
			for (int j = 0; j < LuckyLadderRewardIconList.Count; j++)
			{
				LuckyLadderRewardIconList[j].gameObject.SetActive(false);
			}

			for (int j = 0; j < rewardItems.Count; j++)
			{
				LuckyLadderRewardIconList[j].SetData(rewardItems[j]);
				LuckyLadderRewardIconList[j].gameObject.SetActive(true);
			}
		}

		// Refreshes the UI
		public void Refresh()
		{
			switch (State)
			{
				case DailyRewardState.UNCLAIMED_AVAILABLE:
					CheckMarkImage.gameObject.SetActive(false);
					Lock.gameObject.SetActive(false);
					break;
				case DailyRewardState.UNCLAIMED_UNAVAILABLE:
					CheckMarkImage.gameObject.SetActive(false);
					Lock.gameObject.SetActive(true);
					break;
				case DailyRewardState.CLAIMED:
					CheckMarkImage.gameObject.SetActive(true);
					Lock.gameObject.SetActive(false);
					break;
			}
		}
	}
}
