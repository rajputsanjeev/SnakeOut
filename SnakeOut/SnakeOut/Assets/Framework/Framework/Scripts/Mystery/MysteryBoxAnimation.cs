using DG.Tweening;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace ManuGames
{
	public class MysteryBoxAnimation : GenericSingletonClass<MysteryBoxAnimation>
	{
		public Button SpinButton;
		public Sprite OpenSprite;
		public Sprite CloseSprite;
		public Image BoxImage;
		public Text CoinText;
		public RectTransform CoinRect;

		[Header("Refs (UI RectTransforms)")]
		public RectTransform box;      // chest image
		public RectTransform circle1;  // inner pulse
		public RectTransform circle2;  // expanding ring (needs CanvasGroup)
		public RectTransform circle3;  // rotating rays

		[Header("Box Bob Settings")]
		public float upDistance = 60f;
		public float downDistance = 60f;
		public float upTime = 0.35f;
		public float downTime = 0.45f;
		public float holdTop = 0.08f;
		public float holdBottom = 0.15f;
		public float scaleBig = 1.10f;
		public float scaleSmall = 0.95f;

		[Header("Circles")]
		public float circle1PulseTime = 0.6f;
		public float circle2ExpandTime = 1.2f;
		public float circle3RotateTime = 3f;

		[Header("Complete")]
		public float TotalDuration = 3f;

		Sequence boxLoop, timerSeq;
		Tween c1Loop, c2Loop, c2Fade, c3Loop;  // 👈 changed from Sequence to Tween
		private Vector2 StartAnchoredPosition;

		private void Start()
		{
			PlayAnimation();
		}

		public void PlayAnimation()
		{
			SpinButton.gameObject.SetActive(true);
			Vector2 start = GetStartPosition();
			box.localScale = Vector3.one;
			CircelAnimation();
			BoxAnimation(start);
		}

		private Vector2 GetStartPosition()
		{
			BoxImage.sprite = CloseSprite;
			KillAll();
			var start = box.anchoredPosition;
			StartAnchoredPosition = start;
			return start;
		}

		void OnDisable() { KillAll(); }

		public void Play(System.Action onComplete = null, int reward = 0, float totalDuration = 3)
		{
			Vector2 start = GetStartPosition();
			box.localScale = Vector3.one;

			// Reset
			CircelAnimation();

			// ===== Box move + scale in sequence =====
			BoxAnimation(start);

			// ===== Timer for OnComplete =====
			timerSeq = DOTween.Sequence().AppendInterval(totalDuration).OnComplete(() =>
			{
				onComplete?.Invoke();
				MysteryBoxTextAnim.Instance.ShowRewardText(box.anchoredPosition, box.transform, reward.ToString(), () =>
				{
				});
				boxLoop?.Kill();  // ✅ stop only box animation
				timerSeq?.Kill();
				box.anchoredPosition = StartAnchoredPosition;
				box.localScale = Vector3.one;
				BoxImage.sprite = OpenSprite;
			}).SetLink(gameObject);
			SpinButton.gameObject.SetActive(false);
		}

		public void CircelAnimation()
		{
			if (circle1) circle1.localScale = Vector3.one;
			if (circle2)
			{
				circle2.localScale = Vector3.zero;
				var cg = circle2.GetComponent<CanvasGroup>();
				if (!cg) cg = circle2.gameObject.AddComponent<CanvasGroup>();
				cg.alpha = 1f;
			}
			if (circle3) circle3.localRotation = Quaternion.identity;

			// ===== Circle 1 pump =====
			if (circle1)
			{
				c1Loop = circle1.DOScale(1.2f, circle1PulseTime)
								 .SetEase(Ease.InOutSine)
								 .SetLoops(-1, LoopType.Yoyo)
								 .SetLink(gameObject);
			}

			// ===== Circle 2 expand & fade =====
			if (circle2)
			{
				c2Loop = circle2.DOScale(5f, circle2ExpandTime)
								 .SetEase(Ease.OutCubic)
								 .SetLoops(-1, LoopType.Restart)
								 .SetLink(gameObject);

				var cg = circle2.GetComponent<CanvasGroup>();
				c2Fade = cg.DOFade(0f, circle2ExpandTime)
						   .From(1f)
						   .SetLoops(-1, LoopType.Restart)
						   .SetLink(gameObject);
			}

			// ===== Circle 3 rotation =====
			if (circle3)
			{
				c3Loop = circle3.DOLocalRotate(new Vector3(0, 0, 360), circle3RotateTime, RotateMode.FastBeyond360)
								.SetEase(Ease.Linear)
								.SetLoops(-1, LoopType.Restart)
								.SetLink(gameObject);
			}
		}

		public void BoxAnimation(Vector2 start)
		{
			boxLoop = DOTween.Sequence().SetLink(gameObject);

			boxLoop.Append(box.DOAnchorPosY(start.y + upDistance, upTime).SetEase(Ease.OutSine));
			boxLoop.Join(box.DOScaleX(scaleSmall, upTime).SetEase(Ease.OutSine));
			boxLoop.AppendInterval(holdTop);

			//boxLoop.Append(box.DOAnchorPosY(start.y - downDistance, downTime).SetEase(Ease.InOutSine));
			//boxLoop.Join(box.DOScaleX(scaleSmall, downTime).SetEase(Ease.InOutSine));
			//boxLoop.AppendInterval(holdBottom);

			boxLoop.Append(box.DOAnchorPosY(start.y, upTime * 0.8f).SetEase(Ease.OutSine));
			boxLoop.Join(box.DOScaleX(1f, upTime * 0.8f).SetEase(Ease.OutSine));

			boxLoop.SetLoops(-1, LoopType.Restart);
		}

		public void OnCoinFlowComplete()
		{
			gameObject.SetActive(false);
		}

		void KillAll()
		{
			boxLoop?.Kill();
			timerSeq?.Kill();
			c1Loop?.Kill();
			c2Loop?.Kill();
			c2Fade?.Kill();
			c3Loop?.Kill();
		}
	}
}