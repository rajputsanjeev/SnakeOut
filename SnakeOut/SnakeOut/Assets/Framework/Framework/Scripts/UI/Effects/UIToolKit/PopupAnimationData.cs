// Assets/Scripts/UI/PopupAnimationData.cs
using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(menuName = "Popup / Animation Preset", fileName = "PopupAnimationPreset")]
	public class PopupAnimationData : ScriptableObject
	{
		public PopupAnimationType animationType = PopupAnimationType.ScaleIn;
		public bool autoReverse = true;

		[Header("Common")]
		public float duration = 0.5f;
		public float delay = 0f;
		public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		public bool useUnscaledTime = true;

		[Header("Scale")]
		public Vector3 startScale = Vector3.one * 0.0f;
		public Vector3 endScale = Vector3.one;
		public float overshoot = 1.2f; // for punch/elastic
		public float elasticity = 1.2f;
		public float damping = 0.8f;

		[Header("Position / Slide")]
		public Vector3 slideOffset = new Vector3(0, -200, 0); // local position offset for slide animations
		public float gravity = 9.8f; // for DropAndBounce
		public float bounceAmount = 0.35f;

		[Header("Rotation")]
		public Vector3 startRotation = Vector3.zero;
		public Vector3 endRotation = Vector3.zero;

		[Header("Fade")]
		public float startAlpha = 0f;
		public float endAlpha = 1f;

		[Header("Mask / Reveal")]
		public float maskStart = 0f;
		public float maskEnd = 1f;

		[Header("Particle / Effects")]
		public GameObject particlePrefab;
		public Vector3 particleLocalPos = Vector3.zero;

		[Header("Combo extras")]
		public bool useCanvasGroup = true; // for Fade animations
		public bool autoSetActive = true;   // whether show sets active and hide sets inactive at end

		[Header("Mask Reveal Settings")]
		public bool invertMask;
		public Vector2 rectRevealStartSize = new Vector2(0, 0);
		public Vector2 rectRevealEndSize = new Vector2(600, 600);

		public float wipeStart = 0f;
		public float wipeEnd = 1f;


		// Utility: clone to modify runtime without changing the asset
		public PopupAnimationData Clone()
		{
			var clone = ScriptableObject.CreateInstance<PopupAnimationData>();
			clone.autoReverse = autoReverse;
			clone.animationType = animationType;
			clone.duration = duration;
			clone.delay = delay;
			clone.curve = curve;
			clone.useUnscaledTime = useUnscaledTime;
			clone.startScale = startScale;
			clone.endScale = endScale;
			clone.overshoot = overshoot;
			clone.elasticity = elasticity;
			clone.damping = damping;
			clone.slideOffset = slideOffset;
			clone.gravity = gravity;
			clone.bounceAmount = bounceAmount;
			clone.startRotation = startRotation;
			clone.endRotation = endRotation;
			clone.startAlpha = startAlpha;
			clone.endAlpha = endAlpha;
			clone.maskStart = maskStart;
			clone.maskEnd = maskEnd;
			clone.particlePrefab = particlePrefab;
			clone.particleLocalPos = particleLocalPos;
			clone.useCanvasGroup = useCanvasGroup;
			clone.autoSetActive = autoSetActive;
			clone.invertMask = invertMask;
			clone.rectRevealStartSize = rectRevealStartSize;
			clone.rectRevealEndSize = rectRevealEndSize;
			clone.wipeStart = wipeStart;
			clone.wipeEnd = wipeEnd;
			return clone;
		}

	}
	public enum PopupAnimationType
	{
		// Scale
		ScaleIn,
		PunchScale,
		ElasticScale,
		BounceScale,

		// Position / Slide
		SlideFromTop,
		SlideFromBottom,
		SlideFromLeft,
		SlideFromRight,
		SlideThenPop,
		DropAndBounce,

		// Fade / Alpha
		FadeIn,
		FadeInScale,
		FadeInSlide,

		// Rotation
		RotateInZ,
		FlipInX,
		FlipInY,
		SwingIn,

		// Mask / Reveal (these will rely on a mask component or shader)
		CircularMaskReveal,
		RectMaskReveal,
		WipeReveal,
		WipeRevealLeftToRight,
		WipeRevealRightToLeft,
		WipeRevealTopToBottom,
		WipeRevealBottomToTop,

		// Effects
		ParticleBurst,
		GlowPulse,
		ShockwaveReveal,

		// Combo
		ScaleRotate,
		ScaleFade,
		SlideFade,
		SlideBounce,

		// Special
		Jelly,
		Heartbeat,
		ZoomToCamera,
		ShrinkFromBackground,

		// Exit
		ScaleOut,
		FadeOut,
		SlideOut,
		FlipOut,
		ExplodeOut,
		DropOut,
		MaskClose,
		SlideOutUp,
		SlideOutDown,
		SlideOutLeft,
		SlideOutRight,
		RotateOutZ,
		FlipOutX,
		FlipOutY,
		PopOut,
		PunchOut,
		BounceOut,
		ScaleDropOut,
	}
}