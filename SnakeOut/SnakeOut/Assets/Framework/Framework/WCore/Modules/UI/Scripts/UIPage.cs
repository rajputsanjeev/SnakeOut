using System.Collections.Generic;
using Frameork;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core
{
	[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
	public abstract class UIPage : MonoBehaviour, ISceneSavingCallback
	{
		public List<GameObject> ParticleEffects = new List<GameObject>();
		public GameObject ParticleEffect;
		public bool IsAnimateHomeScreenIcon = true;

		[Hide]
		[SerializeField] Component[] registeredElements;

		protected bool isPageDisplayed;
		public bool IsPageDisplayed { get => isPageDisplayed; set => isPageDisplayed = value; }

		protected Canvas canvas;
		public Canvas Canvas => canvas;

		protected GraphicRaycaster graphicRaycaster;
		public GraphicRaycaster GraphicRaycaster => graphicRaycaster;

		private string defaultName;

		private IUIPageElement[] pageElements;

		protected bool isCached;
		public bool IsCached => isCached;
		protected HomeUIAnimator HomeUIAnimator;


		public void CacheComponents()
		{
			defaultName = name;

			canvas = GetComponent<Canvas>();
			graphicRaycaster = GetComponent<GraphicRaycaster>();
			HomeUIAnimator = FindAnyObjectByType<HomeUIAnimator>();
		}

		public void PreparePage()
		{
			isPageDisplayed = false;
			canvas.enabled = false;

			pageElements = new IUIPageElement[registeredElements.Length];
			if (!pageElements.IsNullOrEmpty())
			{
				for (int i = 0; i < pageElements.Length; i++)
				{
					pageElements[i] = (IUIPageElement)registeredElements[i];
					pageElements[i].Init(this);
				}
			}
		}

		public abstract void Init();

		public void EnableCanvas()
		{
			isPageDisplayed = true;

			canvas.enabled = true;

			if (!pageElements.IsNullOrEmpty())
			{
				for (int i = 0; i < pageElements.Length; i++)
				{
					pageElements[i]?.OnPageStateChanged(true);
				}
			}

#if UNITY_EDITOR
			name = string.Format("{0} (Active)", defaultName);
#endif
		}

		public void DisableCanvas()
		{
			isPageDisplayed = false;

			canvas.enabled = false;

			for (int i = 0; i < pageElements.Length; i++)
			{
				pageElements[i]?.OnPageStateChanged(false);
			}

#if UNITY_EDITOR
			name = defaultName;
#endif
		}

		public virtual void PlayShowAnimation()
		{
			MyEventArgs.GameControllerEvents.OnScreenOpen.Dispatch(true);

			if (ParticleEffects.Count > 0)
			{
				foreach (var item in ParticleEffects)
				{
					item.SetActive(true);
				}
			}

			if (ParticleEffect != null) ParticleEffect.gameObject.SetActive(true);

			if (HomeUIAnimator != null && IsAnimateHomeScreenIcon) HomeUIAnimator.Hide();
		}

		public virtual void PlayHideAnimation()
		{
			MyEventArgs.GameControllerEvents.OnScreenOpen.Dispatch(false);

			if (ParticleEffects.Count > 0)
			{
				foreach (var item in ParticleEffects)
				{
					item.SetActive(false);
				}
			}

			if (ParticleEffect != null) ParticleEffect.gameObject.SetActive(false);

			if (HomeUIAnimator != null && IsAnimateHomeScreenIcon) HomeUIAnimator.Play();
		}

		public virtual void Unload()
		{
			isPageDisplayed = false;

			canvas.enabled = false;
		}

		public void MarkAsCached()
		{
			isCached = true;
		}

		public bool OnPrefabSaving()
		{
			Component[] cachedPageElements = GetComponentsInChildren(typeof(IUIPageElement));

			if (registeredElements == null || registeredElements.Length != cachedPageElements.Length)
			{
				registeredElements = cachedPageElements;

				return true;
			}

			for (int i = 0; i < registeredElements.Length; i++)
			{
				if (registeredElements[i] == null)
				{
					registeredElements = cachedPageElements;

					return true;
				}
			}

			for (int i = 0; i < cachedPageElements.Length; i++)
			{
				if (!ReferenceEquals(registeredElements[i], cachedPageElements[i]))
				{
					registeredElements = cachedPageElements;

					return true;
				}
			}

			return false;
		}

		public void OnSceneSaving()
		{
			void SaveElements(Component[] elements)
			{
				registeredElements = elements;

				RuntimeEditorUtils.SetDirty(this);
			}

			Component[] cachedPageElements = GetComponentsInChildren(typeof(IUIPageElement));

			if (registeredElements == null || registeredElements.Length != cachedPageElements.Length)
			{
				SaveElements(cachedPageElements);

				return;
			}

			for (int i = 0; i < registeredElements.Length; i++)
			{
				if (registeredElements[i] == null)
				{
					SaveElements(cachedPageElements);

					return;
				}
			}

			for (int i = 0; i < cachedPageElements.Length; i++)
			{
				if (!ReferenceEquals(registeredElements[i], cachedPageElements[i]))
				{
					SaveElements(cachedPageElements);

					return;
				}
			}
		}
	}
}