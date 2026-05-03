using System;
using System.Collections.Generic;
using BaseView;
using Framework.UI.Components;
using UnityEngine;

namespace Framework.UI.Controllers
{
	[Serializable]
	public class SlotMachineReward : RewardStepData
	{
		public float Probability;
		public float RewardId;
	}

	public class UISlotMechanismController : Behaviour<UISlotMechanismView>
	{
		private UISlotMechanismView m_View;
		[Header("Reels")]
		public SlotMechanism freeReel;
		public SlotMechanism paidReel;

		[Header("Rewards")]
		public SlotRewardSet freeRewards;
		public SlotRewardSet paidRewards;

		protected override void Init()
		{
			base.Init();
			m_View = (UISlotMechanismView)Prefab;

			freeReel.Setup(GetSprites(freeRewards));
			paidReel.Setup(GetSprites(paidRewards));
		}

		public override void ShowPanel(bool on)
		{
		}

		public void OnSpinClicked()
		{
			int freeIndex = SlotProbabilityPicker.PickIndex(
				freeRewards.Rewards, r => r.Probability);

			int paidIndex = SlotProbabilityPicker.PickIndex(
				paidRewards.Rewards, r => r.Probability);

			freeReel.SpinToIndex(freeIndex, () =>
			{
				Debug.Log("FREE Reward: " + freeRewards.Rewards[freeIndex].RewardId);
			});

			paidReel.SpinToIndex(paidIndex, () =>
			{
				Debug.Log("PAID Reward: " + paidRewards.Rewards[paidIndex].RewardId);
				// Rewarded Ad / Purchase logic
			});
		}

		private List<Sprite> GetSprites(SlotRewardSet rewards)
		{
			List<Sprite> list = new();

			foreach (var rw in rewards.Rewards)
			{
				foreach (var item in rw.items)
				{
					list.Add(item.icon);
				}
			}
			return list;
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}