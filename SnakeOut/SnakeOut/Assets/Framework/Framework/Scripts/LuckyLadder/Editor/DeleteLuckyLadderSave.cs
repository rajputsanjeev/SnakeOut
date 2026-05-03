using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
	public static class DeleteLuckyLadderSave
	{
		private const string SaveFileName = "lucky_ladder_save.json";

		[MenuItem("Tools/Data/Lucky Ladder/Delete Saved JSON")]
		public static void DeleteSaveFile()
		{
			string path = Path.Combine(Application.persistentDataPath, SaveFileName);

			if (File.Exists(path))
			{
				File.Delete(path);
				Debug.Log($"Lucky Ladder save deleted:\n{path}");
				EditorUtility.DisplayDialog("Lucky Ladder", "Saved JSON deleted successfully!", "OK");
			}
			else
			{
				Debug.LogWarning($"Lucky Ladder save not found:\n{path}");
				EditorUtility.DisplayDialog("Lucky Ladder", "No save file found!", "OK");
			}
		}

		// Optional: add a menu to open the save folder
		[MenuItem("Tools/Data/Lucky Ladder/Open Save Folder")]
		public static void OpenSaveFolder()
		{
			string folder = Application.persistentDataPath;
			EditorUtility.RevealInFinder(folder);
		}
	}
}