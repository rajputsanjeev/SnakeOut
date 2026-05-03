using DG.Tweening;
using Framework.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
	[RequireComponent(typeof(Button))]
	public class UIButtonEffects : MonoBehaviour,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerDownHandler,
	IPointerUpHandler,
	IPointerClickHandler
	{
		[Header("GENERAL SETTINGS")]
		public UIButtonEffectPreset preset;   // Preset assigned
		public bool overridePreset = false;   // Allow custom per-button overrides

		public bool enableScaleEffect = true;
		public bool enableColorEffect = false;
		public Image targetGraphic;

		public float animationDuration = 0.15f;

		[Header("HOVER EFFECT")]
		public float hoverScale = 1.05f;
		public Color hoverColor = Color.white;

		[Header("PRESS EFFECT")]
		public float pressScale = 0.9f;
		public Color pressColor = Color.white;

		[Header("CLICK EFFECT")]
		public float clickScale = 1.15f;
		public Color clickColor = Color.white;
		public AudioClip clickSFX;
		public float clickVolume = 1f;

		private Vector3 originalScale;
		private Color originalColor;
		private Button button;
		private bool IsPointerDown;
		private bool IsPointerPressed;
		private bool IsPointerUp;

		void Awake()
		{
			button = GetComponent<Button>();
			originalScale = transform.localScale;

			if (targetGraphic == null)
				targetGraphic = GetComponent<Image>();

			if (targetGraphic != null)
				originalColor = targetGraphic.color;

			ApplyPreset();
		}

		public void ApplyPreset()
		{
			if (preset == null || overridePreset) return;

			enableScaleEffect = preset.enableScaleEffect;
			enableColorEffect = preset.enableColorEffect;

			animationDuration = preset.animationDuration;

			hoverScale = preset.hoverScale;
			hoverColor = preset.hoverColor;

			pressScale = preset.pressScale;
			pressColor = preset.pressColor;

			clickScale = preset.clickScale;
			clickColor = preset.clickColor;

			clickSFX = preset.clickSFX;
			clickVolume = preset.clickVolume;
		}

		// ---------------- HOVER ----------------
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!button.interactable || IsPointerDown) return;

			PlayScale(hoverScale);
			PlayColor(hoverColor);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!button.interactable || IsPointerDown) return;

			ResetScale();
			ResetColor();
		}

		// ---------------- PRESS ----------------
		public void OnPointerDown(PointerEventData eventData)
		{
			if (!button.interactable) return;

			IsPointerDown = true;

			PlayScale(pressScale);
			PlayColor(pressColor);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!button.interactable) return;

			IsPointerDown = false;

			if (eventData.pointerEnter == gameObject)
			{
				PlayScale(hoverScale);
				PlayColor(hoverColor);
			}
			else
			{
				ResetScale();
				ResetColor();
			}
		}

		// ---------------- CLICK ----------------
		public void OnPointerClick(PointerEventData eventData)
		{
			IsPointerDown = false;


			// SFX
			if (clickSFX != null)
				AudioController.PlaySound(clickSFX);

			// Animation
			transform.DOKill();
			transform.DOScale(originalScale * clickScale, animationDuration * 0.8f)
				.OnComplete(() =>
				{
					PlayScale(hoverScale);
				});

			PlayColor(clickColor);

#if MODULE_HAPTIC
			Haptic.Play(Haptic.HAPTIC_HARD);
#endif
		}

		// ---------------- HELPERS ----------------
		private void PlayScale(float target)
		{
			if (!enableScaleEffect) return;
			transform.DOKill(true);
			transform.DOScale(originalScale * target, animationDuration);
		}

		private void ResetScale()
		{
			if (!enableScaleEffect) return;
			transform.DOKill(true);
			transform.DOScale(originalScale, animationDuration);
		}

		private void PlayColor(Color c)
		{
			if (!enableColorEffect || targetGraphic == null) return;
			targetGraphic.DOKill(true);
			targetGraphic.DOColor(c, animationDuration);
		}

		private void ResetColor()
		{
			if (!enableColorEffect || targetGraphic == null) return;
			targetGraphic.DOKill(true);
			targetGraphic.DOColor(originalColor, animationDuration);
		}
	}
}