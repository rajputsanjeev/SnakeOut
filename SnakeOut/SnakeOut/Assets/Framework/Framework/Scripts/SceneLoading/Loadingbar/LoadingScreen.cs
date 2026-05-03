using System.Collections;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Framework
{
	public class LoadingScreen : MonoBehaviour
	{
		[Header("References")]
		public Image progressBar;
		public TextMeshProUGUI progressText;
		public TextMeshProUGUI tipsText;
		public Image backgroundImage;
		public Image nextBackgroundImage;
		public CanvasGroup canvasGroup;
		public LoadingSpinner spinner;

		[Header("Settings")]
		public LoadingSettings settings;
		public bool showProgressText = true;

		private int currentSlide = 0;
		private bool sceneReady = false;

		private void Start()
		{
			ApplySettings();
		}

		private void OnEnable()
		{
			LoadingEvents.OnLoadSceneRequested += LoadScene;
		}

		private void OnDisable()
		{
			LoadingEvents.OnLoadSceneRequested -= LoadScene;
		}

		void ApplySettings()
		{
			if (settings == null) return;

			if (spinner != null)
				spinner.rotationSpeed = settings.spinnerSpeed;

			if (progressText != null)
				progressText.gameObject.SetActive(showProgressText);
		}

		// ====================================================
		// PUBLIC LOAD METHOD
		// ====================================================
		public void LoadScene(string sceneName)
		{
			gameObject.SetActive(true);
			StartCoroutine(LoadRoutine(sceneName));
		}

		// ====================================================
		// MAIN LOADING ROUTINE
		// ====================================================
		private IEnumerator LoadRoutine(string sceneName)
		{
			canvasGroup.alpha = 0;
			yield return StartCoroutine(FadeCanvas(1, settings.fadeInDuration));

			StartCoroutine(TipsRoutine());
			StartCoroutine(SlideshowRoutine());

			AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
			op.allowSceneActivation = false;
			float dummyProgress = 0.1f;

			while (op.progress < 1)
			{
				//float progress = Mathf.Clamp01(op.progress / 0.9f);
				float progress = (op.progress < 9) ? dummyProgress : op.progress;
				dummyProgress += 0.1f;
				dummyProgress = Mathf.Clamp(dummyProgress, 0f, 0.9f);

				if (progressBar != null)
					progressBar.fillAmount = dummyProgress;

				if (showProgressText && progressText != null)
					progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

				if (op.progress >= 0.9f)
				{
					sceneReady = true;

					if (!settings.waitForUserInput)
					{
						yield return new WaitForSeconds(0.2f);
						op.allowSceneActivation = true;
					}
					else
					{
						if (progressText != null)
							progressText.text = "Tap to Continue";

						while (!Input.anyKeyDown)
							yield return null;

						op.allowSceneActivation = true;
					}
				}

				yield return null;
			}

			//float dummyProgress = 0.1f;

			//while (op.progress < 1)
			//{
			//	float progress = (op.progress < 9) ? dummyProgress : op.progress;
			//	dummyProgress += 0.1f;
			//	dummyProgress = Mathf.Clamp(dummyProgress, 0f, 0.9f);
			//	yield return null;
			//}

		}

		// ====================================================
		// FADE ROUTINE
		// ====================================================
		private IEnumerator FadeCanvas(float target, float duration)
		{
			float start = canvasGroup.alpha;
			float t = 0;

			while (t < duration)
			{
				t += Time.deltaTime;
				canvasGroup.alpha = Mathf.Lerp(start, target, t / duration);
				yield return null;
			}
		}

		// ====================================================
		// TIPS ROUTINE
		// ====================================================
		IEnumerator TipsRoutine()
		{
			if (settings.tips == null || settings.tips.Length == 0) yield break;

			while (true)
			{
				tipsText.text = settings.tips[Random.Range(0, settings.tips.Length)];
				yield return new WaitForSeconds(settings.tipChangeInterval);
			}
		}

		// ====================================================
		// SLIDESHOW ROUTINE
		// ====================================================
		IEnumerator SlideshowRoutine()
		{
			if (settings.backgroundSlides == null || settings.backgroundSlides.Length == 0)
				yield break;

			while (true)
			{
				Sprite next = settings.backgroundSlides[currentSlide];
				nextBackgroundImage.sprite = next;

				yield return StartCoroutine(FadeImage(nextBackgroundImage, 1f, settings.slideFadeDuration));
				yield return StartCoroutine(FadeImage(backgroundImage, 0f, settings.slideFadeDuration));

				Image temp = backgroundImage;
				backgroundImage = nextBackgroundImage;
				nextBackgroundImage = temp;

				nextBackgroundImage.color = new Color(1, 1, 1, 0);

				currentSlide = (currentSlide + 1) % settings.backgroundSlides.Length;

				yield return new WaitForSeconds(settings.slideChangeInterval);
			}
		}

		IEnumerator FadeImage(Image img, float targetAlpha, float duration)
		{
			float start = img.color.a;
			float t = 0;

			while (t < duration)
			{
				t += Time.deltaTime;
				Color c = img.color;
				c.a = Mathf.Lerp(start, targetAlpha, t / duration);
				img.color = c;
				yield return null;
			}
		}
	}
}