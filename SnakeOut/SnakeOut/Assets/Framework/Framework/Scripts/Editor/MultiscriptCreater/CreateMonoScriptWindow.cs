using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class CreateMonoScriptWindow : EditorWindow
{
	private string className = string.Empty;
	private string nameSpace = string.Empty;
	private string controllerNamespace = ".UI.Controllers";
	private string controllerName = "Controller";
	private string viewName = "View";
	private string viewNamespace = ".UI.Components";
	private string controllerFolder = "Assets/Scripts/UI/Controllers";
	private string viewFolder = "Assets/Scripts/UI/Baseview";

	// NEW: toggles for using selected folders
	private bool useControllerFolder = false;
	private bool useViewFolder = false;

	[MenuItem("Tools/Create Multi Scripts")]
	public static void ShowWindow()
	{
		GetWindow<CreateMonoScriptWindow>("Create Multi Scripts");
	}

	private void OnGUI()
	{
		GUILayout.Label("Create Interdependent Scripts", EditorStyles.boldLabel);

		// Input fields for Controller
		GUILayout.Label("Controller Settings", EditorStyles.boldLabel);
		nameSpace = EditorGUILayout.TextField("NameSpace Name", nameSpace);
		className = EditorGUILayout.TextField("Class Name", className);
		controllerName = EditorGUILayout.TextField("Controller Name", controllerName);
		controllerNamespace = EditorGUILayout.TextField("Controller Namespace", controllerNamespace);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Controller Folder", controllerFolder);
		if (GUILayout.Button("Select Folder"))
		{
			string selectedPath = EditorUtility.OpenFolderPanel("Select Controller Folder", "Assets", "");
			if (!string.IsNullOrEmpty(selectedPath) && selectedPath.StartsWith(Application.dataPath))
			{
				controllerFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
			}
		}
		useControllerFolder = EditorGUILayout.ToggleLeft("Use This Directory", useControllerFolder, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		// Input fields for View
		GUILayout.Label("View Settings", EditorStyles.boldLabel);
		viewName = EditorGUILayout.TextField("View Name", viewName);
		viewNamespace = EditorGUILayout.TextField("View Namespace", viewNamespace);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("View Folder", viewFolder);
		if (GUILayout.Button("Select Folder"))
		{
			string selectedPath = EditorUtility.OpenFolderPanel("Select View Folder", "Assets", "");
			if (!string.IsNullOrEmpty(selectedPath) && selectedPath.StartsWith(Application.dataPath))
			{
				viewFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
			}
		}
		useViewFolder = EditorGUILayout.ToggleLeft("Use This Directory", useViewFolder, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		// Create button (enabled only if both toggles true)
		GUI.enabled = useControllerFolder && useViewFolder;
		if (GUILayout.Button("Create Scripts"))
		{
			CreateScripts();
		}
		GUI.enabled = true; // Reset GUI
	}

	private void CreateScripts()
	{
		// Validation
		if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(viewName))
		{
			Debug.LogError("Controller and View names cannot be empty.");
			return;
		}

		if (string.IsNullOrEmpty(controllerFolder) || string.IsNullOrEmpty(viewFolder))
		{
			Debug.LogError("Invalid folder paths.");
			return;
		}

		// Ensure directories exist (auto-create)
		if (!Directory.Exists(controllerFolder))
			Directory.CreateDirectory(controllerFolder);

		if (!Directory.Exists(viewFolder))
			Directory.CreateDirectory(viewFolder);

		// Paths
		string controllerPath = Path.Combine(controllerFolder, (className + controllerName) + ".cs");
		string viewPath = Path.Combine(viewFolder, (className + viewName) + ".cs");

		// Check if files already exist
		if (File.Exists(controllerPath) || File.Exists(viewPath))
		{
			Debug.LogError("One or both scripts already exist.");
			return;
		}

		// Generate script content
		string controllerContent = GetControllerScriptContent(className + controllerName, nameSpace + controllerNamespace, nameSpace + viewNamespace, className + viewName);
		string viewContent = GetViewScriptContent(className + viewName, nameSpace + viewNamespace);

		// Create files
		File.WriteAllText(controllerPath, controllerContent);
		File.WriteAllText(viewPath, viewContent);

		// Refresh the Asset Database
		AssetDatabase.Refresh();

		// Apply scripts to selected GameObject
		ApplyScriptsToSelectedGameObject(controllerPath, viewPath);

		Debug.Log($"Controller created at {controllerPath}");
		Debug.Log($"View created at {viewPath}");
	}

	private void ApplyScriptsToSelectedGameObject(string controllerPath, string viewPath)
	{
		if (Selection.activeGameObject == null)
		{
			Debug.LogError("No GameObject selected in the hierarchy to apply the scripts.");
			return;
		}

		GameObject selectedGameObject = Selection.activeGameObject;

		// Load the scripts as MonoScript assets
		MonoScript controllerScript = AssetDatabase.LoadAssetAtPath<MonoScript>(controllerPath);
		MonoScript viewScript = AssetDatabase.LoadAssetAtPath<MonoScript>(viewPath);

		if (controllerScript == null || viewScript == null)
		{
			Debug.LogError("Failed to load the generated scripts.");
			return;
		}

		// Add the scripts as components to the selected GameObject
		selectedGameObject.AddComponent(controllerScript.GetClass());
		selectedGameObject.AddComponent(viewScript.GetClass());

		Debug.Log($"Scripts applied to {selectedGameObject.name}");
	}

	private string GetControllerScriptContent(string className, string namespaceName, string viewNamespace, string viewClassName)
	{
		return $@"using {viewNamespace};
using UnityEngine;
using BaseView;

namespace {namespaceName}
{{
    public class {className} : Behaviour<{viewClassName}>
    {{
        private {viewClassName} m_View;

        protected override void Init()
        {{
            base.Init();
            m_View = ({viewClassName})Prefab;
        }}

public override void ShowPanel(bool on)
		{{
		}}
    }}
}}";
	}

	private string GetViewScriptContent(string className, string namespaceName)
	{
		return $@"using UnityEngine;

namespace {namespaceName}
{{
    public class {className} : View
    {{
    }}
}}";
	}
}
#endif
