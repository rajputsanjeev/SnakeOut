using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.UI.Manager;
using BaseView;
using Framework;
using Framework.Core;
using ManuGames.UI.Components;
using NiobiumStudios;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ManuGames.UI.Controllers
{
	public class UIDailyRewardController : Behaviour<UIDailyRewardView>
	{
		public List<RewardUIController> DailyRewardsUI = new List<RewardUIController>();
		public Action<bool> OnRewardChecked;
		public DoubleReward DoubleRewardRef;

		[Header("Panel Debug")]
		public bool isDebug;

		public LuckyLadderRewardIcon FinalReward;
		public ProgressBarView ProgressBarView;
		public MainMenuButtons MainMenuButton;

		private UIDailyRewardView m_View;
		private bool readyToClaim;                  // Update flag
		private DailyRewards dailyRewards;          // DailyReward Instance      
		private bool _isWatchAds;
		private bool _isClamed; //if user clamed the reward today

		protected override void Init()
		{
			base.Init();
			m_View = (UIDailyRewardView)Prefab;
			dailyRewards = GetComponent<DailyRewards>();
			DailyRewardsUI = GetComponentsInChildren<RewardUIController>(true).ToList();
			if (MainMenuButton != null) MainMenuButton.AssgineAction(OpenCurrentPanel);
		}

		private void OnCheckedReward(bool isRewardAvaliable)
		{
			UIPanelManager.ShowPanel(Panel.REWARDED_SCREEN, isRewardAvaliable);
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
		}

		private void OnEnable()
		{
			dailyRewards.onClaimPrize += OnClaimPrize;
			dailyRewards.onInitialize += OnInitialize;
		}

		private void OnDisable()
		{
			if (dailyRewards != null)
			{
				dailyRewards.onClaimPrize -= OnClaimPrize;
				dailyRewards.onInitialize -= OnInitialize;
			}
		}

		private void Start()
		{
			InitializeDailyRewardsUI();
			DoubleRewardRef.OnActionAdComplete += OnAdComplete;
			m_View.ButtonClose.gameObject.SetActive(false);

			m_View.ButtonClaim.onClick.AddListener(() =>
			{
				OnClaimButtonClick(false);
			});

			DebugsRewards();
		}

		private void OnAdComplete()
		{
			OnClaimButtonClick(true);
		}

		private void OnClaimButtonClick(bool isDouble)
		{
			readyToClaim = false;
			_isWatchAds = isDouble;
			dailyRewards.ClaimPrize();
			UpdateUI();
		}

		private void DebugsRewards()
		{
			if (m_View.panelDebug)
				m_View.panelDebug.SetActive(isDebug);

			// Simulates the next Day
			if (m_View.buttonAdvanceDay)
				m_View.buttonAdvanceDay.onClick.AddListener(() =>
				{
					dailyRewards.debugTime = dailyRewards.debugTime.Add(new TimeSpan(1, 0, 0, 0));
					UpdateUI();
				});

			// Simulates the next hour
			if (m_View.buttonAdvanceHour)
				m_View.buttonAdvanceHour.onClick.AddListener(() =>
				{
					dailyRewards.debugTime = dailyRewards.debugTime.Add(new TimeSpan(1, 0, 0));
					UpdateUI();
				});

			if (m_View.buttonReset)
				// Resets Daily Rewards from Player Preferences
				m_View.buttonReset.onClick.AddListener(() =>
				{
					dailyRewards.Reset();
					dailyRewards.debugTime = new TimeSpan();
					dailyRewards.lastRewardTime = System.DateTime.MinValue;
					readyToClaim = false;
				});

			// Reloads the same scene
			if (m_View.buttonReloadScene)
				m_View.buttonReloadScene.onClick.AddListener(() =>
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				});
		}

		// Initializes the UI List based on the rewards size
		private void InitializeDailyRewardsUI()
		{
			if (dailyRewards.CurrentRewardSet.Count == 0)
			{
				Debug.Log("CurrentRewardSet is null ");
				dailyRewards.SetCurrentRewardSet();
			}

			for (int i = 0; i < dailyRewards.CurrentRewardSet.Count; i++)
			{
				var reward = dailyRewards.GetRewardList(i);
				int day = i + 1;

				DailyRewardsUI[i].Day = day;
				DailyRewardsUI[i].SetImages(reward);
				DailyRewardsUI[i].Initialize();
			}

			FinalReward.SetData(dailyRewards.GetFinalReward());
		}

		public void UpdateUI()
		{
			dailyRewards.CheckRewards();

			bool isRewardAvailableNow = false;

			var lastReward = dailyRewards.lastReward;
			var availableReward = dailyRewards.availableReward;

			foreach (var dailyRewardUI in DailyRewardsUI)
			{
				var day = dailyRewardUI.Day;

				if (day == availableReward)
				{
					dailyRewardUI.State = RewardUIController.DailyRewardState.UNCLAIMED_AVAILABLE;

					isRewardAvailableNow = true;
				}
				else if (day <= lastReward)
				{
					dailyRewardUI.State = RewardUIController.DailyRewardState.CLAIMED;
				}
				else
				{
					dailyRewardUI.State = RewardUIController.DailyRewardState.UNCLAIMED_UNAVAILABLE;
				}

				dailyRewardUI.Refresh();
			}

			m_View.ButtonClaim.gameObject.SetActive(isRewardAvailableNow);

			if (DoubleRewardRef != null)
			{
				DoubleRewardRef.gameObject.SetActive(isRewardAvailableNow);
			}
			OnRewardChecked?.Invoke(isRewardAvailableNow || _isClamed);
			//OnCheckedReward(isRewardAvailableNow || _isClamed);

			m_View.ButtonClose.gameObject.SetActive(!isRewardAvailableNow);
			if (isRewardAvailableNow)
			{
				SnapToReward();
				m_View.TextTimeDue.text = "You can claim your reward!";
			}
			readyToClaim = isRewardAvailableNow;

			UpdateProgressBar();
		}

		// Snap to the next reward
		public void SnapToReward()
		{
			Canvas.ForceUpdateCanvases();

			var lastRewardIdx = dailyRewards.lastReward;

			// Scrolls to the last reward element
			if (DailyRewardsUI.Count - 1 < lastRewardIdx)
				lastRewardIdx++;

			if (lastRewardIdx > DailyRewardsUI.Count - 1)
				lastRewardIdx = DailyRewardsUI.Count - 1;

			var target = DailyRewardsUI[lastRewardIdx].GetComponent<RectTransform>();
		}

		private void CheckTimeDifference()
		{
			if (!readyToClaim)
			{
				TimeSpan difference = dailyRewards.GetTimeDifference();

				// If the counter below 0 it means there is a new reward to claim
				if (difference.TotalSeconds <= 0)
				{
					readyToClaim = true;
					UpdateUI();
					SnapToReward();
					return;
				}

				string formattedTs = dailyRewards.GetFormattedTime(difference);

				m_View.TextTimeDue.text = string.Format("Come back in {0} for your next reward", formattedTs);
			}
		}

		// Delegate
		private void OnClaimPrize(int day)
		{
			var reward = dailyRewards.GetRewardList(day - 1);

			if (day == 7)
			{
				reward.Add(dailyRewards.GetFinalReward());
			}

#if MODULE_UNITY_LOCAL_NOTIFICATION && (UNITY_ANDROID || UNITY_IOS)
			NotificationManager.Instance.Trigger("DailyReward");
#endif

			RewardBaseHandle.Instance.SetReward(reward, _isWatchAds);
			_isClamed = true;
			_isWatchAds = false;
			Debug.Log("You claim the price " + " is double " + _isWatchAds);
		}

		private void OnInitialize(bool error, string errorMessage)
		{
			if (!error)
			{
				var showWhenNotAvailable = dailyRewards.keepOpen;
				var isRewardAvailable = dailyRewards.availableReward > 0;

				UpdateUI();
				UIPanelManager.ShowPanel(Panel.REWARDED_SCREEN, showWhenNotAvailable || (!showWhenNotAvailable && isRewardAvailable));

				UpdateProgressBar();

				CheckTimeDifference();
				StartCoroutine(TickTime());
			}
		}

		private void UpdateProgressBar()
		{
			ProgressBarView.Init(7);
			ProgressBarView.Value(dailyRewards.GetRemainingDays());
		}

		private IEnumerator TickTime()
		{
			for (; ; )
			{
				dailyRewards.TickTime();
				// Updates the time due
				CheckTimeDifference();
				yield return null;
			}
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}