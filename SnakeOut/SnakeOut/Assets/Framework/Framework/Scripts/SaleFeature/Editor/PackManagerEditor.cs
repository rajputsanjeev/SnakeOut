//using UnityEditor;
//using UnityEngine;

//namespace Framework
//{
//	[CustomEditor(typeof(PackManager))]
//	public class PackManagerEditor : Editor
//	{
//		private PackManager manager;
//		private Texture2D previewTexture;
//		SerializedProperty UseUtc, UseOnline;

//		private void OnEnable()
//		{
//			manager = (PackManager)target;
//			EditorApplication.update += EditorUpdate;
//		}

//		private void OnDisable()
//		{
//			EditorApplication.update -= EditorUpdate;
//		}

//		public override void OnInspectorGUI()
//		{
//			DrawDefaultInspector();

//			GUILayout.Space(10);

//			UseUtc = serializedObject.FindProperty("UseUtc");
//			UseOnline = serializedObject.FindProperty("UseOnline");

//			EditorGUILayout.PropertyField(UseUtc);
//			EditorGUILayout.PropertyField(UseOnline);

//			if (GUILayout.Button("Re-evaluate active pack"))
//			{
//				manager.EvaluateActivePack();
//				Repaint();
//			}

//			GUILayout.Space(6);

//			EditorGUILayout.LabelField("Preview / Tools", EditorStyles.boldLabel);
//			manager.overrideSystemDate = EditorGUILayout.Toggle("Override Date (Editor)", manager.overrideSystemDate);

//			if (manager.overrideSystemDate)
//			{
//				var dtProp = serializedObject.FindProperty("overrideDate");
//				EditorGUILayout.PropertyField(dtProp, true);
//				serializedObject.ApplyModifiedProperties();
//			}

//			if (manager.ActivePack != null)
//			{
//				EditorGUILayout.HelpBox($"Active Pack: {manager.ActivePack.displayName}", MessageType.Info);
//				if (GUILayout.Button("Ping active pack asset"))
//					EditorGUIUtility.PingObject(manager.ActivePack);
//			}
//			else
//			{
//				EditorGUILayout.HelpBox("No active pack found for the current settings/date.", MessageType.Warning);
//			}

//			GUILayout.Space(8);
//			EditorGUILayout.LabelField("Portrait Preview (live)", EditorStyles.boldLabel);
//			DrawPortraitPreview();
//		}

//		private void DrawPortraitPreview()
//		{
//			// 9:16 portrait box
//			Rect r = GUILayoutUtility.GetRect(180, 320, GUILayout.ExpandWidth(false));
//			EditorGUI.DrawRect(r, new Color(0.9f, 0.9f, 0.9f));
//			DrawPreviewContents(r, manager.ActivePack);
//		}

//		private void DrawPreviewContents(Rect area, SeasonPack pack)
//		{
//			if (pack == null)
//			{
//				EditorGUI.LabelField(area, "No Pack assigned / active");
//				return;
//			}

//			// background
//			if (pack.backgroundSprite != null)
//			{
//				DrawSpriteFit(pack.backgroundSprite, area);
//			}

//			// upper banner area
//			Rect upperRect = new Rect(area.x + 10, area.y + 10, area.width - 20, 56);
//			if (pack.upperSprite != null) DrawSpriteFit(pack.upperSprite, upperRect);

//			// title
//			var titleRect = new Rect(area.x + 20, area.y + 18, area.width - 40, 40);
//			GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 14 };
//			GUI.Label(titleRect, pack.displayName, titleStyle);

//			// center showcase (snow image)
//			Rect centerRect = new Rect(area.x + 10, area.y + 72, area.width - 20, 160);
//			if (pack.backgroundSprite != null)
//			{
//				// try to reuse background as center if nothing else
//				DrawSpriteFit(pack.backgroundSprite, centerRect);
//			}

//			// reward strip near bottom
//			Rect rewardRect = new Rect(area.x + 12, area.y + area.height - 86, area.width - 24, 56);
//			EditorGUI.DrawRect(rewardRect, new Color(1f, 1f, 1f, 0.6f));
//			GUI.Label(new Rect(rewardRect.x + 6, rewardRect.y + 6, 200, 20), $"Price: {pack.price}   Off: {pack.offPrice}");

//			// badge
//			if (pack.badgeSprite != null)
//			{
//				Rect badgeRect = new Rect(area.x + area.width - 54, area.y + 16, 44, 44);
//				DrawSpriteFit(pack.badgeSprite, badgeRect);
//			}
//		}

//		private void DrawSpriteFit(Sprite s, Rect rect)
//		{
//			if (s == null) return;
//			Texture2D tex = s.texture;
//			if (tex == null) return;

//			// compute UV rect to crop sprite trimmed area
//			Rect spriteRect = s.textureRect;
//			var uv = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);

//			// Draw
//			GUI.DrawTextureWithTexCoords(rect, tex, uv);
//		}

//		private void EditorUpdate()
//		{
//			if (manager == null) return;
//			// auto evaluate active pack while editing so preview updates
//			manager.EvaluateActivePack();
//			// repaint inspector to update preview
//			if (Selection.activeObject == manager)
//				Repaint();
//		}
//	}
//}