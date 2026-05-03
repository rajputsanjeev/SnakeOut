#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

public class NotificationTemplatesWindow : EditorWindow
{
	private Vector2 scroll;
	private NotificationConfig config;

	[MenuItem("Window/Notifications/Notification Manager")]
	public static void ShowWindow()
	{
		var w = GetWindow<NotificationTemplatesWindow>("Notification Manager");
		w.Show();
	}

	private void OnEnable()
	{
		LoadConfig();
	}

	void LoadConfig()
	{
		if (config == null)
		{
			// try to load first NotificationConfig in project
			string[] guids = AssetDatabase.FindAssets("t:NotificationConfig");
			if (guids.Length > 0)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[0]);
				config = AssetDatabase.LoadAssetAtPath<NotificationConfig>(path);
			}
		}
	}

	void OnGUI()
	{
		if (config == null)
		{
			EditorGUILayout.HelpBox("No NotificationConfig found. Create one via Assets > Create > Notifications > Config", MessageType.Warning);
			if (GUILayout.Button("Create NotificationConfig"))
			{
				var asset = ScriptableObject.CreateInstance<NotificationConfig>();
				AssetDatabase.CreateAsset(asset, "Assets/Notifications/NotificationConfig.asset");
				AssetDatabase.SaveAssets();
				config = asset;
			}
			return;
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Config", GUILayout.Width(60));
		EditorGUILayout.ObjectField(config, typeof(NotificationConfig), false);
		if (GUILayout.Button("Ping", GUILayout.Width(60))) EditorGUIUtility.PingObject(config);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		if (GUILayout.Button("Create New Template"))
		{
			CreateNewTemplate();
		}

		if (GUILayout.Button("Generate NotificationEvents static class"))
		{
			GenerateStaticEventClass();
		}

		EditorGUILayout.Space();
		scroll = EditorGUILayout.BeginScrollView(scroll);
		if (config.templates != null)
		{
			for (int i = 0; i < config.templates.Count; ++i)
			{
				var t = config.templates[i];
				if (t == null) continue;

				EditorGUILayout.BeginVertical("box");
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(t.displayName + " [" + t.id + "]", EditorStyles.boldLabel);
				if (GUILayout.Button("Edit", GUILayout.Width(50))) Selection.activeObject = t;
				if (GUILayout.Button("Remove", GUILayout.Width(70)))
				{
					config.templates.RemoveAt(i);
					EditorUtility.SetDirty(config);
					break;
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.LabelField(t.title, GUILayout.MaxWidth(position.width - 30));
				EditorGUILayout.EndVertical();
			}
		}
		EditorGUILayout.EndScrollView();

		if (GUI.changed) EditorUtility.SetDirty(config);
	}

	void CreateNewTemplate()
	{
		var tpl = ScriptableObject.CreateInstance<NotificationTemplate>();
		tpl.id = "NewEvent" + System.DateTime.Now.Ticks;
		tpl.displayName = "New Event";
		string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Notifications/Templates/NewNotificationTemplate.asset");
		AssetDatabase.CreateAsset(tpl, path);
		AssetDatabase.SaveAssets();

		config.templates.Add(tpl);
		EditorUtility.SetDirty(config);
		AssetDatabase.Refresh();
		Selection.activeObject = tpl;
	}

	void GenerateStaticEventClass()
	{
		if (config.templates == null || config.templates.Count == 0)
		{
			EditorUtility.DisplayDialog("Nothing to generate", "No templates in config", "OK");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("public static class NotificationEvents");
		sb.AppendLine("{");
		foreach (var t in config.templates)
		{
			if (t == null) continue;
			var safe = MakeSafeIdentifier(t.id);
			sb.AppendLine($"    public static void {safe}() => UnityEngine.Object.FindObjectOfType<NotificationManager>()?.Trigger(\"{t.id}\");");
		}
		sb.AppendLine("}");

		string folder = "Assets/Scripts/Notifications/Generated";
		if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
		string file = Path.Combine(folder, "NotificationEvents.cs");
		File.WriteAllText(file, sb.ToString());
		AssetDatabase.ImportAsset(file);
		EditorUtility.RevealInFinder(file);
		EditorUtility.DisplayDialog("Generated", $"Generated {file}", "OK");
	}

	string MakeSafeIdentifier(string id)
	{
		var sb = new StringBuilder();
		foreach (char c in id)
		{
			if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
			else sb.Append('_');
		}
		if (char.IsDigit(sb[0])) sb.Insert(0, '_');
		return sb.ToString();
	}
}
#endif
