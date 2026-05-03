using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Framework.EditorFramework
{
	public abstract class RewardEditorBase<T> : EditorFrameworkBase<T>
		where T : ScriptableObject
	{
		private SerializedProperty spriteFoldersProp;
		private List<Sprite> cachedSprites = new();

		// ---------------- BINDING ----------------

		protected void BindSpriteSourceFolders(string propertyName)
		{
			spriteFoldersProp = SO.FindProperty(propertyName);

			if (spriteFoldersProp == null)
			{
				Debug.LogError(
					$"[RewardEditorBase] Property '{propertyName}' not found on {SO.targetObject.name}"
				);
				return;
			}

			RefreshSprites();
		}

		// ---------------- UI ----------------

		protected void DrawSpriteSource()
		{
			if (spriteFoldersProp == null)
			{
				EditorGUILayout.HelpBox(
					"spriteSourceFolders not found on target ScriptableObject.",
					MessageType.Error
				);
				return;
			}

			Box("Sprite Source Folders", () =>
			{
				EditorGUILayout.PropertyField(spriteFoldersProp, true);

				if (GUILayout.Button("Refresh Sprite Palette"))
					RefreshSprites();
			});

			DrawSpritePalette();
		}

		protected void DrawSpriteDrop(SerializedProperty spriteProp)
		{
			Rect r = GUILayoutUtility.GetRect(64, 64);
			GUI.Box(r, GUIContent.none);

			if (spriteProp.objectReferenceValue is Sprite s && s.texture)
				GUI.DrawTexture(r, s.texture, ScaleMode.ScaleToFit);

			HandleDrop(r, spriteProp);
		}

		// ---------------- INTERNAL ----------------

		private void DrawSpritePalette()
		{
			if (cachedSprites.Count == 0)
				return;

			Box("Sprite Palette", () =>
			{
				int size = 56;
				int cols = Mathf.Max(1,
					Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / (size + 8)));

				for (int i = 0; i < cachedSprites.Count;)
				{
					EditorGUILayout.BeginHorizontal();
					for (int c = 0; c < cols && i < cachedSprites.Count; c++, i++)
						DrawDraggableSprite(cachedSprites[i], size);
					EditorGUILayout.EndHorizontal();
				}
			});
		}

		private void DrawDraggableSprite(Sprite sprite, int size)
		{
			Rect r = GUILayoutUtility.GetRect(size, size, GUILayout.ExpandWidth(false));
			GUI.DrawTexture(r, sprite.texture, ScaleMode.ScaleToFit);

			if (Event.current.type == EventType.MouseDrag &&
				r.Contains(Event.current.mousePosition))
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[] { sprite };
				DragAndDrop.StartDrag(sprite.name);
				Event.current.Use();
			}
		}

		private void HandleDrop(Rect r, SerializedProperty prop)
		{
			Event e = Event.current;
			if (!r.Contains(e.mousePosition)) return;

			if ((e.type == EventType.DragUpdated || e.type == EventType.DragPerform) &&
				DragAndDrop.objectReferences.Length > 0 &&
				DragAndDrop.objectReferences[0] is Sprite)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (e.type == EventType.DragPerform)
				{
					prop.objectReferenceValue = DragAndDrop.objectReferences[0];
					DragAndDrop.AcceptDrag();
				}
				e.Use();
			}
		}

		private void RefreshSprites()
		{
			cachedSprites.Clear();

			for (int i = 0; i < spriteFoldersProp.arraySize; i++)
			{
				var folder =
					spriteFoldersProp.GetArrayElementAtIndex(i).objectReferenceValue as DefaultAsset;

				if (folder == null) continue;

				string path = AssetDatabase.GetAssetPath(folder);
				if (!AssetDatabase.IsValidFolder(path)) continue;

				foreach (string guid in AssetDatabase.FindAssets("t:Sprite", new[] { path }))
				{
					var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
						AssetDatabase.GUIDToAssetPath(guid));

					if (sprite != null)
						cachedSprites.Add(sprite);
				}
			}
		}
	}

	public abstract class EditorFrameworkBase<T> : Editor where T : ScriptableObject
	{
		protected T Target;
		protected SerializedObject SO;

		protected virtual void OnEnable()
		{
			Target = (T)target;
			SO = serializedObject;
			OnEditorReady();
		}

		public override void OnInspectorGUI()
		{
			SO.Update();
			DrawEditor();
			SO.ApplyModifiedProperties();
		}

		protected virtual void OnEditorReady() { }
		protected abstract void DrawEditor();

		protected void Box(string title, System.Action draw)
		{
			EditorGUILayout.BeginVertical("box");
			if (!string.IsNullOrEmpty(title))
				EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

			draw?.Invoke();
			EditorGUILayout.EndVertical();
		}
	}
}
