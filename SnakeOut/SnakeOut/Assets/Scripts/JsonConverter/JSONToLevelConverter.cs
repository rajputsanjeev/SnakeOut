using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR

// ============= JSON ONLY EDITOR WINDOW =============
public class JSONLevelConverter : EditorWindow
{
	private List<Object> jsonFiles = new List<Object>();
	private string savePath = "Assets/ArrowOut/Levels/FromJSON";
	private int startingLevelNumber = 1;
	private string levelNamePrefix = "Level";
	private Vector2 scrollPos;

	private List<ConversionResult> results = new List<ConversionResult>();
	private bool showResults = false;

	[System.Serializable]
	private class ConversionResult
	{
		public string fileName;
		public bool success;
		public string message;
		public string jsonPreview;
		public LevelData levelData;
	}

	[MenuItem("Tools/Arrow Out/JSON Converter")]
	static void ShowWindow()
	{
		var window = GetWindow<JSONLevelConverter>("JSON Converter");
		window.minSize = new Vector2(550, 700);
		window.Show();
	}

	void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		GUILayout.Space(10);
		DrawHeader(); // ✅ SAME AS ORIGINAL
		GUILayout.Space(10);
		DrawInstructions();
		GUILayout.Space(10);
		DrawFileSelection();
		GUILayout.Space(10);
		DrawSettings();
		GUILayout.Space(10);
		DrawConvertButton();
		GUILayout.Space(10);

		if (showResults)
			DrawResults();

		EditorGUILayout.EndScrollView();
	}

	// ✅ SAME HEADER
	void DrawHeader()
	{
		GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
		{
			fontSize = 16,
			alignment = TextAnchor.MiddleCenter,
			normal = { textColor = new Color(0.9f, 0.5f, 0.2f) }
		};
		EditorGUILayout.LabelField("🎯 JSON Level Converter", headerStyle);

		GUIStyle subStyle = new GUIStyle(EditorStyles.label)
		{
			fontSize = 11,
			alignment = TextAnchor.MiddleCenter,
			normal = { textColor = Color.gray }
		};
		EditorGUILayout.LabelField("Reads JSON files directly (no YAML needed)", subStyle);
	}

	void DrawInstructions()
	{
		EditorGUILayout.HelpBox(
			"✅ JSON ONLY TOOL\n" +
			"Reads JSON files directly.\n\n" +
			"WHAT IT DOES:\n" +
			"• Reads .json file\n" +
			"• Parses arrow data\n" +
			"• Converts to LevelData\n\n" +
			"SUPPORTED:\n" +
			"✓ Direct JSON files",
			MessageType.Info
		);
	}

	void DrawFileSelection()
	{
		EditorGUILayout.LabelField("JSON Files to Convert", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+ Add File", GUILayout.Width(100)))
			jsonFiles.Add(null);

		if (GUILayout.Button("Clear All", GUILayout.Width(100)))
			jsonFiles.Clear();

		GUILayout.EndHorizontal();

		GUILayout.Space(5);

		EditorGUI.indentLevel++;
		for (int i = 0; i < jsonFiles.Count; i++)
		{
			GUILayout.BeginHorizontal();

			jsonFiles[i] = EditorGUILayout.ObjectField(
				$"File {i + 1}",
				jsonFiles[i],
				typeof(Object),
				false
			);

			if (GUILayout.Button("×", GUILayout.Width(25)))
			{
				jsonFiles.RemoveAt(i);
				i--;
			}

			GUILayout.EndHorizontal();
		}
		EditorGUI.indentLevel--;

		GUILayout.Space(5);

		// Drag & Drop
		Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
		GUI.Box(dropArea, "Drag & Drop JSON Files Here");

		Event evt = Event.current;
		if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
		{
			if (dropArea.Contains(evt.mousePosition))
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					foreach (Object obj in DragAndDrop.objectReferences)
					{
						string path = AssetDatabase.GetAssetPath(obj);
						if (path.EndsWith(".json"))
						{
							if (!jsonFiles.Contains(obj))
								jsonFiles.Add(obj);
						}
					}
				}
				evt.Use();
			}
		}
	}

	void DrawSettings()
	{
		EditorGUILayout.LabelField("Conversion Settings", EditorStyles.boldLabel);

		savePath = EditorGUILayout.TextField("Save Path", savePath);
		startingLevelNumber = EditorGUILayout.IntField("Starting Level Number", startingLevelNumber);
		levelNamePrefix = EditorGUILayout.TextField("Level Name Prefix", levelNamePrefix);
	}

	void DrawConvertButton()
	{
		GUI.enabled = jsonFiles.Count > 0;

		if (GUILayout.Button("🔄 Convert JSON Files", GUILayout.Height(40)))
		{
			ConvertFiles();
		}

		GUI.enabled = true;
	}

	void DrawResults()
	{
		EditorGUILayout.LabelField("Conversion Results", EditorStyles.boldLabel);

		foreach (var result in results)
		{
			GUILayout.BeginVertical(EditorStyles.helpBox);

			if (result.success)
			{
				EditorGUILayout.LabelField($"✓ {result.fileName}");
			}
			else
			{
				EditorGUILayout.LabelField($"✗ {result.fileName}");
				EditorGUILayout.LabelField($"Error: {result.message}");
			}

			GUILayout.EndVertical();
		}
	}

	void ConvertFiles()
	{
		results.Clear();

		if (!Directory.Exists(savePath))
			Directory.CreateDirectory(savePath);

		int levelNumber = startingLevelNumber;

		foreach (Object file in jsonFiles)
		{
			if (file == null) continue;

			string assetPath = AssetDatabase.GetAssetPath(file);

			ConversionResult result = new ConversionResult
			{
				fileName = file.name
			};

			try
			{
				// ✅ ONLY CHANGE FROM YOUR ORIGINAL
				string jsonString = File.ReadAllText(assetPath);

				PrefabFormatLevel prefabLevel =
					JsonUtility.FromJson<PrefabFormatLevel>(jsonString);

				if (prefabLevel == null || prefabLevel.Arrows == null)
				{
					result.success = false;
					result.message = "Invalid JSON";
					results.Add(result);
					continue;
				}

				LevelData level =
					ConvertToNewFormat(prefabLevel, levelNumber, file.name);

				string fileName = $"Level_{levelNumber:D3}_{file.name}.asset";
				AssetDatabase.CreateAsset(level, $"{savePath}/{fileName}");

				result.success = true;
				result.levelData = level;
				
				levelNumber++;
			}
			catch (System.Exception e)
			{
				result.success = false;
				result.message = e.Message;
			}

			results.Add(result);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		showResults = true;

		EditorUtility.DisplayDialog("Done", "JSON Conversion Complete!", "OK");
	}

	// ✅ SAME LOGIC (UNCHANGED)
	LevelData ConvertToNewFormat(PrefabFormatLevel prefabLevel, int levelNumber, string fileName)
	{
		LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();

		newLevel.levelNumber = levelNumber;
		newLevel.levelName = $"{levelNamePrefix} {levelNumber}";
		newLevel.width = prefabLevel.XSize;
		newLevel.height = prefabLevel.YSize;
		newLevel.Duration = 60f + (prefabLevel.Arrows.Count * 30f);

		newLevel.arrowPaths = new List<ArrowPath>();
		newLevel.holes = new List<Vector2Int>();

		foreach (var arrow in prefabLevel.Arrows)
		{
			if (arrow.Indices == null || arrow.Indices.Count == 0)
				continue;

			ArrowPath path = new ArrowPath();
			path.body = new List<Vector2Int>();
			path.color = ArrowColorPalette.GetRandomColor();

			foreach (int index in arrow.Indices)
			{
				int x = index % prefabLevel.XSize;
				int y = index / prefabLevel.XSize;
				path.body.Add(new Vector2Int(x, y));
			}

			newLevel.arrowPaths.Add(path);
		}

		return newLevel;
	}
}

#endif