using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace Framework.Core
{
	[CustomEditor(typeof(ProjectInitSettings))]
	public class ProjectInitSettingsEditor : Editor
	{
		private const string MODULES_PROPERTY_NAME = "modules"; // make sure this matches the field name in ProjectInitSettings

		private SerializedProperty modulesProperty;
		private List<InitModuleContainer> initModulesEditors;
		private ProjectInitSettings projectInitSettings;
		private GenericMenu modulesGenericMenu;
		private static InitModulesHandler modulesHandler;

		[MenuItem("Window/Watermelon Core/Project Init Settings", priority = 50)]
		public static void SelectProjectInitSettings()
		{
			ProjectInitSettings selectedObject = EditorUtils.GetAsset<ProjectInitSettings>();
			if (selectedObject != null)
			{
				Selection.activeObject = selectedObject;
			}
			else
			{
				Debug.LogError("Asset with type \"ProjectInitSettings\" don't exist.");
			}
		}

		protected void OnEnable()
		{
			projectInitSettings = (ProjectInitSettings)target;

			if (initModulesEditors == null)
				initModulesEditors = new List<InitModuleContainer>();

			modulesHandler = new InitModulesHandler();

			// Ensure serializedObject is up-to-date before finding property
			serializedObject.Update();
			modulesProperty = serializedObject.FindProperty(MODULES_PROPERTY_NAME);
			serializedObject.ApplyModifiedProperties();

			InitGenericMenu();

			// update core modules before loading editors
			InitCoreModules(projectInitSettings.Modules);

			// Load editors AFTER core init & after serializedObject is in a correct state
			LoadEditorsList();

			EditorApplication.playModeStateChanged += LogPlayModeState;
		}

		private void InitCoreModules(InitModule[] coreModules)
		{
			RequiredModule[] requiredModules = GetRequiredModules(coreModules);
			if (requiredModules.Length > 0)
			{
				foreach (RequiredModule requiredModule in requiredModules)
				{
					AddModule(requiredModule.Type);
				}

				LoadEditorsList();

				EditorUtility.SetDirty(target);
				AssetDatabase.SaveAssets();
			}
		}

		private void InitGenericMenu()
		{
			modulesGenericMenu = new GenericMenu();

			//Load all modules
			InitModule[] initModules = projectInitSettings.Modules;

			// Get registered types
			IEnumerable<Type> registeredTypes = GetRegisteredAttributes();
			foreach (Type type in registeredTypes)
			{
				// capture local to avoid closure issues
				Type localType = type;
				RegisterModuleAttribute defineAttribute = (RegisterModuleAttribute)Attribute.GetCustomAttribute(localType, typeof(RegisterModuleAttribute));
				if (defineAttribute != null)
				{
					if (!defineAttribute.Core)
					{
						bool isAlreadyActive = initModules != null && initModules.Any(x => x != null && x.GetType() == localType);
						if (isAlreadyActive)
						{
							modulesGenericMenu.AddDisabledItem(new GUIContent("Add Module/" + defineAttribute.Path), false);
						}
						else
						{
							modulesGenericMenu.AddItem(new GUIContent("Add Module/" + defineAttribute.Path), false, (object obj) =>
							{
								AddModule(localType);
								InitGenericMenu();
							}, null);
						}
					}
				}
			}
		}

		private IEnumerable<Type> GetRegisteredAttributes()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types;
				try
				{
					types = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException e)
				{
					types = e.Types.Where(t => t != null).ToArray();
				}

				foreach (Type type in types.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InitModule))))
				{
					yield return type;
				}
			}
		}

		private void OnDisable()
		{
			EditorApplication.playModeStateChanged -= LogPlayModeState;
		}

		private void LogPlayModeState(PlayModeStateChange obj)
		{
			// If user had the settings selected, deselect to avoid serialization issues on play mode change
			if (Selection.activeObject == target)
				Selection.activeObject = null;
		}

		private void LoadEditorsList()
		{
			// Keep this method deterministic: update serializedObject before reading modulesProperty
			serializedObject.Update();
			modulesProperty = serializedObject.FindProperty(MODULES_PROPERTY_NAME);

			ClearInitModules();

			if (modulesProperty == null)
			{
				Debug.LogWarning("[ProjectInitSettingsEditor] modulesProperty is null. Check field name: " + MODULES_PROPERTY_NAME);
				return;
			}

			SerializedProperty initModule;
			SerializedObject initModuleSerializedObject;

			for (int i = 0; i < modulesProperty.arraySize; i++)
			{
				initModule = modulesProperty.GetArrayElementAtIndex(i);

				if (initModule != null && initModule.objectReferenceValue != null)
				{
					initModuleSerializedObject = new SerializedObject(initModule.objectReferenceValue);

					// create editor for the target object (safe guard)
					UnityEngine.Object targetObj = initModuleSerializedObject.targetObject;
					Editor createdEditor = null;
					if (targetObj != null)
						createdEditor = Editor.CreateEditor(targetObj);

					var container = new InitModuleContainer(initModule.objectReferenceValue.GetType(), initModuleSerializedObject, createdEditor, modulesHandler.IsCoreModule(initModule.objectReferenceValue.GetType()));

					initModulesEditors.Add(container);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private InitModuleContainer GetEditor(Type type)
		{
			// DON'T reload editors here; just search the list we already maintain
			if (initModulesEditors == null) return null;

			for (int i = 0; i < initModulesEditors.Count; i++)
			{
				if (initModulesEditors[i].Type == type)
					return initModulesEditors[i];
			}

			return null;
		}

		private void OnDestroy()
		{
			ClearInitModules();
		}

		private void ClearInitModules()
		{
			if (initModulesEditors != null)
			{
				// Destroy old editors
				for (int i = 0; i < initModulesEditors.Count; i++)
				{
					var ed = initModulesEditors[i];
					if (ed != null && ed.Editor != null)
					{
						DestroyImmediate(ed.Editor);
					}
				}

				initModulesEditors.Clear();
			}
			else
			{
				initModulesEditors = new List<InitModuleContainer>();
			}
		}

		private void DrawModules(SerializedProperty arrayProperty)
		{
			// Ensure up-to-date before drawing
			serializedObject.Update();
			arrayProperty = serializedObject.FindProperty(MODULES_PROPERTY_NAME);

			var current = Event.current;

			if (arrayProperty != null && arrayProperty.arraySize > 0)
			{
				for (int i = 0; i < arrayProperty.arraySize; i++)
				{
					SerializedProperty initModuleProperty = arrayProperty.GetArrayElementAtIndex(i);

					if (initModuleProperty != null && initModuleProperty.objectReferenceValue != null)
					{
						InitModule initModule = (InitModule)initModuleProperty.objectReferenceValue;

						SerializedObject moduleSerializedObject = new SerializedObject(initModuleProperty.objectReferenceValue);
						moduleSerializedObject.Update();

						Rect blockRect = EditorGUILayout.BeginVertical(EditorCustomStyles.box, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));

						GUI.Box(new Rect(blockRect.x - 10, blockRect.y, blockRect.width + 10, 21), GUIContent.none);

						GUILayout.Space(14);

						// Use foldout correctly
						initModuleProperty.isExpanded = EditorGUI.Foldout(new Rect(blockRect.x + 8, blockRect.y, blockRect.width - 30, 21), initModuleProperty.isExpanded, initModule.ModuleName, true);

						if (initModuleProperty.isExpanded)
						{
							GUILayout.Space(12);

							InitModuleContainer moduleContainer = GetEditor(initModuleProperty.objectReferenceValue.GetType());
							if (moduleContainer == null)
							{
								moduleSerializedObject.ApplyModifiedProperties();
								EditorGUILayout.HelpBox("Editor for module not found.", MessageType.Warning);
								EditorGUILayout.EndVertical();
								continue;
							}

							moduleContainer.OnInspectorGUI();

							GUILayout.Space(10);

							EditorGUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();

							moduleContainer.DrawButtons();

							if (!moduleContainer.IsCore)
							{
								if (GUILayout.Button("Remove", GUILayout.Width(90)))
								{
									if (EditorUtility.DisplayDialog("This object will be removed!", "Are you sure?", "Remove", "Cancel"))
									{
										moduleContainer.OnRemoved();

										UnityEngine.Object removedObject = initModuleProperty.objectReferenceValue;
										initModuleProperty.isExpanded = false;

										// Use ApplyModifiedProperties after RemoveFromVariableArrayAt if that extension changes serialized data
										arrayProperty.RemoveFromVariableArrayAt(i);

										LoadEditorsList();

										if (removedObject != null)
										{
											AssetDatabase.RemoveObjectFromAsset(removedObject);
											DestroyImmediate(removedObject, true);
										}

										EditorUtility.SetDirty(target);
										AssetDatabase.SaveAssets();

										InitGenericMenu();

										// important: exit to avoid using destroyed objects in the rest of the loop
										return;
									}
								}
							}

							EditorGUILayout.EndHorizontal();
						}

						EditorGUILayout.EndVertical();

						// small local capture for 'i'
						int index = i;
						if (GUI.Button(new Rect(blockRect.x + blockRect.width - 20, blockRect.y + 2, 17, 17), "="))
						{
							GenericMenu genericMenu = new GenericMenu();
							if (index > 0)
							{
								genericMenu.AddItem(new GUIContent("Move Up"), false, (object obj) =>
								{
									bool expandState = arrayProperty.GetArrayElementAtIndex(index - 1).isExpanded;

									arrayProperty.MoveArrayElement(index, index - 1);

									arrayProperty.GetArrayElementAtIndex(index - 1).isExpanded = initModuleProperty.isExpanded;
									arrayProperty.GetArrayElementAtIndex(index).isExpanded = expandState;
									serializedObject.ApplyModifiedProperties();

									LoadEditorsList();
								}, null);
							}
							else
							{
								genericMenu.AddDisabledItem(new GUIContent("Move Up"), false);
							}

							if (index + 1 < arrayProperty.arraySize)
							{
								genericMenu.AddItem(new GUIContent("Move Down"), false, (object obj) =>
								{
									bool expandState = arrayProperty.GetArrayElementAtIndex(index + 1).isExpanded;

									arrayProperty.MoveArrayElement(index, index + 1);

									arrayProperty.GetArrayElementAtIndex(index + 1).isExpanded = initModuleProperty.isExpanded;
									arrayProperty.GetArrayElementAtIndex(index).isExpanded = expandState;

									serializedObject.ApplyModifiedProperties();

									LoadEditorsList();
								}, null);
							}
							else
							{
								genericMenu.AddDisabledItem(new GUIContent("Move Down"), false);
							}

							InitModuleContainer menuModuleContainer = GetEditor(initModuleProperty.objectReferenceValue.GetType());
							if (menuModuleContainer != null)
							{
								menuModuleContainer.PrepareMenuItems(ref genericMenu);
							}

							genericMenu.ShowAsContext();
						}

						moduleSerializedObject.ApplyModifiedProperties();
					}
					else
					{
						EditorGUILayout.BeginHorizontal(EditorCustomStyles.box);
						EditorGUILayout.BeginHorizontal(EditorCustomStyles.padding00);
						EditorGUILayout.LabelField(EditorGUIUtility.IconContent("console.warnicon"), EditorCustomStyles.padding00, GUILayout.Width(16), GUILayout.Height(16));
						EditorGUILayout.LabelField("Object reference is null");
						if (GUILayout.Button("Remove", EditorStyles.miniButton))
						{
							arrayProperty.RemoveFromVariableArrayAt(i);

							InitGenericMenu();

							GUIUtility.ExitGUI();
							Event.current.Use();

							return;
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndHorizontal();
					}
				}
			}
			else
			{
				EditorGUILayout.HelpBox("Modules list is empty!", MessageType.Info);
			}

			serializedObject.ApplyModifiedProperties();
		}

		public override void OnInspectorGUI()
		{
			// update before drawing
			serializedObject.Update();

			EditorGUILayoutCustom.BeginMenuBoxGroup("Modules", modulesGenericMenu);

			DrawModules(modulesProperty);

			EditorGUILayoutCustom.EndBoxGroup();

			GUILayout.FlexibleSpace();

			serializedObject.ApplyModifiedProperties();
		}

		public void AddModule(Type moduleType)
		{
			if (!moduleType.IsSubclassOf(typeof(InitModule)))
			{
				Debug.LogError("[Initializer]: Module type should be subclass of InitModule class!");
				return;
			}

			Undo.RecordObject(target, "Add module");

			// refresh property reference and update
			serializedObject.Update();
			modulesProperty = serializedObject.FindProperty(MODULES_PROPERTY_NAME);

			if (modulesProperty == null)
			{
				Debug.LogError("[ProjectInitSettingsEditor] modulesProperty is null. Check MODULES_PROPERTY_NAME.");
				return;
			}

			modulesProperty.arraySize++;

			InitModule initModule = (InitModule)ScriptableObject.CreateInstance(moduleType);
			initModule.name = moduleType.ToString();
			initModule.hideFlags = HideFlags.HideInHierarchy;

			AssetDatabase.AddObjectToAsset(initModule, target);

			modulesProperty.GetArrayElementAtIndex(modulesProperty.arraySize - 1).objectReferenceValue = initModule;

			serializedObject.ApplyModifiedProperties();

			LoadEditorsList();

			EditorUtility.SetDirty(target);
			AssetDatabase.SaveAssets();

			Editor editor = Editor.CreateEditor(initModule);
			InitModuleEditor initModuleEditor = editor as InitModuleEditor;
			if (initModuleEditor != null)
			{
				initModuleEditor.OnCreated();
			}

			DestroyImmediate(editor);
		}

		[MenuItem("Assets/Create/Data/Core/Project Init Settings")]
		public static void CreateAsset()
		{
			ProjectInitSettings projectInitSettings = EditorUtils.GetAsset<ProjectInitSettings>();
			if (projectInitSettings)
			{
				Debug.Log("Project Init Settings file already exists!");
				EditorGUIUtility.PingObject(projectInitSettings);
				return;
			}

			string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			selectionPath = Path.GetDirectoryName(selectionPath);
			if (string.IsNullOrEmpty(selectionPath) || !Directory.Exists(selectionPath))
				selectionPath = "Assets";

			CreateAsset(selectionPath, true);
		}

		public static ProjectInitSettings CreateAsset(string folderPath, bool ping)
		{
			ProjectInitSettings projectInitSettings = (ProjectInitSettings)ScriptableObject.CreateInstance<ProjectInitSettings>();
			projectInitSettings.name = "Project Init Settings";

			string assetPath = Path.Combine(folderPath, projectInitSettings.name + ".asset");
			assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

			AssetDatabase.CreateAsset(projectInitSettings, assetPath);
			AssetDatabase.SaveAssets();

			SerializedObject serializedObject = new SerializedObject(projectInitSettings);
			serializedObject.Update();

			SerializedProperty coreModulesProperty = serializedObject.FindProperty(MODULES_PROPERTY_NAME);

			RequiredModule[] requiredModules = GetRequiredModules(null);
			List<InitModule> initModules = new List<InitModule>();

			for (int r = 0; r < requiredModules.Length; r++)
			{
				RequiredModule requiredModule = requiredModules[r];

				// Create init module
				InitModule initModule = (InitModule)ScriptableObject.CreateInstance(requiredModule.Type);
				initModule.name = requiredModule.Type.ToString();
				initModule.hideFlags = HideFlags.HideInHierarchy;

				AssetDatabase.AddObjectToAsset(initModule, projectInitSettings);

				coreModulesProperty.arraySize++;
				coreModulesProperty.GetArrayElementAtIndex(coreModulesProperty.arraySize - 1).objectReferenceValue = initModule;

				initModules.Add(initModule);
			}

			serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(projectInitSettings);
			AssetDatabase.SaveAssets();

			foreach (var initModule in initModules)
			{
				Editor editor = Editor.CreateEditor(initModule);
				InitModuleEditor initModuleEditor = editor as InitModuleEditor;
				if (initModuleEditor != null)
				{
					initModuleEditor.OnCreated();
				}

				DestroyImmediate(editor);
			}

			if (ping)
				EditorGUIUtility.PingObject(projectInitSettings);

			return projectInitSettings;
		}

		private static RequiredModule[] GetRequiredModules(InitModule[] coreModules)
		{
			IEnumerable<Type> registeredTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => {
					try { return s.GetTypes(); }
					catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
				})
				.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InitModule)));

			List<RequiredModule> requiredModules = new List<RequiredModule>();
			foreach (Type type in registeredTypes)
			{
				RegisterModuleAttribute[] defineAttributes = (RegisterModuleAttribute[])Attribute.GetCustomAttributes(type, typeof(RegisterModuleAttribute));
				for (int m = 0; m < defineAttributes.Length; m++)
				{
					if (defineAttributes[m].Core)
					{
						bool isExists = coreModules != null && coreModules.Any(x => x != null && x.GetType() == type);
						if (!isExists)
						{
							requiredModules.Add(new RequiredModule(defineAttributes[m], type));
						}
					}
				}
			}

			return requiredModules.OrderByDescending(x => x.Attribute.Order).ToArray();
		}

		private class InitModulesHandler
		{
			private IEnumerable<ModuleData> modulesData;

			public InitModulesHandler()
			{
				modulesData = GetModulesData();
			}

			private IEnumerable<ModuleData> GetModulesData()
			{
				IEnumerable<Type> registeredTypes = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(s => {
						try { return s.GetTypes(); }
						catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
					})
					.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InitModule)));

				foreach (Type type in registeredTypes)
				{
					RegisterModuleAttribute defineAttribute = (RegisterModuleAttribute)Attribute.GetCustomAttribute(type, typeof(RegisterModuleAttribute));
					if (defineAttribute != null)
					{
						yield return new ModuleData()
						{
							ClassType = type,
							Attribute = defineAttribute
						};
					}
				}
			}

			public bool IsCoreModule(Type type)
			{
				foreach (var data in modulesData)
				{
					if (type == data.ClassType && data.Attribute.Core)
						return true;
				}

				return false;
			}

			public class ModuleData
			{
				public Type ClassType;
				public RegisterModuleAttribute Attribute;
			}
		}

		private class InitModuleContainer
		{
			public Type Type;
			public SerializedObject SerializedObject;
			public Editor Editor;

			private bool isModuleInitEditor;
			private InitModuleEditor initModuleEditor;

			public bool IsCore;

			public InitModuleContainer(Type type, SerializedObject serializedObject, Editor editor, bool isCore)
			{
				Type = type;
				SerializedObject = serializedObject;
				Editor = editor;
				IsCore = isCore;

				initModuleEditor = editor as InitModuleEditor;
				isModuleInitEditor = initModuleEditor != null;
			}

			public void OnInspectorGUI()
			{
				if (Editor != null)
				{
					Editor.OnInspectorGUI();
				}
			}

			public void DrawButtons()
			{
				if (!isModuleInitEditor) return;
				initModuleEditor.Buttons();
			}

			public void PrepareMenuItems(ref GenericMenu genericMenu)
			{
				if (!isModuleInitEditor) return;
				initModuleEditor.PrepareMenuItems(ref genericMenu);
			}

			public void OnRemoved()
			{
				if (!isModuleInitEditor) return;
				initModuleEditor.OnRemoved();
			}
		}

		private class RequiredModule
		{
			public RegisterModuleAttribute Attribute { get; private set; }
			public Type Type { get; private set; }

			public RequiredModule(RegisterModuleAttribute attribute, Type type)
			{
				Attribute = attribute;
				Type = type;
			}
		}
	}
}
