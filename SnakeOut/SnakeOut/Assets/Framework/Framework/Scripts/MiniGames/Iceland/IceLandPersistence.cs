using System;
using System.IO;
using Framework;
using UnityEngine;

public class IceLandPersistence
{
	private const string FILE_NAME = "IceLand_save";

	static string FolderPath =>
		Path.Combine(Application.persistentDataPath, "IceLand");

	static string FilePath =>
		Path.Combine(FolderPath, FILE_NAME);

	public static void Save(IcelandAdvantacher data)
	{
		SaveSystem.Save<IcelandAdvantacher>(data, FILE_NAME);
	}

	public static IcelandAdvantacher Load()
	{
		return SaveSystem.LoadOrCreate<IcelandAdvantacher>(FILE_NAME, new IcelandAdvantacher());
	}

	public static void Delete()
	{
		if (File.Exists(FilePath))
			File.Delete(FilePath);
	}

	public static void DeleteAll()
	{
		if (Directory.Exists(FolderPath))
			Directory.Delete(FolderPath, true);
	}

}
public class IcelandAdvantacher : MiniGameCoolDown
{
	public const int TotalStep = 5;
	public bool IsStarted;
	public bool IsComplete;
	public bool IsRewardCollected;
	public int CurrentStep;
	public int LastStep;
}