using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


/// <summary>
/// Example grid generator that uses the wave utility.
/// </summary>
public class GridWaveAnimation : MonoBehaviour
{
	[Header("Grid Settings")]
	public RectTransform dotPrefab;
	public int rows = 20;
	public int columns = 20;
	public float spacing = 30f;

	[Header("Animation Settings")]
	public float duration = 0.4f;
	public float delayMultiplier = 0.03f;
	public Ease ease = Ease.OutBack;


	private List<RectTransform> dots = new List<RectTransform>();

	public void CreateGrid()
	{
		// Clear previous dots (destroy children if needed)
		foreach (Transform child in transform)
		{
			if (Application.isPlaying)
				Destroy(child.gameObject);
			else
				DestroyImmediate(child.gameObject);
		}
		dots.Clear();

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				RectTransform dot = Instantiate(dotPrefab, transform);
				dot.anchoredPosition = new Vector2(c * spacing, -r * spacing);
				dot.localScale = Vector3.zero; // start hidden

				// CanvasGroup is added by the utility if missing
				dots.Add(dot);
			}
		}
	}

	public void PlayAnimation(StartPoint startPoint)
	{
		if (dots == null || dots.Count == 0) return;

		Vector2 origin = GetOrigin(startPoint);
		WaveAnimationUtility.PlayWaveAnimation(
			dots,
			origin,
			duration,
			delayMultiplier,
			startScale: Vector3.zero,
			endScale: Vector3.one,
			startAlpha: 0f,
			endAlpha: 1f,
			ease: ease
		);
	}

	private Vector2 GetOrigin(StartPoint point)
	{
		float width = (columns - 1) * spacing;
		float height = (rows - 1) * spacing;

		switch (point)
		{
			case StartPoint.Center:
				return new Vector2(width / 2f, -height / 2f);
			case StartPoint.TopLeft:
				return new Vector2(0, 0);
			case StartPoint.TopRight:
				return new Vector2(width, 0);
			case StartPoint.BottomLeft:
				return new Vector2(0, -height);
			case StartPoint.BottomRight:
				return new Vector2(width, -height);
			default:
				return Vector2.zero;
		}
	}
}