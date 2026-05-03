using System;
using System.Collections;
using Base.UI.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	[RequireComponent(typeof(CanvasGroup))]
	public class PackUIBinder : MonoBehaviour
	{
		public bool hideWhenNoPack = true;
		public bool buildAtRuntime = true;

		[Header("Runtime references")]
		public PackManager packManager;

		[Header("UI targets")]
		public Image backgroundImage;    // panel background
		public Image upperImage;         // top banner sprite
		public Image bottomImage;        // bottom button sprite
		public Image badgeImage;         // discount badge
		public TextMeshProUGUI titleText;
		public TextMeshProUGUI priceText;
		public TextMeshProUGUI offPriceText;

		[Header("Other Refrence")]
		public Transform rewardsContainerPowerUps;
		public LuckyLadderRewardIcon coinReward;
		public LuckyLadderRewardIcon rewardItemPrefab; // prefab with icon + text (create a small prefab)

		private SeasonPack currentPack;
		private SaleTimerUI _saleTimerUI;

		private void Awake()
		{
			if (packManager == null)
			{
				Debug.LogWarning("PackManager not assigned to PackUIBinder.");
			}
			_saleTimerUI = GetComponent<SaleTimerUI>();
		}

		public void BuildUI()
		{
			// initial bind
			currentPack = packManager.ActivePack;
			BindPack(currentPack);
		}

		private void OnEnable()
		{
			if (packManager != null)
			{
				packManager.OnActivePackChanged += OnPackChanged;
			}
		}

		private void OnDisable()
		{
			if (packManager != null)
				packManager.OnActivePackChanged -= OnPackChanged;
			_saleTimerUI.StopCountdown();
		}

		private void OnPackChanged(SeasonPack pack)
		{
			BindPack(pack);
		}

		private void BindPack(SeasonPack pack)
		{
			currentPack = pack;
			if (pack == null)
			{
				if (hideWhenNoPack) gameObject.SetActive(false);
				_saleTimerUI.StopCountdown();
				return;
			}

			gameObject.SetActive(true);

			if (backgroundImage != null) backgroundImage.sprite = pack.backgroundSprite;
			if (upperImage != null) upperImage.sprite = pack.upperSprite;
			if (bottomImage != null) bottomImage.sprite = pack.bottomSprite;
			if (badgeImage != null) badgeImage.sprite = pack.badgeSprite;

			if (titleText != null) titleText.text = pack.displayName;
			if (offPriceText != null) offPriceText.text = pack.offPrice > 0 ? pack.offPrice.ToString() + "% OFF" : "";

			if (buildAtRuntime) PopulateRewards(pack);

			// start countdown
			_saleTimerUI.StartTimer(pack);
		}

		private void PopulateRewards(SeasonPack pack)
		{
			if (rewardsContainerPowerUps == null || rewardItemPrefab == null) return;

			rewardsContainerPowerUps.DestroyChild();

			foreach (var r in pack.SmallItemReward.steps[0].items)
			{
				var go = Instantiate(rewardItemPrefab, rewardsContainerPowerUps);
				go.SetData(r);
			}

			foreach (var r in pack.CoinReward.steps[0].items)
			{
				coinReward.SetData(r);
			}
		}

#if UNITY_EDITOR
		// Expose a helper for editor preview
		public void EditorBindPreview(SeasonPack pack)
		{
			BindPack(pack);
		}
#endif
	}
}
