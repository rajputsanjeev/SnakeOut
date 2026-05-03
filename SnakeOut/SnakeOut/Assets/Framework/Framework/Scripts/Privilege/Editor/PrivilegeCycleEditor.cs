#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Framework
{
	[CustomEditor(typeof(PrivilegeCycleSet))]
	public class PrivilegeCycleEditor : Editor
	{
		private SerializedProperty cycleNameProp;
		private SerializedProperty stepsProp;

		private bool showSteps = true;
		private bool[] stepFoldouts;

		private void OnEnable()
		{
			cycleNameProp = serializedObject.FindProperty("cycleName");
			stepsProp = serializedObject.FindProperty("steps");

			SyncFoldouts();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(cycleNameProp);
			EditorGUILayout.Space();

			showSteps = EditorGUILayout.Foldout(showSteps, "Reward Steps", true);

			if (showSteps)
			{
				EditorGUILayout.BeginVertical("HelpBox");
				DrawSteps();
				EditorGUILayout.EndVertical();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawSteps()
		{
			SyncFoldouts();

			for (int i = 0; i < stepsProp.arraySize; i++)
			{
				SerializedProperty stepProp = stepsProp.GetArrayElementAtIndex(i);
				stepFoldouts[i] = EditorGUILayout.Foldout(
					stepFoldouts[i],
					$"Reward Step {i + 1}",
					true
				);

				if (!stepFoldouts[i]) continue;

				EditorGUILayout.BeginVertical("box");

				DrawRewardList(stepProp, "freeRewards", "Free Rewards");
				EditorGUILayout.Space(6);
				DrawRewardList(stepProp, "paidRewards", "Paid Rewards");

				EditorGUILayout.Space(8);

				if (GUILayout.Button("Remove Reward Step"))
				{
					stepsProp.DeleteArrayElementAtIndex(i);
					return;
				}

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}

			if (GUILayout.Button("Add Reward Step"))
			{
				stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
			}
		}

		private void DrawIconPreview(SerializedProperty iconProp)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			Texture tex = null;
			if (iconProp != null && iconProp.objectReferenceValue is Sprite sp)
				tex = sp.texture;

			GUILayout.Box(
				tex ? tex : Texture2D.grayTexture,
				GUILayout.Width(64),
				GUILayout.Height(64)
			);

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		private void DrawRewardList(
			SerializedProperty stepProp,
			string listName,
			string label)
		{
			SerializedProperty listProp = stepProp.FindPropertyRelative(listName);
			if (listProp == null) return;

			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

			for (int i = 0; i < listProp.arraySize; i++)
			{
				SerializedProperty itemProp = listProp.GetArrayElementAtIndex(i);

				EditorGUILayout.LabelField($"{label} {i + 1}", EditorStyles.miniBoldLabel);

				DrawField(itemProp, "type");
				DrawField(itemProp, "quantity");

				SerializedProperty iconProp = itemProp.FindPropertyRelative("icon");
				if (iconProp != null)
				{
					EditorGUILayout.PropertyField(iconProp);
					DrawIconPreview(iconProp);
				}

				if (GUILayout.Button("Remove reward"))
				{
					listProp.DeleteArrayElementAtIndex(i);
					return;
				}

				EditorGUILayout.Space(6);
			}

			if (GUILayout.Button("Add reward"))
			{
				listProp.InsertArrayElementAtIndex(listProp.arraySize);
			}
		}

		private void DrawField(SerializedProperty parent, string name)
		{
			SerializedProperty prop = parent.FindPropertyRelative(name);
			if (prop != null)
				EditorGUILayout.PropertyField(prop);
		}

		private void SyncFoldouts()
		{
			if (stepsProp == null) return;

			if (stepFoldouts == null || stepFoldouts.Length != stepsProp.arraySize)
				stepFoldouts = new bool[stepsProp.arraySize];
		}
	}
}
#endif
