using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
	[CustomEditor(typeof(DailyRewardSet))]
	public class DailyRewardEditor : Editor
	{
		private SerializedProperty stepsProp;
		private SerializedProperty finalRewardProp;
		private bool showSteps = true; // Top foldout

		private const int ICON_SIZE = 60;
		private bool[] stepFoldouts;   // Per-step foldouts
		private List<Sprite> cachedSprites = new();
		private DefaultAsset[] spriteFolders;

		private void OnEnable()
		{
			stepsProp = serializedObject.FindProperty("steps");
			finalRewardProp = serializedObject.FindProperty("FinalRewardSet");

			if (stepsProp != null)
				stepFoldouts = new bool[stepsProp.arraySize];
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();

			// --------------------------
			// TOP FOLDOUT (NOT header group)
			// --------------------------

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.PropertyField(finalRewardProp);
			var spriteProp = finalRewardProp.FindPropertyRelative("icon");
			EditorGUILayout.EndVertical();

			showSteps = EditorGUILayout.Foldout(showSteps, "Reward Steps", true);

			if (showSteps)
			{
				EditorGUILayout.BeginVertical("HelpBox");

				DrawStepsList();

				EditorGUILayout.EndVertical();
			}

			DrawSpriteFolderSelector();   // folder selection
			DrawSpritePalette();         // sprite grid

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawStepsList()
		{
			// Ensure foldout array matches size
			if (stepFoldouts == null || stepFoldouts.Length != stepsProp.arraySize)
				stepFoldouts = new bool[stepsProp.arraySize];

			for (int i = 0; i < stepsProp.arraySize; i++)
			{
				SerializedProperty stepProp = stepsProp.GetArrayElementAtIndex(i);

				stepFoldouts[i] = EditorGUILayout.Foldout(stepFoldouts[i], "Reward Step " + (i + 1), true);

				if (stepFoldouts[i])
				{
					EditorGUILayout.BeginVertical("box");

					SerializedProperty itemsProp = stepProp.FindPropertyRelative("items");

					// ITEMS
					for (int r = 0; r < itemsProp.arraySize; r++)
					{
						SerializedProperty itemProp = itemsProp.GetArrayElementAtIndex(r);

						EditorGUILayout.LabelField("Reward " + (r + 1), EditorStyles.boldLabel);

						var unitProp = itemProp.FindPropertyRelative("type");
						var rewardQtProp = itemProp.FindPropertyRelative("quantity");
						var spriteProp = itemProp.FindPropertyRelative("icon");
						spriteProp.objectReferenceValue = EditorGUILayout.ObjectField("Sprite", spriteProp.objectReferenceValue, typeof(Sprite), false);

						EditorGUILayout.PropertyField(unitProp);
						EditorGUILayout.PropertyField(rewardQtProp);
						EditorGUILayout.PropertyField(spriteProp);

						if (GUILayout.Button("Remove reward"))
						{
							itemsProp.DeleteArrayElementAtIndex(r);
						}

						GUILayout.Space(6);
					}

					if (GUILayout.Button("Add reward"))
					{
						itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);
					}

					EditorGUILayout.Space();

					if (GUILayout.Button("Remove reward Step"))
					{
						stepsProp.DeleteArrayElementAtIndex(i);
						return;
					}

					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.Space();
			}

			if (GUILayout.Button("Add reward Step"))
			{
				stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
			}
		}

		private void CacheSprites()
		{
			cachedSprites.Clear();

			if (spriteFolders == null) return;

			foreach (var folder in spriteFolders)
			{
				if (folder == null) continue;

				string path = AssetDatabase.GetAssetPath(folder);
				if (!AssetDatabase.IsValidFolder(path)) continue;

				foreach (string guid in AssetDatabase.FindAssets("t:Sprite", new[] { path }))
				{
					var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
					if (sprite != null)
						cachedSprites.Add(sprite);
				}
			}
		}

		private void DrawSpritePalette()
		{
			if (cachedSprites.Count == 0)
				return;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Sprite Palette", EditorStyles.boldLabel);

			int columns = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / (ICON_SIZE + 10));
			columns = Mathf.Max(1, columns);

			int index = 0;

			while (index < cachedSprites.Count)
			{
				EditorGUILayout.BeginHorizontal();

				for (int c = 0; c < columns && index < cachedSprites.Count; c++, index++)
				{
					DrawDraggableSprite(cachedSprites[index]);
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawDraggableSprite(Sprite sprite)
		{
			Rect rect = GUILayoutUtility.GetRect(ICON_SIZE, ICON_SIZE, GUILayout.ExpandWidth(false));

			GUI.Box(rect, GUIContent.none);

			if (sprite.texture != null)
			{
				GUI.DrawTexture(rect, sprite.texture, ScaleMode.ScaleToFit);
			}

			Event evt = Event.current;

			if (evt.type == EventType.MouseDrag && rect.Contains(evt.mousePosition))
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[] { sprite };
				DragAndDrop.StartDrag(sprite.name);
				evt.Use();
			}
		}

		private void DrawSpriteFolderSelector()
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.LabelField("Sprite Source Folders", EditorStyles.boldLabel);

			int newCount = Mathf.Max(
				0,
				EditorGUILayout.IntField(
					"Folder Count",
					spriteFolders == null ? 0 : spriteFolders.Length
				)
			);

			if (spriteFolders == null || spriteFolders.Length != newCount)
			{
				spriteFolders = new DefaultAsset[newCount];
			}

			for (int i = 0; i < spriteFolders.Length; i++)
			{
				spriteFolders[i] = (DefaultAsset)EditorGUILayout.ObjectField(
					$"Folder {i + 1}",
					spriteFolders[i],
					typeof(DefaultAsset),
					false
				);
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Refresh Sprite Palette"))
			{
				CacheSprites();
			}

			EditorGUILayout.EndVertical();
		}
	}
}
