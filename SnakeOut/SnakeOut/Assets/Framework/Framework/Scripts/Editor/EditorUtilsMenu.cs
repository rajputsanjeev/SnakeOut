using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace HelperEditor.Editor
{
	public static class EditorUtilsMenu
	{
		[MenuItem("Utils/Clear Player Prefs")]
		public static void ClearePlayerPrefs() => PlayerPrefs.DeleteAll();

		[MenuItem("Utils/Find And Remove MissingScripts")]
		private static void FindInSelected()
		{
			GameObject[] go = Selection.gameObjects;
			int goCount = 0;
			int componentsCount = 0;
			int missingCount = 0;

			foreach (var g in go)
			{
				FindMissingScriptsInGo(g, goCount: out int gc, out int cc, out int mc);
				goCount += gc;
				componentsCount += cc;
				missingCount += mc;
			}

			AssetDatabase.SaveAssets();
			Debug.Log($"{goCount} GameObjects Selected, ({componentsCount} Components) of which {missingCount} Components Deleted");
		}

		private static void FindMissingScriptsInGo(GameObject g, out int goCount, out int componentsCount, out int missingCount)
		{
			var components = g.GetComponents<Component>();
			var r = 0;
			goCount = 1;
			componentsCount = 0;
			missingCount = 0;

			for (var i = 0; i < components.Length; i++)
			{
				componentsCount++;
				if (components[i] != null) continue;
				missingCount++;
				var s = g.name;
				var t = g.transform;
				while (t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}

				Debug.Log($"{s} has a missing script at {i}", g);

				var serializedObject = new SerializedObject(g);

				var prop = serializedObject.FindProperty("m_Component");

				prop.DeleteArrayElementAtIndex(i - r);
				r++;

				serializedObject.ApplyModifiedProperties();
			}

			foreach (Transform childT in g.transform)
			{
				FindMissingScriptsInGo(childT.gameObject, out int gc, out int cc, out int mc);
				goCount += gc;
				componentsCount += cc;
				missingCount += mc;
			}
		}
	}
}

#endif