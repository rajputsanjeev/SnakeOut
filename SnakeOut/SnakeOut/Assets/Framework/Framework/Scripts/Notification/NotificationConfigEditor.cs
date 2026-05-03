#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NotificationConfig))]
public class NotificationConfigEditor : Editor
{
	SerializedProperty modeProp;
	SerializedProperty templatesProp;
	SerializedProperty androidChannelIdProp;
	SerializedProperty androidChannelNameProp;
	SerializedProperty androidChannelDescProp;
	SerializedProperty debugLogProp;

	void OnEnable()
	{
		modeProp = serializedObject.FindProperty("notificationMode");
		templatesProp = serializedObject.FindProperty("templates");
		androidChannelIdProp = serializedObject.FindProperty("defaultAndroidChannelId");
		androidChannelNameProp = serializedObject.FindProperty("defaultAndroidChannelName");
		androidChannelDescProp = serializedObject.FindProperty("defaultAndroidChannelDesc");
		debugLogProp = serializedObject.FindProperty("debugLog");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(modeProp);
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Android Channel Defaults", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(androidChannelIdProp, new GUIContent("Channel Id"));
		EditorGUILayout.PropertyField(androidChannelNameProp, new GUIContent("Channel Name"));
		EditorGUILayout.PropertyField(androidChannelDescProp, new GUIContent("Channel Desc"));

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Templates", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(templatesProp, true);

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(debugLogProp);

		if (GUILayout.Button("Open Notification Window"))
		{
			NotificationTemplatesWindow.ShowWindow();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif
