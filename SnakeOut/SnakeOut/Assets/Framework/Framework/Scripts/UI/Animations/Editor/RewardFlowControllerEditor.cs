#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Framework;

[CustomEditor(typeof(RewardFlowController))]
public class RewardFlowControllerEditor : Editor
{
	// Serialized properties
	private SerializedProperty _startingPoint;
	private SerializedProperty _endPoint;
	private SerializedProperty _rwPrefab;
	private SerializedProperty _playSimultaneous;
	private SerializedProperty _rewardTransforms;

	// Foldout states
	private bool _globalFoldout = true;
	private bool _playbackFoldout = true;
	private bool _rewardsFoldout = true;
	private readonly List<bool> _rewardFoldouts = new List<bool>();

	// Lazy-init styles
	private GUIStyle _titleStyle;
	private GUIStyle _headerFoldoutStyle;
	private GUIStyle _cardStyle;
	private GUIStyle _miniLabelStyle;
	private bool _stylesReady;

	// ─────────────────────────────────────────────────────────
	// Setup
	// ─────────────────────────────────────────────────────────

	private void OnEnable()
	{
		_startingPoint = serializedObject.FindProperty("StartingPoint");
		_endPoint = serializedObject.FindProperty("EndPoint");
		_rwPrefab = serializedObject.FindProperty("RwPrefab");
		_playSimultaneous = serializedObject.FindProperty("PlaySimultaneous");
		_rewardTransforms = serializedObject.FindProperty("RewardTransforms");
	}

	// ─────────────────────────────────────────────────────────
	// Inspector draw
	// ─────────────────────────────────────────────────────────

	public override void OnInspectorGUI()
	{
		EnsureStyles();
		serializedObject.Update();

		// ── Title bar ────────────────────────────────────────
		EditorGUILayout.Space(4);
		EditorGUILayout.LabelField("REWARD FLOW CONTROLLER", _titleStyle);
		DrawLine(new Color(0.35f, 0.75f, 1f, 0.9f));
		EditorGUILayout.Space(4);

		// ── Sections ─────────────────────────────────────────
		DrawFoldoutSection(ref _globalFoldout, "⚙   Global Defaults", DrawGlobalSettings);
		EditorGUILayout.Space(3);
		DrawFoldoutSection(ref _playbackFoldout, "▶   Playback", DrawPlaybackSettings);
		EditorGUILayout.Space(3);
		DrawFoldoutSection(ref _rewardsFoldout,
			$"🎁   Reward Transforms  ({_rewardTransforms.arraySize})", DrawRewardList);

		serializedObject.ApplyModifiedProperties();
	}

	// ─────────────────────────────────────────────────────────
	// Section contents
	// ─────────────────────────────────────────────────────────

	private void DrawGlobalSettings()
	{
		EditorGUILayout.PropertyField(_startingPoint, new GUIContent("Start Point"));
		EditorGUILayout.PropertyField(_endPoint, new GUIContent("End Point"));
		EditorGUILayout.PropertyField(_rwPrefab, new GUIContent("Default Prefab"));
	}

	private void DrawPlaybackSettings()
	{
		EditorGUILayout.PropertyField(_playSimultaneous, new GUIContent(
			"Play Simultaneous",
			"When enabled all reward animations fire at once. When disabled they play one by one."));

		if (_playSimultaneous.boolValue)
		{
			EditorGUILayout.HelpBox(
				"All rewards animate together. OnFlowAnimationComplete fires after the last one finishes.",
				MessageType.Info);
		}
	}

	private void DrawRewardList()
	{
		// Keep foldout list in sync
		while (_rewardFoldouts.Count < _rewardTransforms.arraySize) _rewardFoldouts.Add(false);

		if (_rewardTransforms.arraySize == 0)
		{
			EditorGUILayout.HelpBox("No Reward Transforms added yet.", MessageType.Warning);
		}

		for (int i = 0; i < _rewardTransforms.arraySize; i++)
		{
			SerializedProperty element = _rewardTransforms.GetArrayElementAtIndex(i);
			SerializedProperty typeProp = element.FindPropertyRelative("Type");
			string typeName = typeProp != null
				? typeProp.enumDisplayNames[typeProp.enumValueIndex]
				: $"Reward {i}";

			// Card
			using (new EditorGUILayout.VerticalScope(_cardStyle))
			{
				// Card header row
				using (new EditorGUILayout.HorizontalScope())
				{
					_rewardFoldouts[i] = EditorGUILayout.Foldout(
						_rewardFoldouts[i], $"  [{i}]  {typeName}", true, _headerFoldoutStyle);

					// Delete button
					Color prev = GUI.backgroundColor;
					GUI.backgroundColor = new Color(1f, 0.38f, 0.38f);
					if (GUILayout.Button("✕", GUILayout.Width(26), GUILayout.Height(18)))
					{
						GUI.backgroundColor = prev;
						_rewardTransforms.DeleteArrayElementAtIndex(i);
						if (i < _rewardFoldouts.Count) _rewardFoldouts.RemoveAt(i);
						break;
					}
					GUI.backgroundColor = prev;
				}

				// Card body
				if (_rewardFoldouts[i])
				{
					DrawLine(new Color(0.5f, 0.5f, 0.5f, 0.4f), 1f);
					EditorGUILayout.Space(3);
					EditorGUI.indentLevel++;
					DrawRewardTransformBody(element);
					EditorGUI.indentLevel--;
				}
			}

			EditorGUILayout.Space(3);
		}

		EditorGUILayout.Space(4);

		Color prevBg = GUI.backgroundColor;
		GUI.backgroundColor = new Color(0.45f, 0.88f, 0.45f);
		if (GUILayout.Button("＋  Add Reward Transform", GUILayout.Height(28)))
		{
			_rewardTransforms.InsertArrayElementAtIndex(_rewardTransforms.arraySize);
			_rewardFoldouts.Add(true);
		}
		GUI.backgroundColor = prevBg;
	}

	// ─────────────────────────────────────────────────────────
	// RewardTransform body — grouped mini sections
	// ─────────────────────────────────────────────────────────

	private void DrawRewardTransformBody(SerializedProperty prop)
	{
		DrawMiniGroup(prop, "Type & UI",
			"Type", "StartingPoint", "EndPoint", "Prefab", "RewardSprite", "InstantiateAmount", "AmountAdded");

		DrawMiniGroup(prop, "Spawn",
			"spawnDelay", "finalScale");

		DrawMiniGroup(prop, "Appear",
			"StartScale", "EndScale", "ScaleDuration");

		DrawMiniGroup(prop, "Rotation",
			"EnableRotation", "RotationLoops", "RotationDuration");

		DrawMiniGroup(prop, "Scatter",
			"EnableScatter", "ScatterRadius", "ScatterDuration");

		DrawMiniGroup(prop, "Move To Target",
			"MoveType", "MoveDuration");

		DrawMiniGroup(prop, "Target Feedback",
			"PunchTarget", "PunchScale");

		DrawMiniGroup(prop, "Audio",
			"AppearAudioClip", "CollectAudioClip");

		DrawMiniGroup(prop, "Text Popup",
			"IsTextPopup", "TextPopupSettings");
	}

	private void DrawMiniGroup(SerializedProperty parent, string label, params string[] fieldNames)
	{
		EditorGUILayout.LabelField(label, _miniLabelStyle);
		EditorGUI.indentLevel++;
		foreach (string name in fieldNames)
		{
			SerializedProperty p = parent.FindPropertyRelative(name);
			if (p != null) EditorGUILayout.PropertyField(p, true);
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.Space(3);
	}

	// ─────────────────────────────────────────────────────────
	// Utilities
	// ─────────────────────────────────────────────────────────

	private void DrawFoldoutSection(ref bool foldout, string label, System.Action content)
	{
		using (new EditorGUILayout.VerticalScope(_cardStyle))
		{
			foldout = EditorGUILayout.Foldout(foldout, label, true, _headerFoldoutStyle);
			if (foldout)
			{
				DrawLine(new Color(0.5f, 0.5f, 0.5f, 0.3f), 1f);
				EditorGUILayout.Space(3);
				EditorGUI.indentLevel++;
				content?.Invoke();
				EditorGUI.indentLevel--;
				EditorGUILayout.Space(2);
			}
		}
	}

	private static void DrawLine(Color color, float height = 1.5f)
	{
		Rect r = EditorGUILayout.GetControlRect(false, height + 2);
		r.y += 1;
		r.height = height;
		EditorGUI.DrawRect(r, color);
	}

	private void EnsureStyles()
	{
		if (_stylesReady) return;
		_stylesReady = true;

		_titleStyle = new GUIStyle(EditorStyles.boldLabel)
		{
			fontSize = 13,
			alignment = TextAnchor.MiddleCenter,
			padding = new RectOffset(0, 0, 4, 2)
		};

		_headerFoldoutStyle = new GUIStyle(EditorStyles.foldoutHeader)
		{
			fontStyle = FontStyle.Bold,
			fontSize = 11
		};

		_cardStyle = new GUIStyle(EditorStyles.helpBox)
		{
			padding = new RectOffset(8, 8, 6, 6)
		};

		_miniLabelStyle = new GUIStyle(EditorStyles.miniBoldLabel)
		{
			normal = { textColor = new Color(0.65f, 0.85f, 1f) }
		};
	}
}
#endif