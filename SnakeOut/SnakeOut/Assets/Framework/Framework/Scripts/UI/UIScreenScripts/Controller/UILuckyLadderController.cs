using System;
using BaseView;
using Framework.Core;
using Framework.UI.Components;
using UnityEngine;
using static Framework.LadderStepView;

namespace Framework.UI.Controllers
{
	public class UILuckyLadderController : Behaviour<UILuckyLadderView>
	{
		public LadderStepView[] stepViews = new LadderStepView[6];
		public GameObject ClaimButtonWithAds;
		public GameObject ClaimButtonWithoutAds;
		public LuckyLadderRewardIcon FinalRewardView;

		private ProgressBarView _progressBarView;
		private LuckyLadderManager _manager;
		private BaseTimerHandle _baseTimeHandle;
		private DoubleReward _doubleReward;
		private UILuckyLadderView m_View;
		private int _currentSet => _manager.GetCurrentStep();
		private bool _allCollected => _manager.IsAllCurrentSetCollected();
		private bool _finalCollected => _manager.IsFinalRewCollected();
		private bool _finalAndAllCollected => _manager.IsBothRewardCollected();

		public override void OnInit()
		{
			base.OnInit();
			CacheReferences();
		}

		private void CacheReferences()
		{
			GetBaseView();
			m_View = (UILuckyLadderView)Prefab;

			_doubleReward = GetComponentInChildren<DoubleReward>(true);
			_baseTimeHandle = GetComponent<BaseTimerHandle>();
			_manager = GetComponent<LuckyLadderManager>();
			_progressBarView = GetComponent<ProgressBarView>();
		}

		protected override void Init()
		{
			base.Init();
			CacheReferences();
		}

		public override void Subscribe()
		{
			base.Subscribe();
			CacheReferences();

			m_View.ClaimWithoutAds.onClick.AddListener(OnClaimReward);
			_doubleReward.gameObject.SetActive(!_manager.IsAllCurrentSetCollected());

			_doubleReward.OnActionAdComplete += OnClaimReward;
			_manager.OnStateChanged += StartTimer;
			_manager.OnStateChanged += RefreshUI;
		}

		public override bool IsShow()
		{
			return (_manager.IsShow() && IsOfferScreen);
		}

		public void StartTimer()
		{
			TimerManager.CreateTimer(_manager.GetTimeUntilNextReset(), OnTimerCompleted,
				x =>
				{
					var str = !_finalAndAllCollected ? "Claim before " : "Come Back after \n ";

					_baseTimeHandle.UpdateTime(
						 str + _baseTimeHandle.FormatSpan(x));
				},
				"LuckyLadder");
		}

		private void OnTimerCompleted()
		{
			Log("[Lucky Ladder] Completed → refreshing state");
			if (_currentComponent._isOn)
			{
				CloseCurrentPanel();
			}
			_manager.CheckDailyReset();
		}

		private void OnDisable()
		{
			if (_manager != null)
			{
				_manager.OnStateChanged -= RefreshUI;
			}
		}

		public void RefreshUI()
		{
			if (_manager == null)
			{
				CacheReferences();
			}

			var set = _manager.GetActiveSet();

			UpdateProgressBar();

			// CASE 1: Not all steps collected yet
			if (!_allCollected)
			{
				// Can collect WITHOUT ads
				bool canCollectWithoutAds = set.FreeNumber >= _currentSet;

				ClaimButtonWithoutAds.SetActive(canCollectWithoutAds);
				ClaimButtonWithAds.SetActive(!canCollectWithoutAds);
			}
			// CASE 2: All collected but final reward NOT collected
			else if (!_finalCollected)
			{
				// Reset sprites to UnCollect
				foreach (var step in stepViews)
					step.ChangeSprite(StepState.UnCollect);

				ClaimButtonWithAds.SetActive(false);
				ClaimButtonWithoutAds.SetActive(true);
			}
			// CASE 3: Everything collected (final reward also collected)
			else
			{
				ClaimButtonWithAds.SetActive(false);
				ClaimButtonWithoutAds.SetActive(false);
			}

			var collected = _manager.GetCollectedArray();
			for (int i = 0; i < 6; i++)
			{
				var step = set != null ? set.GeteRewardStep(i) : null;
				stepViews[i].Render(step);

				// apply claimed overlay or greyscale if collected
				bool claimed = (i < collected.Length && collected[i]);

				stepViews[i].OnChangeRender(claimed);
			}
			FinalRewardView.SetData(set.FinalRewardSet.items[0]);

			if (!_manager.IsAllCurrentSetCollected())
			{
				stepViews[_currentSet].OnCurrentReward();
			}
			RefreshClaimText();
		}

		private void UpdateProgressBar()
		{
			_progressBarView.Init(6);
			_progressBarView.Value((float)_currentSet);
		}

		private void OnClaimReward()
		{
			if (_finalAndAllCollected)
			{
				// No reward
				ClaimButtonWithAds.gameObject.SetActive(false);
				ClaimButtonWithoutAds.gameObject.SetActive(false);
				Log($"[LuckyLadder][finalAndAllCollected] " + _finalAndAllCollected);
				return;
			}

			var rewardStepData = new RewardStepData();
			if (!_allCollected)
			{
				rewardStepData = _manager.TryCollectCurrentStep();
			}
			else if (!_finalCollected)
			{
				rewardStepData = _manager.GetFinalRewardSet();
			}
			else
			{
				Debug.LogError("No Reward");
				return;
			}

			RewardBaseHandle.Instance.SetReward(rewardStepData);
			UpdateProgressBar();

#if MODULE_UNITY_LOCAL_NOTIFICATION && (UNITY_ANDROID || UNITY_IOS)
			NotificationManager.Instance.Trigger("LuckyLadderReward");
#endif

			ClaimButtonWithAds.gameObject.SetActive(!_allCollected);
			ClaimButtonWithoutAds.gameObject.SetActive(_allCollected && !_finalCollected);

			RefreshClaimText();
		}

		private void RefreshClaimText()
		{
			_manager.MainMenuButton.RefreshCircle(!_manager.IsBothRewardCollected(), _manager.GetRemainReward().ToString());
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
		}
	}
}