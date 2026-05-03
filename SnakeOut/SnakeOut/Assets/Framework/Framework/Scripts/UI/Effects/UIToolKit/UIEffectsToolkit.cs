using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIEffectsToolkit : MonoBehaviour
{
	public enum UIEffectType
	{
		None,
		PopupElastic,
		FallBounce,
		SlideInFromLeft,
		Shake,
		PulseGlow,
		FlipReveal,
		SquashStretch,
		CascadeListReveal,
		LeafFallLoop,
		HeartBeatLoop,
		RotateLoop,
		RotateAndPumpLoop,
		FadeInOutLoop,
		PumpSequence,
		PopupElasticWithPump,
		PopupElasticWithHeartBeat,
		LeafWindEffect,
		LeafBranchSwayEffect,
		BouncePop,
		SinWave,
		SinWaveMove,
		BounceSquashStretch,
		PingPongRotation
	}

	[Header("General Settings")]
	public UIEffectType effectType = UIEffectType.None;
	public RectTransform target;
	public float duration = 0.5f;
	public Ease easeType = Ease.OutBack;
	public bool playOnAwake = false;
	public float delayOnAwake = 0;

	[Header("Callbacks")]
	public UnityEvent OnEffectComplete;          // Single callback
	public UnityEvent PlayAnotherEffectCallback; // Chain effect callback

	// For specific effects
	[Header("Rotation Settings")]
	public float rotationSpeed = 30f; // Degrees per second

	[Header("Fade Settings")]
	public CanvasGroup targetCanvasGroup;
	public float minAlpha = 0f;
	public float maxAlpha = 1f;

	[Header("Pump Settings")]
	public float pumpScaleMultiplier = 1.2f;

	[Header("Fall Bounce Settings")]
	public bool fallStartFromScreenTop = false; // If true, start just outside top
	public RectTransform fallStartRect;         // Optional: explicit start 
	public float fallStartYOffset = 800f;    // How high above to start the fall
	public float fallDropTime = 0.7f;        // Time for dropping down
	public float fallBounceOffset = 25f;     // How much it bounces back up after landing
	public float fallBounceUpTime = 0.3f;    // Time for bouncing up
	public float fallBounceDownTime = 0.3f;  // Time to settle after bounce

	[Header("Pump Sequence Settings")]
	public float pumpLargeScaleMultiplier = 1.3f;   // How big the first pump out is
	public float pumpLargeScaleTime = 0.3f;         // Time for first pump out

	public float pumpSmallScaleMultiplier = 0.8f;   // How small it shrinks after
	public float pumpSmallScaleTime = 0.3f;         // Time for shrinking

	public float pumpMediumScaleMultiplier = 1.1f;  // End size (smaller than first pump)
	public float pumpMediumScaleTime = 0.3f;        // Time for final pump

	[Header("Leaf Fall Loop Settings")]
	public bool useSelfAsFirstStart = false;    // Use target's own transform as the first start position
	public bool useSelfAsLoopStart = false;    // Use target's own transform as the loop start position

	public RectTransform leafFallFirstStartRect; // Explicit first start rect (optional)
	public RectTransform leafFallLoopStartRect;  // Explicit loop start rect (optional)
	public RectTransform leafFallEndRect;
	public float leafFallDuration = 3f;

	// Internal storage for saved starting pos (if using self)
	private Vector2 savedFirstStartPos;
	private Vector2 savedLoopStartPos;

	[Header("Shake Settings")]
	public float shakeDuration = 0.5f;           // How long each shake lasts
	public Vector2 shakeStrength = new Vector2(20f, 0); // Shake strength in X/Y
	public int shakeVibrato = 10;                 // Number of shakes during the duration
	public float shakeRandomness = 90f;           // Random angle variation
	public bool shakeSnapping = false;            // Snap to integer values
	public bool shakeFadeOut = true;              // Fade over time

	[Header("Leaf Fall Wing Effect Settings")]
	public float wingSwayDistance = 100f;       // Horizontal sway distance in px
	public float wingSwayDuration = 1.5f;       // Duration of one sway cycle
	public float wingRotationAngle = 10f;       // Max rotation angle in deg
	public float wingRotationDuration = 1.2f;   // Duration of one rotation flutter

	[Header("Center Wind Effect Settings")]
	public float windDriftRadius = 200f;      // Max distance the leaf will "drift" from center
	public float windDriftDuration = 4f;      // How long one wind drift cycle lasts
	public float windRotationStrength = 30f;  // Max rotation (deg) due to wind
	public float windRotationDuration = 2f;   // How fast leaf spins back and forth

	[Header("Branch Sway/Flow Effect Settings")]
	public bool swayRandomize = false;
	public float swayDistance = 80f;        // Max X distance for swaying (pixels)
	public float swayDuration = 1.5f;       // One full side-to-side "wind" cycle (seconds)
	public float swayYOffset = 0f;          // Optional vertical offset from branch (pixels)

	[Header("Bounce Pop Effect Settings")]
	public bool bounceLoop = false;          // If true, effect repeats 
	public float bounceMoveOffset = 120f;      // How high to move up
	public float bounceMoveTime = 0.2f;        // Time to rise
	public float bounceUpscaleFactor = 1.25f;  // Scale up multiplier
	public float bounceUpscaleTime = 0.2f;     // Time to upscale
	public float bounceDownscaleFactor = 0.85f;// Scale down multiplier
	public float bounceDownscaleTime = 0.13f;  // Time to downscale
	public float bounceRecoverTime = 0.15f;    // Time to return to original

	[Header("Sin Wave Move Settings")]
	public RectTransform sinWaveStartRect;     // horizontal start
	public RectTransform sinWaveEndRect;       // horizontal end
	public float sinWaveDuration = 3f;         // time to go from start → end
	public float sinWaveAmplitude = 50f;       // vertical wave height (px)
	public int sinWaveLoops = -1;              // -1 = infinite
	public bool sinWavePingPong = true;        // go back and forth

	[Header("Bounce Squash Stretch")]
	public Vector3 bagSquashScale = new Vector3(0.85f, 1.15f, 1f);    // Phase 1 & 4
	public Vector3 bagStretchScale = new Vector3(1.15f, 0.85f, 1f);   // Phase 2
	public float bagBounceHeight = 120f;
	public float bagBounceDuration = 0.4f;
	public float apexPause = 0.1f;
	public float bagSettleDuration = 0.15f;
	public float pauseBetweenLoops = 0.5f;

	[Header("Easing")]
	public Ease bagLaunchEase = Ease.OutQuad;
	public Ease bagFallEase = Ease.InQuad;
	public Ease bagSettleEase = Ease.OutQuad;

	[Header("Callbacks")]
	public UnityEvent onTopCoinsMoveUp;
	public UnityEvent onScatterBottomCoins;
	public UnityEvent onTopCoinsMoveDown;

	[Header("Ping Pong Rotation")]
	public float rotationLeftAngle = -15f;
	public float rotationRightAngle = 15f;
	public float rotationDuration = 0.8f;
	public bool pingPongLoop = true;

	[Header("Punch Scale")]
	public float punchInScale = 1.1f;          // Scale at rotation extremes
	public float punchOutScale = 0.95f;        // Scale at center
	public float punchDuration = 0.2f;         // Punch timing
	public float punchIntensity = 0.3f;

	[Header("Looping")]
	public bool loopEffect = false;
	public int loopCount = -1; // -1 = infinite

	public bool windDriftRandomize = true;    // Randomizes next target on each drift
	[Space]
	public bool shakeLoop = false;                // Should it loop?
	public float shakeLoopInterval = 1f;          // Time between shakes if looping

	private Vector3 initialScale;
	private Vector3 initialPos;
	private Sequence currentSequence;
	private Tween currentTween;

	private void Awake()
	{
		if (!target) target = GetComponent<RectTransform>();
		if (!targetCanvasGroup) targetCanvasGroup = GetComponent<CanvasGroup>();

		initialScale = target.localScale;
		initialPos = target.anchoredPosition;

		if (playOnAwake)
		{
			if (delayOnAwake == 0)
			{
				PlayEffect(effectType);
			}
			else
			{
				PlayEffectWithDelay();
			}
		}
	}

	public void PlayCurrentEffect()
	{
		if (delayOnAwake == 0)
		{
			PlayEffect(effectType);
		}
		else
		{
			PlayEffectWithDelay();
		}
	}

	public void PlayEffectWithDelay()
	{
		DOTween.Sequence()
			.AppendInterval(delayOnAwake)
			.AppendCallback(() => PlayEffect(effectType));
	}

	public void PlayEffect(UIEffectType type)
	{
		// Kill previous animations
		DOTween.Kill(target);
		DOTween.Kill(targetCanvasGroup);
		if (currentSequence != null) currentSequence.Kill();

		effectType = type;

		switch (type)
		{
			case UIEffectType.PopupElastic: PopupElastic(); break;
			case UIEffectType.FallBounce: FallBounce(); break;
			case UIEffectType.SlideInFromLeft: SlideInFromLeft(); break;
			case UIEffectType.Shake: Shake(); break;
			case UIEffectType.PulseGlow: PulseGlow(); break;
			case UIEffectType.FlipReveal: FlipReveal(); break;
			case UIEffectType.SquashStretch: SquashStretch(); break;
			case UIEffectType.LeafFallLoop: LeafFallLoop(); break;
			case UIEffectType.HeartBeatLoop: HeartBeatLoop(); break;
			case UIEffectType.RotateLoop: RotateLoop(); break;
			case UIEffectType.RotateAndPumpLoop: RotateAndPumpLoop(); break;
			case UIEffectType.FadeInOutLoop: FadeInOutLoop(); break;
			case UIEffectType.PumpSequence: PumpSequence(); break;
			case UIEffectType.PopupElasticWithPump: PopupPumpSequence(); break;
			case UIEffectType.PopupElasticWithHeartBeat: PopupPumpHeart(); break;
			case UIEffectType.LeafWindEffect: LeafWindEffect(); break;
			case UIEffectType.LeafBranchSwayEffect: LeafBranchSwayEffect(); break;
			case UIEffectType.BouncePop: BouncePop(); break;
			case UIEffectType.SinWaveMove:
				SinWaveMove();
				break;
			case UIEffectType.BounceSquashStretch:
				BounceSquashStretch();
				break;
			case UIEffectType.PingPongRotation:
				PingPongRotation();
				break;
		}
	}

	#region Existing Effects
	public void PopupElastic()
	{
		target.localScale = Vector3.zero;
		target.DOScale(initialScale, duration).SetEase(easeType)
			  .OnComplete(() => EffectDone());
	}

	public void FallBounce()
	{
		float startY;

		// If we have an explicit RectTransform reference → use its anchoredPosition.y offset from initial
		if (fallStartRect != null)
		{
			startY = fallStartRect.anchoredPosition.y - initialPos.y;
		}
		else if (fallStartFromScreenTop)
		{
			// Calculate position outside top of screen dynamically
			Canvas canvas = target.GetComponentInParent<Canvas>();
			if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
			{
				RectTransform canvasRect = canvas.GetComponent<RectTransform>();
				startY = (canvasRect.rect.height / 2f) - initialPos.y + target.rect.height;
			}
			else
			{
				startY = fallStartYOffset; // Fallback value if canvas isn't found
			}
		}
		else
		{
			// Default fallback if no rect and not from top
			startY = fallStartYOffset;
		}

		// Optionally activate object before playing animation
		ActiveCurrentObject(true, false);

		// Place starting position
		target.anchoredPosition = new Vector3(initialPos.x, startY, initialPos.x);

		// Sequence: fall → bounce up → settle
		currentSequence = DOTween.Sequence();
		currentSequence.Append(target.DOAnchorPos(initialPos, fallDropTime).SetEase(Ease.OutQuad));
		currentSequence.Append(target.DOAnchorPos(initialPos + (Vector3)(Vector2.up * fallBounceOffset), fallBounceUpTime).SetEase(Ease.OutQuad));
		currentSequence.Append(target.DOAnchorPos(initialPos, fallBounceDownTime).SetEase(Ease.OutCubic))
						.OnComplete(() => EffectDone());
	}

	private void ActiveCurrentObject(bool isActive, bool isChangeAnchorPos = false)
	{
		gameObject.SetActive(isActive);
		if (isChangeAnchorPos) initialPos = target.anchoredPosition;
	}

	public void SlideInFromLeft()
	{
		target.anchoredPosition = initialPos + new Vector3(-800f, 0, 0);
		target.DOAnchorPos(initialPos, duration).SetEase(easeType)
			  .OnComplete(() => EffectDone());
	}

	public void Shake()
	{
		DOTween.Kill(target); // Stop any old shake

		if (shakeLoop)
		{
			// Loop indefinitely: shake → wait → shake again
			currentSequence = DOTween.Sequence();
			currentSequence.AppendCallback(() =>
			{
				target.DOShakeAnchorPos(
					shakeDuration,
					shakeStrength,
					shakeVibrato,
					shakeRandomness,
					shakeSnapping,
					shakeFadeOut
				);
			});
			currentSequence.AppendInterval(shakeLoopInterval);
			currentSequence.SetLoops(-1);
		}
		else
		{
			target.DOShakeAnchorPos(
				shakeDuration,
				shakeStrength,
				shakeVibrato,
				shakeRandomness,
				shakeSnapping,
				shakeFadeOut
			)
			.OnComplete(() => EffectDone());
		}
	}

	public void PulseGlow()
	{
		target.DOScale(initialScale * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
	}

	public void FlipReveal()
	{
		target.localScale = new Vector3(0, initialScale.y, initialScale.z);
		target.DOScaleX(initialScale.x, duration).SetEase(easeType)
			  .OnComplete(() => EffectDone());
	}

	public void SquashStretch()
	{
		currentSequence = DOTween.Sequence();
		currentSequence.Append(target.DOScale(initialScale + new Vector3(1.1f, 0.9f, 1f), 0.1f));
		currentSequence.Append(target.DOScale(initialScale, 0.1f))
			.OnComplete(() => EffectDone());
	}

	public void SinWaveMove()
	{
		if (sinWaveStartRect == null || sinWaveEndRect == null) return;

		DOTween.Kill(target);
		if (currentSequence != null && currentSequence.IsActive()) currentSequence.Kill();

		Vector2 start = sinWaveStartRect.anchoredPosition;
		Vector2 end = sinWaveEndRect.anchoredPosition;

		target.anchoredPosition = start;

		float t = 0f; // 0→1 progress along path

		Tween tTween = DOTween
			.To(() => t, x => t = x, 1f, sinWaveDuration)
			.SetEase(Ease.Linear)
			.SetLoops(sinWaveLoops, sinWavePingPong ? LoopType.Yoyo : LoopType.Restart)
			.OnUpdate(() =>
			{
				// Horizontal lerp
				Vector2 pos = Vector2.Lerp(start, end, t);

				// Vertical sine offset
				float wave = Mathf.Sin(t * Mathf.PI * 2f) * sinWaveAmplitude;

				pos.y += wave;
				target.anchoredPosition = pos;
			});

		currentTween = tTween;
	}


	#endregion

	#region New Effects

	// 1. Leaf Fall Loop
	public void LeafFallLoop()
	{
		if (leafFallEndRect == null) return;

		int loopCount = 0;

		// Save the self position if requested
		if (useSelfAsFirstStart)
			savedFirstStartPos = target.anchoredPosition;
		if (useSelfAsLoopStart)
			savedLoopStartPos = target.anchoredPosition;

		void PerformFall()
		{
			Vector2 fromPos = Vector2.zero;

			if (loopCount == 0)
			{
				// First loop
				if (useSelfAsFirstStart)
					fromPos = savedFirstStartPos;
				else if (leafFallFirstStartRect != null)
					fromPos = leafFallFirstStartRect.anchoredPosition;
				else if (!useSelfAsFirstStart && !leafFallFirstStartRect)
					fromPos = target.anchoredPosition; // fallback: self
			}
			else
			{
				// Subsequent loops
				if (useSelfAsLoopStart)
					fromPos = savedLoopStartPos;
				else if (leafFallLoopStartRect != null)
					fromPos = leafFallLoopStartRect.anchoredPosition;
				else if (!useSelfAsLoopStart && !leafFallLoopStartRect)
					fromPos = target.anchoredPosition; // fallback: self
			}

			target.anchoredPosition = fromPos;
			DOTween.Kill(target);

			currentSequence = DOTween.Sequence();
			currentSequence.Append(
				target.DOAnchorPos(leafFallEndRect.anchoredPosition, leafFallDuration)
					  .SetEase(Ease.InOutSine)
			);
			currentSequence.AppendCallback(() =>
			{
				loopCount++;
				PerformFall();
			});
		}

		PerformFall();
	}

	// 2. Heart Beat Loop
	public void HeartBeatLoop()
	{
		target.DOScale(initialScale * pumpScaleMultiplier, duration / 2f)
			  .SetLoops(-1, LoopType.Yoyo)
			  .SetEase(Ease.InOutSine);
	}

	// 3. Continuous Rotation
	public void RotateLoop()
	{
		target.DOLocalRotate(new Vector3(0, 0, 360), rotationSpeed, RotateMode.FastBeyond360)
			  .SetLoops(-1, LoopType.Restart)
			  .SetEase(Ease.Linear);
	}

	// 4. Rotate with Pump
	public void RotateAndPumpLoop()
	{
		currentSequence = DOTween.Sequence();
		currentSequence.Append(
			target.DOScale(initialScale * pumpScaleMultiplier, 0.5f).SetLoops(-1, LoopType.Yoyo)
		);
		target.DORotate(new Vector3(0, 0, 360), rotationSpeed, RotateMode.FastBeyond360)
			  .SetLoops(-1, LoopType.Restart)
			  .SetEase(Ease.Linear);
	}

	// 5. Fade In-Out Loop
	public void FadeInOutLoop()
	{
		if (!targetCanvasGroup) targetCanvasGroup = gameObject.AddComponent<CanvasGroup>();
		targetCanvasGroup.DOFade(maxAlpha, duration / 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
	}

	// 6. Pump Sequence (big → small → medium)
	public void PumpSequence()
	{
		currentSequence = DOTween.Sequence();
		currentSequence.Append(
			target.DOScale(initialScale * pumpLargeScaleMultiplier, pumpLargeScaleTime)
				   .SetEase(Ease.OutQuad)
		);
		currentSequence.Append(
			target.DOScale(initialScale * pumpSmallScaleMultiplier, pumpSmallScaleTime)
				   .SetEase(Ease.InOutQuad)
		);
		currentSequence.Append(
			target.DOScale(initialScale * pumpMediumScaleMultiplier, pumpMediumScaleTime)
				   .SetEase(Ease.OutQuad)
		)
		.OnComplete(() => EffectDone());
	}

	public void PopupPumpSequence()
	{
		target.localScale = Vector3.zero;
		target.DOScale(initialScale, duration).SetEase(easeType)
			  .OnComplete(() =>
			  {

				  currentSequence = DOTween.Sequence();
				  currentSequence.Append(
					  target.DOScale(initialScale * pumpLargeScaleMultiplier, pumpLargeScaleTime)
							 .SetEase(Ease.OutQuad)
				  );
				  currentSequence.Append(
					  target.DOScale(initialScale * pumpSmallScaleMultiplier, pumpSmallScaleTime)
							 .SetEase(Ease.InOutQuad)
				  );
				  currentSequence.Append(
					  target.DOScale(initialScale * pumpMediumScaleMultiplier, pumpMediumScaleTime)
							 .SetEase(Ease.OutQuad)
				  )
				  .OnComplete(() => EffectDone());
			  });
	}

	public void PopupPumpHeart()
	{
		target.localScale = Vector3.zero;
		target.DOScale(initialScale, duration).SetEase(easeType)
			  .OnComplete(() =>
			  {

				  currentSequence = DOTween.Sequence();
				  currentSequence.Append(
					  target.DOScale(initialScale * pumpLargeScaleMultiplier, pumpLargeScaleTime)
							 .SetEase(Ease.OutQuad)
				  );
				  currentSequence.Append(
					  target.DOScale(initialScale * pumpSmallScaleMultiplier, pumpSmallScaleTime)
							 .SetEase(Ease.InOutQuad)
				  );
				  currentSequence.Append(
					  target.DOScale(initialScale * pumpMediumScaleMultiplier, pumpMediumScaleTime)
							 .SetEase(Ease.OutQuad)
				  )
				  .OnComplete(() =>
				  {
					  EffectDone();
					  target.DOScale(initialScale * pumpScaleMultiplier, duration / 2f)
							.SetLoops(-1, LoopType.Yoyo)
							.SetEase(Ease.InOutSine);
				  });
			  });
	}

	public void LeafWindEffect()
	{
		DOTween.Kill(target);
		DOTween.Kill(target.gameObject); // stop rotation too

		// Main drifting loop: move to a random point in a circular area around the center
		void DriftLoop()
		{
			Vector2 center = target.anchoredPosition;
			Vector2 next =
				windDriftRandomize
				? center + UnityEngine.Random.insideUnitCircle * windDriftRadius
				: center + Vector2.right * windDriftRadius; // fixed if not randomized

			target.DOAnchorPos(next, windDriftDuration)
				  .SetEase(Ease.InOutSine)
				  .OnComplete(DriftLoop); // loop for endless wind drifting
		}

		DriftLoop();

		// Continuous flutter: rotate/swirl like wind turbulence
		target.DORotate(
			new Vector3(0, 0, windRotationStrength),
			windRotationDuration
		)
		.SetLoops(-1, LoopType.Yoyo)
		.SetEase(Ease.InOutSine);
	}

	public void LeafBranchSwayEffect()
	{
		if (target == null) return;

		// Optionally start at a given local position or reference point (e.g. a "branch" empty GameObject)
		// Here we just use target's current position:
		Vector2 baseAnchor = initialPos + (Vector3)(new Vector2(0, swayYOffset));

		target.anchoredPosition = baseAnchor;

		DOTween.Kill(target);

		// Create a gentle side-to-side (X axis) movement, simulating leaf flexing from the petiole
		if (swayRandomize)
		{
			// Randomly start on left or right side for each play
			float dir = UnityEngine.Random.value < 0.5f ? -1f : 1f;
			target.DOAnchorPosX(baseAnchor.x + dir * swayDistance, swayDuration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
		}
		else
		{
			// Standard left/right sway
			target.DOAnchorPosX(baseAnchor.x + swayDistance, swayDuration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
		}
		// No rotation tween! Leaf always keeps the same angle, so it looks tethered to its branch.
	}

	public void BouncePop()
	{
		if (target == null) return;

		DOTween.Kill(target);
		target.localScale = initialScale;
		target.anchoredPosition = initialPos;

		Sequence bounceSeq = DOTween.Sequence();

		bounceSeq.Append(
			target.DOAnchorPos((Vector2)initialPos + Vector2.up * bounceMoveOffset, bounceMoveTime).SetEase(Ease.OutQuad)
		);
		bounceSeq.Join(
			target.DOScale(initialScale * bounceUpscaleFactor, bounceUpscaleTime).SetEase(Ease.OutQuad)
		);

		bounceSeq.Append(
			target.DOAnchorPos((Vector2)initialPos, bounceDownscaleTime).SetEase(Ease.InQuad)
		);
		bounceSeq.Join(
			target.DOScale(initialScale * bounceDownscaleFactor, bounceDownscaleTime).SetEase(Ease.InQuad)
		);

		bounceSeq.Append(
			target.DOScale(initialScale, bounceRecoverTime).SetEase(Ease.OutQuad)
		);

		if (bounceLoop)
			bounceSeq.SetLoops(-1); // Infinite repeat
		else
			bounceSeq.OnComplete(() => EffectDone());

		currentSequence = bounceSeq;
	}

	public void BounceSquashStretch()
	{
		DOTween.Kill(target);
		target.localScale = initialScale;
		target.anchoredPosition = initialPos;

		Sequence seq = DOTween.Sequence();

		// --- PHASE 1: Anticipation Squash ---
		seq.Append(target.DOScale(bagSquashScale, 0.1f)
				   .SetEase(Ease.OutQuad));

		// --- PHASE 2: Stretch + Jump Up ---
		seq.Append(target.DOScale(bagStretchScale, bagBounceDuration * 0.4f)
				   .SetEase(bagLaunchEase));
		seq.Join(target.DOAnchorPosY(initialPos.y + bagBounceHeight, bagBounceDuration * 0.4f)
				 .SetEase(bagLaunchEase));
		seq.JoinCallback(() => onTopCoinsMoveUp?.Invoke());

		// --- PHASE 3: Apex Pause ---
		seq.AppendInterval(apexPause);

		// --- PHASE 4: Fall Down + Land Squash ---
		seq.Append(target.DOAnchorPosY(initialPos.y, bagBounceDuration * 0.6f)
				   .SetEase(bagFallEase));
		seq.Join(target.DOScale(bagSquashScale, bagBounceDuration * 0.6f)
				 .SetEase(bagFallEase));

		// --- PHASE 5: Impact ---
		seq.AppendCallback(() => onScatterBottomCoins?.Invoke());

		// --- PHASE 6: Bag Settle ---
		seq.Append(target.DOScale(initialScale, bagSettleDuration)
				   .SetEase(bagSettleEase));
		seq.JoinCallback(() => onTopCoinsMoveDown?.Invoke());

		// --- Loop Setup ---
		if (loopEffect)
		{
			seq.AppendInterval(pauseBetweenLoops);
			seq.SetLoops(loopCount); // -1 = infinite, or specific count
		}
		else
		{
			seq.OnComplete(() => EffectDone());
		}

		currentSequence = seq;
	}
	public void PingPongRotation()
	{
		DOTween.Kill(target);
		target.localScale = initialScale;
		target.localEulerAngles = Vector3.zero;

		float t = 0f; // 0→1 animation time

		currentTween = DOTween.To(
			() => t,
			x =>
			{
				t = x;

				// SMOOTH ROTATION: sine wave between left/right
				float rotRange = (rotationRightAngle - rotationLeftAngle) * 0.5f;
				float rotCenter = (rotationLeftAngle + rotationRightAngle) * 0.5f;
				float rotation = Mathf.Sin(t * Mathf.PI * 2f) * rotRange + rotCenter;
				target.localEulerAngles = new Vector3(0, 0, rotation);

				// SMOOTH PUNCH: secondary sine wave (offset phase for punch timing)
				float punchPhase = t * Mathf.PI * 2f + Mathf.PI * 0.5f; // 90° offset
				float punchValue = Mathf.Sin(punchPhase) * punchIntensity;
				float currentScale = 1f + punchValue;
				currentScale = Mathf.Clamp(currentScale, punchOutScale, punchInScale);

				target.localScale = initialScale * currentScale;
			},
			1f,
			rotationDuration
		)
		.SetEase(Ease.Linear) // Linear time for perfect sine waves
		.SetLoops(-1);

		if (!pingPongLoop)
			currentTween.OnComplete(() => EffectDone());
	}

	#endregion

	private void EffectDone()
	{
		OnEffectComplete?.Invoke();
		PlayAnotherEffectCallback?.Invoke();
	}

	//private void OnDestroy()
	//{
	//	DOTween.KillAll();
	//}
}
