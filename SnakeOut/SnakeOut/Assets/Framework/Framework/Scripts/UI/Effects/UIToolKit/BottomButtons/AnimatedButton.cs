using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System;

namespace Framework
{
	public class AnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public static AnimatedButton activeButton;

		public enum Axis { Vertical, Horizontal }
		public enum VerticalDirection { Both, Up, Down }
		public enum ButtonType { Sprite, Color, None }

		// ─────────────────────────────────────────────────────────────
		//  References
		// ─────────────────────────────────────────────────────────────
		[Header("References")]
		public Button button;
		public RectTransform icon;
		public TextMeshProUGUI label;
		public RectTransform ButtonRect;
		public Image ButtonImage;
		public Image IconImage;

		// ─────────────────────────────────────────────────────────────
		//  Behavior Flags
		// ─────────────────────────────────────────────────────────────
		[Header("Behavior Flags")]
		public bool IsSelfComponent = true;
		public bool IsBothMove;
		public bool IsIconMoveUpOnly;
		public bool IsIconMoveScale;
		public bool IsScaleBackgroundOnly;
		public bool IsHomeButton;

		// ─────────────────────────────────────────────────────────────
		//  Appearance Settings
		// ─────────────────────────────────────────────────────────────
		[Header("Appearance Settings")]
		public ButtonType ButtonTypeEnum = ButtonType.Color;
		public Sprite ButtonPressedSprite;
		public Sprite ButtonUnPressedSprite;
		public Color ButtonPressedColor = Color.white;
		public Color ButtonUnPressedColor = Color.white;

		// ─────────────────────────────────────────────────────────────
		//  Icon Settings
		// ─────────────────────────────────────────────────────────────
		[Header("Icon Settings")]
		public bool IsChangeIconOnPressed;
		public Sprite IconPressedSprite;
		public Sprite IconUnPressedSprite;

		// ─────────────────────────────────────────────────────────────
		//  Animation Settings
		// ─────────────────────────────────────────────────────────────
		[Header("Animation Settings")]
		public Axis expandAxis = Axis.Vertical;
		public VerticalDirection verticalDirection = VerticalDirection.Up;
		[Range(1f, 2f)] public float expandAmount = 1.2f;
		public float moveDistance = 20f;
		public float duration = 0.3f;
		public Ease easeType = Ease.OutBack;

		// After the Callbacks header/field, add a new region:

		// ─────────────────────────────────────────────────────────────
		//  Sound Settings
		// ─────────────────────────────────────────────────────────────
		[Header("Sound Settings")]
		public AudioSource AudioSource;
		public AudioClip ClickSound;
		[Range(0f, 1f)] public float SoundVolume = 1f;
		[Range(0.5f, 2f)] public float SoundPitch = 1f;

		// ─────────────────────────────────────────────────────────────
		//  Squash & Stretch Settings
		// ─────────────────────────────────────────────────────────────
		[Header("Squash & Stretch (Icon)")]
		[Tooltip("Enable squash-and-stretch juice on icon movement.")]
		public bool UseSquashStretch = true;

		[Tooltip("Per-axis scale multiplier applied during the squash phase.\n" +
				 "Each component multiplies the icon's original scale on that axis.\n" +
				 "Vertical squash example → X: 1.25  Y: 0.80  Z: 1.00\n" +
				 "Horizontal squash example → X: 0.80  Y: 1.25  Z: 1.00")]
		public Vector3 SquashScale = new Vector3(1.25f, 0.80f, 1f);

		[Tooltip("Per-axis scale multiplier applied during the stretch (launch) phase.\n" +
				 "Each component multiplies the icon's original scale on that axis.\n" +
				 "Vertical stretch example → X: 0.75  Y: 1.35  Z: 1.00\n" +
				 "Horizontal stretch example → X: 1.35  Y: 0.75  Z: 1.00")]
		public Vector3 StretchScale = new Vector3(0.75f, 1.35f, 1f);

		[Tooltip("Duration of the squash phase (fraction of total duration).")]
		[Range(0.1f, 0.5f)] public float SquashPhase = 0.2f;

		[Tooltip("Duration of the stretch phase (fraction of total duration).")]
		[Range(0.1f, 0.5f)] public float StretchPhase = 0.4f;

		[Tooltip("Ease for squash/stretch transitions.")]
		public Ease SquashStretchEase = Ease.OutQuad;

		// ─────────────────────────────────────────────────────────────
		//  Hover Settings
		// ─────────────────────────────────────────────────────────────
		[Header("Hover Settings")]
		public bool UseHoverEffect = true;
		[Range(0.9f, 1.2f)] public float HoverScale = 1.05f;
		public float HoverDuration = 0.15f;
		public Ease HoverEase = Ease.OutSine;

		// ─────────────────────────────────────────────────────────────
		//  Callbacks
		// ─────────────────────────────────────────────────────────────
		[Header("Callbacks")]
		[Tooltip("Fired when the button is pressed (use SetAction() for code-side callbacks).")]
		public UnityEngine.Events.UnityEvent OnClickEvent;

		// ─────────────────────────────────────────────────────────────
		//  Private state
		// ─────────────────────────────────────────────────────────────
		private Vector3 _originalScale;
		private Vector3 _originalIconScale;
		private Vector2 _originalIconPos;
		private Vector2 _originalPivot;
		private bool _isActive;
		private Action _onClickAction;

		// ─────────────────────────────────────────────────────────────
		//  Lifecycle
		// ─────────────────────────────────────────────────────────────
		private void Awake()
		{
			if (!button) button = GetComponent<Button>();
			if (!ButtonImage) ButtonImage = GetComponent<Image>();
			if (!icon) icon = GetComponentInChildren<Image>(true)?.rectTransform;
			if (IsSelfComponent) ButtonRect = GetComponent<RectTransform>();

			if (!ButtonRect)
			{
				Debug.LogError($"[AnimatedButton] {name}: Missing ButtonRect reference!");
				return;
			}

			if (label) label.gameObject.SetActive(false);

			_originalScale = ButtonRect.localScale;
			_originalIconScale = icon ? icon.localScale : Vector3.one;
			if (icon) _originalIconPos = icon.anchoredPosition;
			_originalPivot = ButtonRect.pivot;

			button.onClick.AddListener(OnClick);

			bool startActive = IsHomeButton;
			ApplyVisualState(startActive);
			if (startActive) OnClick();
		}

		private void OnDestroy()
		{
			// Kill any running tweens to avoid leaks
			ButtonRect?.DOKill();
			icon?.DOKill();
			label?.DOKill();
		}

		// ─────────────────────────────────────────────────────────────
		//  Public API
		// ─────────────────────────────────────────────────────────────
		public void SetAction(Action action) => _onClickAction = action;

		/// <summary>Trigger the press animation programmatically (e.g. from a NavManager).</summary>
		public void InvokeAnimation()
		{
			if (activeButton && activeButton != this)
				activeButton.ResetButton();

			activeButton = this;
			_isActive = true;

			Animate();
			ApplyVisualState(true);
		}

		// ─────────────────────────────────────────────────────────────
		//  Pointer events
		// ─────────────────────────────────────────────────────────────
		public void OnPointerEnter(PointerEventData _)
		{
			if (!UseHoverEffect || _isActive) return;
			ButtonRect.DOKill();
			ButtonRect.DOScale(_originalScale * HoverScale, HoverDuration)
					  .SetEase(HoverEase);
		}

		public void OnPointerExit(PointerEventData _)
		{
			if (!UseHoverEffect || _isActive) return;
			ButtonRect.DOKill();
			ButtonRect.DOScale(_originalScale, HoverDuration)
					  .SetEase(HoverEase);
		}

		// ─────────────────────────────────────────────────────────────
		//  Click handler
		// ─────────────────────────────────────────────────────────────
		private void OnClick()
		{
			if (activeButton && activeButton != this)
				activeButton.ResetButton();

			activeButton = this;
			_isActive = true;

			Animate();
			ApplyVisualState(true);
			PlayClickSound();

			_onClickAction?.Invoke();
			OnClickEvent?.Invoke();
		}

		// ─────────────────────────────────────────────────────────────
		//  Animation dispatch
		// ─────────────────────────────────────────────────────────────
		private void Animate()
		{
			if (IsBothMove) { AnimateBackground(); AnimateIconWithSquashStretch(); }
			else if (IsIconMoveUpOnly) { AnimateIconWithSquashStretch(); }
			else if (IsIconMoveScale) { AnimateIconExtend(); }
			else if (IsScaleBackgroundOnly) { AnimateBackground(); }

			AnimateLabel(true);
		}

		// ─────────────────────────────────────────────────────────────
		//  Icon – with Squash & Stretch
		// ─────────────────────────────────────────────────────────────

		/// <summary>
		/// Plays a three-phase Squash → Stretch → Settle sequence on the icon:
		///   1. Squash  – compress on movement axis, expand on perpendicular
		///   2. Stretch – as the icon launches, elongate on movement axis
		///   3. Settle  – snap back to original scale at the destination
		/// </summary>
		private void AnimateIconWithSquashStretch()
		{
			if (!icon) return;

			// Kill previous tweens on this transform
			icon.DOKill();

			Vector2 targetPos = _originalIconPos;
			if (expandAxis == Axis.Vertical) targetPos.y += moveDistance;
			else targetPos.x += moveDistance;

			if (!UseSquashStretch)
			{
				// Plain move without squash/stretch
				icon.DOAnchorPos(targetPos, duration).SetEase(easeType);
				return;
			}

			// ── Phase timings ──────────────────────────────────────────
			float squashDur = duration * SquashPhase;
			float stretchDur = duration * StretchPhase;
			float settleDur = duration * (1f - SquashPhase - StretchPhase);
			settleDur = Mathf.Max(settleDur, 0.05f);

			// ── Squash & Stretch scales ────────────────────────────────
			// SquashScale / StretchScale are per-axis multipliers set in the Inspector.
			// Vector3.Scale multiplies each component against the icon's original scale,
			// so the user has full independent X / Y / Z control.
			Vector3 squashScale = Vector3.Scale(_originalIconScale, SquashScale);
			Vector3 stretchScale = Vector3.Scale(_originalIconScale, StretchScale);

			// ── Scale sequence: Squash → Stretch → Settle ─────────────
			Sequence scaleSeq = DOTween.Sequence();
			scaleSeq.Append(icon.DOScale(squashScale, squashDur)
								.SetEase(SquashStretchEase))
					.Append(icon.DOScale(stretchScale, stretchDur)
								.SetEase(SquashStretchEase))
					.Append(icon.DOScale(_originalIconScale, settleDur)
								.SetEase(easeType));

			// ── Position: delayed launch (fires after squash) ──────────
			// The icon stays in place during squash, then moves during stretch/settle.
			float moveDur = stretchDur + settleDur;
			icon.DOAnchorPos(targetPos, moveDur)
				.SetDelay(squashDur)
				.SetEase(easeType);
		}

		// ─────────────────────────────────────────────────────────────
		//  Icon – scale extend variant (IsIconMoveScale)
		// ─────────────────────────────────────────────────────────────
		private void AnimateIconExtend()
		{
			if (!icon) return;
			icon.DOKill();

			Vector2 targetPos = _originalIconPos;
			if (expandAxis == Axis.Vertical) targetPos.y += moveDistance;

			if (UseSquashStretch)
			{
				// Squash → Stretch → Settle on scale, then move + expand
				AnimateIconWithSquashStretch();
				// Override: also reach expandAmount as final scale goal
				icon.DOScale(_originalIconScale * expandAmount, duration * 0.5f)
					.SetDelay(duration * 0.5f)
					.SetEase(easeType);
			}
			else
			{
				icon.DOAnchorPos(targetPos, duration).SetEase(easeType);
				icon.DOScale(_originalIconScale * expandAmount, duration).SetEase(easeType);
			}
		}

		// ─────────────────────────────────────────────────────────────
		//  Background scale
		// ─────────────────────────────────────────────────────────────
		private void AnimateBackground()
		{
			if (!ButtonRect) return;

			ButtonRect.DOKill();

			if (expandAxis == Axis.Vertical)
			{
				ButtonRect.pivot = verticalDirection switch
				{
					VerticalDirection.Up => new Vector2(_originalPivot.x, 0f),
					VerticalDirection.Down => new Vector2(_originalPivot.x, 1f),
					_ => _originalPivot,
				};
			}

			Vector3 targetScale = _originalScale;
			if (expandAxis == Axis.Vertical) targetScale.y *= expandAmount;
			else targetScale.x *= expandAmount;

			ButtonRect.DOScale(targetScale, duration).SetEase(easeType);
		}

		// ─────────────────────────────────────────────────────────────
		//  Label
		// ─────────────────────────────────────────────────────────────
		private void AnimateLabel(bool show)
		{
			if (!label) return;

			label.DOKill();

			if (show)
			{
				label.gameObject.SetActive(true);
				label.alpha = 0f;
				label.DOFade(1f, duration * 0.6f);
			}
			else
			{
				label.DOFade(0f, duration * 0.5f)
					 .OnComplete(() => label.gameObject.SetActive(false));
			}
		}

		// ─────────────────────────────────────────────────────────────
		//  Visual state
		// ─────────────────────────────────────────────────────────────
		private void ApplyVisualState(bool isPressed)
		{
			if (ButtonImage)
			{
				switch (ButtonTypeEnum)
				{
					case ButtonType.Color:
						ButtonImage.color = isPressed ? ButtonPressedColor : ButtonUnPressedColor;
						break;
					case ButtonType.Sprite:
						ButtonImage.sprite = isPressed ? ButtonPressedSprite : ButtonUnPressedSprite;
						break;
				}
			}

			if (IsChangeIconOnPressed && IconImage)
			{
				IconImage.sprite = isPressed ? IconPressedSprite : IconUnPressedSprite;
				IconImage.SetNativeSize();
			}
		}

		// ─────────────────────────────────────────────────────────────
		//  Reset
		// ─────────────────────────────────────────────────────────────
		public void ResetButton()
		{
			if (!ButtonRect) return;

			_isActive = false;

			ButtonRect.DOKill();
			ButtonRect.pivot = _originalPivot;
			ButtonRect.DOScale(_originalScale, duration).SetEase(easeType);

			if (icon)
			{
				icon.DOKill();
				icon.DOAnchorPos(_originalIconPos, duration).SetEase(easeType);
				icon.DOScale(_originalIconScale, duration).SetEase(easeType);
			}

			AnimateLabel(false);
			ApplyVisualState(false);
		}

		private void PlayClickSound()
		{
			if (!ClickSound) return;

			if (AudioSource)
			{
				AudioSource.pitch = SoundPitch;
				AudioSource.volume = SoundVolume;
				AudioSource.PlayOneShot(ClickSound);
			}
			else
			{
				// Fallback: play at world position with no AudioSource reference
				AudioSource.PlayClipAtPoint(ClickSound, Camera.main
					? Camera.main.transform.position
					: Vector3.zero, SoundVolume);
			}
		}

		// ─────────────────────────────────────────────────────────────
		//  Editor helpers
		// ─────────────────────────────────────────────────────────────
#if UNITY_EDITOR
		[ContextMenu("Preview Press")]
		private void EditorPreviewPress() => InvokeAnimation();

		[ContextMenu("Preview Reset")]
		private void EditorPreviewReset() => ResetButton();
#endif
	}
}