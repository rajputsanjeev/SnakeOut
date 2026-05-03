using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class LevelDuplicatorEditor : EditorWindow
{
	private DefaultAsset targetFolder;

	private int duplicateFrom = 0;
	private int duplicateTo = 0;
	private string prefix = "Level_";

	[MenuItem("Tools/Level Duplicator")]
	public static void Open()
	{
		GetWindow<LevelDuplicatorEditor>("Level Duplicator");
	}

	private void OnGUI()
	{
		GUILayout.Label("Level Duplicator Tool", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
			"Target Folder",
			targetFolder,
			typeof(DefaultAsset),
			false
		);

		prefix = EditorGUILayout.TextField("Level Prefix", prefix);
		duplicateFrom = EditorGUILayout.IntField("Duplicate From", duplicateFrom);
		duplicateTo = EditorGUILayout.IntField("Duplicate To", duplicateTo);

		EditorGUILayout.Space();

		if (GUILayout.Button("Duplicate Levels"))
		{
			DuplicateLevels();
		}
	}

	private void DuplicateLevels()
	{
		if (targetFolder == null)
		{
			Debug.LogError("Please select a folder.");
			return;
		}

		if (duplicateFrom > duplicateTo)
		{
			Debug.LogError("Duplicate From must be <= Duplicate To.");
			return;
		}

		string folderPath = AssetDatabase.GetAssetPath(targetFolder);
		if (!AssetDatabase.IsValidFolder(folderPath))
		{
			Debug.LogError("Selected asset is not a valid folder.");
			return;
		}

		var assets = Directory.GetFiles(folderPath)
			.Where(p => !p.EndsWith(".meta"))
			.ToList();

		if (assets.Count == 0)
		{
			Debug.LogError("No assets found in folder.");
			return;
		}

		// Regex to extract number from Level_XXX
		Regex regex = new Regex($"{prefix}(\\d+)$");

		Dictionary<int, string> levelMap = new Dictionary<int, string>();
		int maxLevelNumber = -1;

		foreach (var path in assets)
		{
			string name = Path.GetFileNameWithoutExtension(path);
			Match match = regex.Match(name);

			if (!match.Success)
				continue;

			int levelNumber = int.Parse(match.Groups[1].Value);
			levelMap[levelNumber] = path;

			if (levelNumber > maxLevelNumber)
				maxLevelNumber = levelNumber;
		}

		if (maxLevelNumber < 0)
		{
			Debug.LogError("No valid level files found with given prefix.");
			return;
		}

		int newLevelIndex = maxLevelNumber + 1;

		for (int i = duplicateFrom; i <= duplicateTo; i++)
		{
			if (!levelMap.ContainsKey(i))
			{
				Debug.LogWarning($"Level {i} not found. Skipping.");
				continue;
			}

			string sourcePath = levelMap[i];
			string extension = Path.GetExtension(sourcePath);
			string newName = $"{prefix}{newLevelIndex}{extension}";
			string newPath = Path.Combine(folderPath, newName);

			AssetDatabase.CopyAsset(sourcePath, newPath);
			Debug.Log($"Duplicated {prefix}{i} → {prefix}{newLevelIndex}");

			newLevelIndex++;
		}

		AssetDatabase.Refresh();
		Debug.Log("Level duplication completed successfully!");
	}
}
