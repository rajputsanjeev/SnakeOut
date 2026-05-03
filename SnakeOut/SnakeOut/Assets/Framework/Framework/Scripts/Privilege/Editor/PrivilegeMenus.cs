// Assets/Editor/PrivilegeMenus.cs
#if UNITY_EDITOR
using ColorBlockJam;
using UnityEditor;
using UnityEngine;

namespace Framework
{
	public static class PrivilegeMenus
	{
		[MenuItem("Tools/Data/Privilege/Delete Save JSON")]
		public static void DeleteSave()
		{
			PrivilegePersistence.DeleteSave();
			EditorUtility.DisplayDialog("Privilege", "Save deleted.", "OK");
		}

		[MenuItem("Tools/Data/Privilege/Open Save Folder")]
		public static void OpenFolder()
		{
			EditorUtility.RevealInFinder(PrivilegePersistence.FilePath);
		}
	}
}
#endif
