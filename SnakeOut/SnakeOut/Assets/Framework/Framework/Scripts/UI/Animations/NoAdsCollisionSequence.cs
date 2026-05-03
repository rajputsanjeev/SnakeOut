using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Animates a TV falling from the top, then a "No Ads" icon colliding with it.
/// Attach this to a manager GameObject in your scene.
/// </summary>
public class NoAdsCollisionSequence : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private RectTransform tvTransform;
	[SerializeField] private Image tvImage;
	[SerializeField] private Sprite tvNormalSprite;
	[SerializeField] private Sprite tvBrokenSprite;

	[SerializeField] private RectTransform noAdsIconTransform;
	[SerializeField] private Image noAdsIconImage;

	[Header("Impact Effects")]
	[SerializeField] private GameObject impactParticlesPrefab;
	[SerializeField] private RectTransform impactFlashImage;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip tvLandSoundClip;       // ← NEW: thud on landing
	[SerializeField] private AudioClip impactSoundClip;
	[SerializeField] private AudioClip tvBreakSoundClip;

	[Header("TV Drop Settings")]
	[SerializeField] private float tvDropStartY = 1200f;   // How far above screen TV starts
	[SerializeField] private float tvDropDuration = 0.55f;   // Time to fall to ground
	[SerializeField] private float tvBounceHeight = 60f;     // How high it bounces on land
	[SerializeField] private float tvBounceDuration = 0.22f;  // Duration of each bounce arc

	[Header("Offscreen Entry Position")]
	[SerializeField] private Vector2 iconOffscreenPosition = new Vector2(900f, 300f);

	[Header("Timings")]
	[SerializeField] private float tvAppearFadeDuration = 0.15f;  // Fade in as TV drops
	[SerializeField] private float iconEnterDuration = 0.70f;
	[SerializeField] private float iconMoveToDuration = 0.45f;
	[SerializeField] private float postImpactDelay = 0.15f;
	[SerializeField] private float tvBreakDuration = 0.30f;
	[SerializeField] private float exitDelay = 1.20f;

	// ─── Private State ──────────────────────────────────────────────────────

	private Vector2 _iconRestPosition = new Vector2(300f, 200f);
	private Vector2 _tvRestPosition;       // The final resting Y after drop
	private Vector2 _tvDropStartPosition;  // Where the TV spawns (top of screen)
	private Sequence _mainSequence;

	// ───────────────────────────────────────────────────────────────────────

	public void OnStart()
	{
		InitializePositions();
		PlaySequence();
	}

	private void InitializePositions()
	{
		// Store where the TV should rest (set this in the Inspector via RectTransform)
		_tvRestPosition = tvTransform.anchoredPosition;

		// Calculate the off-screen top start position
		_tvDropStartPosition = new Vector2(_tvRestPosition.x, tvDropStartY);

		// --- TV starts at the TOP, invisible ---
		tvTransform.anchoredPosition = _tvDropStartPosition;
		tvTransform.localScale = Vector3.one;           // Full scale — it's falling, not popping
		tvImage.color = new Color(1, 1, 1, 0); // Invisible until it starts falling
		tvImage.sprite = tvNormalSprite;

		// Icon starts offscreen to the side
		noAdsIconTransform.anchoredPosition = iconOffscreenPosition;
		noAdsIconTransform.localScale = Vector3.one;
		noAdsIconImage.color = Color.white;

		// Flash starts hidden
		if (impactFlashImage != null)
		{
			impactFlashImage.localScale = Vector3.zero;
			impactFlashImage.GetComponent<Image>().color = new Color(1f, 0.9f, 0.2f, 0f);
		}
	}

	// ─────────────────────────────────────────────────────────────────────
	// MAIN SEQUENCE
	// ─────────────────────────────────────────────────────────────────────

	public void PlaySequence()
	{
		_mainSequence?.Kill();
		_mainSequence = DOTween.Sequence();

		// ════════════════════════════════════════════════════════════════
		// PHASE 0 — TV FALLS FROM TOP
		// ════════════════════════════════════════════════════════════════

		// Fade the TV in quickly as it starts dropping
		_mainSequence
			.Append(tvImage
				.DOFade(1f, tvAppearFadeDuration)
				.SetEase(Ease.OutQuad))

			// Drop with gravity acceleration (InQuart = slow start, fast finish)
			.Append(tvTransform
				.DOAnchorPosY(_tvRestPosition.y, tvDropDuration)
				.SetEase(Ease.InQuart))

			// ── Land callback: thud sound + screen shake ────────────────────
			.AppendCallback(OnTvLand)

			// ── Bounce #1: quick hop up ─────────────────────────────────────
			.Append(tvTransform
				.DOAnchorPosY(_tvRestPosition.y + tvBounceHeight, tvBounceDuration)
				.SetEase(Ease.OutQuad))

			// Squash on the way up
			.Join(tvTransform
				.DOScaleX(0.92f, tvBounceDuration * 0.5f)
				.SetEase(Ease.OutSine)
				.SetLoops(2, LoopType.Yoyo))
			.Join(tvTransform
				.DOScaleY(1.08f, tvBounceDuration * 0.5f)
				.SetEase(Ease.OutSine)
				.SetLoops(2, LoopType.Yoyo))

			// ── Bounce #1 fall back down ─────────────────────────────────────
			.Append(tvTransform
				.DOAnchorPosY(_tvRestPosition.y, tvBounceDuration)
				.SetEase(Ease.InQuad))
			.AppendCallback(OnTvBounce)      // small dust puff / sound on 2nd land

			// ── Bounce #2: smaller hop ───────────────────────────────────────
			.Append(tvTransform
				.DOAnchorPosY(_tvRestPosition.y + tvBounceHeight * 0.35f, tvBounceDuration * 0.7f)
				.SetEase(Ease.OutQuad))
			.Append(tvTransform
				.DOAnchorPosY(_tvRestPosition.y, tvBounceDuration * 0.7f)
				.SetEase(Ease.InQuad))

			// ── Settle squash-and-stretch ────────────────────────────────────
			.Append(tvTransform
				.DOScaleX(1.06f, 0.08f).SetEase(Ease.OutSine))
			.Join(tvTransform
				.DOScaleY(0.94f, 0.08f).SetEase(Ease.OutSine))
			.Append(tvTransform
				.DOScale(Vector3.one, 0.18f).SetEase(Ease.OutElastic))

			// Brief pause — TV has landed, breathe ───────────────────────────
			.AppendInterval(0.3f)

			// ════════════════════════════════════════════════════════════════
			// PHASE 1 — ICON ENTERS FROM SIDE
			// ════════════════════════════════════════════════════════════════

			.Append(noAdsIconTransform
				.DOAnchorPos(_iconRestPosition, iconEnterDuration)
				.SetEase(Ease.OutCubic))

			.Join(noAdsIconTransform
				.DOScale(new Vector3(1.05f, 0.95f, 1f), iconEnterDuration * 0.5f)
				.SetEase(Ease.OutSine)
				.SetLoops(2, LoopType.Yoyo))

			// ════════════════════════════════════════════════════════════════
			// PHASE 2 — ANTICIPATION BEAT
			// ════════════════════════════════════════════════════════════════

			.AppendInterval(0.25f)

			.Append(noAdsIconTransform
				.DOScale(new Vector3(0.85f, 1.15f, 1f), 0.12f)
				.SetEase(Ease.InSine))

			// ════════════════════════════════════════════════════════════════
			// PHASE 3 — ICON CHARGES AT TV
			// ════════════════════════════════════════════════════════════════

			.Append(noAdsIconTransform
				.DOAnchorPos(_tvRestPosition, iconMoveToDuration)
				.SetEase(Ease.InExpo))

			// ════════════════════════════════════════════════════════════════
			// PHASE 4 — IMPACT
			// ════════════════════════════════════════════════════════════════

			.AppendCallback(OnImpact)
			.AppendInterval(postImpactDelay)

			// ════════════════════════════════════════════════════════════════
			// PHASE 5 — TV BREAKS
			// ════════════════════════════════════════════════════════════════

			.AppendCallback(SwapToBrokenSprite)

			.Append(tvTransform
				.DOShakePosition(tvBreakDuration, strength: 18f, vibrato: 20, randomness: 45))

			.Join(tvTransform
				.DOPunchScale(new Vector3(0.2f, -0.15f, 0f), tvBreakDuration, vibrato: 5)
				.SetEase(Ease.OutElastic))

			// ════════════════════════════════════════════════════════════════
			// PHASE 6 — ICON BOUNCES BACK & EXITS
			// ════════════════════════════════════════════════════════════════

			//.Join(noAdsIconTransform
			//	.DOAnchorPos(_iconRestPosition + new Vector2(-80f, 40f), 0.25f)
			//	.SetEase(Ease.OutBack))

			//.Join(noAdsIconTransform
			//	.DOScale(new Vector3(1.3f, 0.7f, 1f), 0.12f)
			//	.SetEase(Ease.OutSine)
			//	.SetLoops(2, LoopType.Yoyo))

			//.AppendInterval(exitDelay)

			//.Append(noAdsIconTransform
			//	.DOAnchorPos(iconOffscreenPosition, 0.4f)
			//	.SetEase(Ease.InBack))

			.Join(noAdsIconImage
				.DOFade(0f, 0.1f).SetEase(Ease.InQuad))

			.OnComplete(OnSequenceComplete);
	}

	// ─────────────────────────────────────────────────────────────────────
	// CALLBACKS
	// ─────────────────────────────────────────────────────────────────────

	/// <summary>Called the moment the TV hits the ground for the first time.</summary>
	private void OnTvLand()
	{
		// Heavy thud screenshake
		Camera.main?.DOShakePosition(0.3f,
			strength: new Vector3(0.35f, 0.5f, 0f),
			vibrato: 18, randomness: 45, fadeOut: true);

		// Thud sound
		if (audioSource != null && tvLandSoundClip != null)
			audioSource.PlayOneShot(tvLandSoundClip, 1f);

		// Spawn dust/landing particles at TV base
		if (impactParticlesPrefab != null)
		{
			Vector3 spawnPos = tvTransform.position + new Vector3(0f, -tvTransform.rect.height * 0.5f, 0f);
			var dust = Instantiate(impactParticlesPrefab, spawnPos, Quaternion.identity);
			Destroy(dust, 2f);
		}
	}

	/// <summary>Smaller reaction on the secondary bounce landing.</summary>
	private void OnTvBounce()
	{
		Camera.main?.DOShakePosition(0.12f,
			strength: new Vector3(0.1f, 0.2f, 0f),
			vibrato: 10, randomness: 30, fadeOut: true);

		// Optional: play a quieter version of the thud at lower volume
		if (audioSource != null && tvLandSoundClip != null)
			audioSource.PlayOneShot(tvLandSoundClip, 0.45f);
	}

	private void OnImpact()
	{
		Camera.main?.DOShakePosition(0.3f,
			strength: new Vector3(0.25f, 0.25f, 0f),
			vibrato: 15, randomness: 90, fadeOut: true);

		if (impactFlashImage != null)
		{
			var flashImg = impactFlashImage.GetComponent<Image>();
			DOTween.Sequence()
				.Append(impactFlashImage.DOScale(1.5f, 0.08f).SetEase(Ease.OutExpo))
				.Join(flashImg.DOFade(0.85f, 0.08f))
				.Append(impactFlashImage.DOScale(0f, 0.25f).SetEase(Ease.InCubic))
				.Join(flashImg.DOFade(0f, 0.25f));
		}

		if (impactParticlesPrefab != null)
		{
			var vfx = Instantiate(impactParticlesPrefab, tvTransform.position, Quaternion.identity);
			Destroy(vfx, 3f);
		}

		if (audioSource != null && impactSoundClip != null)
			audioSource.PlayOneShot(impactSoundClip, 1f);
	}

	private void SwapToBrokenSprite()
	{
		tvImage.sprite = tvBrokenSprite;

		if (audioSource != null && tvBreakSoundClip != null)
			audioSource.PlayOneShot(tvBreakSoundClip, 0.9f);

		tvImage.DOColor(Color.white, 0.05f).SetLoops(2, LoopType.Yoyo);
		//tvImage.DOColor(new Color(0.78f, 0.78f, 0.85f, 1f), 0.4f)
		//	   .SetDelay(0.1f).SetEase(Ease.OutSine);
	}

	private void OnSequenceComplete()
	{
		Debug.Log("Sequence complete!");
	}

	private void OnDestroy()
	{
		_mainSequence?.Kill();
	}
}