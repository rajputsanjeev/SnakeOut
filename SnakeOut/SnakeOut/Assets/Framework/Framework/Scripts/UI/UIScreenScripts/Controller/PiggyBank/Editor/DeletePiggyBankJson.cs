using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
	public class DeletePiggyBankJson
	{
		private static string SavePath => Application.persistentDataPath + "/piggybank.json";

		[MenuItem("Tools/Data/Piggy Bank/Delete Save File")]
		private static void DeletePiggyBankFile()
		{
			if (File.Exists(SavePath))
			{
				File.Delete(SavePath);
				Debug.Log($"Piggy Bank save deleted: {SavePath}");

				EditorUtility.DisplayDialog(
					"Piggy Bank Save Deleted",
					"The piggybank.json file has been successfully deleted.",
					"OK"
				);
			}
			else
			{
				EditorUtility.DisplayDialog(
					"File Not Found",
					"No piggybank.json file exists in persistentDataPath.",
					"OK"
				);
			}
		}

		[MenuItem("Tools/Data/Piggy Bank/Open Save Location")]
		private static void OpenSaveLocation()
		{
			EditorUtility.RevealInFinder(Application.persistentDataPath);
		}
	}
}