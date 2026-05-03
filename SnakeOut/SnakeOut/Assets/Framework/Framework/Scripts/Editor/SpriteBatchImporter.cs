using UnityEditor;
using UnityEngine;
using System.IO;

public class SpriteBatchImporter : EditorWindow
{
	private DefaultAsset targetFolder;

	private int maxSize = 1024;
	private TextureImporterCompression compression = TextureImporterCompression.Compressed;
	private bool useCrunch = false;
	private int crunchQuality = 50;
	private bool generateMipMaps = false;

	private enum Platform { Default, Android, iOS }
	private Platform platform = Platform.Default;

	[MenuItem("Tools/Sprite Batch Importer")]
	public static void Open()
	{
		GetWindow<SpriteBatchImporter>("Sprite Batch Importer");
	}

	private void OnGUI()
	{
		GUILayout.Label("Batch Sprite Import Settings", EditorStyles.boldLabel);

		targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
			"Target Folder", targetFolder, typeof(DefaultAsset), false);

		GUILayout.Space(10);

		maxSize = EditorGUILayout.IntPopup(
			"Max Size",
			maxSize,
			new[] { "256", "512", "1024", "2048", "4096" },
			new[] { 256, 512, 1024, 2048, 4096 });

		compression = (TextureImporterCompression)EditorGUILayout.EnumPopup(
			"Compression", compression);

		generateMipMaps = EditorGUILayout.Toggle("Generate MipMaps", generateMipMaps);

		useCrunch = EditorGUILayout.Toggle("Use Crunch Compression", useCrunch);

		if (useCrunch)
		{
			crunchQuality = EditorGUILayout.IntSlider("Crunch Quality", crunchQuality, 0, 100);
		}

		platform = (Platform)EditorGUILayout.EnumPopup("Platform Override", platform);

		GUILayout.Space(15);

		if (GUILayout.Button("Apply To All Sprites", GUILayout.Height(35)))
		{
			ApplySettings();
		}
	}

	private void ApplySettings()
	{
		if (targetFolder == null)
		{
			EditorUtility.DisplayDialog("Error", "Please select a folder.", "OK");
			return;
		}

		string folderPath = AssetDatabase.GetAssetPath(targetFolder);
		string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

		int count = 0;

		foreach (string file in files)
		{
			if (!file.EndsWith(".png") && !file.EndsWith(".jpg"))
				continue;

			TextureImporter importer = AssetImporter.GetAtPath(file) as TextureImporter;
			if (importer == null)
				continue;

			importer.textureType = TextureImporterType.Sprite;
			importer.mipmapEnabled = generateMipMaps;
			importer.textureCompression = compression;
			importer.crunchedCompression = useCrunch;
			importer.compressionQuality = crunchQuality;
			importer.alphaIsTransparency = true;

			ApplyPlatform(importer);

			importer.SaveAndReimport();
			count++;
		}

		EditorUtility.DisplayDialog("Done", $"Processed {count} sprites.", "OK");
	}

	private void ApplyPlatform(TextureImporter importer)
	{
		if (platform == Platform.Default)
		{
			importer.maxTextureSize = maxSize;
			return;
		}

		string platformName = platform == Platform.Android ? "Android" : "iPhone";

		TextureImporterPlatformSettings settings =
			importer.GetPlatformTextureSettings(platformName);

		settings.overridden = true;
		settings.maxTextureSize = maxSize;
		settings.textureCompression = compression;
		settings.crunchedCompression = useCrunch;
		settings.compressionQuality = crunchQuality;

		importer.SetPlatformTextureSettings(settings);
	}
}
