using UnityEditor;
using UnityEngine;

public class ApplyShinyEffectSelectObject : EditorWindow
{
	// 🔹 Set the exact asset paths
	private const string ScriptPath = "Assets/Framework/Framework/UIEffect/Scripts/UIShiny.cs";               // full path to script
	private const string AnimatorPath = "Assets/Framework/Framework/UIEffect/UIEffect_Demo_For Thumbnail.controller"; // full path to animator

	[MenuItem("Tools/Apply Script + Animator To Selected Buttons")]
	public static void ApplyToSelectedButtons()
	{
		// --- Load script from path ---
		MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(ScriptPath);
		if (monoScript == null)
		{
			EditorUtility.DisplayDialog("Error", $"Could not find script at path:\n{ScriptPath}", "OK");
			return;
		}

		System.Type scriptType = monoScript.GetClass();
		if (scriptType == null || !scriptType.IsSubclassOf(typeof(MonoBehaviour)))
		{
			EditorUtility.DisplayDialog("Error", "The selected file is not a valid MonoBehaviour script.", "OK");
			return;
		}

		// --- Load animator controller from path ---
		RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AnimatorPath);
		if (controller == null)
		{
			EditorUtility.DisplayDialog("Error", $"Could not find Animator Controller at path:\n{AnimatorPath}", "OK");
			return;
		}

		// --- Apply to selected buttons ---
		int count = 0;
		foreach (GameObject obj in Selection.gameObjects)
		{
			// Add script
			if (obj.GetComponent(scriptType) == null)
			{
				Undo.AddComponent(obj, scriptType);
			}

			// Add animator
			Animator animator = obj.GetComponent<Animator>();
			if (animator == null)
			{
				animator = Undo.AddComponent<Animator>(obj);
			}

			animator.runtimeAnimatorController = controller;
			count++;
		}

		EditorUtility.DisplayDialog("Done", $"Applied script + animator to {count} button(s).", "OK");
	}
}
