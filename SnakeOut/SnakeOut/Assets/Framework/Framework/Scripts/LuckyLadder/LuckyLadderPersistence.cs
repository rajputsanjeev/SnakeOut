using UnityEngine;
using System.IO;

namespace Framework
{
	public static class LuckyLadderPersistence
	{
		private static string FileName => Path.Combine(Application.persistentDataPath, "lucky_ladder_save.json");

		public static void Save(LuckyLadderSaveData data)
		{
			try
			{
				string json = JsonUtility.ToJson(data);
				File.WriteAllText(FileName, json);
			}
			catch (System.Exception e)
			{
				Debug.LogError("LuckyLadder save failed: " + e);
			}
		}

		public static LuckyLadderSaveData Load()
		{
			try
			{
				if (!File.Exists(FileName)) return new LuckyLadderSaveData { collected = new bool[6] };


				string json = File.ReadAllText(FileName);
				Debug.Log("FileName Lucky Ladder " + FileName);
				var d = JsonUtility.FromJson<LuckyLadderSaveData>(json);
				if (d.collected == null || d.collected.Length != 6) d.collected = new bool[6];
				return d;
			}
			catch (System.Exception e)
			{
				Debug.LogError("LuckyLadder load failed: " + e);
				return new LuckyLadderSaveData { collected = new bool[6] };
			}
		}
	}
}