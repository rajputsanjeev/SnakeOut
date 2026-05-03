using System.Collections;
using DG.Tweening; // DOTween
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	[RequireComponent(typeof(RectTransform))]
	public class PopupAnimator : MonoBehaviour
	{
		public System.Action<bool> OnAnimationComplete;

		[Header("Presets")]
		public PopupAnimationData showPreset;
		public PopupAnimationData hidePreset;

		[Header("References")]
		public CanvasGroup canvasGroup; // optional; used for fades
		public Transform particleParent; // where to spawn particles

		// internal
		public Transform rect;
		Vector3 initialLocalPos;
		Vector3 initialLocalScale;
		Vector3 initialLocalRotEuler;

		Tween activeTween;

		void Awake()
		{
			initialLocalPos = rect.localPosition;
			initialLocalScale = rect.localScale;
			initialLocalRotEuler = rect.localEulerAngles;

			if (canvasGroup == null)
			{
				canvasGroup = GetComponent<CanvasGroup>();
			}
		}

		void Reset()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		// Public control
		public void Show(bool instantly = false)
		{
			PlayPreset(showPreset, true, instantly);
		}

		public void Hide(bool instantly = false)
		{
			PlayPreset(hidePreset, false, instantly);
		}

		public void Toggle()
		{
			if (gameObject.activeInHierarchy && canvasGroup != null && canvasGroup.alpha > 0.5f)
				Hide();
			else
				Show();
		}

		private void KillActive()
		{
			if (activeTween != null && activeTween.IsActive())
				activeTween.Kill();

			if (rect != null)
			{
				rect.DOKill(true); // kill all tweens on rect
			}

			if (canvasGroup != null)
			{
				canvasGroup.DOKill(true); // kill fade tweens
			}
		}

		void PlayPreset(PopupAnimationData preset, bool isShow, bool instantly = false)
		{
			if (preset == null)
			{
				Debug.LogWarning("[PopupAnimator] No preset assigned.");
				return;
			}

			KillActive();

			if (preset.autoSetActive && isShow) gameObject.SetActive(true);

			// create runtime copy to avoid mutating asset
			var presetData = preset.Clone();

			if (instantly)
			{
				// apply final states immediately
				ApplyFinalState(presetData, isShow);
				return;
			}

			// Ensure canvasGroup for fade use
			if (presetData.useCanvasGroup && canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}

			// Play particle if assigned (only on show)
			if (isShow && presetData.particlePrefab != null)
			{
				var parent = particleParent != null ? particleParent : transform;
				var go = Instantiate(presetData.particlePrefab, parent);
				go.transform.localPosition = presetData.particleLocalPos;
				Destroy(go, 3f);
			}

			// Route by animation type
			switch (presetData.animationType)
			{
				// ---------- SCALE ----------
				case PopupAnimationType.ScaleIn:
					rect.localScale = presetData.startScale;
					activeTween = rect.DOScale(presetData.endScale, presetData.duration)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.SetEase(curveToEase(presetData.curve)).OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.PunchScale:
					rect.localScale = presetData.endScale;
					activeTween = rect.DOPunchScale(Vector3.one * presetData.overshoot, presetData.duration, 1, presetData.elasticity)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.ElasticScale:
					rect.localScale = presetData.startScale;
					activeTween = rect.DOScale(presetData.endScale * presetData.overshoot, presetData.duration * 0.7f)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.SetEase(Ease.OutBack)
						.OnComplete(() =>
						{
							activeTween = rect.DOScale(presetData.endScale, presetData.duration * 0.3f)
								.SetUpdate(presetData.useUnscaledTime)
								.SetEase(Ease.OutBack)
								.OnComplete(() => OnCompleteState(presetData, isShow));
						});
					break;
				case PopupAnimationType.BounceScale:
					rect.localScale = presetData.startScale;
					activeTween = rect.DOScale(presetData.endScale, presetData.duration)
						.SetDelay(presetData.delay)
						.SetEase(Ease.OutBounce)
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				// ---------- SLIDE ----------
				case PopupAnimationType.SlideFromTop:
					rect.localPosition = initialLocalPos + new Vector3(0, Mathf.Abs(presetData.slideOffset.y), 0);
					activeTween = rect.DOLocalMove(initialLocalPos, presetData.duration)
						.SetDelay(presetData.delay)
						.SetEase(curveToEase(presetData.curve))
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.SlideFromBottom:
					rect.localPosition = initialLocalPos - new Vector3(0, Mathf.Abs(presetData.slideOffset.y), 0);
					activeTween = rect.DOLocalMove(initialLocalPos, presetData.duration)
						.SetDelay(presetData.delay)
						.SetEase(curveToEase(presetData.curve))
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.SlideFromLeft:
					rect.localPosition = initialLocalPos - new Vector3(Mathf.Abs(presetData.slideOffset.x), 0, 0);
					activeTween = rect.DOLocalMove(initialLocalPos, presetData.duration)
						.SetDelay(presetData.delay)
						.SetEase(curveToEase(presetData.curve))
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.SlideFromRight:
					rect.localPosition = initialLocalPos + new Vector3(Mathf.Abs(presetData.slideOffset.x), 0, 0);
					activeTween = rect.DOLocalMove(initialLocalPos, presetData.duration)
						.SetDelay(presetData.delay)
						.SetEase(curveToEase(presetData.curve))
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.SlideThenPop:
					rect.localPosition = initialLocalPos + presetData.slideOffset;
					rect.localScale = presetData.startScale;
					activeTween = DOTween.Sequence()
						.SetUpdate(presetData.useUnscaledTime)
						.Append(rect.DOLocalMove(initialLocalPos, presetData.duration * 0.6f).SetEase(Ease.OutCubic))
						.Append(rect.DOScale(presetData.endScale * presetData.overshoot, presetData.duration * 0.25f).SetEase(Ease.OutBack))
						.Append(rect.DOScale(presetData.endScale, presetData.duration * 0.15f))
						.SetDelay(presetData.delay)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.DropAndBounce:
					rect.localPosition = initialLocalPos + new Vector3(0, Mathf.Abs(presetData.slideOffset.y), 0);
					activeTween = rect.DOLocalMove(initialLocalPos, presetData.duration)
						.SetEase(Ease.OutBounce)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				// ---------- FADE ----------
				case PopupAnimationType.FadeIn:
					if (presetData.useCanvasGroup && canvasGroup != null)
					{
						canvasGroup.alpha = presetData.startAlpha;
						activeTween = canvasGroup.DOFade(presetData.endAlpha, presetData.duration)
							.SetDelay(presetData.delay)
							.SetEase(curveToEase(presetData.curve))
							.SetUpdate(presetData.useUnscaledTime)
							.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					else
					{
						// fallback: just scale
						rect.localScale = presetData.startScale;
						activeTween = rect.DOScale(presetData.endScale, presetData.duration)
							.SetDelay(presetData.delay)
							.SetEase(curveToEase(presetData.curve))
							.SetUpdate(presetData.useUnscaledTime)
							.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;

				case PopupAnimationType.FadeInScale:
					if (presetData.useCanvasGroup && canvasGroup != null) canvasGroup.alpha = presetData.startAlpha;
					rect.localScale = presetData.startScale;
					activeTween = DOTween.Sequence()
						.SetUpdate(presetData.useUnscaledTime)
						.Append(canvasGroup != null ? canvasGroup.DOFade(presetData.endAlpha, presetData.duration) : rect.DOScale(presetData.endScale, presetData.duration))
						.Join(rect.DOScale(presetData.endScale, presetData.duration))
						.SetDelay(presetData.delay)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.FadeInSlide:
					if (presetData.useCanvasGroup && canvasGroup != null) canvasGroup.alpha = presetData.startAlpha;
					rect.localPosition = initialLocalPos + presetData.slideOffset;
					activeTween = DOTween.Sequence()
						.SetUpdate(presetData.useUnscaledTime)
						.Append(rect.DOLocalMove(initialLocalPos, presetData.duration))
						.Join(canvasGroup != null ? canvasGroup.DOFade(presetData.endAlpha, presetData.duration) : rect.DOScale(presetData.endScale, presetData.duration))
						.SetDelay(presetData.delay)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				// ---------- ROTATION ----------
				case PopupAnimationType.RotateInZ:
					rect.localEulerAngles = presetData.startRotation;
					activeTween = rect.DOLocalRotate(presetData.endRotation, presetData.duration, RotateMode.Fast)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.SetEase(curveToEase(presetData.curve))
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.FlipInX:
				case PopupAnimationType.FlipInY:
					{
						var rotAxis = presetData.animationType == PopupAnimationType.FlipInX ? Vector3.right : Vector3.up;
						rect.localEulerAngles = presetData.startRotation;
						activeTween = rect.DOLocalRotate(presetData.endRotation, presetData.duration, RotateMode.Fast)
							.SetDelay(presetData.delay)
							.SetEase(Ease.OutBack)
							.SetUpdate(presetData.useUnscaledTime)
							.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;

				case PopupAnimationType.SwingIn:
					rect.localEulerAngles = new Vector3(0, 0, presetData.startRotation.z + 60f);
					activeTween = rect.DOLocalRotate(presetData.endRotation, presetData.duration).SetEase(Ease.OutElastic).SetDelay(presetData.delay).SetUpdate(presetData.useUnscaledTime).OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				// Mask / Reveal (these will rely on a mask component or shader)
				case PopupAnimationType.CircularMaskReveal:
					{
						var img = rect.GetComponent<Image>();
						if (img == null)
						{
							Debug.LogError("CircularMaskReveal requires Image on same GameObject.");
							break;
						}

						img.type = Image.Type.Filled;
						img.fillMethod = Image.FillMethod.Radial360;

						img.fillAmount = isShow ? 0f : 1f;

						activeTween = DOTween.To(
							() => img.fillAmount,
							x => img.fillAmount = x,
							isShow ? 1f : 0f,
							presetData.duration
						)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.SetEase(curveToEase(presetData.curve))
						.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;
				case PopupAnimationType.RectMaskReveal:
					{
						var rt = rect.GetComponent<RectTransform>();
						if (rt == null)
						{
							Debug.LogError("RectMaskReveal requires RectTransform.");
							break;
						}

						rt.sizeDelta = isShow ? presetData.rectRevealStartSize : presetData.rectRevealEndSize;

						activeTween = rt.DOSizeDelta(
							isShow ? presetData.rectRevealEndSize : presetData.rectRevealStartSize,
							presetData.duration
						)
						.SetDelay(presetData.delay)
						.SetEase(curveToEase(presetData.curve))
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;

				case PopupAnimationType.WipeRevealLeftToRight:
				case PopupAnimationType.WipeRevealRightToLeft:
				case PopupAnimationType.WipeRevealTopToBottom:
				case PopupAnimationType.WipeRevealBottomToTop:
					{
						var img = rect.GetComponent<UnityEngine.UI.Image>();
						if (img == null)
						{
							Debug.LogError("WipeReveal requires Image.");
							break;
						}

						img.type = Image.Type.Filled;

						// Axis mode
						if (presetData.animationType == PopupAnimationType.WipeRevealLeftToRight || presetData.animationType == PopupAnimationType.WipeRevealRightToLeft)
							img.fillMethod = Image.FillMethod.Horizontal;
						else
							img.fillMethod = Image.FillMethod.Vertical;

						// Direction
						bool reverse = presetData.animationType == PopupAnimationType.WipeRevealRightToLeft || presetData.animationType == PopupAnimationType.WipeRevealBottomToTop;
						img.fillOrigin = reverse ? 1 : 0;

						img.fillAmount = isShow ? 0f : 1f;

						activeTween = DOTween.To(
							() => img.fillAmount,
							x => img.fillAmount = x,
							isShow ? 1f : 0f,
							presetData.duration
						)
						.SetDelay(presetData.delay)
						.SetEase(curveToEase(presetData.curve))
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;


				// ---------- COMBO / SPECIAL (examples) ----------
				case PopupAnimationType.ScaleRotate:
					rect.localScale = presetData.startScale;
					rect.localEulerAngles = presetData.startRotation;
					activeTween = DOTween.Sequence()
						.SetUpdate(presetData.useUnscaledTime)
						.Append(rect.DOScale(presetData.endScale, presetData.duration))
						.Join(rect.DOLocalRotate(presetData.endRotation, presetData.duration))
						.SetDelay(presetData.delay)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.Heartbeat:
				case PopupAnimationType.Jelly:
					// simple repeated pulse
					rect.localScale = presetData.endScale;
					activeTween = rect.DOPunchScale(Vector3.one * presetData.bounceAmount, presetData.duration, 2, presetData.elasticity)
						.SetLoops(presetData.animationType == PopupAnimationType.Heartbeat ? 2 : 1)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				// ---------- EXIT (hide) examples ----------
				case PopupAnimationType.ScaleOut:
					rect.localScale = presetData.endScale;
					activeTween = rect.DOScale(presetData.startScale, presetData.duration)
						.SetDelay(presetData.delay)
						.SetEase(Ease.InBack)
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.FadeOut:
					if (presetData.useCanvasGroup && canvasGroup != null)
					{
						canvasGroup.alpha = presetData.endAlpha;
						activeTween = canvasGroup.DOFade(presetData.startAlpha, presetData.duration)
							.SetDelay(presetData.delay)
							.SetUpdate(presetData.useUnscaledTime)
							.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					else
					{
						activeTween = rect.DOScale(Vector3.zero, presetData.duration)
							.SetDelay(presetData.delay)
							.SetUpdate(presetData.useUnscaledTime)
							.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;

				case PopupAnimationType.SlideOut:
					activeTween = rect.DOLocalMove(initialLocalPos + presetData.slideOffset, presetData.duration)
						.SetDelay(presetData.delay)
						.SetUpdate(presetData.useUnscaledTime)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;
				case PopupAnimationType.SlideOutDown:
					activeTween = rect.DOLocalMove(initialLocalPos - new Vector3(0, presetData.slideOffset.y, 0), presetData.duration)
						.SetEase(Ease.InBack)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.SlideOutLeft:
					activeTween = rect.DOLocalMove(initialLocalPos - new Vector3(presetData.slideOffset.x, 0, 0), presetData.duration)
						.SetEase(Ease.InBack)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.SlideOutRight:
					activeTween = rect.DOLocalMove(initialLocalPos + new Vector3(presetData.slideOffset.x, 0, 0), presetData.duration)
						.SetEase(Ease.InBack)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;

				case PopupAnimationType.PunchOut:
					activeTween = rect.DOPunchScale(-Vector3.one * presetData.overshoot, presetData.duration, 1, presetData.elasticity)
						.OnComplete(() => OnCompleteState(presetData, isShow));
					break;
				case PopupAnimationType.GlowPulse:
					{
						// If material has "_Glow" or "_Intensity", animate that — otherwise scale pulse
						var mat = rect.GetComponent<Graphic>()?.material;
						if (mat != null && mat.HasProperty("_Glow"))
						{
							float start = isShow ? 0f : 1f;
							float end = isShow ? 1f : 0f;

							activeTween = DOTween.To(() => mat.GetFloat("_Glow"), x => mat.SetFloat("_Glow", x), end, presetData.duration)
								.SetLoops(1, LoopType.Yoyo)
								.SetEase(Ease.InOutQuad)
								.OnComplete(() => OnCompleteState(presetData, isShow));
						}
						else
						{
							// Scale fallback
							rect.localScale = isShow ? Vector3.zero : Vector3.one;
							activeTween = rect.DOScale(isShow ? Vector3.one : Vector3.zero, presetData.duration)
								.SetEase(Ease.OutElastic)
								.OnComplete(() => OnCompleteState(presetData, isShow));
						}
					}
					break;
				case PopupAnimationType.ShockwaveReveal:
					{
						rect.localScale = isShow ? Vector3.zero : Vector3.one * 1.2f;
						if (canvasGroup != null) canvasGroup.alpha = isShow ? 0 : 1;

						Sequence seq = DOTween.Sequence();

						seq.Join(rect.DOScale(isShow ? presetData.endScale : Vector3.zero, presetData.duration)
							.SetEase(Ease.OutExpo));

						if (canvasGroup != null)
							seq.Join(canvasGroup.DOFade(isShow ? 1f : 0f, presetData.duration * 0.6f));

						activeTween = seq.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;
				case PopupAnimationType.SlideFade:
					{
						Vector3 offset = new Vector3(0, -presetData.slideOffset.y, 0);
						rect.localPosition = isShow ? initialLocalPos + offset : initialLocalPos;

						if (canvasGroup != null)
							canvasGroup.alpha = isShow ? 0 : 1;

						Sequence seq = DOTween.Sequence();

						seq.Join(rect.DOLocalMove(isShow ? initialLocalPos : initialLocalPos + offset, presetData.duration)
							.SetEase(Ease.OutCubic));

						if (canvasGroup != null)
							seq.Join(canvasGroup.DOFade(isShow ? 1 : 0, presetData.duration));

						activeTween = seq.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;
				case PopupAnimationType.ScaleFade:
					{
						rect.localScale = isShow ? Vector3.one * 0.5f : Vector3.one;

						if (canvasGroup != null)
							canvasGroup.alpha = isShow ? 0 : 1;

						Sequence seq = DOTween.Sequence();

						seq.Join(rect.DOScale(isShow ? 1f : 0.5f, presetData.duration)
							.SetEase(Ease.OutBack));

						if (canvasGroup != null)
							seq.Join(canvasGroup.DOFade(isShow ? 1 : 0, presetData.duration));

						activeTween = seq.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;
				case PopupAnimationType.SlideBounce:
					{
						Vector3 startPos = initialLocalPos + new Vector3(0, presetData.slideOffset.y, 0);
						rect.localPosition = isShow ? startPos : initialLocalPos;

						Sequence seq = DOTween.Sequence();

						seq.Append(rect.DOLocalMove(isShow ? initialLocalPos : startPos, presetData.duration)
							.SetEase(Ease.OutCubic));

						seq.Join(rect.DOScale(isShow ? 1.1f : 0.9f, presetData.duration * 0.6f).SetEase(Ease.OutQuad));
						seq.Append(rect.DOScale(1f, 0.12f));

						activeTween = seq.OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;

				default:
					// fallback: simple scale in/out
					if (isShow)
					{
						rect.localScale = presetData.startScale;
						activeTween = rect.DOScale(presetData.endScale, presetData.duration).SetDelay(presetData.delay).SetUpdate(presetData.useUnscaledTime).OnComplete(() => OnCompleteState(presetData, isShow));
					}
					else
					{
						activeTween = rect.DOScale(Vector3.zero, presetData.duration).SetDelay(presetData.delay).SetUpdate(presetData.useUnscaledTime).OnComplete(() => OnCompleteState(presetData, isShow));
					}
					break;
			}
		}

		void ApplyFinalState(PopupAnimationData p, bool isShow)
		{
			// apply final transforms/alpha without animation
			switch (p.animationType)
			{
				default:
					rect.localScale = isShow ? p.endScale : p.startScale;
					rect.localPosition = initialLocalPos;
					rect.localEulerAngles = isShow ? p.endRotation : p.startRotation;
					if (p.useCanvasGroup && canvasGroup != null)
						canvasGroup.alpha = isShow ? p.endAlpha : p.startAlpha;
					if (p.autoSetActive && !isShow) gameObject.SetActive(false);
					break;
			}
		}

		void OnCompleteState(PopupAnimationData p, bool isShow)
		{
			if (p.autoSetActive && !isShow)
			{
				gameObject.SetActive(false);
			}

			// reset transforms to initial for consistency
			OnAnimationComplete?.Invoke(isShow);
			rect.localPosition = initialLocalPos;
			rect.localScale = isShow ? p.endScale : p.startScale;
			rect.localEulerAngles = isShow ? p.endRotation : p.startRotation;
		}

		// convert AnimationCurve to an Ease if possible; fallback to linear
		Ease curveToEase(AnimationCurve curve)
		{
			// DOTween can't accept arbitrary AnimationCurve for SetEase except as custom
			// We'll try to detect common shapes (easeInOut) — fallback to Linear
			// For complex curves, user can rely on explicit DOTween sequences or override.
			if (curve == null) return Ease.Linear;
			// Simple heuristic:
			var keys = curve.keys;
			if (keys.Length == 2 && Mathf.Approximately(keys[0].value, 0f) && Mathf.Approximately(keys[1].value, 1f))
			{
				// linear or ease depending on tangents
				if (keys[0].outTangent > 1f && keys[1].inTangent < -1f) return Ease.InOutSine;
				return Ease.Linear;
			}
			return Ease.OutQuad;
		}
	}
}