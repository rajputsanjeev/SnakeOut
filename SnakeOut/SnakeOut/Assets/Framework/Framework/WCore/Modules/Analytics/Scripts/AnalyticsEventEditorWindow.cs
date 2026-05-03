using System.IO;
using System.Text;
using Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
namespace Framework
{
	public class AnalyticsEventEditorWindow : EditorWindow
	{
		private AnalyticsEventList eventList;
		private Vector2 scrollPos;

		[MenuItem("Tools/Analytics Event Manager")]
		public static void ShowWindow()
		{
			GetWindow<AnalyticsEventEditorWindow>("Analytics Event Manager");
		}

		private void OnGUI()
		{
			GUILayout.Label("Analytics Event List", EditorStyles.boldLabel);

			eventList = (AnalyticsEventList)EditorGUILayout.ObjectField(
				"Event List", eventList, typeof(AnalyticsEventList), false);

			if (eventList == null)
			{
				EditorGUILayout.HelpBox("Please assign an AnalyticsEventList ScriptableObject.", MessageType.Info);
				return;
			}

			DrawEventList();
			GUILayout.Space(15);

			if (GUILayout.Button("Generate AnalyticsMethods.cs", GUILayout.Height(40)))
			{
				GenerateCode();
			}
		}

		private void DrawEventList()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			for (int i = 0; i < eventList.eventNames.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				eventList.eventNames[i] = EditorGUILayout.TextField(eventList.eventNames[i]);

				if (GUILayout.Button("X", GUILayout.Width(25)))
				{
					eventList.eventNames.RemoveAt(i);
					EditorUtility.SetDirty(eventList);
					break;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (GUILayout.Button("+ Add Event"))
			{
				eventList.eventNames.Add("new_event");
				EditorUtility.SetDirty(eventList);
			}

			EditorGUILayout.EndScrollView();
		}

		private void GenerateCode()
		{
			string folderPath = "Assets/Generated";
			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			string filePath = Path.Combine(folderPath, "AnalyticsMethods.cs");

			StringBuilder sb = new StringBuilder();

			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine();
			sb.AppendLine("// AUTO-GENERATED FILE — DO NOT EDIT MANUALLY");
			sb.AppendLine("public static class AnalyticsMethods");
			sb.AppendLine("{");

			foreach (var eventName in eventList.eventNames)
			{
				string safeName = CleanMethodName(eventName);

				sb.AppendLine($"\tpublic static void {safeName}(Dictionary<string, object> parameters = null)");
				sb.AppendLine("\t{");
				sb.AppendLine($"\t\tCombinedAnalyticsManager.Instance.LogEvent(\"{eventName}\", parameters);");
				sb.AppendLine("\t}");
				sb.AppendLine();
			}

			sb.AppendLine("}");

			File.WriteAllText(filePath, sb.ToString());
			AssetDatabase.Refresh();

			EditorUtility.DisplayDialog("Success", "AnalyticsMethods.cs has been generated!", "OK");
		}

		private string CleanMethodName(string raw)
		{
			string result = raw.Replace(" ", "_").Replace("-", "_").Replace(".", "_");
			return result;
		}
	}
}
#endif