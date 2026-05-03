using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Framework 
{
	public static class ScrollRectHelper
	{
		// ================= INDEX =================
		public static void ScrollToIndex(
			ScrollRect scrollRect,
			int index,
			bool smooth = false,
			float duration = 0.25f,
			MonoBehaviour runner = null)
		{
			if (!IsValid(scrollRect)) return;
			if (index < 0 || index >= scrollRect.content.childCount) return;

			var target = scrollRect.content.GetChild(index) as RectTransform;
			ScrollToItem(scrollRect, target, smooth, duration, runner);
		}

		// ================= ITEM =================
		public static void ScrollToItem(
			ScrollRect scrollRect,
			RectTransform target,
			bool smooth = false,
			float duration = 0.25f,
			MonoBehaviour runner = null)
		{
			if (!IsValid(scrollRect) || !target) return;

			Canvas.ForceUpdateCanvases();

			Vector2 normalized = CalculateNormalizedPosition(scrollRect, target);

			if (smooth)
				StartSmooth(scrollRect, normalized, duration, runner);
			else
				scrollRect.normalizedPosition = normalized;
		}

		// ================= NORMALIZED =================
		public static void ScrollToNormalized(
			ScrollRect scrollRect,
			Vector2 normalized,
			bool smooth = false,
			float duration = 0.25f,
			MonoBehaviour runner = null)
		{
			if (!scrollRect) return;

			if (smooth)
				StartSmooth(scrollRect, normalized, duration, runner);
			else
				scrollRect.normalizedPosition = normalized;
		}

		public static void ScrollToNormalized(
			ScrollRect scrollRect,
			float value,
			bool smooth = false,
			float duration = 0.25f,
			MonoBehaviour runner = null)
		{
			Vector2 pos = scrollRect.horizontal
				? new Vector2(value, 1)
				: new Vector2(0, value);

			ScrollToNormalized(scrollRect, pos, smooth, duration, runner);
		}

		// ================= CORE =================
		private static Vector2 CalculateNormalizedPosition(
			ScrollRect scrollRect,
			RectTransform target)
		{
			RectTransform content = scrollRect.content;
			RectTransform viewport = scrollRect.viewport;

			Vector2 contentSize = content.rect.size;
			Vector2 viewportSize = viewport.rect.size;

			Vector2 childLocalPos = (Vector2)content.InverseTransformPoint(target.position);
			Vector2 contentLocalPos = (Vector2)content.localPosition;

			Vector2 offset = contentLocalPos - childLocalPos;
			Vector2 scrollable = contentSize - viewportSize;

			float x = scrollable.x > 0 ? Mathf.Clamp01(offset.x / scrollable.x) : 0f;
			float y = scrollable.y > 0 ? Mathf.Clamp01(1 - (offset.y / scrollable.y)) : 1f;

			return new Vector2(x, y);
		}

		// ================= SMOOTH =================
		private static void StartSmooth(
			ScrollRect scrollRect,
			Vector2 target,
			float duration,
			MonoBehaviour runner)
		{
			if (runner == null)
			{
				scrollRect.normalizedPosition = target;
				return;
			}

			runner.StartCoroutine(SmoothScroll(scrollRect, target, duration));
		}

		private static IEnumerator SmoothScroll(
			ScrollRect scrollRect,
			Vector2 target,
			float duration)
		{
			Vector2 start = scrollRect.normalizedPosition;
			float t = 0f;

			while (t < duration)
			{
				t += Time.unscaledDeltaTime;
				scrollRect.normalizedPosition = Vector2.Lerp(start, target, t / duration);
				yield return null;
			}

			scrollRect.normalizedPosition = target;
		}

		// ================= VALIDATION =================
		private static bool IsValid(ScrollRect scrollRect)
		{
			return scrollRect != null &&
				   scrollRect.content != null &&
				   scrollRect.viewport != null;
		}
	}
}