using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Framework
{
	public class ButtonClickEffect : MonoBehaviour
	{
		[Header("Animation Settings")]
		public float scaleDown = 0.9f;     // Size when pressed
		public float duration = 0.1f;      // Speed of animation

		private Vector3 originalScale;
		private Button button;

		void Awake()
		{
			button = GetComponent<Button>();
			originalScale = transform.localScale;

			if (button != null)
				button.onClick.AddListener(AnimateClick);
		}

		void AnimateClick()
		{
			// Kill existing tweens (avoids stacking animations)
			transform.DOKill();

			// Scale down then back to normal
			transform.DOScale(originalScale * scaleDown, duration).OnComplete(() =>
			{
				transform.DOScale(originalScale, duration);
			});
		}
	}
}