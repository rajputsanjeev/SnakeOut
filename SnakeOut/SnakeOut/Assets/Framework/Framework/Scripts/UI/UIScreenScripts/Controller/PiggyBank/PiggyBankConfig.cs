using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "PiggyBankConfig", menuName = "PiggyBank/PiggyBankData")]
	public class PiggyBankConfig : ScriptableObject
	{
		[Header("Level Based Trigger")]
		public RequiredLevelScriptableObject RequiredLevel;

		[Header("Offer Duration")]
		public int offerDurationSecond = 3;
		public int offerDurationHMin = 3;
		public int offerDurationHours = 3;
		public int offerDurationDays = 3;

		[Header("Reappearance After Offer End")]
		public int reappearAfterOfferEndSecond = 5;
		public int reappearAfterOfferEndMin = 5;
		public int reappearAfterOfferEndHours = 5;
		public int reappearAfterOfferEndDays = 5;

		[Header("Reappearance After Purchase")]
		public int reappearAfterPurchaseSecond = 7;
		public int reappearAfterPurchaseMin = 7;
		public int reappearAfterPurchaseHours = 7;
		public int reappearAfterPurchaseDays = 7;
		public bool showOnlyOnce = false;

		[Header("Coin add after level win")]
		public int levelWinCoin = 100;

		[Header("Coins Requirement")]
		public int baseMaxCoins = 5000;
		public int maxCoinsIncreasePerPurchase = 2000;

		[Header("Pricing")]
		public float basePrice = 200f;
		public float baseDiscountPercent = 50f;
		public float priceIncreaseFactor = 1.5f;

		[Header("UI Config")]
		public Sprite piggyIcon;
		public Sprite coinIcon;
		public Sprite grayBtnIcon;
		public Sprite greenBtnIcon;

	}
}