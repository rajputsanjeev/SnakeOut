using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

namespace Framework
{
	public class UIPulseRings : MonoBehaviour
	{
		[Header("References")]
		public RectTransform center;
		public GameObject pulseRingPrefab;

		[Header("Pulse Count")]
		[Range(1, 5)]
		public int ringCount = 3;

		[Header("Change Sibling")]
		public bool isChangeSibling = true;

		[Header("Scale")]
		public float startScale = 0.9f;
		public float endScale = 2f;

		[Header("Timing")]
		public float pulseDuration = 1.5f;
		public float delayBetweenRings = 0.3f;

		[Header("Fade")]
		public float startAlpha = 1f;
		public float endAlpha = 0f;

		[Header("Loop")]
		public bool loop = true;

		private readonly List<Image> rings = new();
		private Sequence pulseSequence;

		private void OnEnable()
		{
			CreateRings();
			Play();
		}

		private void OnDisable()
		{
			pulseSequence?.Kill();
			Cleanup();
		}

		private void CreateRings()
		{
			Cleanup();
			if (center == null)
			{
				return;
			}
			for (var i = 0; i < ringCount; i++)
			{
				var go = Instantiate(pulseRingPrefab, center);
				go.SetActive(true);

				var rt = go.GetComponent<RectTransform>();
				if (isChangeSibling) rt.SetSiblingIndex(0);
				rt.anchorMin = Vector2.one / 2;
				rt.anchorMax = Vector2.one / 2;
				rt.pivot = Vector2.one / 2;
				rt.anchoredPosition = Vector2.zero;
				rt.sizeDelta = center.sizeDelta;
				rt.localScale = Vector3.one * startScale;

				var img = go.GetComponent<Image>();
				img.color = SetAlpha(img.color, startAlpha);

				rings.Add(img);
			}
		}

		private void Play()
		{
			pulseSequence?.Kill();
			pulseSequence = DOTween.Sequence();

			for (int i = 0; i < rings.Count; i++)
			{
				var ring = rings[i];
				ring.rectTransform.localScale = Vector3.one * startScale;
				ring.color = SetAlpha(ring.color, startAlpha);

				pulseSequence.Insert(
					i * delayBetweenRings,
					CreateRingTween(ring)
				);
			}

			if (loop)
				pulseSequence.SetLoops(-1);
		}

		private Tween CreateRingTween(Image ring)
		{
			return DOTween.Sequence()
				.Join(ring.rectTransform.DOScale(endScale, pulseDuration))
				.Join(ring.DOFade(endAlpha, pulseDuration))
				.SetEase(Ease.OutQuad);
		}

		private void Cleanup()
		{
			foreach (var r in rings)
				if (r) Destroy(r.gameObject);

			rings.Clear();
		}

		private Color SetAlpha(Color c, float a)
		{
			c.a = a;
			return c;
		}
	}
}