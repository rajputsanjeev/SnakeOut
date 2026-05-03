using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{
	[CustomEditor(typeof(TMPPopupTester))]
	public class TMPPopupTesterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			TMPPopupTester tester = (TMPPopupTester)target;

			DrawDefaultInspector();
			GUILayout.Space(10);

			GUI.enabled = Application.isPlaying;

			if (GUILayout.Button("▶ Play Popup", GUILayout.Height(30)))
			{
				if (tester.Settings == null)
				{
					Debug.LogWarning("TMPPopupSettings not assigned");
					return;
				}

				if (tester.Target != null)
				{
					tester.Settings.WorldPosition = tester.Target.position;
				}

				TMPPopupPlayer.Play(tester.TestText, tester.Settings);
			}

			GUI.enabled = true;

			if (!Application.isPlaying)
			{
				EditorGUILayout.HelpBox(
					"Enter Play Mode to test popup animation.",
					MessageType.Info
				);
			}
		}
	}
}

