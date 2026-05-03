using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public enum RadialWaveType { Ripple, GridWave, ScaleFade }

public static class WaveAnimationUtility3D
{
	public static Action OnComplete { set; get; }

	public static void PlayRadialWave(
		IList<Transform> elements,
		Vector3 origin,
		RadialWaveType type,
		RadialWaveConfig config = null)
	{
		config ??= new RadialWaveConfig();

		switch (type)
		{
			case RadialWaveType.GridWave:
				PlayGridWaveAnimation(elements, origin,
					config.TotalWaveDuration, config.BandWidthFraction,
					config.PeakScaleMultiplier, config.RestScale);
				break;

			case RadialWaveType.ScaleFade:
				PlayRadialScaleFadeAnimation(elements, origin,
					config.Duration, config.DelayMultiplier,
					config.EdgeScaleFactor, config.EdgeAlpha,
					config.StartScale, config.Ease);
				break;
		}
	}

	private static void EffectDone(List<Sequence> sequences)
	{
		foreach (var seq in sequences)
			seq?.Kill();

		sequences.Clear();
		OnComplete?.Invoke();
	}

	public static void PlayWaveAnimation(
		IList<Transform> elements,
		Vector3 origin,
		float duration = 0.4f,
		float delayMultiplier = 0.03f,
		Vector3? startScale = null,
		Vector3? endScale = null,
		Ease ease = Ease.OutBack)
	{
		if (elements == null || elements.Count == 0) return;

		Vector3 actualStartScale = startScale ?? Vector3.zero;
		Vector3 actualEndScale = endScale ?? Vector3.one;

		var sequences = new List<Sequence>();

		Sequence lastSeq = null;
		float maxDelay = -1f;

		for (int i = 0; i < elements.Count; i++)
		{
			Transform element = elements[i];
			if (element == null) continue;

			DOTween.Kill(element);

			float distance = Vector3.Distance(element.localPosition, origin);
			float delay = distance * delayMultiplier;

			element.localScale = actualStartScale;

			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(delay);
			seq.Append(
				element.DOScale(actualEndScale, duration)
					   .SetEase(ease)
					   .OnStart(() =>
					   {
						   if (element == null)
							   seq.Kill();
					   })
			);
			seq.SetTarget(element);
			sequences.Add(seq);

			if (delay > maxDelay)
			{
				maxDelay = delay;
				lastSeq = seq;
			}
		}

		lastSeq?.OnComplete(() => EffectDone(sequences));
	}

	public static void PlayRipplePulseAnimation(
		IList<Transform> elements,
		Vector2Int originCell,
		int gridWidth,
		RadialWaveConfig config = null)
	{
		if (elements == null || elements.Count == 0) return;

		config ??= new RadialWaveConfig();

		Vector3 actualRestScale = config.RestScale ?? Vector3.one;
		Vector3 peakScale = actualRestScale * config.PeakScaleMultiplier;

		int maxDist = 0;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i] == null) continue;
			Vector2Int cell = new Vector2Int(i % gridWidth, i / gridWidth);
			int dist = Mathf.Max(Mathf.Abs(cell.x - originCell.x), Mathf.Abs(cell.y - originCell.y));
			maxDist = Mathf.Max(maxDist, dist);
		}
		if (maxDist == 0) maxDist = 1;

		var sequences = new List<Sequence>();
		Sequence lastSeq = null;
		float maxDelay = -1f;

		for (int i = 0; i < elements.Count; i++)
		{
			var element = elements[i];
			if (element == null) continue;

			DOTween.Kill(element);

			Vector2Int cell = new Vector2Int(i % gridWidth, i / gridWidth);
			int dist = Mathf.Max(Mathf.Abs(cell.x - originCell.x), Mathf.Abs(cell.y - originCell.y));
			float delay = (dist / (float)maxDist) * config.WavePropagationTime;

			element.localScale = actualRestScale;

			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(delay);
			seq.Append(element.DOScale(peakScale, config.PulseDuration * 0.4f).SetEase(Ease.OutQuad));
			seq.Append(element.DOScale(actualRestScale, config.PulseDuration * 0.6f).SetEase(Ease.InQuad));
			seq.SetTarget(element);
			sequences.Add(seq);

			if (delay > maxDelay) { maxDelay = delay; lastSeq = seq; }
		}

		lastSeq?.OnComplete(()
			=>
		EffectDone(sequences)
		
		);
	}

	/// <summary>
	/// Elements closer to origin end up larger and more opaque.
	/// Requires each element to have a CanvasGroup component for alpha fade.
	/// </summary>
	public static void PlayRadialScaleFadeAnimation(
		IList<Transform> elements,
		Vector3 origin,
		float duration = 0.5f,
		float delayMultiplier = 0.03f,
		float edgeScaleFactor = 0.4f,
		float edgeAlpha = 0f,
		Vector3? startScale = null,
		Ease ease = Ease.OutBack)
	{
		if (elements == null || elements.Count == 0) return;

		Vector3 actualStartScale = startScale ?? Vector3.zero;

		float maxDistance = 0f;
		foreach (var e in elements)
			if (e != null) maxDistance = Mathf.Max(maxDistance, Vector3.Distance(e.localPosition, origin));

		if (maxDistance <= 0f) maxDistance = 1f;

		var sequences = new List<Sequence>();
		Sequence lastSeq = null;
		float maxDelay = -1f;

		foreach (var element in elements)
		{
			if (element == null) continue;

			DOTween.Kill(element);

			float dist = Vector3.Distance(element.localPosition, origin);
			float normalizedDist = dist / maxDistance;
			float delay = dist * delayMultiplier;

			Vector3 targetScale = Vector3.Lerp(Vector3.one, Vector3.one * edgeScaleFactor, normalizedDist);
			float targetAlpha = Mathf.Lerp(1f, edgeAlpha, normalizedDist);

			element.localScale = actualStartScale;

			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(delay);
			seq.Append(element.DOScale(targetScale, duration).SetEase(ease));

			CanvasGroup cg = element.GetComponent<CanvasGroup>();
			if (cg != null)
			{
				cg.alpha = 0f;
				seq.Join(cg.DOFade(targetAlpha, duration).SetEase(Ease.OutQuad));
			}

			seq.SetTarget(element);
			sequences.Add(seq);

			if (delay > maxDelay)
			{
				maxDelay = delay;
				lastSeq = seq;
			}
		}

		lastSeq?.OnComplete(() => EffectDone(sequences));
	}

	public static void PlayGridWaveAnimation(
		IList<Transform> elements,
		Vector3 origin,
		float totalWaveDuration = 1.0f,
		float bandWidthFraction = 0.25f,
		float peakScaleMultiplier = 1.25f,
		Vector3? restScale = null)
	{
		if (elements == null || elements.Count == 0) return;

		Vector3 actualRestScale = restScale ?? Vector3.one;
		Vector3 peakScale = actualRestScale * peakScaleMultiplier;

		float maxDistance = 0f;
		foreach (var e in elements)
			if (e != null) maxDistance = Mathf.Max(maxDistance, Vector3.Distance(e.localPosition, origin));

		if (maxDistance <= 0f) maxDistance = 1f;

		float bandDuration = totalWaveDuration * bandWidthFraction;

		var sequences = new List<Sequence>();
		Sequence lastSeq = null;
		float maxDelay = -1f;

		foreach (var element in elements)
		{
			if (element == null) continue;

			DOTween.Kill(element);

			float normalizedDist = Vector3.Distance(element.localPosition, origin) / maxDistance;
			float delay = normalizedDist * totalWaveDuration;

			element.localScale = actualRestScale;

			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(delay);
			seq.Append(element.DOScale(peakScale, bandDuration * 0.5f).SetEase(Ease.OutBack));
			seq.Append(element.DOScale(actualRestScale, bandDuration * 0.5f).SetEase(Ease.InCubic));
			seq.SetTarget(element);
			sequences.Add(seq);

			if (delay > maxDelay)
			{
				maxDelay = delay;
				lastSeq = seq;
			}
		}

		lastSeq?.OnComplete(() => EffectDone(sequences));
	}

	[System.Serializable]
	public class RadialWaveConfig
	{
		// Shared
		public float Duration = 0.5f;
		public float DelayMultiplier = 0.03f;
		public Vector3? RestScale = null;
		public Vector3? StartScale = null;
		public Ease Ease = Ease.OutBack;
		public float PeakScaleMultiplier = 1.3f;

		// Ripple
		public float WavePropagationTime = 0.6f;
		public float PulseDuration = 0.35f;

		// Grid Wave
		public float TotalWaveDuration = 1.0f;
		public float BandWidthFraction = 0.25f;

		// Scale Fade
		public float EdgeScaleFactor = 0.4f;
		public float EdgeAlpha = 0f;
	}
}