using System;
using Base.UI.Controller;
using Base.UI.Manager;
using Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base.UI.Components
{
	public class UIPanelComponent : View
	{
		public static Action<UIPanelComponent> OnSendComponent { get; set; }
		public event Action<UIPanelComponent> OnPanelClosed;

		public bool IsOutSideManager;
		public bool IsAnimationApplied = true;
		public Panel panelType;

		public UIBaseController CurrentUIBaseController { get; set; }
		public CanvasGroup CanvasGroup { get; set; }
		private PopupAnimator _popupAnimator;
		public bool _isOn { get; private set; }

		public virtual void OnInitlize()
		{
			CurrentUIBaseController = GetComponent<UIBaseController>();
			CanvasGroup = GetComponent<CanvasGroup>();
			_popupAnimator = GetComponent<PopupAnimator>();

			if (_popupAnimator != null)
			{
				_popupAnimator.OnAnimationComplete += OnAnimationComplete;
			}
		}

		private void OnAnimationComplete(bool turnOffCanvas)
		{
			if (!turnOffCanvas) EnableCanvas(false);
		}

		void OnEnable()
		{
			// Subscribe to scene loaded event
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnDisable()
		{
			// Unsubscribe (important to avoid memory leaks!)
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		// Called every time a scene is loaded
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Debug.Log("Scene changed to: " + scene.name);
			if (IsOutSideManager) OnSendComponent?.Invoke(this);
		}

		public void Show()
		{
			_isOn = true;
			Show(true, true);
		}

		public void Hide()
		{
			_isOn = false;
			Show(false, false);
		}

		public void Show(bool on, bool isEnableInteractable = true)
		{
			if (!TryGetComponent(out CanvasGroup canvasGroup))
			{
				Debug.LogError($"{nameof(CanvasGroup)} is missing on {gameObject.name}");
			}
			else
			{
				CanvasGroup = canvasGroup;
			}

			if (on)
			{
				_isOn = true;
				EnableCanvas(true);
			}
			else if (!IsAnimationApplied)
			{
				_isOn = false;
				EnableCanvas(false);
			}

			if (CurrentUIBaseController == null)
			{
				CurrentUIBaseController = GetComponent<UIBaseController>();
			}
			CurrentUIBaseController.ShowPanel(on);
			if (on) CurrentUIBaseController.Enable();

			if (!on)
			{
				OnPanelClosed?.Invoke(this);
			}
		}

		private void EnableCanvas(bool activateCanvas)
		{
			CanvasGroup.interactable = activateCanvas;
			CanvasGroup.blocksRaycasts = activateCanvas;
			CanvasGroup.alpha = activateCanvas ? 1 : 0;
		}

		public virtual void CloseCurrentPanel()
		{
			// Close the current panel
		}
		public void OnDestroy()
		{
			if(_popupAnimator != null) _popupAnimator.OnAnimationComplete -= OnAnimationComplete;
		}
	}
}

