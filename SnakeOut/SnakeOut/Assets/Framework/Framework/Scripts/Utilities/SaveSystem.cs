using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
	public static class SaveSystem
	{
		private static readonly string RootPath = Application.persistentDataPath;

		// ------------------------------
		// File Exists Check
		// ------------------------------
		public static bool Exists(string fileName)
		{
			string fullPath = Path.Combine(RootPath, fileName + ".json");
			return File.Exists(fullPath);
		}

		// ------------------------------
		// Save (Sync)
		// ------------------------------
		public static void Save<T>(T data, string fileName, bool prettyPrint = true)
		{
			string fullPath = Path.Combine(RootPath, fileName + ".json");
			string json = JsonUtility.ToJson(data, prettyPrint);

			File.WriteAllText(fullPath, json);
			Debug.Log($"[SaveSystem] Saved: {fullPath}");
		}

		// ------------------------------
		// Load (Sync) + Auto Create
		// ------------------------------
		public static T LoadOrCreate<T>(string fileName, T defaultData, bool prettyPrint = true)
		{
			string fullPath = Path.Combine(RootPath, fileName + ".json");

			if (!File.Exists(fullPath))
			{
				// Create default file
				Save(defaultData, fileName, prettyPrint);

				Debug.Log($"[SaveSystem] File missing. Auto-created default file: {fileName}.json");
				return defaultData;
			}

			string json = File.ReadAllText(fullPath);
			return JsonUtility.FromJson<T>(json);
		}

		// ------------------------------
		// Delete File
		// ------------------------------
		public static void Delete(string fileName)
		{
			string fullPath = Path.Combine(RootPath, fileName + ".json");

			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
				Debug.Log($"[SaveSystem] Deleted: {fullPath}");
			}
		}

		// =====================================================
		//                     ASYNC METHODS
		// =====================================================

		public static async Task SaveAsync<T>(T data, string fileName, bool prettyPrint = true)
		{
			string fullPath = Path.Combine(RootPath, fileName + ".json");
			string json = JsonUtility.ToJson(data, prettyPrint);

			await Task.Run(() => File.WriteAllText(fullPath, json));
			Debug.Log($"[SaveSystem] Saved Async: {fullPath}");
		}

		public static async Task<T> LoadOrCreateAsync<T>(string fileName, T defaultData, bool prettyPrint = true)
		{
			string fullPath = Path.Combine(RootPath, fileName + ".json");

			if (!File.Exists(fullPath))
			{
				// Auto-create default file async
				await SaveAsync(defaultData, fileName, prettyPrint);

				Debug.Log($"[SaveSystem] File missing. Auto-created default file (Async): {fileName}.json");
				return defaultData;
			}

			string json = await Task.Run(() => File.ReadAllText(fullPath));
			return JsonUtility.FromJson<T>(json);
		}
	}
}