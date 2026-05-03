using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Framework
{
	public class ToastMessage : GenericSingletonClass<ToastMessage>
	{
		[Header("Toast Prefab")]
		[SerializeField] private RectTransform toastPrefab;
		[SerializeField] private RectTransform toastParent;

		[Header("Animation Settings")]
		public float slideDistance = 250f;
		public float slideDuration = 0.4f;
		public float stayDuration = 1.5f;
		public float fadeDuration = 0.3f;

		public void Show(string message)
		{
			// ---------- Instantiate ----------
			var toast = Instantiate(toastPrefab, toastParent);
			toast.localScale = Vector3.one;

			var canvasGroup = toast.GetComponent<CanvasGroup>();
			var messageText = toast.GetComponentInChildren<TextMeshProUGUI>();

			if (canvasGroup == null || messageText == null)
			{
				Debug.LogError("Toast prefab missing CanvasGroup or TextMeshProUGUI");
				Destroy(toast.gameObject);
				return;
			}

			messageText.text = message;

			// ---------- Reset ----------
			Vector2 startPos = toast.anchoredPosition;
			canvasGroup.alpha = 0f;

			DOTween.Kill(toast); // safety

			// ---------- Animation ----------
			Sequence seq = DOTween.Sequence().SetUpdate(true);

			// Fade in
			seq.Append(canvasGroup.DOFade(1f, 0.1f));

			// Slide up
			seq.Append(
				toast.DOAnchorPosY(startPos.y + slideDistance, slideDuration)
					.SetEase(Ease.OutBack)
			);

			// Stay
			seq.AppendInterval(stayDuration);

			// Fade out + slight down
			seq.Append(canvasGroup.DOFade(0f, fadeDuration));
			seq.Join(
				toast.DOAnchorPosY(startPos.y + slideDistance - 40f, fadeDuration)
			);

			// ---------- Cleanup ----------
			seq.OnComplete(() =>
			{
				Destroy(toast.gameObject);
			});

			seq.OnKill(() =>
			{
				Destroy(toast.gameObject);
			});
		}
	}
}
