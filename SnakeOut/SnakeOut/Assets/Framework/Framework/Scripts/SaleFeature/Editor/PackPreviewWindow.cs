using UnityEditor;
using UnityEngine;

namespace Framework
{
	public class PackPreviewWindow : EditorWindow
	{
		private PackManager targetManager;

		[MenuItem("Window/Sales/Pack Preview")]
		public static void OpenWindow()
		{
			GetWindow<PackPreviewWindow>("Pack Preview");
		}

		private void OnGUI()
		{
			targetManager = (PackManager)EditorGUILayout.ObjectField("Pack Manager", targetManager, typeof(PackManager), true);

			if (targetManager == null)
			{
				EditorGUILayout.HelpBox("Assign a PackManager to preview.", MessageType.Info);
				return;
			}

			if (GUILayout.Button("Refresh"))
				targetManager.EvaluateActivePack();

			var pack = targetManager.ActivePack;
			if (pack == null)
			{
				EditorGUILayout.HelpBox("No active pack for selected criteria.", MessageType.Warning);
				return;
			}

			// draw bigger portrait
			Rect r = GUILayoutUtility.GetRect(300, 533, GUILayout.ExpandWidth(false));
			EditorGUI.DrawRect(r, Color.black);
			if (pack.backgroundSprite != null)
				DrawSpriteFit(pack.backgroundSprite, r);

			GUILayout.Space(6);
			EditorGUILayout.LabelField(pack.displayName, EditorStyles.boldLabel);
			EditorGUILayout.LabelField($"Price: {pack.price} Off: {pack.offPrice}");
			EditorGUILayout.LabelField($"Start/End: {pack.GetEffectiveStartEnd(System.DateTime.UtcNow).start} -> {pack.GetEffectiveStartEnd(System.DateTime.UtcNow).end}");
		}

		private void DrawSpriteFit(Sprite s, Rect rect)
		{
			if (s == null) return;
			var tex = s.texture;
			Rect spriteRect = s.textureRect;
			var uv = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);
			GUI.DrawTextureWithTexCoords(rect, tex, uv);
		}
	}
}