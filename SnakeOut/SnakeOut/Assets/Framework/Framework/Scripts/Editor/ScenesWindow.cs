#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

namespace HelperEditor.Editor
{

	class ScenesWindow : EditorWindow
	{
		private static string _dataPathGuids => Application.persistentDataPath + "\\Scenes.dat";

		static GUILayoutOption miniButtonWidth => GUILayout.Width(25);
		private Dictionary<string, string> buildScenes = new Dictionary<string, string>();
		private Dictionary<string, string> addedScenes = new Dictionary<string, string>();
		private Vector2 scrollPos;

		[MenuItem("Utils/Scenes", priority = 2)]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			EditorWindow.GetWindow(typeof(ScenesWindow), false, "Scene").Show();
		}

		private void OnEnable()
		{
			ReloadScenesList();
			LoadData();
		}

		static bool DrawSceneButton(KeyValuePair<string, string> scene, string active, bool isGameNotPlaying, bool isAdded,
			GUIStyle pressedButtonStyle, GUILayoutOption megaWidth)
		{
			if (scene.Value == active)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("@", miniButtonWidth))
					EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scene.Value));

				GUILayout.Label(scene.Key, pressedButtonStyle, megaWidth);
				if (isGameNotPlaying && GUILayout.Button("▶", miniButtonWidth))
				{
					EditorSceneManager.OpenScene(scene.Value);
				}
				if (isAdded && GUILayout.Button("X", miniButtonWidth))
				{
					return true;
				}

				GUILayout.EndHorizontal();
				return false;
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("@", miniButtonWidth))
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scene.Value));

			if (GUILayout.Button(scene.Key, megaWidth))
			{
				if (isGameNotPlaying)
				{
					EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
					EditorSceneManager.OpenScene(scene.Value);
					EditorApplication.isPlaying = true;
				}
				else
				{
					SceneManager.LoadScene(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.Value).name);
				}
			}

			if (isGameNotPlaying && GUILayout.Button("▶", miniButtonWidth))
			{
				EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
				EditorSceneManager.OpenScene(scene.Value);
			}

			if (isAdded && GUILayout.Button("X", miniButtonWidth))
			{
				return true;
			}

			GUILayout.EndHorizontal();
			return false;
		}

		static string NameFromPath(string path)
		{
			string theName = path.Substring(path.LastIndexOf('/') + 1);
			theName = theName.Substring(0, theName.Length - 6);
			if (theName.Length < 20) return theName;

			return theName.Substring(0, 10) + "..." + theName.Substring(theName.Length - 5);
		}

		public void SaveData()
		{
			string[] toSave = new string[this.addedScenes.Count];
			try
			{
				int count = 0;
				foreach (KeyValuePair<string, string> pair in addedScenes)
				{
					toSave[count] = AssetDatabase.GUIDFromAssetPath(pair.Value).ToString();
					count++;
				}


				FileInfo fileInfo = new FileInfo(_dataPathGuids);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}

				if (!fileInfo.Exists)
				{
					fileInfo.Create();
				}

				File.WriteAllText(fileInfo.FullName, JsonConvert.SerializeObject(toSave));
			}
			catch
			{
				Debug.LogError("Failed to save data");
			}
		}

		public void LoadData()
		{
			addedScenes = new Dictionary<string, string>();
			if (!File.Exists(_dataPathGuids)) return;

			try
			{
				string[] allGuids = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(_dataPathGuids));
				foreach (string guid in allGuids)
				{
					string path = AssetDatabase.GUIDToAssetPath(new GUID(guid));
					addedScenes.Add(NameFromPath(path), path);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Unable to Load Data {e.Message}");
			}
		}

		public void OnGUI()
		{
			if (GUILayout.Button("Refresh List"))
			{
				ReloadScenesList();
				return;
			}

			EditorGUILayout.LabelField("Make Build");
			if (GUILayout.Button("Build"))
			{
				AnotherEditorWindow window = (AnotherEditorWindow)EditorWindow.GetWindow(typeof(AnotherEditorWindow), true, "Another Editor Window");
				window.Show();
			}

			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width));
			GUIStyle pressedButtonStyle = EditorStyles.miniButton;
			pressedButtonStyle.normal.textColor = Color.green;
			string active = EditorSceneManager.GetActiveScene().path;
			bool isGameNotPlaying = !Application.isPlaying;

			GUILayoutOption megaWidth;
			if (isGameNotPlaying) megaWidth = GUILayout.Width(this.position.width - 78);
			else megaWidth = GUILayout.Width(this.position.width - 53);

			if (buildScenes.Count != 0)
			{
				EditorGUILayout.LabelField("Build Scenes");
				foreach (KeyValuePair<string, string> scene in buildScenes)
				{
					DrawSceneButton(scene, active, isGameNotPlaying, false, pressedButtonStyle, megaWidth);
				}
			}

			EditorGUILayout.Space(15);
			EditorGUILayout.LabelField("Added Scenes");
			SceneAsset asset =
				(SceneAsset)EditorGUILayout.ObjectField(null, typeof(SceneAsset), false,
					GUILayout.Width(position.width - 20));
			if (asset != null)
			{
				string path = AssetDatabase.GetAssetPath(asset);
				addedScenes.Add(NameFromPath(path), path);
				SaveData();
			}

			if (addedScenes.Count == 0)
			{
				GUILayout.EndScrollView();
				return;
			}

			if (isGameNotPlaying) megaWidth = GUILayout.Width(this.position.width - 103);
			else megaWidth = GUILayout.Width(this.position.width - 53);
			foreach (KeyValuePair<string, string> scene in addedScenes)
			{
				bool shouldDelete = DrawSceneButton(scene, active, isGameNotPlaying, true, pressedButtonStyle, megaWidth);
				if (shouldDelete)
				{
					addedScenes.Remove(scene.Key);
					SaveData();
					break;
				}
			}

			GUILayout.EndScrollView();
		}

		public void ReloadScenesList()
		{
			buildScenes.Clear();
			foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
			{
				if (S.enabled)
				{
					buildScenes.Add(NameFromPath(S.path), S.path);
				}
			}
		}
	}

	public class AnotherEditorWindow : EditorWindow
	{
		private void OnEnable()
		{
			position = new Rect(0, 0, 300, 300);
		}

		private void OnGUI()
		{
			GUILayoutOption[] options = new GUILayoutOption[] {
			  GUILayout.Width(400.0f),
			  GUILayout.MinWidth(250.0f),
			  GUILayout.ExpandWidth(true)
			  };

			EditorGUILayout.TextArea("Before Making build please clear all addressable \n (from Window/Asset Management/Addressable/Group/Build/Clear Build) \n and change the version.", options);

			EditorGUILayout.Space(50);
			if (GUILayout.Button("Build Player"))
			{
				BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
				buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
				buildPlayerOptions.locationPathName = "Build/MyBuild.exe";
				buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
				buildPlayerOptions.options = BuildOptions.None;

				BuildPipeline.BuildPlayer(buildPlayerOptions);
			}

			if (GUILayout.Button("Clear Addressable"))
			{
				Debug.Log("Performing another action");
			}
		}
	}
}


#endif