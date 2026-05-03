#if UNITY_EDITOR
using Framework;
using UnityEditor;
using UnityEngine;

public class UIButtonPresetApplier : EditorWindow
{
	UIButtonEffectPreset preset;

	[MenuItem("Tools/UI/Apply Button Preset")]
	public static void ShowWindow()
	{
		GetWindow<UIButtonPresetApplier>("Button Preset Applier");
	}

	void OnGUI()
	{
		GUILayout.Label("Apply Preset To All Buttons", EditorStyles.boldLabel);

		preset = (UIButtonEffectPreset)EditorGUILayout.ObjectField("Preset:", preset, typeof(UIButtonEffectPreset), false);

		if (GUILayout.Button("Apply to All UIButtonEffects"))
		{
			if (preset == null)
			{
				EditorUtility.DisplayDialog("Error", "Please select a preset.", "OK");
				return;
			}

			var all = FindObjectsOfType<UIButtonEffects>();

			foreach (var fx in all)
			{
				fx.preset = preset;
				fx.overridePreset = false;
				fx.ApplyPreset();
				EditorUtility.SetDirty(fx);
			}

			EditorUtility.DisplayDialog("Done", "Preset applied to all UI buttons!", "OK");
		}
	}
}
#endif
