using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlotMechanism : MonoBehaviour
{
	[Header("UI")]
	[Tooltip("Masked viewport (RectTransform with Mask/RectMask2D).")]
	public RectTransform viewport;
	public Image iconPrefab;

	[Header("Spin Settings")]
	public float iconHeight = 180f;
	public float iconSpacing = 0f;
	public float spinSpeed = 1400f;
	public float minSpinTime = 1.5f;
	public float overshoot = 60f;
	public Ease slowDownEase = Ease.OutQuad;
	public Ease snapEase = Ease.OutBack;

	[Header("Debug")]
	public bool debugAlignment = true;

	private readonly List<Image> _icons = new();
	private Tween _loopTween;
	private bool _spinning;

	private float IconSlotHeight => iconHeight + iconSpacing;

	private float ViewportCenterY =>
		-viewport.rect.height * (0.5f - viewport.pivot.y);

	#region Setup

	public void Setup(List<Sprite> sprites)
	{
		Clear();

		for (int i = 0; i < sprites.Count; i++)
		{
			Image img = Instantiate(iconPrefab, viewport);
			RectTransform rt = img.rectTransform;

			img.sprite = sprites[i];
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);

			rt.anchoredPosition = new Vector2(0, -i * IconSlotHeight);
			_icons.Add(img);
		}
	}

	#endregion

	#region Spin

	public void SpinToIndex(int targetIndex, Action onComplete = null)
	{
		if (_spinning || _icons.Count == 0)
			return;

		_spinning = true;
		_loopTween?.Kill();

		_loopTween = DOTween.To(
				() => 0f,
				_ => MoveIcons(),
				1f,
				1f / spinSpeed
			)
			.SetLoops(-1)
			.SetEase(Ease.Linear);

		DOVirtual.DelayedCall(minSpinTime, () =>
		{
			StopAt(targetIndex, onComplete);
		});
	}

	#endregion

	#region Loop movement

	private void MoveIcons()
	{
		float move = spinSpeed * Time.deltaTime;
		float loopLimit = _icons.Count * IconSlotHeight;

		foreach (var img in _icons)
		{
			RectTransform rt = img.rectTransform;
			rt.anchoredPosition -= Vector2.up * move;

			if (rt.anchoredPosition.y <= -loopLimit)
				rt.anchoredPosition += Vector2.up * loopLimit;
		}
	}

	#endregion

	#region Stop & Align

	private void StopAt(int targetIndex, Action onComplete)
	{
		if (targetIndex < 0 || targetIndex >= _icons.Count)
		{
			Debug.LogError($"SlotReel: targetIndex {targetIndex} out of range.");
			_spinning = false;
			return;
		}

		_loopTween?.Kill();

		RectTransform targetRT = _icons[targetIndex].rectTransform;
		Vector2 currentPos = targetRT.anchoredPosition;

		// Calculate where the icon CENTER needs to be
		float iconCenterY = currentPos.y - iconHeight * (0.5f - targetRT.pivot.y);
		float delta = ViewportCenterY - iconCenterY;

		if (debugAlignment)
		{
			Debug.Log($"<b>Target Index:</b> {targetIndex}");
			Debug.Log($"<b>Current Position:</b> {currentPos.y}");
			Debug.Log($"<b>Icon Center Y:</b> {iconCenterY}");
			Debug.Log($"<b>Viewport Center Y:</b> {ViewportCenterY}");
			Debug.Log($"<b>Delta:</b> {delta}");
		}

		Sequence seq = DOTween.Sequence();

		// Ease-out deceleration (overshoot)
		seq.Append(MoveAll(delta + overshoot, 0.25f, slowDownEase));

		// Elastic snap back
		seq.Append(MoveAll(-overshoot, 0.2f, snapEase));

		seq.OnComplete(() =>
		{
			if (debugAlignment)
			{
				Debug.Log($"<b>FINAL Position:</b> {targetRT.anchoredPosition.y}");
				Debug.Log($"<b>FINAL Icon Center Y:</b> {targetRT.anchoredPosition.y - iconHeight * (0.5f - targetRT.pivot.y)}");
			}

			_spinning = false;
			onComplete?.Invoke();
		});
	}

	private Tween MoveAll(float delta, float duration, Ease ease)
	{
		Sequence s = DOTween.Sequence();
		foreach (var img in _icons)
		{
			RectTransform rt = img.rectTransform;
			s.Join(rt.DOAnchorPosY(rt.anchoredPosition.y + delta, duration)
				.SetEase(ease));
		}
		return s;
	}

	#endregion

	#region Cleanup

	public void Clear()
	{
		_loopTween?.Kill();
		foreach (var img in _icons)
		{
			if (img)
				Destroy(img.gameObject);
		}

		_icons.Clear();
		_spinning = false;
	}

	#endregion
}
