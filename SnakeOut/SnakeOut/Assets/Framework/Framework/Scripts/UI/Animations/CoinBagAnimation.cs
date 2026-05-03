using UnityEngine;
using DG.Tweening;

public class CoinBagUIAnimation : MonoBehaviour
{
	[Header("── Bag Icon ──────────────────────")]
	public RectTransform bagIcon;
	public float bagBounceHeight = 80f;
	public float bagBounceDuration = 0.35f;
	public Vector3 bagSquashScale = new Vector3(1.25f, 0.75f, 1f);
	public Vector3 bagStretchScale = new Vector3(0.85f, 1.2f, 1f);
	public Ease bagLaunchEase = Ease.OutQuad;
	public Ease bagFallEase = Ease.InQuad;
	public Ease bagSettleEase = Ease.OutElastic;
	public float bagSettleDuration = 0.18f;

	[Header("── Top Coins ─────────────────────")]
	public RectTransform[] topCoins;
	public float topCoinJumpHeight = 18f;
	public float topCoinDuration = 0.2f;
	public float topCoinStaggerDelay = 0.03f;
	public Ease topCoinUpEase = Ease.OutQuad;
	public Ease topCoinDownEase = Ease.InQuad;

	[Header("── Bottom Coins ──────────────────")]
	public RectTransform[] bottomCoins;
	public float bottomCoinJumpHeight = 50f;
	public float bottomCoinDuration = 0.35f;
	public float bottomCoinStaggerDelay = 0.03f;
	public Ease bottomCoinUpEase = Ease.OutQuad;
	public Ease bottomCoinDownEase = Ease.InBounce;

	[Header("── Bottom Coin Rotation ──────────")]
	public float bottomCoinMaxRotation = 25f;
	public Ease bottomCoinRotateEase = Ease.OutQuad;
	public Ease bottomCoinRotateReturnEase = Ease.OutBack;

	[Header("── Loop ──────────────────────────")]
	public float pauseBetweenLoops = 1.5f;
	public float apexPause = 0.08f;

	private Vector2 bagOriginAnchorPos;
	private Vector2[] bottomCoinOrigins;
	private Vector2[] topCoinOrigins;

	void Start()
	{
		bagOriginAnchorPos = bagIcon.anchoredPosition;

		topCoinOrigins = new Vector2[topCoins.Length];
		for (int i = 0; i < topCoins.Length; i++)
			topCoinOrigins[i] = topCoins[i].anchoredPosition;

		bottomCoinOrigins = new Vector2[bottomCoins.Length];
		for (int i = 0; i < bottomCoins.Length; i++)
			bottomCoinOrigins[i] = bottomCoins[i].anchoredPosition;

		PlayLoop();
	}

	void PlayLoop()
	{
		Sequence seq = DOTween.Sequence();

		// --- PHASE 1: Anticipation Squash ---
		seq.Append(bagIcon.DOScale(bagSquashScale, 0.1f)
			.SetEase(Ease.OutQuad));

		// --- PHASE 2: Stretch + Jump Up ---
		seq.Append(bagIcon.DOScale(bagStretchScale, bagBounceDuration * 0.4f)
			.SetEase(bagLaunchEase));

		seq.Join(bagIcon.DOAnchorPosY(bagOriginAnchorPos.y + bagBounceHeight, bagBounceDuration * 0.4f)
			.SetEase(bagLaunchEase));

		seq.Join(TopCoinsMove(up: true));

		// --- PHASE 3: Apex Pause ---
		seq.AppendInterval(apexPause);

		// --- PHASE 4: Fall Down + Land Squash ---
		seq.Append(bagIcon.DOAnchorPosY(bagOriginAnchorPos.y, bagBounceDuration * 0.6f)
			.SetEase(bagFallEase));

		seq.Join(bagIcon.DOScale(bagSquashScale, bagBounceDuration * 0.6f)
			.SetEase(bagFallEase));

		// --- PHASE 5: Impact ---
		seq.AppendCallback(ScatterBottomCoins);

		// --- PHASE 6: Bag Settle ---
		seq.Append(bagIcon.DOScale(Vector3.one, bagSettleDuration)
			.SetEase(bagSettleEase));

		seq.Join(TopCoinsMove(up: false));

		// --- Loop Pause ---
		seq.AppendInterval(pauseBetweenLoops);
		seq.AppendCallback(PlayLoop);
	}

	// ── Top Coins ─────────────────────────────────────────────────
	Tween TopCoinsMove(bool up)
	{
		Sequence s = DOTween.Sequence();

		for (int i = 0; i < topCoins.Length; i++)
		{
			RectTransform coin = topCoins[i];
			Vector2 origin = topCoinOrigins[i];

			float targetY = up ? origin.y + topCoinJumpHeight : origin.y;

			s.Join(
				coin.DOAnchorPosY(targetY, topCoinDuration)
					.SetDelay(i * topCoinStaggerDelay)
					.SetEase(up ? topCoinUpEase : topCoinDownEase)
			);
		}

		return s;
	}

	// ── Bottom Coins ──────────────────────────────────────────────
	void ScatterBottomCoins()
	{
		for (int i = 0; i < bottomCoins.Length; i++)
		{
			RectTransform coin = bottomCoins[i];
			Vector2 origin = bottomCoinOrigins[i];

			Sequence cs = DOTween.Sequence();

			// Stagger each coin
			cs.AppendInterval(i * bottomCoinStaggerDelay);

			// Jump straight up
			cs.Append(coin.DOAnchorPosY(origin.y + bottomCoinJumpHeight, bottomCoinDuration * 0.45f)
				.SetEase(bottomCoinUpEase));

			// Spin while going up
			cs.Join(coin.DORotate(new Vector3(0, 0, Random.Range(-bottomCoinMaxRotation, bottomCoinMaxRotation)), bottomCoinDuration * 0.45f)
				.SetEase(bottomCoinRotateEase));

			// Rotate back
			cs.Append(coin.DORotate(Vector3.zero, bottomCoinDuration * 0.3f)
				.SetEase(bottomCoinRotateReturnEase));

			// Fall back down
			cs.Join(coin.DOAnchorPosY(origin.y, bottomCoinDuration * 0.55f)
				.SetEase(bottomCoinDownEase));
		}
	}

	public void StopAnimation()
	{
		DOTween.Kill(bagIcon);
		foreach (var c in topCoins) DOTween.Kill(c);
		foreach (var c in bottomCoins) DOTween.Kill(c);
	}
}