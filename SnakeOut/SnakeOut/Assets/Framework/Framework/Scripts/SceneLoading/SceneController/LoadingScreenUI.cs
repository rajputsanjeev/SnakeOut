using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Framework;



namespace Framework
{
	public class LoadingScreenUI : MonoBehaviour, ILoadingScreen
	{
		public LoadingScreenConfig _loadingScreenConfig;

		[Header("References")]
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private Image progressBar;
		[SerializeField] private TMP_Text percentageText;
		[SerializeField] private TMP_Text statusText;
		[SerializeField] private TMP_Text tipsText;
		[SerializeField] private Image backgroundImage;

		private LoadingScreenConfig config;
		private Coroutine tipsRoutine;
		private Coroutine slideRoutine;

		public void Initialize(LoadingScreenConfig config)
		{
			this.config = config;
		}

		public void Show()
		{
			canvasGroup.blocksRaycasts = true;
			canvasGroup.interactable = true;

			tipsRoutine = StartCoroutine(TipsRoutine());
			slideRoutine = StartCoroutine(SlideRoutine());
		}

		public IEnumerator FadeIn()
		{
			canvasGroup.alpha = 0;
			yield return canvasGroup
				.DOFade(1f, config.fadeInTime)
				.SetUpdate(true)
				.WaitForCompletion();
		}

		public IEnumerator FadeOut()
		{
			yield return canvasGroup
				.DOFade(0f, config.fadeOutTime)
				.SetUpdate(true)
				.WaitForCompletion();
		}

		public void HideInstant()
		{
			if (canvasGroup == null)
			{
				return;
			}
			canvasGroup.alpha = 0;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;

			if (tipsRoutine != null) StopCoroutine(tipsRoutine);
			if (slideRoutine != null) StopCoroutine(slideRoutine);

			SetProgress(0);
			SetStatus("Loading...");
		}

		public void SetProgress(float value)
		{
			progressBar.fillAmount = value;
			percentageText.text = $"{Mathf.RoundToInt(value * 100f)}%";
		}

		public void SetStatus(string message)
		{
			statusText.text = message;
		}

		private IEnumerator TipsRoutine()
		{
			if (config.tips == null || config.tips.Length == 0)
				yield break;

			int index = Random.Range(0, config.tips.Length);

			while (true)
			{
				tipsText.text = config.tips[index];
				index = (index + 1) % config.tips.Length;
				yield return new WaitForSecondsRealtime(config.tipChangeInterval);
			}
		}

		private IEnumerator SlideRoutine()
		{
			if (config.backgroundSlides == null ||
				config.backgroundSlides.Length == 0 ||
				backgroundImage == null)
				yield break;

			int index = Random.Range(0, config.backgroundSlides.Length);

			while (true)
			{
				backgroundImage.sprite = config.backgroundSlides[index];
				index = (index + 1) % config.backgroundSlides.Length;
				yield return new WaitForSecondsRealtime(config.slideChangeInterval);
			}
		}
	}
}