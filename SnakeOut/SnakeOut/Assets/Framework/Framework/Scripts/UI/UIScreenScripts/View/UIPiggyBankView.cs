using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class UIPiggyBankView : View
    {
		[Header("Coins")]
		public TextMeshProUGUI PiggyCoinsText;
		public Image PiggyBankFill;

		[Header("Buttons")]
		public Button ClaimButton;
		public Image claimBtnImage;

		[Header("Extras")]
		public GameObject FullIndicator;

		[Header("Icons")]
		public Image PiggyIcon;
		public Image CoinIcon;

		[Header("Pricing")]
		public TextMeshProUGUI PriceText;
	}
}