using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ColorBlockJam
{
	public class UsingAdderWindow : EditorWindow
	{
		private DefaultAsset targetFolder;
		private string usingToAdd = "using Core;";

		[MenuItem("Tools/Add Using Statements")]
		public static void ShowWindow()
		{
			GetWindow<UsingAdderWindow>("Add Using");
		}

		private void OnGUI()
		{
			GUILayout.Label("Add Using Statement to All Scripts", EditorStyles.boldLabel);

			targetFolder = EditorGUILayout.ObjectField("Folder", targetFolder, typeof(DefaultAsset), false) as DefaultAsset;

			usingToAdd = EditorGUILayout.TextField("Using to Add", usingToAdd);

			GUI.enabled = targetFolder != null && usingToAdd.StartsWith("using ") && usingToAdd.EndsWith(";");

			if (GUILayout.Button("Apply"))
			{
				string folderPath = AssetDatabase.GetAssetPath(targetFolder);
				ProcessFolder(folderPath);
				AssetDatabase.Refresh();
				Debug.Log("✔ Using statement added to scripts.");
			}

			GUI.enabled = true;
		}

		private void ProcessFolder(string folderPath)
		{
			string absolutePath = Application.dataPath.Replace("Assets", "") + folderPath;

			string[] csFiles = Directory.GetFiles(absolutePath, "*.cs", SearchOption.AllDirectories);

			foreach (string file in csFiles)
			{
				AddUsingToScript(file);
			}
		}

		private void AddUsingToScript(string filePath)
		{
			string text = File.ReadAllText(filePath);

			// Check if the using already exists → skip
			if (Regex.IsMatch(text, @"^\s*" + Regex.Escape(usingToAdd) + @"\s*$", RegexOptions.Multiline))
				return;

			// Find first non-using line to insert new using above it
			string[] lines = File.ReadAllLines(filePath);
			List<string> newLines = new List<string>();

			bool inserted = false;

			foreach (string line in lines)
			{
				if (!inserted && !line.Trim().StartsWith("using "))
				{
					newLines.Add(usingToAdd);
					newLines.Add(""); // spacing
					inserted = true;
				}

				newLines.Add(line);
			}

			File.WriteAllLines(filePath, newLines);
		}
	}
}
