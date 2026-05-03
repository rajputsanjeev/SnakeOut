using System.IO;
using UnityEngine;

namespace Framework
{
	public static class WatchAdsManager
	{
		private static string FILE_NAME = "powerup_data";
		public static PowerUpSaveModel Data = new PowerUpSaveModel();

		public static void Save()
		{
			SaveSystem.Save<PowerUpSaveModel>(Data, FILE_NAME);
		}

		public static void Load()
		{
			Data = SaveSystem.LoadOrCreate<PowerUpSaveModel>(FILE_NAME, new PowerUpSaveModel());
		}

		public static PowerUpData GetPowerUp(string name)
		{
			if (Data == null)
			{
				Load();
				return null;
			}

			return Data.allPowerUps.Find(x => x.powerUpName == name);
		}

		public static void SetPowerUp(PowerUpData newData)
		{
			var existing = GetPowerUp(newData.powerUpName);

			if (existing != null)
			{
				int index = Data.allPowerUps.IndexOf(existing);
				Data.allPowerUps[index] = newData;
				Save();
			}
			else
			{
				Debug.LogWarning($"PowerUp not found: {newData.powerUpName}");
			}
		}

		public static void RegisterPowerUp(PowerUpData data)
		{
			if (GetPowerUp(data.powerUpName) == null)
			{
				Data.allPowerUps.Add(data);
				Save();
			}
		}
	}
}