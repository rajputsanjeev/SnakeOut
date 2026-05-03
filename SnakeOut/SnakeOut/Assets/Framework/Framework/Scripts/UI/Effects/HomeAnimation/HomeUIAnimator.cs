using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework
{
	[System.Serializable]
	public class UIItem
	{
		public RectTransform rect;
		public Vector2 startOffset;
		public float delay;

		[HideInInspector] public Vector2 endPosition;
	}

	public enum PlayMode
	{
		Together,
		InOrder
	}

	public class HomeUIAnimator : MonoBehaviour
	{
		public event Action OnAnimationComplete;
		public UnityEvent OnAnimationStarted;
		public UnityEvent OnShowComplete;
		public UnityEvent OnHideComplete;

		public static OnLoadingScreenCompleteDelegate OnSceneLoadComplete;
		public delegate void OnLoadingScreenCompleteDelegate();

		[Header("Settings")]
		public float overAllDelayOnStart;
		public float duration = 0.45f;
		public Ease ease = Ease.OutBack;
		public PlayMode playMode = PlayMode.InOrder;
		public bool playOnAwake = true;
		public bool resetOnStart = true;

		[Header("UI Items")]
		public List<UIItem> uiItems = new List<UIItem>();

		private Sequence sequence;

		void Awake()
		{
			StoreEndPositions();

			if (resetOnStart)
				ResetToStart();

			OnSceneLoadComplete += Play;

			if (playOnAwake)
				Play();
		}

		void StoreEndPositions()
		{
			foreach (var item in uiItems)
			{
				if (item.rect != null)
					item.endPosition = item.rect.anchoredPosition;
			}
		}

		public void Play()
		{
			OnAnimationStarted?.Invoke();

			KillSequence();

			sequence = DOTween.Sequence().SetDelay(overAllDelayOnStart)
				.SetUpdate(true);

			if (playMode == PlayMode.Together)
			{
				PlayTogether();
			}
			else
			{
				PlayInOrder();
			}

			sequence.OnComplete(() =>
			{
				OnAnimationComplete?.Invoke();
				OnShowComplete?.Invoke();
			});
		}

		void PlayTogether()
		{
			foreach (var item in uiItems)
			{
				if (item.rect == null) continue;

				sequence.Join(
					item.rect
						.DOAnchorPos(item.endPosition, duration)
						.SetDelay(item.delay)
						.SetEase(ease)
				);
			}
		}

		void PlayInOrder()
		{
			foreach (var item in uiItems)
			{
				if (item.rect == null) continue;

				sequence.Append(
					item.rect
						.DOAnchorPos(item.endPosition, duration)
						.SetEase(ease)
				);

				if (item.delay > 0)
					sequence.AppendInterval(item.delay);
			}
		}

		public void ResetToStart()
		{
			foreach (var item in uiItems)
			{
				if (item.rect == null) continue;
				item.rect.anchoredPosition = item.endPosition + item.startOffset;
			}
		}

		public void Hide(Action onComplete = null)
		{
			OnAnimationStarted?.Invoke();
			KillSequence();

			sequence = DOTween.Sequence()
				.SetUpdate(true);

			if (playMode == PlayMode.Together)
			{
				foreach (var item in uiItems)
				{
					if (item.rect == null) continue;

					sequence.Join(
						item.rect
							.DOAnchorPos(item.endPosition + item.startOffset, duration).SetDelay(item.delay)
							.SetEase(ease)
					);
				}
			}
			else // InOrder (reverse order looks better)
			{
				for (int i = uiItems.Count - 1; i >= 0; i--)
				{
					var item = uiItems[i];
					if (item.rect == null) continue;

					sequence.Append(
						item.rect
							.DOAnchorPos(item.endPosition + item.startOffset, duration).SetDelay(item.delay)
							.SetEase(ease)
					);
				}
			}

			sequence.OnComplete(() =>
			{
				onComplete?.Invoke();
				OnHideComplete?.Invoke();
			});
		}


		public void PlayExternally()
		{
			OnAnimationStarted?.Invoke();
			Play();
		}

		void KillSequence()
		{
			if (sequence != null && sequence.IsActive())
				sequence.Kill();
		}

		void OnDestroy()
		{
			KillSequence();
		}
	}
}
