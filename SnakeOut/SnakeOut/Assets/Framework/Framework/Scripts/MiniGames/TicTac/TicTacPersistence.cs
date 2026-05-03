using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;
using System.IO;

namespace Framework
{
	public static class TicTacPersistence
	{
		private const string FILE_NAME = "tictac_save.json";

		static string FolderPath =>
			Path.Combine(Application.persistentDataPath, "tictac");

		static string FilePath =>
			Path.Combine(FolderPath, FILE_NAME);

		// ---------------- SAVE ----------------
		public static void Save(TicTacSaveData data)
		{
			try
			{
				if (!Directory.Exists(FolderPath))
					Directory.CreateDirectory(FolderPath);

				string json = JsonUtility.ToJson(data, true);
				File.WriteAllText(FilePath, json);
			}
			catch (Exception e)
			{
				Debug.LogError($"[TicTacSave] Save failed: {e}");
			}
		}

		// ---------------- LOAD ----------------
		public static TicTacSaveData Load()
		{
			try
			{
				if (!File.Exists(FilePath)) return new TicTacSaveData();

				string json = File.ReadAllText(FilePath);
				return JsonUtility.FromJson<TicTacSaveData>(json);
			}
			catch (Exception e)
			{
				Debug.LogError($"[TicTacSave] Load failed: {e}");
				return null;
			}
		}

		// ---------------- DELETE (OPTIONAL) ----------------
		public static void Delete()
		{
			if (File.Exists(FilePath))
				File.Delete(FilePath);
		}

		// ---------------- DEBUG ----------------
		public static void DeleteAll()
		{
			if (Directory.Exists(FolderPath))
				Directory.Delete(FolderPath, true);
		}
	}


	[Serializable]
	public class CellRevealData
	{
		public bool revealed;     // Was this cell revealed?
		public bool hasFruit;     // Does this cell contain a fruit?
		public int fruitIndex;    // Index of fruit in possibleFruits list
		public int groupIndex;   // which group of that fruit (0..requiredReveals-1)
	}

	[Serializable]
	public class TicTacSaveData : MiniGameCoolDown
	{
		public int CurrentChest;
		public bool[] CurrentChestCollected;
		public CellRevealData[] Cells;
		public bool IsComplete;
		public List<TicTacFruitRevelData> TicTacFruitRevels = new List<TicTacFruitRevelData>();
		public int SavedQuestionMark;
		public int QuestionRewardVideo;
	}

	[Serializable]
	public class TicTacFruitRevelData
	{
		public int CurrentRevel;
	}

	[Serializable]
	public class MiniGameCoolDown : FeatureUnlocked
	{
		public string cooldownEndUtc = "";
		public string lastResetUtc = "";
	}
}