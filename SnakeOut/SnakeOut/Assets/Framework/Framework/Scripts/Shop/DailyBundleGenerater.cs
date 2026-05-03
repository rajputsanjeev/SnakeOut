using System;
using Framework.Core;
using UnityEngine;

namespace Framework
{
	public class DailyBundleGenerater : MonoBehaviour
	{
		public DailyBundleScriptableObject DailyBundle;
		public RectTransform Content;

		public void Start()
		{
			if (DailyBundle == null)
			{
				Debug.Log("Assgine daily bundle scriptable object");
				return;
			}
			GenerateBundle();
		}

		private void GenerateBundle()
		{
			foreach (var item in DailyBundle.DailyBundleData)
			{
				var dailyBundleUI = Instantiate(DailyBundle.UIPrefab, Content);
				dailyBundleUI.SetData(item, DailyBundle.CoinIcon, this);
			}
		}

		public void Purchase(DailyBundleData price)
		{
			var amount = CurrencyController.Get(CurrencyType.Coins);

			if (price.Price > amount)
			{
				Debug.LogError("Not have enoght coin");
				ToastMessage.Instance.Show("Not have enoght coin");
				return;
			}
			CurrencyController.Substract(CurrencyType.Coins, price.Price);
			RewardBaseHandle.Instance.SetReward(price.Reward);
		}
	}
}