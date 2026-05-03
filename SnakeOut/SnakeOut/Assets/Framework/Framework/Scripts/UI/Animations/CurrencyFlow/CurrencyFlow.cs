using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public enum CurrencyMoveEffect
	{
		Jump,
		Bezier,
		Spiral,
		Magnet,
		Wobble,
		Orbit,
		Vortex,
		Overshoot,
		Pulse,
	}

	[System.Serializable]
	public class RewardTransform
	{
		public RewardType Type;

		[Header("UI")]
		public RectTransform StartingPoint;
		public RectTransform EndPoint;
		public GameObject Prefab;
		public Sprite RewardSprite;
		public int InstantiateAmount;
		public int AmountAdded;

		[Header("Spawn")]
		public float spawnDelay = 0.05f;
		public float finalScale = 0.3f;

		[Header("Appear")]
		public Vector3 StartScale = Vector3.zero;
		public Vector3 EndScale = Vector3.one;
		public float ScaleDuration = 0.15f;

		[Header("Rotation")]
		public bool EnableRotation = true;
		public int RotationLoops = 3;
		public float RotationDuration = 0.4f;

		[Header("Scatter")]
		public bool EnableScatter = true;
		public float ScatterRadius = 40f;
		public float ScatterDuration = 0.4f;

		[Header("Move To Target")]
		public CurrencyMoveEffect MoveType = CurrencyMoveEffect.Jump;
		public float MoveDuration = 0.5f;
		// Removed: MoveCurve — was never passed to any tween

		[Header("Target Feedback")]
		public bool PunchTarget = true;
		public float PunchScale = 1.2f;

		[Header("Audio")]
		public AudioClip AppearAudioClip;
		public AudioClip CollectAudioClip;
		internal AudioSource CollectClipHandler;

		[Header("Text Popup")]
		public bool IsTextPopup;
		public TMPPopupSettings TextPopupSettings;
	}

	public static class CurrencyFlow
	{
		public static Action OnAnimationComplete;

		public static void SpawnCurrency(RewardTransform rewardTransform, Action onCurrencyHittedTarget = null)
		{
			new Animation(rewardTransform, onCurrencyHittedTarget).PlayAnimation();
		}

		public class Animation
		{
			private readonly RewardTransform rewardTransform;
			private readonly RectTransform rectTransform;
			private readonly RectTransform targetTransform;
			private readonly int elementsAmount;
			private readonly Action onCurrencyHittedTarget;
			private readonly Transform fakeTargetTransform;
			private readonly GameObject elementPrefab;

			public Animation(RewardTransform rewardTransform, Action onCurrencyHittedTarget)
			{
				this.rewardTransform = rewardTransform;
				this.rectTransform = rewardTransform.StartingPoint;
				this.targetTransform = rewardTransform.EndPoint;
				this.elementPrefab = rewardTransform.Prefab;
				this.elementsAmount = rewardTransform.InstantiateAmount;
				this.onCurrencyHittedTarget = onCurrencyHittedTarget;

				GameObject fake = new GameObject("Fake Target");
				fakeTargetTransform = fake.transform;
				fakeTargetTransform.SetParent(targetTransform.parent);
				fakeTargetTransform.position = targetTransform.position;
				fakeTargetTransform.localScale = targetTransform.localScale;
				fakeTargetTransform.localRotation = targetTransform.localRotation;
			}

			// ─────────────────────────────────────────────────────────
			// Entry point
			// ─────────────────────────────────────────────────────────

			public void PlayAnimation()
			{
				Vector3 centerPoint = rectTransform.position;
				RectTransform targetRect = targetTransform;
				int finishedCount = 0;
				bool currencyHittedTarget = false;
				Tween targetPunchTween = null;

				for (int i = 0; i < elementsAmount; i++)
				{
					int index = i;

					var (elementObject, elementRect, elementImage) = SpawnElement(centerPoint);

					Sequence seq = DOTween.Sequence().SetUpdate(true).SetAutoKill(true);

					// Stagger
					seq.AppendInterval(index * rewardTransform.spawnDelay);

					// Appear audio (per element — staggered cascade feel)
					seq.AppendCallback(() => PlayClipSafe(rewardTransform.AppearAudioClip));

					// Fade + scale in
					seq.Append(elementImage.DOFade(1f, 0.2f));
					seq.Append(elementRect.DOScale(rewardTransform.EndScale, rewardTransform.ScaleDuration)
						.SetEase(Ease.OutBack));

					// Scatter
					if (rewardTransform.EnableScatter)
					{
						Vector2 offset = UnityEngine.Random.insideUnitCircle * rewardTransform.ScatterRadius;
						seq.Join(elementRect.DOAnchorPos(offset, rewardTransform.ScatterDuration)
							.SetEase(Ease.OutCubic));
					}

					// Rotation
					if (rewardTransform.EnableRotation)
					{
						seq.Join(elementRect
							.DOLocalRotate(Vector3.up * 360f, rewardTransform.RotationDuration, RotateMode.FastBeyond360)
							.SetRelative()
							.SetLoops(rewardTransform.RotationLoops)
							.SetEase(Ease.Linear));
					}

					// Move + shrink
					seq.Append(PlayMoveEffect(elementRect, rewardTransform.MoveType, rewardTransform.MoveDuration));
					seq.Join(elementRect.DOScale(rewardTransform.finalScale, 0.2f).SetEase(Ease.InExpo));

					seq.OnComplete(() =>
					{
						if (!currencyHittedTarget)
						{
							currencyHittedTarget = true;
							onCurrencyHittedTarget?.Invoke();
						}

						// Play every time an element hits — not just the first
						PlayClipSafe(rewardTransform.CollectAudioClip);

						HandleTargetPunch(ref targetPunchTween, targetRect, elementRect, elementImage, elementObject);

						finishedCount++;
						if (finishedCount >= elementsAmount)
							FinalizeAnimation();
					});

					seq.Play();
				}
			}

			/// <summary>Instantiate, parent, and reset an element at worldPos.</summary>
			private (GameObject obj, RectTransform rt, Image img) SpawnElement(Vector3 worldPos)
			{
				GameObject obj = GameObject.Instantiate(elementPrefab);
				Image img = obj.GetComponent<Image>();
				RectTransform rt = obj.GetComponent<RectTransform>();

				if (rewardTransform.RewardSprite != null)
					img.sprite = rewardTransform.RewardSprite;

				rt.SetParent(fakeTargetTransform, false);
				rt.position = worldPos;
				rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.one * 0.5f;
				rt.localRotation = Quaternion.identity;
				rt.localScale = rewardTransform.StartScale;

				// Start invisible so fade-in lands cleanly
				Color c = img.color; c.a = 0f; img.color = c;

				return (obj, rt, img);
			}

			/// <summary>Punch the target on hit, then destroy the element. Skipped if PunchTarget = false.</summary>
			private void HandleTargetPunch(
				ref Tween punchTween,
				RectTransform targetRect,
				RectTransform elementRect,
				Image elementImage,
				GameObject elementObject)
			{
				if (rewardTransform.PunchTarget && (punchTween == null || !punchTween.IsActive()))
				{
					punchTween?.Kill();
					punchTween = targetRect
						.DOScale(rewardTransform.PunchScale, 0.15f)
						.SetUpdate(true)
						.OnComplete(() =>
							targetRect.DOScale(1f, 0.1f).SetUpdate(true).OnComplete(() =>
								DestroyElement(elementRect, elementImage, elementObject)));
				}
				else
				{
					DestroyElement(elementRect, elementImage, elementObject);
				}
			}

			private static void DestroyElement(RectTransform rt, Image img, GameObject obj)
			{
				rt.DOKill(true);
				img.DOKill(true);
				GameObject.Destroy(obj);
			}

			/// <summary>Fire completion events and clean up the fake parent.</summary>
			private void FinalizeAnimation()
			{
				OnAnimationComplete?.Invoke();

				if (rewardTransform.IsTextPopup)
				{
					if (rewardTransform.TextPopupSettings.Parent == null)
						rewardTransform.TextPopupSettings.Parent = rewardTransform.EndPoint;
					TMPPopupPlayer.Play(rewardTransform.AmountAdded.ToString(), rewardTransform.TextPopupSettings);
				}

				GameObject.Destroy(fakeTargetTransform.gameObject);
			}

			public void Clear()
			{
				if (fakeTargetTransform == null) return;
				fakeTargetTransform.DOKill(true);
				foreach (Transform t in fakeTargetTransform) t.DOKill(true);
				GameObject.Destroy(fakeTargetTransform.gameObject);
			}

			// ─────────────────────────────────────────────────────────
			// Audio + move dispatch
			// ─────────────────────────────────────────────────────────

			private static void PlayClipSafe(AudioClip clip)
			{
				if (clip == null || Camera.main == null) return;
				AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
			}

			private static Tween PlayMoveEffect(RectTransform element, CurrencyMoveEffect moveType, float duration)
			{
				Vector2 target = Vector2.zero;
				switch (moveType)
				{
					case CurrencyMoveEffect.Jump: return UICurrencyEffects.DOJumpAnchor(element, target, 200f, 1, duration);
					case CurrencyMoveEffect.Bezier: return UICurrencyEffects.DOBezierAnchor(element, target, 140f, duration);
					case CurrencyMoveEffect.Spiral: return UICurrencyEffects.DOSpiralTo(element, target, 60f, 2f, duration);
					case CurrencyMoveEffect.Magnet: return UICurrencyEffects.DOMagnetTo(element, target, duration);
					case CurrencyMoveEffect.Wobble: return UICurrencyEffects.DOWobbleMove(element, target, duration);
					case CurrencyMoveEffect.Orbit: return UICurrencyEffects.DOOrbitSnap(element, target, 80f, duration * 0.5f, duration * 0.5f);
					case CurrencyMoveEffect.Vortex: return UICurrencyEffects.DOVortexPull(element, target, 70f, 3f, duration);
					case CurrencyMoveEffect.Overshoot: return UICurrencyEffects.DOOvershootSnap(element, target, 20f, duration);
					case CurrencyMoveEffect.Pulse: return UICurrencyEffects.DOPulseWhileMoving(element, target, duration);
					default: return element.DOAnchorPos(target, duration);
				}
			}
		}
	}
}