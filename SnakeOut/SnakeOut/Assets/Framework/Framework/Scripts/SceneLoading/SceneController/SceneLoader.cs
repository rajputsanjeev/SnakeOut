using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Framework;

namespace Framework
{
	public class SceneLoader : MonoBehaviour
	{
		public static SceneLoader Instance;

		public static event Action<string> BeforeLoad;
		public static event Action<string> AfterLoad;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public static void LoadAsync(int sceneIndex, ILoadingScreen loadingScreen, LoadingScreenConfig config)
		{
			Instance.StartCoroutine(
				Instance.LoadRoutine(sceneIndex, loadingScreen, config)
			);
		}

		private IEnumerator LoadRoutine(int sceneIndex, ILoadingScreen loadingScreen, LoadingScreenConfig config)
		{
			string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
			BeforeLoad?.Invoke(sceneName);

			loadingScreen.Initialize(config);
			loadingScreen.Show();
			yield return loadingScreen.FadeIn();

			float startTime = Time.realtimeSinceStartup;
			float minimumFinishTime = startTime + config.minimumLoadingTime;

			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
			operation.allowSceneActivation = false;

			while (!operation.isDone || Time.realtimeSinceStartup < minimumFinishTime)
			{
				float mappedProgress = Mathf.Lerp(
					config.progressStart,
					config.progressEnd,
					operation.progress
				);

				loadingScreen.SetProgress(mappedProgress);

				if (operation.progress >= 0.9f &&
					Time.realtimeSinceStartup >= minimumFinishTime)
				{
					break;
				}

				yield return null;
			}

			loadingScreen.SetProgress(1f);
			loadingScreen.SetStatus("Completed");

			operation.allowSceneActivation = true;
			yield return null;

			AfterLoad?.Invoke(sceneName);

			yield return loadingScreen.FadeOut();
			loadingScreen.HideInstant();
		}

		public static void LoadAsync(string sceneName, ILoadingScreen loadingScreen, LoadingScreenConfig config)
		{
			int buildIndex = GetBuildIndex(sceneName);

			if (buildIndex < 0)
			{
				Debug.LogError($"[SceneLoader] Scene not found: {sceneName}");
				return;
			}

			Instance.StartCoroutine(
				Instance.LoadRoutine(buildIndex, loadingScreen, config)
			);
		}

		private static int GetBuildIndex(string sceneName)
		{
			// Try direct name
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = SceneUtility.GetScenePathByBuildIndex(i);
				string name = System.IO.Path.GetFileNameWithoutExtension(path);

				if (name.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
					return i;
			}

			return -1;
		}
	}
}
