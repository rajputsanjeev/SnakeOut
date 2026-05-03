using System;
using System.Collections.Generic;
using Framework.UI.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class PrivilegeStepView : MonoBehaviour
	{
		[Header("UI refs")]
		public TextMeshProUGUI stepNumberText;
		public Transform freeContainer;    // content container for free rewards
		public Transform paidContainer;    // content container for paid rewards
		public GameObject rewardItemPrefab; // prefab: contains Image "Icon" and TMP "Qty"
		public Image lockIcon;             // lock image to show when paid rewards locked
		public Image collectedFreeTick;        // check free mark image
		public Image collectedPaidTick;        // check paid mark image
		public Image slotBackground;       // background that changes color for current/normal/collected
		public GameObject[] ClaimButtons = new GameObject[2];
		public int CurrentIndex;

		// Colors for state
		public Color normalColor = Color.white;
		public Color currentColor = new Color(0.7f, 0.9f, 1f);
		public Color collectedColor = new Color(0.6f, 0.6f, 0.6f);

		private void Awake()
		{
			ClaimButtons[0].GetComponent<Button>().onClick.AddListener(ClaimFreeReward);
			ClaimButtons[1].GetComponent<Button>().onClick.AddListener(ClaimPaidReward);
		}

		private void ClaimFreeReward()
		{
			UIPrivilegeController.ClaimFreeRewardEvent?.Invoke(CurrentIndex);
		}

		private void ClaimPaidReward()
		{
			UIPrivilegeController.ClaimPaidRewardEvent?.Invoke(CurrentIndex);
		}

		// Render the step UI based on data and state
		public void Render(PrivilegeStepData stepData, int stepIndex, bool canCollectFree, bool canCollectPaid, bool isPaidActive, bool isCurrentSlot, bool isFreeReward, bool isPaidReward)
		{
			CurrentIndex = stepIndex;
			stepNumberText.text = (stepIndex + 1).ToString();

			collectedFreeTick.gameObject.SetActive(canCollectFree);
			collectedPaidTick.gameObject.SetActive(canCollectPaid);

			freeContainer.DestroyChildrenExcept(new List<string>() { "Ring" });
			paidContainer.DestroyChildrenExcept(new List<string>() { "Ring" });

			// set background
			if (canCollectFree) slotBackground.color = collectedColor;
			else if (isCurrentSlot) slotBackground.color = currentColor;
			else slotBackground.color = normalColor;

			// Populate free rewards
			if (stepData.freeRewards != null)
			{
				foreach (var r in stepData.freeRewards)
					CreateRewardItemView(r, freeContainer);
			}

			// Populate paid rewards
			if (stepData.paidRewards != null)
			{
				foreach (var r in stepData.paidRewards)
					CreateRewardItemView(r, paidContainer);
			}

			// Show lock: if user hasn't purchased, show lock on paidContainer region
			lockIcon.gameObject.SetActive(!isPaidActive /*&& stepData.paidRewards != null && stepData.paidRewards.Length > 0*/);
			UpdateClaimButtonsState((!canCollectFree && isFreeReward), !canCollectPaid && isPaidActive && isPaidReward);
		}

		private void CreateRewardItemView(RewardItem item, Transform parent)
		{
			if (rewardItemPrefab == null) return;
			var go = Instantiate(rewardItemPrefab, parent);
			var icon = go.GetComponent<LuckyLadderRewardIcon>();
			icon.SetData(item);
			UpdateClaimButtonsState(false);
		}


		public void UpdateClaimButtonsState(bool isFreeActive, bool isPaidActive = false)
		{
			ClaimButtons[0].gameObject.SetActive(isFreeActive);
			ClaimButtons[1].gameObject.SetActive(isPaidActive);

			if (isFreeActive || isPaidActive)
			{
				UIPrivilegeController.ClaimButtonActive?.Invoke(CurrentIndex);
			}
		}
	}
}