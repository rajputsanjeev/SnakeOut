using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Framework;


namespace Framework
{
	public class SceneController : MonoBehaviour
	{
		public static SceneController m_Instance;
		public static Action<string> OnNewLevelLoaded;
		public static Action<string> OnUnloadSceneCompleted;
		public static string CurrentScene;

		[SerializeField] public GameObject loadingCanvas;
		[SerializeField] private LoadingProgressView loadingView;

		private void Awake()
		{
			m_Instance = this;
			Screen.fullScreen = true;
			SceneManager.sceneLoaded += OnLevelLoaded;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnLevelLoaded;
		}

		private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
		{
			Time.timeScale = 1f;
			SceneManager.SetActiveScene(scene);
			OnNewLevelLoaded?.Invoke(scene.name);
			CurrentScene = scene.name;
		}

		public static void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		public static void LoadScene(SceneProperties sceneProperties, UnityAction callback = null)
		{
			string sceneName = sceneProperties.sceneName;

			if (sceneProperties.isAsync)
			{
				m_Instance.StartCoroutine(m_Instance.LoadSceneAsync(sceneName, sceneProperties, callback));
			}
			else
			{
				SceneManager.LoadScene(sceneName, sceneProperties.loadSceneMode);
			}
		}

		private IEnumerator LoadSceneAsync(string sceneName, SceneProperties sceneProperties, UnityAction callback)
		{

			yield return StartCoroutine(ShowLoading(sceneProperties));

			AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, sceneProperties.loadSceneMode);

			float dummyProgress = 0.1f;

			while (op.progress < 1)
			{
				float progress = (op.progress < 9) ? dummyProgress : op.progress;
				dummyProgress += 0.1f;
				dummyProgress = Mathf.Clamp(dummyProgress, 0f, 0.9f);

				loadingView?.SetLoading(progress);
				yield return null;
			}

			loadingView?.SetLoading(1f);
			yield return new WaitForSeconds(1f);

			if (loadingCanvas != null)
			{
				loadingCanvas.SetActive(false);
			}

			callback?.Invoke();
		}


		private IEnumerator ShowLoading(SceneProperties sceneProperties)
		{
			if (sceneProperties.showLoading)
			{
				loadingCanvas?.SetActive(true);
				yield return new WaitForSeconds(1f);
			}
		}

		private IEnumerator WaitForUnloadScene(string sceneName)
		{
			AsyncOperation sceneUnload = SceneManager.UnloadSceneAsync(sceneName);
			while (sceneUnload != null && !sceneUnload.isDone)
			{
				yield return null;
			}

			OnUnloadSceneCompleted?.Invoke(sceneName);
		}

		public static void ReloadCurrentScene()
		{
			SceneProperties sceneProperties = new SceneProperties()
			{
				isAsync = true,
				loadSceneMode = LoadSceneMode.Single,
				sceneName = CurrentScene,
				showLoading = true
			};

			LoadScene(sceneProperties);
		}
	}

	[System.Serializable]
	public class SceneProperties
	{
		public string sceneName;
		public LoadSceneMode loadSceneMode;
		public bool isAsync;
		public bool showLoading;

		public SceneProperties()
		{
		}

		public SceneProperties(string sceneName)
		{
			this.sceneName = sceneName;
			loadSceneMode = LoadSceneMode.Single;
			isAsync = true;
			showLoading = true;
		}
	}
}
