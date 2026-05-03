using UnityEngine;
using System.IO;

namespace Framework
{
	public static class PiggyBankPersistence
	{
		private static string FileName => Application.persistentDataPath + "/piggybank.json";

		public static void Save(PiggyBankSaveData data)
		{
			try
			{
				string json = JsonUtility.ToJson(data);
				File.WriteAllText(FileName, json);
			}
			catch (System.Exception e)
			{
				Debug.LogError("Piggybank save failed: " + e);
			}
		}

		public static PiggyBankSaveData Load()
		{
			if (!File.Exists(FileName))
				return new PiggyBankSaveData();

			return JsonUtility.FromJson<PiggyBankSaveData>(File.ReadAllText(FileName));
		}

		public static void DeleteSave()
		{
			if (File.Exists(FileName))
				File.Delete(FileName);
		}
	}
}
