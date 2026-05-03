using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Framework
{
	public class DailyBundleUIController : MonoBehaviour
	{
		public Image CoinImage;
		public Image Icon;
		public TextMeshProUGUI Price;
		public TextMeshProUGUI Quantity;
		public Button PurchaseButton;
		private DailyBundleGenerater dailyBundleGenerater;
		private DailyBundleData BundleData;

		private void OnPurchaseClicked()
		{
			dailyBundleGenerater.Purchase(BundleData);
		}

		public void SetData(DailyBundleData dailyBundleData, Sprite coinSprite, DailyBundleGenerater _bundleGenerater)
		{
			CoinImage.sprite = coinSprite;
			Icon.sprite = dailyBundleData.Reward.icon;
			Price.text = dailyBundleData.Price.ToString();
			Quantity.text = dailyBundleData.Reward.quantity.ToString() + "x";
			BundleData = dailyBundleData;
			dailyBundleGenerater = _bundleGenerater;
			if (PurchaseButton != null) PurchaseButton.onClick.AddListener(OnPurchaseClicked);
		}
	}
}