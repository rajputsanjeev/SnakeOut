#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Framework
{
	public static class TicTacSaveEditor
	{
		private const string FILE_NAME = "tictac_save.json";

		static string FolderPath =>
			Path.Combine(Application.persistentDataPath, "tictac");

		static string FilePath =>
			Path.Combine(FolderPath, FILE_NAME);

		// ---------------- OPEN FOLDER ----------------
		[MenuItem("Tools/Data/Mini/TicTac/Open Save Folder")]
		public static void OpenSaveFolder()
		{
			if (!Directory.Exists(FolderPath))
			{
				Debug.LogWarning("[TicTac] Save folder does not exist.");
				return;
			}

			EditorUtility.RevealInFinder(FolderPath);
		}

		// ---------------- OPEN FILE ----------------
		[MenuItem("Tools/Data/Mini/TicTac/Open Save File")]
		public static void OpenSaveFile()
		{
			if (!File.Exists(FilePath))
			{
				Debug.LogWarning("[TicTac] Save file does not exist.");
				return;
			}

			EditorUtility.OpenWithDefaultApp(FilePath);
		}

		// ---------------- DELETE FILE ----------------
		[MenuItem("Tools/Data/Mini/TicTac/Delete Save File")]
		public static void DeleteSaveFile()
		{
			if (!File.Exists(FilePath))
			{
				Debug.LogWarning("[TicTac] No save file to delete.");
				return;
			}

			if (EditorUtility.DisplayDialog(
				"Delete TicTac Save",
				"Are you sure you want to delete the TicTac save file?",
				"Delete",
				"Cancel"))
			{
				File.Delete(FilePath);
				Debug.Log("[TicTac] Save file deleted.");
			}
		}

		// ---------------- DELETE ALL ----------------
		[MenuItem("Tools/Data/Mini/TicTac/Delete All TicTac Saves")]
		public static void DeleteAllSaves()
		{
			if (!Directory.Exists(FolderPath))
			{
				Debug.LogWarning("[TicTac] No save folder exists.");
				return;
			}

			if (EditorUtility.DisplayDialog(
				"Delete ALL TicTac Saves",
				"This will delete the entire TicTac save folder.\n\nAre you sure?",
				"Delete All",
				"Cancel"))
			{
				Directory.Delete(FolderPath, true);
				Debug.Log("[TicTac] All TicTac save data deleted.");
			}
		}
	}
}
#endif
