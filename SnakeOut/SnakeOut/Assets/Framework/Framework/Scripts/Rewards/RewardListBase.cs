using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
	public class RewardListBase : ScriptableObject
	{
		public List<RewardStepData> steps = new List<RewardStepData>(); // exactly 6 steps

		public List<Sprite> GetRewardSprite()
		{
			var icons = steps
	.SelectMany(step => step.items)
	.Select(item => item.icon)
	.ToList();

			return icons;
		}

		public List<string> GetRewardText()
		{
			var text = steps
	.SelectMany(step => step.items)
	.Select(item => item.quantity.ToString())
	.ToList();

			return text;
		}
	}
}