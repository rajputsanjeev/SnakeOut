#if UNITY_EDITOR
using Framework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuntimeSpinWheel))]
public class RuntimeSpinWheelEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		RuntimeSpinWheel wheel = (RuntimeSpinWheel)target;

		GUILayout.Space(10);
		GUILayout.Label("Editor Tools", EditorStyles.boldLabel);

		if (GUILayout.Button("Align Dots In Circle"))
		{
			Undo.RecordObject(wheel, "Align Dots");
			wheel.AlignDotsInCircle();
			EditorUtility.SetDirty(wheel);
		}
	}
}
#endif
