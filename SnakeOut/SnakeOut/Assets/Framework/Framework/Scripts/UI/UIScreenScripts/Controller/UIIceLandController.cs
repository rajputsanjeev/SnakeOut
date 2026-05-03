using System;
using System.Collections.Generic;
using BaseView;
using DG.Tweening;
using Framework.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Controllers
{
	public class UIIceLandController : Behaviour<UIIceLandView>
	{
		public List<Image> LandImages = new List<Image>();
		public Sprite CompleteSprite;
		public Sprite InCompleteSprite;
		public List<RewardItem> Reward = new List<RewardItem>();

		private UIIceLandView m_View;
		private IceAdvantureCycleManager _manager;
		private BaseTimerHandle _baseTimeHandle;
		private IcelandAdvantacher _save => _manager.SaveData;

		protected override void Init()
		{
			base.Init();
			CacheReferences();
		}

		private void CacheReferences()
		{
			GetBaseView();
			m_View = (UIIceLandView)Prefab;

			_manager = GetComponent<IceAdvantureCycleManager>();
			_baseTimeHandle = GetComponent<BaseTimerHandle>();
		}

		public override void Subscribe()
		{
			base.Subscribe();
			CacheReferences();

			m_View.StartButton.onClick.AddListener(StartGame);
			m_View.ClaimButton.onClick.AddListener(ClaimReward);

			_manager.OnStateChanged += StartTimer;
		}

		private void StartGame()
		{
			CloseCurrentPanel();
			_manager.StartGame();
		}

		public override void ShowPanel(bool on)
		{
			if (_save.IsStarted)
			{
				var recttransform = LandImages[_save.CurrentStep].GetComponent<RectTransform>().anchoredPosition;
				var newPos = new Vector3(recttransform.x, recttransform.y + 150, 0);

				if (_save.CurrentStep == 0 || _save.CurrentStep == _save.LastStep)
				{
					m_View.Character.anchoredPosition = newPos;
				}
				else
				{
					m_View.Character.DOLocalJump(newPos, 200f, 1, 1);
				}
			}

			if (_save.CurrentStep == 4)
			{
				_manager.MarkCompleted();
			}
			StartButton();
			DebugExtension.Log(_save.CurrentStep.ToString(), "red");
			_save.LastStep = _save.CurrentStep;
		}

		private void StartButton()
		{
			m_View.StartButton.gameObject.SetActive(!_save.IsComplete && !_save.IsRewardCollected);
			m_View.ClaimButton.gameObject.SetActive(_save.IsComplete && !_save.IsRewardCollected);

			_manager.MainMenuButton.RefreshCircle(_save.IsComplete && !_save.IsRewardCollected, "1");
		}

		public override bool IsShow()
		{
			return (_manager.IsShow() && IsOfferScreen);
		}

		public void StartTimer()
		{
			if (_save.IsStarted && !_save.IsComplete)
			{
				_manager.SetClaimBtnState();
			}

			for (var i = 0; i < LandImages.Count; i++)
			{
				LandImages[i].sprite = _save.CurrentStep < i ? InCompleteSprite : CompleteSprite;
			}

			m_View.ProgressText.text = (_save.CurrentStep + 1) + "/5";

			StartButton();
			TimerManager.CreateTimer(_manager.GetTimeUntilNextReset(), OnTimerCompleted,
				x =>
				{
					var str = !_save.IsRewardCollected ? "Claim before " : "Come Back after \n ";

					_baseTimeHandle.UpdateTime(
					str + _baseTimeHandle.FormatSpan(x));
				},
				"Ice_Time");
		}

		private void OnTimerCompleted()
		{
			if (_currentComponent._isOn)
			{
				CloseCurrentPanel();
			}
			_manager.CheckDailyReset();
			_manager.Load();
			StartButton();
		}

		private void ClaimReward()
		{
			if (_save.IsComplete)
			{
				RewardBaseHandle.Instance.SetReward(Reward);
				_save.IsRewardCollected = true;
				_manager.CheckDailyReset();
				Save();
				StartButton();
			}
		}

		public void Save()
		{
			_manager.Save(_save);
		}
	}
}