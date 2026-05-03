using Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	[CustomEditor(typeof(LoadingScreen))]
	public class LoadingScreenEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			LoadingScreen ls = (LoadingScreen)target;

			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);

			if (GUILayout.Button("Auto-Assign Components"))
			{
				TryAutoAssign(ls);
			}

			if (GUILayout.Button("Test Fade In"))
			{
				ls.gameObject.SetActive(true);
				ls.StartCoroutine("FadeCanvas", new object[] { 1f, 1f });
			}

			if (GUILayout.Button("Test Fade Out"))
			{
				ls.StartCoroutine("FadeCanvas", new object[] { 0f, 1f });
			}
		}

		void TryAutoAssign(LoadingScreen ls)
		{
			ls.progressBar = ls.GetComponentInChildren<Image>();
			ls.progressText = ls.GetComponentInChildren<TMPro.TextMeshProUGUI>();
			ls.tipsText = ls.transform.Find("TipsText")?.GetComponent<TMPro.TextMeshProUGUI>();
			ls.backgroundImage = ls.transform.Find("BG1")?.GetComponent<Image>();
			ls.nextBackgroundImage = ls.transform.Find("BG2")?.GetComponent<Image>();
			ls.canvasGroup = ls.GetComponent<CanvasGroup>();
			ls.spinner = ls.GetComponentInChildren<LoadingSpinner>();

			EditorUtility.SetDirty(ls);
			Debug.Log("Auto-assign complete.");
		}
	}
}