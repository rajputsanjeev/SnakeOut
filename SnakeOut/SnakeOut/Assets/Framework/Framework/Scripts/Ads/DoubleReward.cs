using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Framework.Core;

namespace Framework
{
	public class DoubleReward : MonoBehaviour
	{
		public UnityEvent OnAdsComplete;
		public Action OnActionAdComplete;
		public string AnalyticsEvent = "Default";
		private Button DoublePriceButton;

		private void Awake()
		{
			DoublePriceButton = GetComponent<Button>();
			DoublePriceButton.onClick.AddListener(ShowRewardAds);
		}

		public void ShowRewardAds()
		{
			AdsManager.ShowRewardBasedVideo((hasReward) =>
			{
				if (hasReward)
				{
					Debug.Log("[AdsManager]: Reward is received");

					OnAdsComplete?.Invoke();
					OnActionAdComplete?.Invoke();
				}
				else
				{
					Debug.Log("[AdsManager]: Reward isn't received");
				}
			}, AnalyticsEvent);
		}
	}
}
