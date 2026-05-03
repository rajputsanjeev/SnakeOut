using UnityEngine;
using UnityEditor;

namespace Framework
{
	[CustomEditor(typeof(PopupAnimationData))]
	public class PopupAnimationDataEditor : Editor
	{
		SerializedProperty animationType;

		SerializedProperty duration, delay, curve, useUnscaledTime;
		SerializedProperty startScale, endScale, overshoot, elasticity, damping;
		SerializedProperty slideOffset, gravity, bounceAmount;
		SerializedProperty startRotation, endRotation;
		SerializedProperty startAlpha, endAlpha;
		SerializedProperty maskStart, maskEnd;
		SerializedProperty particlePrefab, particleLocalPos;
		SerializedProperty useCanvasGroup, autoSetActive;
		SerializedProperty autoReverse;
		SerializedProperty invertMask, rectRevealStartSize, rectRevealEndSize, wipeStart, wipeEnd;

		void OnEnable()
		{
			animationType = serializedObject.FindProperty("animationType");
			autoReverse = serializedObject.FindProperty("autoReverse");

			duration = serializedObject.FindProperty("duration");
			delay = serializedObject.FindProperty("delay");
			curve = serializedObject.FindProperty("curve");
			useUnscaledTime = serializedObject.FindProperty("useUnscaledTime");

			startScale = serializedObject.FindProperty("startScale");
			endScale = serializedObject.FindProperty("endScale");
			overshoot = serializedObject.FindProperty("overshoot");
			elasticity = serializedObject.FindProperty("elasticity");
			damping = serializedObject.FindProperty("damping");

			slideOffset = serializedObject.FindProperty("slideOffset");
			gravity = serializedObject.FindProperty("gravity");
			bounceAmount = serializedObject.FindProperty("bounceAmount");

			startRotation = serializedObject.FindProperty("startRotation");
			endRotation = serializedObject.FindProperty("endRotation");

			startAlpha = serializedObject.FindProperty("startAlpha");
			endAlpha = serializedObject.FindProperty("endAlpha");

			maskStart = serializedObject.FindProperty("maskStart");
			maskEnd = serializedObject.FindProperty("maskEnd");

			particlePrefab = serializedObject.FindProperty("particlePrefab");
			particleLocalPos = serializedObject.FindProperty("particleLocalPos");

			useCanvasGroup = serializedObject.FindProperty("useCanvasGroup");
			autoSetActive = serializedObject.FindProperty("autoSetActive");

			invertMask = serializedObject.FindProperty("invertMask");
			rectRevealStartSize = serializedObject.FindProperty("rectRevealStartSize");
			rectRevealEndSize = serializedObject.FindProperty("rectRevealEndSize");
			wipeStart = serializedObject.FindProperty("wipeStart");
			wipeEnd = serializedObject.FindProperty("wipeEnd");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(animationType);
			EditorGUILayout.PropertyField(autoReverse);
			EditorGUILayout.Space(10);

			var type = (PopupAnimationType)animationType.enumValueIndex;

			Property(startScale);
			Property(endScale);

			// Always show core settings
			EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(duration);
			EditorGUILayout.PropertyField(delay);
			EditorGUILayout.PropertyField(curve);
			EditorGUILayout.PropertyField(useUnscaledTime);
			EditorGUILayout.Space(10);

			// DRAW BY TYPE
			DrawFieldsForType(type);

			// Common options
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(useCanvasGroup);
			EditorGUILayout.PropertyField(autoSetActive);

			EditorGUILayout.PropertyField(invertMask);
			EditorGUILayout.PropertyField(rectRevealStartSize);
			EditorGUILayout.PropertyField(rectRevealEndSize);
			EditorGUILayout.PropertyField(wipeStart);
			EditorGUILayout.PropertyField(wipeEnd);

			serializedObject.ApplyModifiedProperties();
		}

		void DrawFieldsForType(PopupAnimationType type)
		{
			switch (type)
			{
				// SCALE TYPES -------------------------
				case PopupAnimationType.ScaleIn:
				case PopupAnimationType.ElasticScale:
				case PopupAnimationType.BounceScale:
					Header("Scale Settings");
					Property(startScale);
					Property(endScale);
					break;

				case PopupAnimationType.PunchScale:
					Header("Punch Scale");
					Property(overshoot);
					Property(elasticity);
					break;

				// SLIDE TYPES -------------------------
				case PopupAnimationType.SlideFromTop:
				case PopupAnimationType.SlideFromBottom:
				case PopupAnimationType.SlideFromLeft:
				case PopupAnimationType.SlideFromRight:
					Header("Slide Settings");
					Property(slideOffset);
					break;

				case PopupAnimationType.SlideThenPop:
					Header("Slide + Pop");
					Property(slideOffset);
					Property(startScale);
					Property(endScale);
					Property(overshoot);
					break;

				case PopupAnimationType.DropAndBounce:
					Header("Drop Settings");
					Property(slideOffset);
					Property(bounceAmount);
					break;

				// FADE TYPES -------------------------
				case PopupAnimationType.FadeIn:
				case PopupAnimationType.FadeInScale:
				case PopupAnimationType.FadeInSlide:
					Header("Fade Settings");
					Property(startAlpha);
					Property(endAlpha);
					break;

				// ROTATION TYPES -------------------------
				case PopupAnimationType.RotateInZ:
				case PopupAnimationType.FlipInX:
				case PopupAnimationType.FlipInY:
				case PopupAnimationType.SwingIn:
					Header("Rotation Settings");
					Property(startRotation);
					Property(endRotation);
					break;

				// MASK TYPES -------------------------
				case PopupAnimationType.CircularMaskReveal:
				case PopupAnimationType.RectMaskReveal:
				case PopupAnimationType.WipeReveal:
				case PopupAnimationType.WipeRevealLeftToRight:
				case PopupAnimationType.WipeRevealRightToLeft:
				case PopupAnimationType.WipeRevealTopToBottom:
				case PopupAnimationType.WipeRevealBottomToTop:
					Header("Mask Settings");
					Property(maskStart);
					Property(maskEnd);
					Property(invertMask);
					Property(rectRevealStartSize);
					Property(rectRevealEndSize);
					Property(wipeStart);
					Property(wipeEnd);
					break;

				// PARTICLE EFFECTS -------------------------
				case PopupAnimationType.ParticleBurst:
				case PopupAnimationType.GlowPulse:
				case PopupAnimationType.ShockwaveReveal:
					Header("Particle Settings");
					Property(particlePrefab);
					Property(particleLocalPos);
					break;

				// COMBO TYPES -------------------------
				case PopupAnimationType.ScaleRotate:
					Header("Scale + Rotate");
					Property(startScale);
					Property(endScale);
					Property(startRotation);
					Property(endRotation);
					break;

				case PopupAnimationType.ScaleFade:
					Header("Scale + Fade");
					Property(startScale);
					Property(endScale);
					Property(startAlpha);
					Property(endAlpha);
					break;

				case PopupAnimationType.SlideFade:
					Header("Slide + Fade");
					Property(slideOffset);
					Property(startAlpha);
					Property(endAlpha);
					break;

				case PopupAnimationType.SlideBounce:
					Header("Slide + Bounce");
					Property(slideOffset);
					Property(bounceAmount);
					break;

				// SPECIAL -------------------------
				case PopupAnimationType.Jelly:
				case PopupAnimationType.Heartbeat:
					Header("Special Scale");
					Property(startScale);
					Property(endScale);
					break;

				case PopupAnimationType.ZoomToCamera:
				case PopupAnimationType.ShrinkFromBackground:
					Header("Z-Scale");
					Property(startScale);
					Property(endScale);
					break;

				// EXIT TYPES -------------------------
				case PopupAnimationType.ScaleOut:
					Header("Scale Out");
					Property(startScale);
					Property(endScale);
					break;

				case PopupAnimationType.FadeOut:
					Header("Fade Out");
					Property(startAlpha);
					Property(endAlpha);
					break;

				case PopupAnimationType.SlideOut:
					Header("Slide Out");
					Property(slideOffset);
					break;

				case PopupAnimationType.FlipOut:
					Header("Flip Out");
					Property(startRotation);
					Property(endRotation);
					break;

				case PopupAnimationType.ExplodeOut:
				case PopupAnimationType.DropOut:
					Header("Explode / Drop");
					Property(slideOffset);
					break;

				case PopupAnimationType.MaskClose:
					Header("Mask Close");
					Property(maskStart);
					Property(maskEnd);
					break;
			}
		}

		void Header(string title)
		{
			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
		}

		void Property(SerializedProperty p)
		{
			EditorGUILayout.PropertyField(p);
		}
	}
}