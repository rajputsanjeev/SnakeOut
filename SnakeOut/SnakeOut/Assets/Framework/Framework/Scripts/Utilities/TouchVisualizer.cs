using UnityEngine;
using UnityEngine.InputSystem; // Imports the New Input System
using DG.Tweening;

public class TouchVisualizer : MonoBehaviour
{
	[Header("References")]
	public RectTransform feedbackDot;
	public RectTransform canvasRect;
	[Tooltip("Leave empty if Canvas is 'Screen Space - Overlay', otherwise assign your UI Camera")]
	public Camera uiCamera;

	[Header("Settings")]
	public float smoothSpeed = 15f;
	public float scaleDuration = 0.15f;

	private bool isActive = false;
	private Tween scaleTween;

	void Start()
	{
		feedbackDot.localScale = Vector3.zero;
	}

	void Update()
	{
		Vector2 inputPosition;
		bool isPressing = GetInput(out inputPosition);

		if (isPressing)
		{
			if (!isActive)
			{
				ShowDot(inputPosition);
			}
			MoveDot(inputPosition);
		}
		else if (isActive)
		{
			HideDot();
		}
	}

	bool GetInput(out Vector2 pos)
	{
		pos = Vector2.zero;

		// Pointer.current cleanly abstracts Touch, Mouse, and Pen interactions
		if (Pointer.current != null && Pointer.current.press.isPressed)
		{
			pos = Pointer.current.position.ReadValue();
			return true;
		}

		return false;
	}

	void ShowDot(Vector2 screenPos)
	{
		isActive = true;
		canvasRect.gameObject.SetActive(true);

		feedbackDot.anchoredPosition = ScreenToCanvas(screenPos);

		scaleTween?.Kill();
		scaleTween = feedbackDot.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack);
	}

	void MoveDot(Vector2 screenPos)
	{
		Vector2 targetPos = ScreenToCanvas(screenPos);

		feedbackDot.anchoredPosition = Vector2.Lerp(
			feedbackDot.anchoredPosition,
			targetPos,
			Time.deltaTime * smoothSpeed
		);
	}

	void HideDot()
	{
		isActive = false;

		scaleTween?.Kill();
		scaleTween = feedbackDot.DOScale(Vector3.zero, scaleDuration)
			.SetEase(Ease.InBounce)
			.OnComplete(() => canvasRect.gameObject.SetActive(false));
	}

	Vector2 ScreenToCanvas(Vector2 screenPos)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvasRect, screenPos, uiCamera, out Vector2 localPoint);
		return localPoint;
	}

	void OnDestroy()
	{
		scaleTween?.Kill();
	}
}
