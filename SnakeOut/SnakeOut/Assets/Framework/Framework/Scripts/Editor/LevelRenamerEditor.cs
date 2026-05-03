using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class LevelRenamerEditor : EditorWindow
{
	private DefaultAsset targetFolder;
	private int startNumber = 0;
	private string prefix = "Level_";

	[MenuItem("Tools/Level Renamer")]
	public static void Open()
	{
		GetWindow<LevelRenamerEditor>("Level Renamer");
	}

	private void OnGUI()
	{
		GUILayout.Label("Level Renamer Tool", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
			"Target Folder",
			targetFolder,
			typeof(DefaultAsset),
			false
		);

		startNumber = EditorGUILayout.IntField("Start Number", startNumber);
		prefix = EditorGUILayout.TextField("Name Prefix", prefix);

		EditorGUILayout.Space();

		if (GUILayout.Button("Rename Levels"))
		{
			RenameLevels();
		}
	}

	private void RenameLevels()
	{
		if (targetFolder == null)
		{
			Debug.LogError("Please select a folder.");
			return;
		}

		string folderPath = AssetDatabase.GetAssetPath(targetFolder);

		if (!AssetDatabase.IsValidFolder(folderPath))
		{
			Debug.LogError("Selected asset is not a folder.");
			return;
		}

		// Get all assets inside folder (excluding meta files)
		var assetPaths = Directory.GetFiles(folderPath)
			.Where(p => !p.EndsWith(".meta"))
			.OrderBy(p => p)
			.ToList();

		if (assetPaths.Count == 0)
		{
			Debug.LogWarning("No assets found in folder.");
			return;
		}

		// Rename safely using temp names first
		for (int i = 0; i < assetPaths.Count; i++)
		{
			string tempPath = assetPaths[i];
			AssetDatabase.RenameAsset(
				tempPath,
				$"__temp__{i}"
			);
		}

		AssetDatabase.Refresh();

		assetPaths = Directory.GetFiles(folderPath)
			.Where(p => !p.EndsWith(".meta"))
			.OrderBy(p => p)
			.ToList();

		for (int i = 0; i < assetPaths.Count; i++)
		{
			int newIndex = startNumber + i;
			AssetDatabase.RenameAsset(
				assetPaths[i],
				$"{prefix}{newIndex}"
			);
		}

		AssetDatabase.Refresh();
		Debug.Log("Levels renamed successfully!");
	}
}
