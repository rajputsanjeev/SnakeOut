#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Framework
{
	public static class IceLandSaveEditor
	{
		private const string FILE_NAME = "IceLand_save.json";

		static string FolderPath =>
			Path.Combine(Application.persistentDataPath);

		static string FilePath =>
			Path.Combine(FolderPath, FILE_NAME);

		// ---------------- OPEN FOLDER ----------------
		[MenuItem("Tools/Data/Mini/IceLand/Open Save Folder")]
		public static void OpenSaveFolder()
		{
			if (!Directory.Exists(FolderPath))
			{
				Debug.LogWarning("[IceLand] Save folder does not exist.");
				return;
			}

			EditorUtility.RevealInFinder(FolderPath);
		}

		// ---------------- OPEN FILE ----------------
		[MenuItem("Tools/Data/Mini/IceLand/Open Save File")]
		public static void OpenSaveFile()
		{
			if (!File.Exists(FilePath))
			{
				Debug.LogWarning("[IceLand] Save file does not exist.");
				return;
			}

			EditorUtility.OpenWithDefaultApp(FilePath);
		}

		// ---------------- DELETE FILE ----------------
		[MenuItem("Tools/Data/Mini/IceLand/Delete Save File")]
		public static void DeleteSaveFile()
		{
			if (!File.Exists(FilePath))
			{
				Debug.LogWarning("[IceLand] No save file to delete.");
				return;
			}

			if (EditorUtility.DisplayDialog(
				"Delete IceLand Save",
				"Are you sure you want to delete the IceLand save file?",
				"Delete",
				"Cancel"))
			{
				File.Delete(FilePath);
				Debug.Log("[IceLand] Save file deleted.");
			}
		}

		// ---------------- DELETE ALL ----------------
		[MenuItem("Tools/Data/Mini/IceLand/Delete All IceLand Saves")]
		public static void DeleteAllSaves()
		{
			if (!Directory.Exists(FolderPath))
			{
				Debug.LogWarning("[IceLand] No save folder exists.");
				return;
			}

			if (EditorUtility.DisplayDialog(
				"Delete ALL IceLand Saves",
				"This will delete the entire IceLand save folder.\n\nAre you sure?",
				"Delete All",
				"Cancel"))
			{
				Directory.Delete(FolderPath, true);
				Debug.Log("[IceLand] All TicTac save data deleted.");
			}
		}
	}
}
#endif
