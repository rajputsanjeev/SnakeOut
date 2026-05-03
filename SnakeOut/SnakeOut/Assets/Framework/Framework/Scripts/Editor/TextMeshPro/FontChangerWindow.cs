using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

namespace Framework
{
	public class FontChangerWindow : EditorWindow
	{
		private TMP_FontAsset selectedFont;
		private Vector2 scroll;
		private List<TMP_FontAsset> allFonts = new List<TMP_FontAsset>();

		// Restored: font style option
		private FontStyles newFontStyle = FontStyles.Normal;

		[MenuItem("Tools/Font Changer")]
		public static void ShowWindow()
		{
			GetWindow<FontChangerWindow>("Font Changer");
		}

		private void OnEnable()
		{
			LoadAllFonts();
			// Refresh when project changes (so new fonts appear)
			EditorApplication.projectChanged += LoadAllFonts;
		}

		private void OnDisable()
		{
			EditorApplication.projectChanged -= LoadAllFonts;
		}

		void LoadAllFonts()
		{
			allFonts.Clear();

			string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
			foreach (string id in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(id);
				TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
				if (font != null)
					allFonts.Add(font);
			}

			Repaint();
		}

		private void OnGUI()
		{
			GUILayout.Label("Select Font From Project", EditorStyles.boldLabel);

			// Font list
			scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(260));
			foreach (var font in allFonts)
			{
				EditorGUILayout.BeginHorizontal("box");

				// Left: select font by clicking its name
				if (GUILayout.Button(font.name, GUILayout.Height(22)))
				{
					selectedFont = font;
				}

				// Right: ping/reveal asset in Project window
				if (GUILayout.Button("@", GUILayout.Width(28), GUILayout.Height(22)))
				{
					EditorGUIUtility.PingObject(font);
					Selection.activeObject = font;
				}

				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();

			if (selectedFont == null)
			{
				EditorGUILayout.HelpBox("Select a font from the list above.", MessageType.Info);
				return;
			}

			// Selected font display
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.LabelField("Selected Font:", EditorStyles.boldLabel);
			EditorGUILayout.ObjectField(selectedFont, typeof(TMP_FontAsset), false);
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();

			// Restored: Font Style picker (EnumFlagsField allows combined styles)
			newFontStyle = (FontStyles)EditorGUILayout.EnumFlagsField("Font Style", newFontStyle);

			EditorGUILayout.Space();

			// Action buttons
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Apply To Selected"))
				ChangeSelectedOnly();

			if (GUILayout.Button("Apply To Selected + Children"))
				ChangeSelectedAndChildren();
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Apply To Entire Scene"))
				ChangeFontInScene();
		}

		private void ChangeSelectedOnly()
		{
			if (Selection.activeGameObject == null)
			{
				Debug.LogWarning("No GameObject selected!");
				return;
			}

			var tmp = Selection.activeGameObject.GetComponent<TMP_Text>();
			if (tmp == null)
			{
				Debug.LogWarning("Selected GameObject has no TMP_Text component.");
				return;
			}

			Undo.RecordObject(tmp, "Change TMP Font/Style");
			tmp.font = selectedFont;
			tmp.fontStyle = newFontStyle;
			EditorUtility.SetDirty(tmp);

			Debug.Log($"Font changed for selected object: {tmp.gameObject.name}");
		}

		private void ChangeSelectedAndChildren()
		{
			if (Selection.activeGameObject == null)
			{
				Debug.LogWarning("No GameObject selected!");
				return;
			}

			TMP_Text[] texts = Selection.activeGameObject.GetComponentsInChildren<TMP_Text>(true);
			if (texts == null || texts.Length == 0)
			{
				Debug.LogWarning("No TMP_Text components found on selected object or its children.");
				return;
			}

			Undo.RecordObjects(texts, "Change TMP Font/Style");
			int count = 0;
			foreach (TMP_Text t in texts)
			{
				if (t == null) continue;
				t.font = selectedFont;
				t.fontStyle = newFontStyle;
				EditorUtility.SetDirty(t);
				count++;
			}

			Debug.Log($"Font changed for selected object & children ({count} texts).");
		}

		private void ChangeFontInScene()
		{
			TMP_Text[] allTexts = FindObjectsOfType<TMP_Text>(true);
			if (allTexts == null || allTexts.Length == 0)
			{
				Debug.LogWarning("No TMP_Text components found in the scene.");
				return;
			}

			Undo.RecordObjects(allTexts, "Change TMP Font/Style");
			int count = 0;
			foreach (TMP_Text t in allTexts)
			{
				if (t == null) continue;
				t.font = selectedFont;
				t.fontStyle = newFontStyle;
				EditorUtility.SetDirty(t);
				count++;
			}

			Debug.Log($"Font changed for entire scene ({count} texts).");
		}
	}
}
