#pragma warning disable 0649

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Core
{
	[DefaultExecutionOrder(-999)]
	[Define("MODULE_CONFIG_CAT", "ConfigCat.Client.ConfigCatClient", "ConfigCat.Client.dll")]
	public class Initializer : MonoBehaviour
	{
		private static Initializer initializer;

		public static bool IS_TESTING_ADS;
		public bool UseTestingAds;

		public string CONFIG_API_KEY;
		[SerializeField] ProjectInitSettings initSettings;
		[SerializeField] SDKInitializer sdkInitializer;
		[SerializeField] SystemMessage systemMessage;
		[SerializeField] MusicSource globalMusicSource;
		[SerializeField] EventSystem eventSystem;

		public static GameObject GameObject { get; private set; }
		public static Transform Transform { get; private set; }

		public static ProjectInitSettings InitSettings { get; private set; }
		private Action _callBack;

		public void Init()
		{
			if (initializer != null) return;

			initializer = this;

			InitSettings = initSettings;

			GameObject = gameObject;
			Transform = transform;

#if MODULE_INPUT_SYSTEM
			eventSystem.gameObject.GetOrSetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
			eventSystem.gameObject.GetOrSetComponent<StandaloneInputModule>();
#endif

			systemMessage.Init();

#if MODULE_CONFIG_CAT && (UNITY_ANDROID || UNITY_IOS)
			ConfigCatHandler.Initialize(CONFIG_API_KEY);
#endif

			AnalyticsModules.Init();

			DontDestroyOnLoad(gameObject);
		}

		public void InitModules(Action callBack = null)
		{
			_callBack = callBack;
			if (UseTestingAds)
			{
#if MODULE_CONFIG_CAT && (UNITY_ANDROID || UNITY_IOS)
				ConfigCatHandler.LoadConfigAsync(OnConfigLoaded, "blockslidetestingads");
#endif
			}
			else
			{
				OnConfigLoaded(false);
			}
		}

		private void OnConfigLoaded(bool isTestingAds)
		{
			IS_TESTING_ADS = isTestingAds;

			initSettings.Init(this);

			StaticModules.InitStaticModules();

			if (globalMusicSource != null)
			{
				globalMusicSource.Init();
				globalMusicSource.Activate();
			}
			_callBack?.Invoke();
		}

		public void InitSDKs()
		{
			sdkInitializer.Init();
		}
	}
}
