using UnityEngine;
using System.IO;

namespace Framework
{
	public static class PrivilegePersistence
	{
		private const string FileName = "privilege_save.json";
		public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

		public static void Save(PrivilegeSaveData data)
		{
			try
			{
				string json = JsonUtility.ToJson(data);
				File.WriteAllText(FilePath, json);
			}
			catch (System.Exception e)
			{
				Debug.LogError("Privilege save failed: " + e);
			}
		}

		public static PrivilegeSaveData Load()
		{
			try
			{
				if (!File.Exists(FilePath))
				{
					var d = NewDefault();
					Save(d);
					return d;
				}
				var json = File.ReadAllText(FilePath);
				var savedJson = JsonUtility.FromJson<PrivilegeSaveData>(json);
				if (savedJson.FreeCollectedFlag == null || savedJson.FreeCollectedFlag.Length != 28)
				{
					savedJson.FreeCollectedFlag = new bool[28];
					savedJson.PaidCollectedFlag = new bool[28];
				}
				return savedJson;
			}
			catch (System.Exception e)
			{
				Debug.LogError("Privilege load failed: " + e);
				return NewDefault();
			}
		}

		public static void DeleteSave()
		{
			if (File.Exists(FilePath)) File.Delete(FilePath);
		}

		private static PrivilegeSaveData NewDefault()
		{
			return new PrivilegeSaveData
			{
				SelectedCycleIndex = 0,
				CycleStartUtc = System.DateTime.UtcNow.ToString("o"),
				TotalAvaliableKeys = 0,
				FreeCollectedFlag = new bool[28],
				PaidCollectedFlag = new bool[28],
				PaidActive = false
			};
		}
	}
}
