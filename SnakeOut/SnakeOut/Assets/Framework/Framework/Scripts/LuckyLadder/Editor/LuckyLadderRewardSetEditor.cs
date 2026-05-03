using UnityEditor;
using UnityEngine;

namespace Framework
{
	[CustomEditor(typeof(LuckyLadderRewardSet))]
	public class LuckyLadderRewardSetEditor : Editor
	{
		SerializedProperty finalRewardProp;
		SerializedProperty stepsProp;

		// Optional: per-step foldouts for better navigation
		bool[] stepFoldouts = new bool[6];

		private void OnEnable()
		{
			finalRewardProp = serializedObject.FindProperty("FinalRewardSet");
			stepsProp = serializedObject.FindProperty("steps");

			// Initialize foldout states if needed
			if (stepFoldouts == null || stepFoldouts.Length != 6)
				stepFoldouts = new bool[6];

			// Ensure array exists and is exactly 6
			if (stepsProp == null)
				return;

			if (stepsProp.isArray && stepsProp.arraySize != 6)
				stepsProp.arraySize = 6;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Reward Set", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(finalRewardProp);

			EditorGUILayout.Space(10);

			EditorGUILayout.LabelField("Reward Steps (6)", EditorStyles.boldLabel);

			if (stepsProp == null || !stepsProp.isArray)
			{
				EditorGUILayout.HelpBox("Steps array not found or not an array.", MessageType.Error);
			}
			else
			{
				// Clamp to 6 (safety)
				if (stepsProp.arraySize != 6)
					stepsProp.arraySize = 6;

				for (int i = 0; i < stepsProp.arraySize; i++)
				{
					SerializedProperty step = stepsProp.GetArrayElementAtIndex(i);
					DrawRewardStep(step, i);
					EditorGUILayout.Space(6);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		// ---------------- Step Block ----------------
		private void DrawRewardStep(SerializedProperty step, int stepIndex)
		{
			if (step == null) return;

			EditorGUILayout.BeginVertical("box");

			// Optional: foldout header to save space
			stepFoldouts[stepIndex] = EditorGUILayout.Foldout(
				stepFoldouts[stepIndex],
				$"Step {stepIndex + 1}",
				true
			);

			if (stepFoldouts[stepIndex])
			{
				EditorGUI.indentLevel++;

				SerializedProperty items = step.FindPropertyRelative("items");
				SerializedProperty desc = step.FindPropertyRelative("description");

				// Items list
				EditorGUILayout.LabelField("Items", EditorStyles.miniBoldLabel);

				if (items != null && items.isArray)
				{
					// Optional: cap to 4 items with a notice
					EditorGUILayout.HelpBox(
						items.arraySize <= 4
							? "UI will show up to 4 items for this step."
							: "This step has more than 4 items; only the first 4 will be shown in the UI.",
						MessageType.None
					);

					for (int i = 0; i < Mathf.Min(items.arraySize, 4); i++)
					{
						SerializedProperty item = items.GetArrayElementAtIndex(i);
						DrawRewardItem(item, items, i);
					}

					// If there are more than 4, we won't render extra ones, but you can expand this if needed.
				}

				// Add item button (only if there are less than 4)
				if (items != null && items.isArray && items.arraySize < 4)
				{
					EditorGUILayout.Space(2);
					if (GUILayout.Button("+ Add Reward Item"))
					{
						int newIndex = items.arraySize;
						items.InsertArrayElementAtIndex(newIndex);

						SerializedProperty newItem = items.GetArrayElementAtIndex(newIndex);
						var typeProp = newItem.FindPropertyRelative("type");
						var quantityProp = newItem.FindPropertyRelative("quantity");
						var iconProp = newItem.FindPropertyRelative("icon");

						if (typeProp != null)
							typeProp.enumValueIndex = 0; // default to first enum value
						if (quantityProp != null)
							quantityProp.intValue = 1;
						if (iconProp != null)
							iconProp.objectReferenceValue = null;
					}
				}

				EditorGUILayout.Space(4);

				// Description field
				if (desc != null)
				{
					EditorGUILayout.LabelField("Description", EditorStyles.miniBoldLabel);
					desc.stringValue = EditorGUILayout.TextArea(
						desc.stringValue,
						GUILayout.MinHeight(40)
					);
				}

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		// ---------------- Each Reward Item ----------------
		private void DrawRewardItem(
			SerializedProperty item,
			SerializedProperty list,
			int index
		)
		{
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Item {index + 1}", EditorStyles.boldLabel);

			// Remove button
			Color oldColor = GUI.color;
			GUI.color = Color.red;
			if (GUILayout.Button("X", GUILayout.Width(25)))
			{
				// Safe removal: delete the item at index
				list.DeleteArrayElementAtIndex(index);

				// If the element became 'null' placeholder (Unity quirk), delete again to remove placeholder
				if (index < list.arraySize)
					list.DeleteArrayElementAtIndex(index);

				GUI.color = oldColor;
				return;
			}
			GUI.color = oldColor;

			EditorGUILayout.EndHorizontal();

			// Fields
			EditorGUILayout.PropertyField(item.FindPropertyRelative("type"));
			EditorGUILayout.PropertyField(item.FindPropertyRelative("quantity"));
			EditorGUILayout.PropertyField(item.FindPropertyRelative("icon"));

			EditorGUILayout.EndVertical();
		}
	}
}