using UnityEditor;
using UnityEngine;

namespace Framework
{
	[CustomEditor(typeof(HomeUIAnimator))]
	public class HomeUIAnimatorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			HomeUIAnimator animator = (HomeUIAnimator)target;

			GUILayout.Space(10);

			if (Application.isPlaying)
			{
				if (GUILayout.Button("▶ Play Animation"))
				{
					animator.Play();
				}

				if (GUILayout.Button("⟲ Reset To Start"))
				{
					animator.ResetToStart();
				}
			}
			else
			{
				EditorGUILayout.HelpBox(
					"Enter Play Mode to test animation",
					MessageType.Info
				);
			}
		}
	}
}

