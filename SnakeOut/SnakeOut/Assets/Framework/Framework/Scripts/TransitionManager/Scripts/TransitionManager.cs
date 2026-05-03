namespace MaskTransitions
{
	using DG.Tweening;
	using System;
	using System.Collections;
	using Unity.VectorGraphics;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;

	public enum TransitionType
	{
		Scale,
		Radial360,
		LeftToRight,
		RightToLeft,
		TopToBottom,
		BottomToTop,
		TvHorizontal,
		TvVertical
	}

	public class TransitionManager : MonoBehaviour
	{
		public static event Action<string> BeforeLoad;
		public static event Action<string> AfterLoad;

		public static TransitionManager Instance;

		private float screenWidth;
		private float screenHeight;
		[HideInInspector] public static float maxSize;
		private float individualTransitionTime;

		[Header("Transition Properties")]
		public TransitionType transitionInType = TransitionType.Scale;
		public TransitionType transitionOutType = TransitionType.Scale;

		public Sprite transitionImage;
		public Color transitionColor;
		public Color transitionEndColor;
		public bool rotation;
		[Tooltip("Time taken for one half of the transition to complete")]
		public float transitionTime;

		[Header("Image Components")]
		[SerializeField] private RectTransform parentMaskRect;
		[SerializeField] private RectTransform maskRect;
		[SerializeField] private RectTransform transitionCanvas;
		[SerializeField] private Image parentMaskImage;
		[SerializeField] private CutoutMaskUI cutoutMask;
		[SerializeField] private GameObject canvas;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			// Assign the transition sprite and color
			parentMaskImage.sprite = transitionImage;
			cutoutMask.sprite = transitionImage;
			cutoutMask.color = transitionColor;

			individualTransitionTime = transitionTime / 2;

			SetupMaxSize();
		}

		#region Setup
		void SetupMaxSize()
		{
			screenWidth = transitionCanvas.rect.width;
			screenHeight = transitionCanvas.rect.height;

			maxSize = Mathf.Max(screenWidth, screenHeight);
			maxSize += maxSize / 4;
		}

		private void ResetMaskStateForTransition()
		{
			// Reset common properties to defaults before starting fresh tween sequences
			maskRect.DOKill();
			parentMaskRect.DOKill();

			maskRect.anchoredPosition = Vector2.zero;
			parentMaskRect.anchoredPosition = Vector2.zero;

			maskRect.localScale = Vector3.one;
			parentMaskRect.localScale = Vector3.one;

			maskRect.localRotation = Quaternion.identity;
			parentMaskRect.localRotation = Quaternion.identity;

			cutoutMask.type = Image.Type.Simple;
			cutoutMask.fillAmount = 1f;
		}

		void StartAnimation(float? totalTime = null)
		{
			float animationTime = totalTime ?? individualTransitionTime;
			ResetMaskStateForTransition();
			cutoutMask.color = transitionColor;

			Sequence seq = CreateInAnimationSequence(animationTime);
			seq.Play();
		}

		Tween StartAnimationForLoad(float? totalTime = null)
		{
			float animationTime = totalTime ?? individualTransitionTime;
			ResetMaskStateForTransition();
			cutoutMask.color = transitionColor;

			Sequence seq = CreateInAnimationSequence(animationTime);
			return seq.Play();
		}

		void EndAnimation(float? totalTime = null)
		{
			float animationTime = totalTime ?? individualTransitionTime;
			ResetMaskStateForTransition();
			cutoutMask.color = transitionEndColor;

			Sequence seq = CreateOutAnimationSequence(animationTime);
			seq.Play().OnComplete(() =>
			{
				canvas.SetActive(false);
				AfterLoad?.Invoke("");
			});
		}

		private Sequence CreateInAnimationSequence(float animationTime)
		{
			Sequence animationSequence = DOTween.Sequence();

			switch (transitionInType)
			{
				case TransitionType.Scale:
					maskRect.sizeDelta = Vector2.zero;
					parentMaskRect.sizeDelta = Vector2.zero;

					animationSequence.Join(maskRect.DOSizeDelta(new Vector2(maxSize, maxSize), animationTime).SetEase(Ease.InOutQuad));
					if (rotation)
						animationSequence.Join(maskRect.DORotate(new Vector3(0, 0, 180), animationTime, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.Radial360:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					cutoutMask.type = Image.Type.Filled;
					cutoutMask.fillMethod = Image.FillMethod.Radial360;
					cutoutMask.fillOrigin = (int)Image.Origin360.Top;
					cutoutMask.fillClockwise = true;
					cutoutMask.fillAmount = 0f;

					animationSequence.Join(cutoutMask.DOFillAmount(1f, animationTime).SetEase(Ease.InOutQuad));
					if (rotation)
						animationSequence.Join(maskRect.DORotate(new Vector3(0, 0, 180), animationTime, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.LeftToRight:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = new Vector2(-maxSize, 0);
					animationSequence.Join(maskRect.DOAnchorPosX(0, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.RightToLeft:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = new Vector2(maxSize, 0);
					animationSequence.Join(maskRect.DOAnchorPosX(0, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.TopToBottom:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = new Vector2(0, maxSize);
					animationSequence.Join(maskRect.DOAnchorPosY(0, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.BottomToTop:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = new Vector2(0, -maxSize);
					animationSequence.Join(maskRect.DOAnchorPosY(0, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.TvHorizontal:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.localScale = new Vector3(0, 1, 1);
					animationSequence.Join(maskRect.DOScaleX(1f, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.TvVertical:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.localScale = new Vector3(1, 0, 1);
					animationSequence.Join(maskRect.DOScaleY(1f, animationTime).SetEase(Ease.InOutQuad));
					break;
			}

			return animationSequence;
		}

		private Sequence CreateOutAnimationSequence(float animationTime)
		{
			Sequence animationSequence = DOTween.Sequence();

			switch (transitionOutType)
			{
				case TransitionType.Scale:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;
					parentMaskRect.rotation = Quaternion.identity;

					animationSequence.Join(parentMaskRect.DOSizeDelta(new Vector2(maxSize, maxSize), animationTime).SetEase(Ease.InOutQuad));
					if (rotation)
						animationSequence.Join(parentMaskRect.DORotate(new Vector3(0, 0, 180), animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.Radial360:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					cutoutMask.type = Image.Type.Filled;
					cutoutMask.fillMethod = Image.FillMethod.Radial360;
					cutoutMask.fillOrigin = (int)Image.Origin360.Top;
					cutoutMask.fillClockwise = false; // Empties in the same clockwise direction
					cutoutMask.fillAmount = 1f;

					animationSequence.Join(cutoutMask.DOFillAmount(0f, animationTime).SetEase(Ease.InOutQuad));
					if (rotation)
						animationSequence.Join(maskRect.DORotate(new Vector3(0, 0, 180), animationTime, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.LeftToRight:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = Vector2.zero;
					animationSequence.Join(maskRect.DOAnchorPosX(maxSize, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.RightToLeft:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = Vector2.zero;
					animationSequence.Join(maskRect.DOAnchorPosX(-maxSize, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.TopToBottom:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = Vector2.zero;
					animationSequence.Join(maskRect.DOAnchorPosY(-maxSize, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.BottomToTop:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.anchoredPosition = Vector2.zero;
					animationSequence.Join(maskRect.DOAnchorPosY(maxSize, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.TvHorizontal:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.localScale = new Vector3(1, 1, 1);
					animationSequence.Join(maskRect.DOScaleX(0f, animationTime).SetEase(Ease.InOutQuad));
					break;

				case TransitionType.TvVertical:
					maskRect.sizeDelta = new Vector2(maxSize, maxSize);
					parentMaskRect.sizeDelta = Vector2.zero;

					maskRect.localScale = new Vector3(1, 1, 1);
					animationSequence.Join(maskRect.DOScaleY(0f, animationTime).SetEase(Ease.InOutQuad));
					break;
			}

			return animationSequence;
		}

		#endregion

		#region Transition Without Scene Load
		public void PlayTransition(float transitionTime, float startDelay = 0f)
		{
			StartCoroutine(PlayTransitionWithDelay(transitionTime, startDelay));
		}

		IEnumerator PlayTransitionWithDelay(float transitionTime, float startDelay)
		{
			float dividedTime = transitionTime / 3;

			//Optional Delay
			yield return new WaitForSeconds(startDelay);

			StartAnimation(dividedTime);
			yield return new WaitForSeconds(dividedTime);
			EndAnimation(dividedTime);
		}
		#endregion

		#region Transition With Scene Load 
		public void LoadLevel(string sceneName, float delay = 0f)
		{
			canvas.SetActive(true);
			BeforeLoad?.Invoke(sceneName);
			StartCoroutine(LoadLevelWithWait(sceneName, delay));
		}

		IEnumerator LoadLevelWithWait(string sceneName, float delay)
		{
			yield return new WaitForSeconds(delay);

			Tween animationTween = StartAnimationForLoad();

			// Wait for the animation to complete
			yield return animationTween.WaitForCompletion();

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}

			EndAnimation();
		}
		#endregion

		#region Play Partial Transitions
		public void PlayStartHalfTransition(float transitionTime, float startDelay = 0f)
		{
			StartCoroutine(PlayStartHalfTransitionWithDelay(transitionTime, startDelay));
		}
		public void PlayEndHalfTransition(float transitionTime, float startDelay = 0f)
		{
			StartCoroutine(PlayEndHalfTransitionWithDelay(transitionTime, startDelay));
		}
		IEnumerator PlayStartHalfTransitionWithDelay(float transitionTime, float startDelay)
		{
			yield return new WaitForSeconds(startDelay);
			StartAnimation(transitionTime);
		}
		IEnumerator PlayEndHalfTransitionWithDelay(float transitionTime, float startDelay)
		{
			yield return new WaitForSeconds(startDelay);
			EndAnimation(transitionTime);
		}
		#endregion
	}
}

