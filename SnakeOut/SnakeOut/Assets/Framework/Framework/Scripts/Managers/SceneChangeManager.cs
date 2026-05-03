using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Framework
{
	public class SceneChangeManager : MonoBehaviour
	{
		// Action to notify listeners when a scene changes
		public static Action<string> OnSceneChanged;

		void OnEnable()
		{
			// Subscribe to Unity's sceneLoaded callback
			SceneManager.sceneLoaded += HandleSceneLoaded;
		}

		void OnDisable()
		{
			SceneManager.sceneLoaded -= HandleSceneLoaded;
		}

		private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Debug.Log("Scene Changed to: " + scene.name);

			// Invoke our Action so other scripts know about it
			OnSceneChanged?.Invoke(scene.name);
		}
	}
}