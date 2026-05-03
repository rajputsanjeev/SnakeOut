using UnityEngine;
using UnityEditor;
using TMPro;
using System;
using System.Linq;

public class ApplyLanguageChangeScriptTxtpro : EditorWindow
{
	private MonoScript scriptToAdd;

	[MenuItem("Tools/Apply Script To TMP")]
	public static void ShowWindow()
	{
		GetWindow<ApplyLanguageChangeScriptTxtpro>("Apply Script to TMP");
	}

	private void OnGUI()
	{
		GUILayout.Label("Apply Script to TextMeshProUGUI", EditorStyles.boldLabel);

		scriptToAdd = (MonoScript)EditorGUILayout.ObjectField(
			"Script",
			scriptToAdd,
			typeof(MonoScript),
			false
		);

		if (scriptToAdd == null)
		{
			EditorGUILayout.HelpBox("Please assign a script first.", MessageType.Info);
			return;
		}

		Type scriptType = scriptToAdd.GetClass();
		if (scriptType == null || !scriptType.IsSubclassOf(typeof(MonoBehaviour)))
		{
			EditorGUILayout.HelpBox("Selected file is NOT a valid MonoBehaviour script.", MessageType.Error);
			return;
		}

		EditorGUILayout.Space(10);

		if (GUILayout.Button("Apply To Selected TextMeshProUGUI"))
			ApplyToSelected(scriptType);

		if (GUILayout.Button("Apply To Children of Selected"))
			ApplyToChildren(scriptType);

		if (GUILayout.Button("Apply To All In Scene"))
			ApplyToAllInScene(scriptType);
	}

	private void ApplyToSelected(Type scriptType)
	{
		foreach (var obj in Selection.objects)
		{
			if (obj is GameObject go)
			{
				var tmp = go.GetComponent<TextMeshProUGUI>();
				if (tmp != null)
					AddScript(go, scriptType);
			}
		}
		Debug.Log("Applied script to selected TMP components.");
	}

	private void ApplyToChildren(Type scriptType)
	{
		foreach (var obj in Selection.objects)
		{
			if (obj is GameObject go)
			{
				var tmps = go.GetComponentsInChildren<TextMeshProUGUI>(true);
				foreach (var tmp in tmps)
					AddScript(tmp.gameObject, scriptType);
			}
		}
		Debug.Log("Applied script to child TMP components.");
	}

	private void ApplyToAllInScene(Type scriptType)
	{
		var allTMP = FindObjectsOfType<TextMeshProUGUI>(true);
		foreach (var tmp in allTMP)
			AddScript(tmp.gameObject, scriptType);

		Debug.Log("Applied script to ALL TMP components in scene.");
	}

	private void AddScript(GameObject go, Type scriptType)
	{
		if (go.GetComponent(scriptType) == null)
		{
			Undo.AddComponent(go, scriptType);
		}
	}
}
