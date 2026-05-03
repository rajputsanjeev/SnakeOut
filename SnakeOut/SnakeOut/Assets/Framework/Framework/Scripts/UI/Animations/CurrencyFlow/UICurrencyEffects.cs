using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public static class UICurrencyEffects
	{
		// =========================================================
		// BASIC HELPERS
		// =========================================================

		private static void EnsureUI(RectTransform rt)
		{
			rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.one * 0.5f;
		}

		// =========================================================
		// 1. SCALE / POP EFFECTS
		// =========================================================

		public static Tween DOScaleIn(RectTransform target, Vector3 endScale, float duration)
		{
			target.localScale = Vector3.zero;
			return target.DOScale(endScale, duration)
				.SetEase(Ease.OutBack)
				.SetTarget(target);
		}

		public static Tween DOElasticPop(RectTransform target, float duration)
		{
			target.localScale = Vector3.zero;
			return target.DOScale(1f, duration)
				.SetEase(Ease.OutElastic)
				.SetTarget(target);
		}

		// =========================================================
		// 2. ROTATION (UI SAFE)
		// =========================================================

		public static Tween DORotateLoop(
			RectTransform target,
			float angle,
			float duration,
			int loops)
		{
			return target.DOLocalRotate(
					Vector3.forward * angle,
					duration,
					RotateMode.FastBeyond360)
				.SetRelative()
				.SetLoops(loops, LoopType.Restart)
				.SetEase(Ease.Linear)
				.SetTarget(target);
		}

		// =========================================================
		// 3. JUMP (ANCHOR POS)
		// =========================================================

		public static Sequence DOJumpAnchor(
			RectTransform target,
			Vector2 endValue,
			float jumpPower,
			int jumps,
			float duration)
		{
			EnsureUI(target);
			return target.DOJumpAnchorPos(endValue, jumpPower, jumps, duration)
				.SetEase(Ease.OutQuad)
				.SetTarget(target);
		}

		// =========================================================
		// 4. BEZIER / ARC MOVE
		// =========================================================

		public static Tween DOBezierAnchor(
			RectTransform target,
			Vector2 endValue,
			float height,
			float duration)
		{
			EnsureUI(target);

			Vector2 start = target.anchoredPosition;
			Vector2 control = (start + endValue) * 0.5f + Vector2.up * height;

			return target.DOPath(
					new Vector3[] { start, control, endValue },
					duration,
					PathType.CatmullRom)
				.SetEase(Ease.InOutSine)
				.SetTarget(target);
		}

		// =========================================================
		// 5. SPIRAL MOVE
		// =========================================================

		public static Tween DOSpiralTo(
			RectTransform target,
			Vector2 endValue,
			float radius,
			float rotations,
			float duration)
		{
			EnsureUI(target);

			Vector2 start = target.anchoredPosition;
			float elapsed = 0f;

			return DOTween.To(() => 0f, _ =>
			{
				elapsed += Time.unscaledDeltaTime;
				float t = Mathf.Clamp01(elapsed / duration);

				float angle = rotations * 360f * t * Mathf.Deg2Rad;
				float r = Mathf.Lerp(radius, 0f, t);

				Vector2 spiral = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
				target.anchoredPosition = Vector2.Lerp(start, endValue, t) + spiral;

			}, 1f, duration)
			.SetEase(Ease.Linear)
			.SetTarget(target);
		}

		// =========================================================
		// 6. MAGNET / ATTRACT
		// =========================================================

		public static Tween DOMagnetTo(
			RectTransform target,
			Vector2 endValue,
			float duration)
		{
			EnsureUI(target);

			return target.DOAnchorPos(endValue, duration)
				.SetEase(Ease.InExpo)
				.SetTarget(target);
		}

		// =========================================================
		// 7. WOBBLE MOVE
		// =========================================================

		public static Sequence DOWobbleMove(
			RectTransform target,
			Vector2 endValue,
			float duration)
		{
			EnsureUI(target);

			Tween move = target.DOAnchorPos(endValue, duration)
				.SetEase(Ease.OutCubic);

			Tween wobble = target.DOLocalRotate(
				Vector3.forward * 15f,
				0.1f,
				RotateMode.Fast)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);

			return DOTween.Sequence()
				.Join(move)
				.Join(wobble)
				.OnComplete(() => wobble.Kill())
				.SetTarget(target);
		}

		// =========================================================
		// 8. FADE EFFECTS
		// =========================================================

		public static Tween DOFadeIn(Image image, float duration)
		{
			Color c = image.color;
			c.a = 0;
			image.color = c;

			return image.DOFade(1f, duration)
				.SetTarget(image);
		}

		public static Tween DOFadeOut(Image image, float duration)
		{
			return image.DOFade(0f, duration)
				.SetTarget(image);
		}

		// =========================================================
		// 9. SHAKE + EXIT
		// =========================================================

		public static Sequence DOShakeFadeOut(
			RectTransform target,
			Image image,
			float duration)
		{
			return DOTween.Sequence()
				.Join(target.DOShakeAnchorPos(duration, 10, 10))
				.Join(image.DOFade(0, duration))
				.SetTarget(target);
		}

		// =========================================================
		// 10. TARGET PUNCH
		// =========================================================

		public static Tween DOTargetPunch(
			RectTransform target,
			float punchScale,
			float duration)
		{
			Vector3 original = target.localScale;

			return target.DOScale(original * punchScale, duration)
				.SetEase(Ease.OutBack)
				.OnComplete(() =>
					target.DOScale(original, duration * 0.6f))
				.SetTarget(target);
		}

		public static Tween DOOrbitSnap(
	RectTransform rt,
	Vector2 end,
	float radius,
	float orbitTime,
	float snapTime)
		{
			float angle = Random.Range(0f, 360f);
			Vector2 center = end;

			Tween orbit = DOTween.To(() => angle, a =>
			{
				angle = a;
				Vector2 pos = center + new Vector2(
					Mathf.Cos(angle * Mathf.Deg2Rad),
					Mathf.Sin(angle * Mathf.Deg2Rad)
				) * radius;
				rt.anchoredPosition = pos;
			}, angle + 360f, orbitTime).SetEase(Ease.Linear);

			return DOTween.Sequence()
				.Append(orbit)
				.Append(rt.DOAnchorPos(end, snapTime).SetEase(Ease.InBack));
		}

		public static Tween DOFloatThenCurve(
	RectTransform rt,
	Vector2 end,
	float height,
	float duration)
		{
			Vector2 start = rt.anchoredPosition;
			Vector2 up = start + Vector2.up * height;

			return DOTween.Sequence()
				.Append(rt.DOAnchorPos(up, duration * 0.4f).SetEase(Ease.OutQuad))
				.Append(rt.DOAnchorPos(end, duration * 0.6f).SetEase(Ease.InCubic));
		}

		public static Tween DOAccelerationBurst(
	RectTransform rt,
	Vector2 end,
	float duration)
		{
			return rt.DOAnchorPos(end, duration)
				.SetEase(new AnimationCurve(
					new Keyframe(0, 0),
					new Keyframe(0.7f, 0.1f),
					new Keyframe(1, 1)
				));
		}

		public static Tween DOVortexPull(
	RectTransform rt,
	Vector2 end,
	float radius,
	float rotations,
	float duration)
		{
			Vector2 start = rt.anchoredPosition;
			float t = 0f;

			return DOTween.To(() => 0f, _ =>
			{
				t += Time.unscaledDeltaTime / duration;
				float r = Mathf.Lerp(radius, 0, t);
				float a = rotations * Mathf.PI * 2 * t;

				Vector2 offset = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
				rt.anchoredPosition = Vector2.Lerp(start, end, t) + offset;

				rt.localScale = Vector3.one * Mathf.Lerp(1f, 0.3f, t);
			}, 1f, duration);
		}

		public static Tween DOOvershootSnap(
	RectTransform rt,
	Vector2 end,
	float overshoot,
	float duration)
		{
			Vector2 dir = (end - rt.anchoredPosition).normalized;
			Vector2 overshootPos = end + dir * overshoot;

			return DOTween.Sequence()
				.Append(rt.DOAnchorPos(overshootPos, duration * 0.7f).SetEase(Ease.OutQuad))
				.Append(rt.DOAnchorPos(end, duration * 0.3f).SetEase(Ease.InQuad));
		}

		public static Tween DOPulseWhileMoving(
	RectTransform rt,
	Vector2 end,
	float duration)
		{
			Tween move = rt.DOAnchorPos(end, duration).SetEase(Ease.InOutCubic);
			Tween pulse = rt.DOScale(1.15f, 0.15f)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);

			return DOTween.Sequence()
				.Join(move)
				.Join(pulse)
				.OnComplete(() => pulse.Kill());
		}

		public static Tween DOChaosToOrder(
	RectTransform rt,
	Vector2 end,
	float scatterRadius,
	float duration)
		{
			Vector2 chaos = rt.anchoredPosition + Random.insideUnitCircle * scatterRadius;

			return DOTween.Sequence()
				.Append(rt.DOAnchorPos(chaos, duration * 0.4f).SetEase(Ease.OutQuad))
				.Append(rt.DOAnchorPos(end, duration * 0.6f).SetEase(Ease.InExpo));
		}
	}
}
