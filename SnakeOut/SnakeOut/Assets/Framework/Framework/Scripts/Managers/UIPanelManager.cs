using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.Controller;
using Base.UI.Components;
using Base.UI.Controller;
using DG.Tweening;
using Frameork;
using Framework;
using UnityEngine;

namespace Base.UI.Manager
{
	public enum Panel
	{
		HOME,
		SETTINGS,
		CART_SCREEN,
		REMOVE_AD_SCREEN,
		REWARDED_SCREEN,
		REMOVE_AD_30MIN_SCREEN,
		LANGUAGE_SCREEN,
		SETTING_SCREEN,
		NO_COIN_SCREEN,
		PIGGY_BANK_SCREEN,
		SPIN_WHEEL,
		LUCKY_LADDER,
		PRIVILAGE,
		SALE_SEASON,
		SALE_WELCOME,
		SALE_WEEKEND,
		PRIVILEGE_PURCHASE_SCREEN,
		TIC_TAC,
		ICE_LAND,
		Theme
	}

	public class UIPanelManager : BaseGameController
	{
		public static UIPanelManager Instance;
		public static bool HasRewardCollection = false;
		public static bool HasOffersAnimationPlayed = false;
		public static bool IsRewardFlowAnimation = false;

		public bool IsPanelOpened { get; private set; }

		public List<UIPanelComponent> PanelList => m_PanelsList;

		public Panel StartingPanel;
		public Panel CurrentPanel => m_LastActivePanel.panelType;
		public Panel CurrentScreen => m_LastActiveScreen.panelType;

		[Header("Offer Queue")]
		public bool EnableOfferQueue = true;
		public int MaxPanelsPerSession = 2;
		public float OfferStartDelay = 3f;

		[Header("Reward Flow")]
		public bool IsWaitRewardFlow;

		// 🔥 LIST instead of Queue (visible in Inspector)
		[SerializeField] private List<UIPanelComponent> panelQueue = new List<UIPanelComponent>();

		[SerializeField] private List<UIPanelComponent> m_PanelsList;
		[SerializeField] private List<UIPanelComponent> m_PanelsListOutSideScreen;

		[Tooltip("Add a panel here if it will be on the scene from starting by default")]
		public UIPanelComponent m_LastActivePanel { get; private set; }

		private UIPanelComponent m_LastActiveScreen;
		private List<UIBaseController> m_ControllerList;

		private int _sessionOpenCount = 0;
		private bool _offerQueueStarted = false;

		#region UNITY

		protected override void Awake()
		{
			base.Awake();
			Instance = this;

			m_LastActiveScreen = m_LastActivePanel;

			var imanagers = GetComponentsInChildren<IManager>();

			foreach (var manager in imanagers)
			{
				manager.OnInitialized();
			}

			m_PanelsList = GetComponentsInChildren<UIPanelComponent>(true).ToList();
			m_PanelsList.AddRange(m_PanelsListOutSideScreen);

			foreach (var panel in m_PanelsList)
			{
				panel.OnInitlize();
			}

			m_ControllerList = GetComponentsInChildren<UIBaseController>(true).ToList();
			foreach (var c in m_ControllerList)
				c.OnInit();

			UIPanelComponent.OnSendComponent += OnRecievedComponent;

			UIPanelOfferSystem.Load();

			// Open base screen
			StartCoroutine(OpenInternal(GetPanelFromType(StartingPanel)));

			// Start offer flow
			if (EnableOfferQueue && !HasOffersAnimationPlayed)
				Invoke(nameof(StartOfferQueue), OfferStartDelay);
		}

		#endregion

		#region OFFER FLOW

		private void StartOfferQueue()
		{
			if (_offerQueueStarted)
				return;

			HasOffersAnimationPlayed = true;
			_offerQueueStarted = true;
			_sessionOpenCount = 0;

			BuildOfferQueue();
			TryOpenNext();
		}

		private void BuildOfferQueue()
		{
			var eligible = m_ControllerList
				.Where(c => c.CanShow())
				.Select(c => c.GetComponent<UIPanelComponent>())
				.Where(p => p != null)
				.OrderBy(x => UnityEngine.Random.value)
				.Take(MaxPanelsPerSession)
				.ToList();

			foreach (var p in eligible)
				panelQueue.Add(p);
		}

		#endregion

		#region QUEUE CORE
		private void Enqueue(UIPanelComponent panel)
		{
			if (panel == null)
				return;

			if (!panelQueue.Contains(panel))
				panelQueue.Add(panel);

			TryOpenNext();
		}

		private void TryOpenNext()
		{
			if (IsPanelOpened)
				return;

			if (panelQueue.Count == 0)
				return;

			var panel = panelQueue[0];
			panelQueue.RemoveAt(0);
			StartCoroutine(OpenInternal(panel));
		}

		private void HandlePanelClosed(UIPanelComponent panel)
		{
			IsPanelOpened = false;
			TryOpenNext();
		}

		#endregion

		#region PUBLIC API
		public void ShowPanel(Panel type, bool bol = true, bool isEnableInteractable = true)
		{
			RoutePanel(type, bol, isEnableInteractable);
		}

		public void Show(Panel type, bool isPopUp = true, bool isEnableInteractable = true)
		{
			RoutePanel(type, isPopUp, isEnableInteractable);
		}
		#endregion

		#region ROUTER
		private void RoutePanel(Panel type, bool show, bool interactable)
		{
			var panel = GetPanelFromType(type);
			if (panel == null)
				return;

			if (show)
			{
				Enqueue(panel);
			}
			else
			{
				CloseInternal(panel);
				if (m_LastActivePanel == panel)
				{
					IsPanelOpened = false;
					if (!HasRewardCollection) TryOpenNext();
				}
				else
				{
					panelQueue.Remove(panel);
				}
			}
		}
		#endregion

		#region INTERNAL OPEN/CLOSE
		private IEnumerator OpenInternal(UIPanelComponent panel)
		{
			if (panel == null)
				yield break;

			if (IsWaitRewardFlow)
			{
				// Wait only if reward flow is currently running
				yield return new WaitWhile(() => IsRewardFlowAnimation);
			}
			MyEventArgs.GameControllerEvents.OnScreenOpen.Dispatch(true);

			panel.Show(true);
			IsPanelOpened = true;

			m_LastActivePanel = panel;
			m_LastActiveScreen = panel;
		}

		private void CloseInternal(UIPanelComponent panel)
		{
			if (panel == null) return;

			MyEventArgs.GameControllerEvents.OnScreenOpen.Dispatch(false);
			panel.Show(false);
		}

		#endregion

		#region UTILS

		public UIPanelComponent GetPanelFromType(Panel type)
		{
			return m_PanelsList.Find(x => x.panelType == type);
		}

		private void OnRecievedComponent(UIPanelComponent component)
		{
			if (!m_PanelsList.Contains(component))
			{
				m_PanelsList.Add(component);
				component.OnPanelClosed += HandlePanelClosed;
			}
		}

		public override void SetGameManagerListner()
		{
			base.SetGameManagerListner();
			foreach (var c in m_ControllerList)
				c.SetGameManagerListner(gameEventListener);
		}

		public void TurnOffAll()
		{
			foreach (var item in m_PanelsList)
				item.Show(false);
		}

		#endregion
	}
}
