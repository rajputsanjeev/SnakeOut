using System;
using System.Collections.Generic;
using System.Linq;
using Base.Interfaces;
using Base.UI.Components;
using Base.UI.Manager;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Base.UI.Controller
{
	public enum PanelOpenMode
	{
		Always,
		DailyLimited,
		TimeGated
	}
	public abstract class UIBaseController : MonoBehaviour
	{
		public Color DebugColor = Color.whiteSmoke;
		public bool IsShowDebug;
		public bool IsBackgroundClose;
		public bool IsOfferScreen;

		[Header("Offer Rules")]
		public PanelOpenMode OpenMode = PanelOpenMode.Always;

		[Tooltip("Used only for DailyLimited")]
		public int MaxOpenPerDay = 2;

		[Tooltip("Used only for TimeGated (minutes)")]
		public int MinGapMinutes = 30;

		public Button _closeButton;

		protected UIPanelManager UIPanelManager => UIPanelManager.Instance;
		protected IGameEventListener gameEventListener;
		protected List<Animator> UIAnimators = new List<Animator>();
		protected PopupAnimator PopupAnimator;
		protected HomeUIAnimator HomeUIAnimator;
		protected UIPanelComponent _currentComponent;
		private Button _backGround;

		public virtual void OnInit()
		{
			if (IsBackgroundClose)
			{
				_backGround = transform.Find("Background").GetComponent<Button>();
				if (_backGround != null) _backGround.onClick.AddListener(BackMainMenu);
				GetRefreshes();
			}

			if (_closeButton != null) _closeButton.onClick.AddListener(BackMainMenu);
		}

		private void GetRefreshes()
		{
			UIAnimators = GetComponentsInChildren<Animator>().ToList();
			_currentComponent = GetComponent<UIPanelComponent>();
			PopupAnimator = GetComponent<PopupAnimator>();
			HomeUIAnimator = FindAnyObjectByType<HomeUIAnimator>();
		}

		public virtual void SetGameManagerListner(IGameEventListener gameEventListener)
		{
			this.gameEventListener = gameEventListener;
		}

		public abstract void ShowPanel(bool on);

		public void Enable()
		{
			_closeButton.interactable = true;
			if (_backGround != null) _backGround.interactable = true;
		}

		protected virtual void Awake()
		{
			GetRefreshes();
		}

		public virtual void BackMainMenu()
		{
			_closeButton.interactable = false;
			if (_backGround != null) _backGround.interactable = false;
			CloseCurrentPanel();
		}

		public virtual void OpenCurrentPanel()
		{
			if (_currentComponent == null)
			{
				GetRefreshes();
			}

			if (UIPanelManager == null)
			{
				var uIpanelManager = FindAnyObjectByType<UIPanelManager>();
				uIpanelManager.ShowPanel(_currentComponent.panelType, true);
			}
			else
			{
				UIPanelManager.ShowPanel(_currentComponent.panelType, true);
				var uitoolKit = GetComponentsInChildren<UIEffectsToolkit>();
				foreach (var toolkit in uitoolKit)
				{
					toolkit.PlayCurrentEffect();
				}
			}
		}

		public virtual void CloseCurrentPanel()
		{
			if (_currentComponent == null)
			{
				GetRefreshes();
			}

			if (UIPanelManager == null)
			{
				var uIpanelManager = FindAnyObjectByType<UIPanelManager>();
				uIpanelManager.ShowPanel(_currentComponent.panelType, true);
			}
			else
				UIPanelManager.ShowPanel(_currentComponent.panelType, false);
		}

		public void Log(string message)
		{
			if (IsShowDebug) DebugExtension.Log(message, DebugColor);
		}

		public virtual void Subscribe()
		{

		}

		public abstract bool IsShow();

		public bool CanShow()
		{
			if (!IsShow())
				return false;

			if (!IsOfferScreen)
				return false;

			switch (OpenMode)
			{
				case PanelOpenMode.Always:
					return true;

				case PanelOpenMode.DailyLimited:
					return UIPanelOfferSystem.CanOpenToday(MaxOpenPerDay);

				case PanelOpenMode.TimeGated:
					return UIPanelOfferSystem.CanOpenByTime(MinGapMinutes);
			}

			return false;
		}

	}
}