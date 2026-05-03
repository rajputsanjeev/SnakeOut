using UnityEditor;
using UnityEngine;
using Framework;

namespace Framework.EditorTools
{
	[CustomEditor(typeof(AnimatedButton))]
	[CanEditMultipleObjects]
	public class AnimatedButtonEditor : Editor
	{
		// ── References ───────────────────────────────────────────────
		private SerializedProperty button, icon, label, ButtonRect, ButtonImage, IconImage;

		// ── Behavior Flags ───────────────────────────────────────────
		private SerializedProperty IsSelfComponent, IsBothMove, IsIconMoveUpOnly;
		private SerializedProperty IsIconMoveScale, IsScaleBackgroundOnly, IsHomeButton;

		// ── Appearance ───────────────────────────────────────────────
		private SerializedProperty ButtonTypeEnum;
		private SerializedProperty ButtonPressedSprite, ButtonUnPressedSprite;
		private SerializedProperty ButtonPressedColor, ButtonUnPressedColor;

		// ── Icon ─────────────────────────────────────────────────────
		private SerializedProperty IsChangeIconOnPressed, IconPressedSprite, IconUnPressedSprite;

		// ── Animation ────────────────────────────────────────────────
		private SerializedProperty expandAxis, verticalDirection, expandAmount;
		private SerializedProperty moveDistance, duration, easeType;

		// ── Squash & Stretch ─────────────────────────────────────────
		private SerializedProperty UseSquashStretch;
		private SerializedProperty SquashScale, StretchScale;
		private SerializedProperty SquashPhase, StretchPhase, SquashStretchEase;

		// ── Hover ────────────────────────────────────────────────────
		private SerializedProperty UseHoverEffect, HoverScale, HoverDuration, HoverEase;

		// ── Callbacks ────────────────────────────────────────────────
		private SerializedProperty OnClickEvent;

		// ── Sound ────────────────────────────────────────────────────
		private SerializedProperty AudioSource, ClickSound, SoundVolume, SoundPitch;

		// ── Foldout ──────────────────────────────────────────────────
		private bool _foldSound = true;

		// ── Foldout state (persisted via EditorPrefs) ────────────────
		private bool _foldReferences = true;
		private bool _foldFlags = true;
		private bool _foldAppearance = true;
		private bool _foldAnimation = true;
		private bool _foldSquash = true;
		private bool _foldHover = true;
		private bool _foldCallbacks = true;

		// ── Styles (built lazily) ────────────────────────────────────
		private GUIStyle _headerStyle;
		private GUIStyle _boxStyle;

		private void OnEnable()
		{
			// References
			button = serializedObject.FindProperty(nameof(AnimatedButton.button));
			icon = serializedObject.FindProperty(nameof(AnimatedButton.icon));
			label = serializedObject.FindProperty(nameof(AnimatedButton.label));
			ButtonRect = serializedObject.FindProperty(nameof(AnimatedButton.ButtonRect));
			ButtonImage = serializedObject.FindProperty(nameof(AnimatedButton.ButtonImage));
			IconImage = serializedObject.FindProperty(nameof(AnimatedButton.IconImage));

			// Flags
			IsSelfComponent = serializedObject.FindProperty(nameof(AnimatedButton.IsSelfComponent));
			IsBothMove = serializedObject.FindProperty(nameof(AnimatedButton.IsBothMove));
			IsIconMoveUpOnly = serializedObject.FindProperty(nameof(AnimatedButton.IsIconMoveUpOnly));
			IsIconMoveScale = serializedObject.FindProperty(nameof(AnimatedButton.IsIconMoveScale));
			IsScaleBackgroundOnly = serializedObject.FindProperty(nameof(AnimatedButton.IsScaleBackgroundOnly));
			IsHomeButton = serializedObject.FindProperty(nameof(AnimatedButton.IsHomeButton));

			// Appearance
			ButtonTypeEnum = serializedObject.FindProperty(nameof(AnimatedButton.ButtonTypeEnum));
			ButtonPressedSprite = serializedObject.FindProperty(nameof(AnimatedButton.ButtonPressedSprite));
			ButtonUnPressedSprite = serializedObject.FindProperty(nameof(AnimatedButton.ButtonUnPressedSprite));
			ButtonPressedColor = serializedObject.FindProperty(nameof(AnimatedButton.ButtonPressedColor));
			ButtonUnPressedColor = serializedObject.FindProperty(nameof(AnimatedButton.ButtonUnPressedColor));

			// Icon visuals
			IsChangeIconOnPressed = serializedObject.FindProperty(nameof(AnimatedButton.IsChangeIconOnPressed));
			IconPressedSprite = serializedObject.FindProperty(nameof(AnimatedButton.IconPressedSprite));
			IconUnPressedSprite = serializedObject.FindProperty(nameof(AnimatedButton.IconUnPressedSprite));

			// Animation
			expandAxis = serializedObject.FindProperty(nameof(AnimatedButton.expandAxis));
			verticalDirection = serializedObject.FindProperty(nameof(AnimatedButton.verticalDirection));
			expandAmount = serializedObject.FindProperty(nameof(AnimatedButton.expandAmount));
			moveDistance = serializedObject.FindProperty(nameof(AnimatedButton.moveDistance));
			duration = serializedObject.FindProperty(nameof(AnimatedButton.duration));
			easeType = serializedObject.FindProperty(nameof(AnimatedButton.easeType));

			// Squash & Stretch
			UseSquashStretch = serializedObject.FindProperty(nameof(AnimatedButton.UseSquashStretch));
			SquashScale = serializedObject.FindProperty(nameof(AnimatedButton.SquashScale));
			StretchScale = serializedObject.FindProperty(nameof(AnimatedButton.StretchScale));
			SquashPhase = serializedObject.FindProperty(nameof(AnimatedButton.SquashPhase));
			StretchPhase = serializedObject.FindProperty(nameof(AnimatedButton.StretchPhase));
			SquashStretchEase = serializedObject.FindProperty(nameof(AnimatedButton.SquashStretchEase));

			// Hover
			UseHoverEffect = serializedObject.FindProperty(nameof(AnimatedButton.UseHoverEffect));
			HoverScale = serializedObject.FindProperty(nameof(AnimatedButton.HoverScale));
			HoverDuration = serializedObject.FindProperty(nameof(AnimatedButton.HoverDuration));
			HoverEase = serializedObject.FindProperty(nameof(AnimatedButton.HoverEase));

			// Callbacks
			OnClickEvent = serializedObject.FindProperty(nameof(AnimatedButton.OnClickEvent));

			// Sound
			AudioSource = serializedObject.FindProperty(nameof(AnimatedButton.AudioSource));
			ClickSound = serializedObject.FindProperty(nameof(AnimatedButton.ClickSound));
			SoundVolume = serializedObject.FindProperty(nameof(AnimatedButton.SoundVolume));
			SoundPitch = serializedObject.FindProperty(nameof(AnimatedButton.SoundPitch));
		}

		public override void OnInspectorGUI()
		{
			BuildStyles();
			serializedObject.Update();

			// ── References ───────────────────────────────────────────
			_foldReferences = DrawSection("  References", _foldReferences, DrawReferences);

			// ── Behavior Flags ───────────────────────────────────────
			_foldFlags = DrawSection("  Behavior Flags", _foldFlags, DrawFlags);

			// ── Appearance ───────────────────────────────────────────
			_foldAppearance = DrawSection("  Appearance", _foldAppearance, DrawAppearance);

			// ── Animation ────────────────────────────────────────────
			_foldAnimation = DrawSection("  Animation", _foldAnimation, DrawAnimation);

			// ── Squash & Stretch ─────────────────────────────────────
			_foldSquash = DrawSection("  Squash & Stretch", _foldSquash, DrawSquashStretch);

			// ── Hover ────────────────────────────────────────────────
			_foldHover = DrawSection("  Hover Effect", _foldHover, DrawHover);

			// ── Callbacks ────────────────────────────────────────────
			_foldCallbacks = DrawSection("  Callbacks", _foldCallbacks, DrawCallbacks);

			_foldSound = DrawSection("  Sound", _foldSound, DrawSound);

			// ── Footer ───────────────────────────────────────────────
			EditorGUILayout.Space(4);
			EditorGUILayout.HelpBox("Requires DOTween. Ensure DOTween is initialized before playing.", MessageType.Info);

			// ── Editor Preview Buttons ───────────────────────────────
			if (Application.isPlaying)
			{
				EditorGUILayout.Space(4);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("▶  Preview Press", GUILayout.Height(26)))
					((AnimatedButton)target).InvokeAnimation();
				if (GUILayout.Button("↺  Preview Reset", GUILayout.Height(26)))
					((AnimatedButton)target).ResetButton();
				EditorGUILayout.EndHorizontal();
			}

			serializedObject.ApplyModifiedProperties();
		}

		// ─────────────────────────────────────────────────────────────
		//  Section drawers
		// ─────────────────────────────────────────────────────────────

		private void DrawReferences()
		{
			EditorGUILayout.PropertyField(button);
			EditorGUILayout.PropertyField(icon);
			EditorGUILayout.PropertyField(label);
			EditorGUILayout.PropertyField(ButtonImage);
			EditorGUILayout.PropertyField(IconImage);

			EditorGUILayout.Space(2);
			EditorGUILayout.PropertyField(IsSelfComponent, new GUIContent("Is Self Component"));
			if (!IsSelfComponent.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(ButtonRect);
				EditorGUI.indentLevel--;
			}
		}

		private void DrawFlags()
		{
			EditorGUILayout.PropertyField(IsHomeButton, new GUIContent("Home Button (auto-press on Start)"));
			EditorGUILayout.Space(2);
			EditorGUILayout.LabelField("Animation Mode", EditorStyles.miniLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(IsBothMove, new GUIContent("Background + Icon Move"));
			EditorGUILayout.PropertyField(IsIconMoveUpOnly, new GUIContent("Icon Move Only"));
			EditorGUILayout.PropertyField(IsIconMoveScale, new GUIContent("Icon Move + Scale"));
			EditorGUILayout.PropertyField(IsScaleBackgroundOnly, new GUIContent("Background Scale Only"));
			EditorGUI.indentLevel--;

			// Warn if multiple modes are enabled at once
			int modesEnabled = (IsBothMove.boolValue ? 1 : 0)
							 + (IsIconMoveUpOnly.boolValue ? 1 : 0)
							 + (IsIconMoveScale.boolValue ? 1 : 0)
							 + (IsScaleBackgroundOnly.boolValue ? 1 : 0);
			if (modesEnabled > 1)
				EditorGUILayout.HelpBox("Multiple animation modes are active. Only one should be enabled at a time.", MessageType.Warning);
		}

		private void DrawAppearance()
		{
			EditorGUILayout.PropertyField(ButtonTypeEnum, new GUIContent("Button Type"));
			EditorGUILayout.Space(2);

			var btnType = (AnimatedButton.ButtonType)ButtonTypeEnum.enumValueIndex;
			if (btnType == AnimatedButton.ButtonType.Color)
			{
				EditorGUILayout.PropertyField(ButtonPressedColor, new GUIContent("Pressed Color"));
				EditorGUILayout.PropertyField(ButtonUnPressedColor, new GUIContent("Default Color"));
			}
			else if (btnType == AnimatedButton.ButtonType.Sprite)
			{
				EditorGUILayout.PropertyField(ButtonPressedSprite, new GUIContent("Pressed Sprite"));
				EditorGUILayout.PropertyField(ButtonUnPressedSprite, new GUIContent("Default Sprite"));
			}

			EditorGUILayout.Space(4);
			EditorGUILayout.PropertyField(IsChangeIconOnPressed, new GUIContent("Change Icon on Press"));
			if (IsChangeIconOnPressed.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(IconPressedSprite, new GUIContent("Icon Pressed"));
				EditorGUILayout.PropertyField(IconUnPressedSprite, new GUIContent("Icon Default"));
				EditorGUI.indentLevel--;
			}
		}

		private void DrawAnimation()
		{
			EditorGUILayout.PropertyField(expandAxis, new GUIContent("Expand Axis"));

			if ((AnimatedButton.Axis)expandAxis.enumValueIndex == AnimatedButton.Axis.Vertical)
				EditorGUILayout.PropertyField(verticalDirection, new GUIContent("Vertical Direction"));

			EditorGUILayout.Space(2);
			EditorGUILayout.Slider(expandAmount, 1f, 2f, new GUIContent("Expand Amount"));
			EditorGUILayout.PropertyField(moveDistance, new GUIContent("Move Distance"));
			EditorGUILayout.PropertyField(duration, new GUIContent("Duration (s)"));
			EditorGUILayout.PropertyField(easeType, new GUIContent("Ease Type"));
		}

		private void DrawSquashStretch()
		{
			EditorGUILayout.PropertyField(UseSquashStretch, new GUIContent("Enable"));

			if (!UseSquashStretch.boolValue)
			{
				EditorGUILayout.HelpBox("Enable to add squash & stretch juice to icon movement.", MessageType.None);
				return;
			}

			EditorGUILayout.Space(4);

			// ── Scale fields with hint labels ────────────────────────
			EditorGUILayout.LabelField("Scale Multipliers  (multiplied against icon's original scale)", EditorStyles.miniLabel);
			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(SquashScale, new GUIContent("Squash Scale",
				"Per-axis multiplier on the squash phase.\n" +
				"Vertical example  →  X: 1.25  Y: 0.80  Z: 1.00\n" +
				"Horizontal example →  X: 0.80  Y: 1.25  Z: 1.00"));

			EditorGUILayout.PropertyField(StretchScale, new GUIContent("Stretch Scale",
				"Per-axis multiplier on the stretch/launch phase.\n" +
				"Vertical example  →  X: 0.75  Y: 1.35  Z: 1.00\n" +
				"Horizontal example →  X: 1.35  Y: 0.75  Z: 1.00"));

			EditorGUI.indentLevel--;

			// ── Inline preset buttons ────────────────────────────────
			EditorGUILayout.Space(2);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Presets:", GUILayout.Width(52));
			if (GUILayout.Button("Vertical", EditorStyles.miniButton)) ApplyPreset(vertical: true);
			if (GUILayout.Button("Horizontal", EditorStyles.miniButton)) ApplyPreset(vertical: false);
			if (GUILayout.Button("Subtle", EditorStyles.miniButton)) ApplySubtlePreset();
			if (GUILayout.Button("Bouncy", EditorStyles.miniButton)) ApplyBouncyPreset();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(4);

			// ── Phase timings ────────────────────────────────────────
			EditorGUILayout.LabelField("Phase Timing  (fraction of total duration)", EditorStyles.miniLabel);
			EditorGUI.indentLevel++;

			EditorGUILayout.Slider(SquashPhase, 0.1f, 0.5f, new GUIContent("Squash Phase",
				"How long the squash lasts as a fraction of total duration.\n0.2 = 20% of duration."));
			EditorGUILayout.Slider(StretchPhase, 0.1f, 0.5f, new GUIContent("Stretch Phase",
				"How long the stretch lasts as a fraction of total duration.\n0.4 = 40% of duration."));

			// Show the remaining settle phase as read-only
			float settle = 1f - SquashPhase.floatValue - StretchPhase.floatValue;
			using (new EditorGUI.DisabledScope(true))
				EditorGUILayout.FloatField(new GUIContent("Settle Phase (auto)", "Remainder = 1 - Squash - Stretch"), Mathf.Max(settle, 0.05f));

			if (settle < 0.05f)
				EditorGUILayout.HelpBox("Squash + Stretch phases exceed 95%. Settle phase will be clamped to 0.05.", MessageType.Warning);

			EditorGUI.indentLevel--;

			EditorGUILayout.Space(2);
			EditorGUILayout.PropertyField(SquashStretchEase, new GUIContent("Squash/Stretch Ease"));
		}

		private void DrawHover()
		{
			EditorGUILayout.PropertyField(UseHoverEffect, new GUIContent("Enable"));

			if (!UseHoverEffect.boolValue)
			{
				EditorGUILayout.HelpBox("Enable to scale the button slightly on pointer hover.", MessageType.None);
				return;
			}

			EditorGUI.indentLevel++;
			EditorGUILayout.Slider(HoverScale, 0.9f, 1.2f, new GUIContent("Hover Scale"));
			EditorGUILayout.PropertyField(HoverDuration, new GUIContent("Duration (s)"));
			EditorGUILayout.PropertyField(HoverEase, new GUIContent("Ease Type"));
			EditorGUI.indentLevel--;
		}

		private void DrawCallbacks()
		{
			EditorGUILayout.PropertyField(OnClickEvent, new GUIContent("On Click"));
		}

		// ─────────────────────────────────────────────────────────────
		//  Presets
		// ─────────────────────────────────────────────────────────────

		private void ApplyPreset(bool vertical)
		{
			Undo.RecordObject(target, "Apply Squash/Stretch Preset");
			SquashScale.vector3Value = vertical
				? new Vector3(1.25f, 0.80f, 1f)
				: new Vector3(0.80f, 1.25f, 1f);
			StretchScale.vector3Value = vertical
				? new Vector3(0.75f, 1.35f, 1f)
				: new Vector3(1.35f, 0.75f, 1f);
		}

		private void ApplySubtlePreset()
		{
			Undo.RecordObject(target, "Apply Subtle Preset");
			SquashScale.vector3Value = new Vector3(1.10f, 0.92f, 1f);
			StretchScale.vector3Value = new Vector3(0.90f, 1.15f, 1f);
		}

		private void ApplyBouncyPreset()
		{
			Undo.RecordObject(target, "Apply Bouncy Preset");
			SquashScale.vector3Value = new Vector3(1.50f, 0.65f, 1f);
			StretchScale.vector3Value = new Vector3(0.60f, 1.60f, 1f);
		}

		// ─────────────────────────────────────────────────────────────
		//  Helpers
		// ─────────────────────────────────────────────────────────────

		/// <summary>
		/// Draws a collapsible section with a thin colored header bar.
		/// Returns the new foldout state.
		/// </summary>
		private bool DrawSection(string title, bool foldout, System.Action drawContent)
		{
			EditorGUILayout.Space(4);

			// Header bar
			Rect headerRect = EditorGUILayout.BeginVertical(_boxStyle);
			foldout = EditorGUILayout.Foldout(foldout, title, true, _headerStyle);
			EditorGUILayout.EndVertical();

			if (foldout)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.Space(2);
				drawContent();
				EditorGUILayout.Space(4);
				EditorGUI.indentLevel--;
			}

			return foldout;
		}

		private void BuildStyles()
		{
			if (_headerStyle != null) return;

			_headerStyle = new GUIStyle(EditorStyles.foldout)
			{
				fontStyle = FontStyle.Bold,
				fontSize = 12,
			};
			_headerStyle.normal.textColor = EditorGUIUtility.isProSkin
				? new Color(0.85f, 0.85f, 0.85f)
				: new Color(0.15f, 0.15f, 0.15f);
			_headerStyle.onNormal.textColor = _headerStyle.normal.textColor;

			_boxStyle = new GUIStyle("HelpBox") { padding = new RectOffset(6, 6, 4, 4) };
		}

		private void DrawSound()
		{
			EditorGUILayout.PropertyField(ClickSound, new GUIContent("Click Clip"));

			EditorGUILayout.Space(2);
			EditorGUILayout.PropertyField(AudioSource, new GUIContent("Audio Source",
				"Optional. If left empty, AudioSource.PlayClipAtPoint is used as fallback."));

			if (AudioSource.objectReferenceValue == null)
				EditorGUILayout.HelpBox("No AudioSource assigned — will use PlayClipAtPoint at Camera position.", MessageType.None);

			EditorGUI.indentLevel++;
			EditorGUILayout.Slider(SoundVolume, 0f, 1f, new GUIContent("Volume"));
			EditorGUILayout.Slider(SoundPitch, 0.5f, 2f, new GUIContent("Pitch"));
			EditorGUI.indentLevel--;
		}
	}
}