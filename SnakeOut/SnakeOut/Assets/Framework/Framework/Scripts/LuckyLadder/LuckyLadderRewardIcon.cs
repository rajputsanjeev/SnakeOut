using Framework.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class LuckyLadderRewardIcon : MonoBehaviour
	{
		public Image itemIcons;
		public TextMeshProUGUI itemQtyTexts;
		private string durationFormat = "{mm}min";

		public void SetData(Sprite icon, string quantity)
		{
			itemIcons.sprite = icon;
			itemQtyTexts.text = quantity.ToString() + "x";
		}

		public void SetData(RewardItem item)
		{
			itemIcons.sprite = item.icon;
			if (item.type == RewardType.Power4)
			{
				float min = (float)item.quantity;
				itemQtyTexts.text = TimeUtils.GetFormatedTime(min, durationFormat);
			}
			else
			{
				itemQtyTexts.text = item.quantity.ToString() + "x";
			}
		}
	}
}


