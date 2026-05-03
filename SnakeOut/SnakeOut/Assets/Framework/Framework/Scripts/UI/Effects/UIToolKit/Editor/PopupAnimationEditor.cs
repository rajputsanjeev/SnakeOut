// Assets/Editor/PopupAnimationEditor.cs
using UnityEngine;
using UnityEditor;

namespace Framework
{
	[CustomEditor(typeof(PopupAnimator))]
	public class PopupAnimationEditor : Editor
	{
		SerializedProperty showPresetProp;
		SerializedProperty hidePresetProp;
		SerializedProperty canvasGroupProp;
		SerializedProperty particleParentProp;
		SerializedProperty rectTransformParentProp;

		void OnEnable()
		{
			showPresetProp = serializedObject.FindProperty("showPreset");
			hidePresetProp = serializedObject.FindProperty("hidePreset");
			canvasGroupProp = serializedObject.FindProperty("canvasGroup");
			particleParentProp = serializedObject.FindProperty("particleParent");
			rectTransformParentProp = serializedObject.FindProperty("rect");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Popup Animator (DOTween)", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(showPresetProp, new GUIContent("Show Preset"));
			if (showPresetProp.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Assign a Show preset (PopupAnimationData) to define show animation.", MessageType.Info);
			}
			EditorGUILayout.PropertyField(hidePresetProp, new GUIContent("Hide Preset"));
			if (hidePresetProp.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Assign a Hide preset (PopupAnimationData) to define hide animation.", MessageType.Info);
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(canvasGroupProp, new GUIContent("Canvas Group (optional)"));
			EditorGUILayout.PropertyField(particleParentProp, new GUIContent("Particle Parent (optional)"));
			EditorGUILayout.PropertyField(rectTransformParentProp, new GUIContent("Rect Transform"));


			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Play Show (Play Mode)"))
			{
				if (Application.isPlaying)
				{
					(target as PopupAnimator).Show(false);
				}
				else
				{
					Debug.LogWarning("Enter Play Mode to preview animations.");
				}
			}

			if (GUILayout.Button("Play Hide (Play Mode)"))
			{
				if (Application.isPlaying)
				{
					(target as PopupAnimator).Hide(false);
				}
				else
				{
					Debug.LogWarning("Enter Play Mode to preview animations.");
				}
			}
			GUILayout.EndHorizontal();

			if (!Application.isPlaying)
			{
				EditorGUILayout.HelpBox("Editor preview works only in Play Mode. Use 'Apply Preset' to apply final state in Editor.", MessageType.Info);
				if (GUILayout.Button("Apply Show Preset Immediately (Editor)"))
				{
					var pa = (PopupAnimator)target;
					if (pa.showPreset != null)
					{
						pa.gameObject.SetActive(true);
						pa.Show(true);
						EditorUtility.SetDirty(pa);
					}
					else Debug.LogWarning("No show preset assigned.");
				}
				if (GUILayout.Button("Apply Hide Preset Immediately (Editor)"))
				{
					var pa = (PopupAnimator)target;
					if (pa.hidePreset != null)
					{
						pa.Hide(true);
						EditorUtility.SetDirty(pa);
					}
					else Debug.LogWarning("No hide preset assigned.");
				}
			}

			EditorGUILayout.Space();
			if (GUILayout.Button("Open Preset Folder"))
			{
				// tries to open folder where presets would typically be created
				EditorUtility.RevealInFinder("Assets");
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}