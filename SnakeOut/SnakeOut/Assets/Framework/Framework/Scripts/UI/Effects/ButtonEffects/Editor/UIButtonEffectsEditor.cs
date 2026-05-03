#if UNITY_EDITOR
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{
	[CustomEditor(typeof(UIButtonEffects))]
	public class UIButtonEffectsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			UIButtonEffects fx = (UIButtonEffects)target;

			EditorGUILayout.LabelField("Preset System", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("preset"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("overridePreset"));

			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Button FX Settings", EditorStyles.boldLabel);

			if (!fx.overridePreset)
			{
				GUI.enabled = false;
				EditorGUILayout.HelpBox("Settings are controlled by the preset. Enable 'Override Preset' to edit.", MessageType.Info);
			}

			DrawPropertiesExcluding(serializedObject, "m_Script", "preset", "overridePreset");

			GUI.enabled = true;
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
