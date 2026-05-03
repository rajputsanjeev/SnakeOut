using System;
using System.Collections;
using System.Collections.Generic;
using Frameork;
using Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Watermelon;

namespace Framework.Core
{
	[StaticUnload]
	public class GameLoading : MonoBehaviour
	{
		public LoadingScreenUI loadingScreenUI;

		private const float MINIMUM_LOADING_TIME = 5.0f;

		private static GameLoading gameLoading;

		[SerializeField] Initializer initializer;
		[SerializeField] LoadingGraphics loadingGraphics;

		[Space]
		[Tooltip("If manual mode is enabled, the loading screen will be active until GameLoading.MarkAsReadyToHide method has been called.")]
		[SerializeField] bool useManualControl;
		[SerializeField] bool checkNetworkConnection = true;

		private RemoteConfigHandler remoteConfigHandler;

		private static AsyncOperation loadingOperation;

		private static bool isReadyToHide;

		private static string loadingMessage;
		private static List<LoadingTask> loadingTasks = new List<LoadingTask>();

		private Coroutine initCoroutine;

		public static int LoadingSceneBuildIndex = -1;
		public static int LevelRequiredUnlockMenuScreen = 15;
		private int Index;

		private void Awake()
		{
			gameLoading = this;

			DontDestroyOnLoad(gameObject);

			remoteConfigHandler = initializer.GetComponent<RemoteConfigHandler>();

			initializer.Init();
			loadingGraphics.Init(this);
			initCoroutine = StartCoroutine(ConnectionCheckCoroutine());
		}

		private void Loadgame()
		{
			var levelSave = SaveController.GetSaveObject<LevelSave>();
			ActiveSession session = ActiveSession.Current;
			session.SetLevelIndex(levelSave.MaxReachedLevelIndex);

			if (LivesSystem.Lives > 0 || LivesSystem.InfiniteMode)
			{
				Index = LevelRequiredUnlockMenuScreen <= levelSave.MaxReachedLevelIndex ? 1 : 2;
			}
			else
			{
				Index = LevelRequiredUnlockMenuScreen = 1;
			}
		}

		public void RetryConnection()
		{
			if (initCoroutine == null)
			{
				initCoroutine = StartCoroutine(ConnectionCheckCoroutine());
			}
		}

		private IEnumerator ConnectionCheckCoroutine()
		{
			loadingGraphics.HideErrorMessage();
			loadingGraphics.SetLoadingState(0.0f, "Checking connection..");

			if (checkNetworkConnection)
			{
				bool isConnected = false;

				NetworkConnection networkConnection = new NetworkConnection("https://google.com/");
				IEnumerator connectionCheck = networkConnection.CheckConnection((state) => isConnected = state);

				yield return connectionCheck;

				if (!isConnected)
				{
					loadingGraphics.ShowErrorMessage("Connection error");

					initCoroutine = null;

					yield break;
				}
			}

			if (remoteConfigHandler != null)
			{
				bool isConfigLoaded = false;

				loadingGraphics.SetLoadingState(0.1f, "Loading Data..");

				IEnumerator configLoad = remoteConfigHandler.LoadConfig((state) => isConfigLoaded = state);

				yield return configLoad;

				if (!isConfigLoaded)
				{
					loadingGraphics.ShowErrorMessage("Failed to load data");

					initCoroutine = null;

					yield break;
				}
			}
			else
			{
				loadingGraphics.SetLoadingState(0.1f, "Loading..");
			}

			initializer.InitModules(ConfigCatLoaded);
		}

		private void ConfigCatLoaded()
		{
			StartCoroutine(ConfigCatIEnumerator());
		}

		private IEnumerator ConfigCatIEnumerator()
		{
			initializer.InitSDKs();
			Loadgame();
			GetCurrentLevelAbstract.Instance.SetCurrenTLevel();

			int taskIndex = 0;
			while (taskIndex < loadingTasks.Count)
			{
				if (!loadingTasks[taskIndex].IsActive)
					loadingTasks[taskIndex].Activate();

				if (loadingTasks[taskIndex].IsFinished)
				{
					taskIndex++;
				}

				yield return null;
			}

			var realtimeSinceStartup = Time.realtimeSinceStartup;
			var minimumFinishTime = realtimeSinceStartup + MINIMUM_LOADING_TIME;

			loadingOperation = SceneManager.LoadSceneAsync(Index);

			loadingMessage = "Loading..";

			while (!loadingOperation.isDone || realtimeSinceStartup < minimumFinishTime)
			{
				yield return null;

				realtimeSinceStartup = Time.realtimeSinceStartup;

				loadingGraphics.SetLoadingState(Mathf.Lerp(0.2f, 0.9f, loadingOperation.progress), loadingMessage);
			}

			loadingGraphics.SetLoadingState(1.0f, "Completed");

			if (useManualControl)
			{
				// Debug check if MarkAsReadyToHide is implemented
				Tween.DelayedCall(10, () =>
				{
					if (!isReadyToHide)
						Debug.LogError("[Loading]: Seems like you forget to call MarkAsReadyToHide method to finish the loading process.");
				});

				while (!isReadyToHide)
				{
					yield return null;
				}
			}

			loadingGraphics.OnLoadingFinished();

			HomeUIAnimator.OnSceneLoadComplete?.Invoke();

			Destroy(gameObject);
		}

		public static void SetLoadingMessage(string message)
		{
			loadingMessage = message;

			float progress = 0.0f;
			if (loadingOperation != null)
				progress = loadingOperation.progress;
		}

		public static void AddTask(LoadingTask loadingTask)
		{
			loadingTasks.Add(loadingTask);
		}

		public static void MarkAsReadyToHide()
		{
			isReadyToHide = true;
		}

		private static void UnloadStatic()
		{
			isReadyToHide = false;
			loadingTasks.Clear();
		}

		public delegate void LoadingCallback(float state, string message);
	}
}