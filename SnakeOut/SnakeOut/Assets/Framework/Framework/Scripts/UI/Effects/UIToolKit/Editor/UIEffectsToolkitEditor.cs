using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIEffectsToolkit))]
public class UIEffectsToolkitEditor : Editor
{
	SerializedProperty effectType, target, duration, easeType, playOnAwake,delayOnAwake;
	SerializedProperty rotationSpeed, targetCanvasGroup, minAlpha, maxAlpha;
	SerializedProperty pumpScaleMultiplier;
	SerializedProperty onEffectComplete, playAnotherEffect;
	SerializedProperty fallStartYOffset, fallDropTime, fallBounceOffset, fallBounceUpTime, fallBounceDownTime;
	SerializedProperty fallStartFromScreenTop;
	SerializedProperty pumpLargeScaleMultiplier, pumpLargeScaleTime;
	SerializedProperty pumpSmallScaleMultiplier, pumpSmallScaleTime;
	SerializedProperty pumpMediumScaleMultiplier, pumpMediumScaleTime;
	SerializedProperty useSelfAsFirstStart, useSelfAsLoopStart;
	SerializedProperty leafFallFirstStartRect, leafFallLoopStartRect, leafFallEndRect, leafFallDuration;
	SerializedProperty fallStartRect;
	SerializedProperty shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, shakeSnapping, shakeFadeOut;
	SerializedProperty shakeLoop, shakeLoopInterval;
	SerializedProperty wingSwayDistance, wingSwayDuration, wingRotationAngle, wingRotationDuration;
	SerializedProperty windDriftRadius, windDriftDuration, windRotationStrength, windRotationDuration, windDriftRandomize;
	SerializedProperty swayRandomize, swayDistance, swayDuration, swayYOffset;
	SerializedProperty bounceMoveOffset, bounceMoveTime, bounceUpscaleFactor, bounceUpscaleTime, bounceDownscaleFactor, bounceDownscaleTime, bounceRecoverTime, bounceLoop;
	SerializedProperty sinWaveHorizontal, sinWaveAmplitude, sinWavePeriod, sinWaveLoops;
	SerializedProperty sinWaveUseInitialPos, sinWaveCenterRect;
	SerializedProperty sinWaveStartRect, sinWaveEndRect;
	SerializedProperty sinWaveDuration, sinWavePingPong;
	SerializedProperty bagSquashScale, bagStretchScale, bagBounceHeight, bagBounceDuration;
	SerializedProperty apexPause, bagSettleDuration, pauseBetweenLoops;
	SerializedProperty bagLaunchEase, bagFallEase, bagSettleEase;
	SerializedProperty onTopCoinsMoveUp, onScatterBottomCoins, onTopCoinsMoveDown;
	SerializedProperty loopEffect, loopCount;
	SerializedProperty rotationLeftAngle, rotationRightAngle, rotationDuration, pingPongLoop;
	SerializedProperty punchInScale, punchOutScale, punchDuration , punchIntensity;

	void OnEnable()
	{
		effectType = serializedObject.FindProperty("effectType");
		target = serializedObject.FindProperty("target");
		duration = serializedObject.FindProperty("duration");
		easeType = serializedObject.FindProperty("easeType");
		playOnAwake = serializedObject.FindProperty("playOnAwake");
		delayOnAwake = serializedObject.FindProperty("delayOnAwake");

		rotationSpeed = serializedObject.FindProperty("rotationSpeed");
		targetCanvasGroup = serializedObject.FindProperty("targetCanvasGroup");
		minAlpha = serializedObject.FindProperty("minAlpha");
		maxAlpha = serializedObject.FindProperty("maxAlpha");
		pumpScaleMultiplier = serializedObject.FindProperty("pumpScaleMultiplier");

		// NEW: callbacks
		onEffectComplete = serializedObject.FindProperty("OnEffectComplete");
		playAnotherEffect = serializedObject.FindProperty("PlayAnotherEffectCallback");

		// Add these lines:
		fallStartRect = serializedObject.FindProperty("fallStartRect");
		fallStartYOffset = serializedObject.FindProperty("fallStartYOffset");
		fallDropTime = serializedObject.FindProperty("fallDropTime");
		fallBounceOffset = serializedObject.FindProperty("fallBounceOffset");
		fallBounceUpTime = serializedObject.FindProperty("fallBounceUpTime");
		fallBounceDownTime = serializedObject.FindProperty("fallBounceDownTime");
		fallStartFromScreenTop = serializedObject.FindProperty("fallStartFromScreenTop");

		pumpLargeScaleMultiplier = serializedObject.FindProperty("pumpLargeScaleMultiplier");
		pumpLargeScaleTime = serializedObject.FindProperty("pumpLargeScaleTime");

		pumpSmallScaleMultiplier = serializedObject.FindProperty("pumpSmallScaleMultiplier");
		pumpSmallScaleTime = serializedObject.FindProperty("pumpSmallScaleTime");

		pumpMediumScaleMultiplier = serializedObject.FindProperty("pumpMediumScaleMultiplier");
		pumpMediumScaleTime = serializedObject.FindProperty("pumpMediumScaleTime");

		useSelfAsFirstStart = serializedObject.FindProperty("useSelfAsFirstStart");
		useSelfAsLoopStart = serializedObject.FindProperty("useSelfAsLoopStart");
		leafFallFirstStartRect = serializedObject.FindProperty("leafFallFirstStartRect");
		leafFallLoopStartRect = serializedObject.FindProperty("leafFallLoopStartRect");
		leafFallEndRect = serializedObject.FindProperty("leafFallEndRect");
		leafFallDuration = serializedObject.FindProperty("leafFallDuration");

		shakeDuration = serializedObject.FindProperty("shakeDuration");
		shakeStrength = serializedObject.FindProperty("shakeStrength");
		shakeVibrato = serializedObject.FindProperty("shakeVibrato");
		shakeRandomness = serializedObject.FindProperty("shakeRandomness");
		shakeSnapping = serializedObject.FindProperty("shakeSnapping");
		shakeFadeOut = serializedObject.FindProperty("shakeFadeOut");

		shakeLoop = serializedObject.FindProperty("shakeLoop");
		shakeLoopInterval = serializedObject.FindProperty("shakeLoopInterval");

		wingSwayDistance = serializedObject.FindProperty("wingSwayDistance");
		wingSwayDuration = serializedObject.FindProperty("wingSwayDuration");
		wingRotationAngle = serializedObject.FindProperty("wingRotationAngle");
		wingRotationDuration = serializedObject.FindProperty("wingRotationDuration");

		windDriftRadius = serializedObject.FindProperty("windDriftRadius");
		windDriftDuration = serializedObject.FindProperty("windDriftDuration");
		windRotationStrength = serializedObject.FindProperty("windRotationStrength");
		windRotationDuration = serializedObject.FindProperty("windRotationDuration");
		windDriftRandomize = serializedObject.FindProperty("windDriftRandomize");

		bounceLoop = serializedObject.FindProperty("bounceLoop");
		bounceMoveOffset = serializedObject.FindProperty("bounceMoveOffset");
		bounceMoveTime = serializedObject.FindProperty("bounceMoveTime");
		bounceUpscaleFactor = serializedObject.FindProperty("bounceUpscaleFactor");
		bounceUpscaleTime = serializedObject.FindProperty("bounceUpscaleTime");
		bounceDownscaleFactor = serializedObject.FindProperty("bounceDownscaleFactor");
		bounceDownscaleTime = serializedObject.FindProperty("bounceDownscaleTime");
		bounceRecoverTime = serializedObject.FindProperty("bounceRecoverTime");

		sinWaveHorizontal = serializedObject.FindProperty("sinWaveHorizontal");
		sinWaveAmplitude = serializedObject.FindProperty("sinWaveAmplitude");
		sinWavePeriod = serializedObject.FindProperty("sinWavePeriod");
		sinWaveLoops = serializedObject.FindProperty("sinWaveLoops");
		sinWaveUseInitialPos = serializedObject.FindProperty("sinWaveUseInitialPos");
		sinWaveCenterRect = serializedObject.FindProperty("sinWaveCenterRect");

		sinWaveStartRect = serializedObject.FindProperty("sinWaveStartRect");
		sinWaveEndRect = serializedObject.FindProperty("sinWaveEndRect");
		sinWaveDuration = serializedObject.FindProperty("sinWaveDuration");
		sinWavePingPong = serializedObject.FindProperty("sinWavePingPong");

		bagSquashScale = serializedObject.FindProperty("bagSquashScale");
		bagStretchScale = serializedObject.FindProperty("bagStretchScale");
		bagBounceHeight = serializedObject.FindProperty("bagBounceHeight");
		bagBounceDuration = serializedObject.FindProperty("bagBounceDuration");
		apexPause = serializedObject.FindProperty("apexPause");
		bagSettleDuration = serializedObject.FindProperty("bagSettleDuration");
		pauseBetweenLoops = serializedObject.FindProperty("pauseBetweenLoops");

		bagSquashScale = serializedObject.FindProperty("bagSquashScale");
		bagStretchScale = serializedObject.FindProperty("bagStretchScale");
		bagLaunchEase = serializedObject.FindProperty("bagLaunchEase");
		bagFallEase = serializedObject.FindProperty("bagFallEase");
		bagSettleEase = serializedObject.FindProperty("bagSettleEase");
		onTopCoinsMoveUp = serializedObject.FindProperty("onTopCoinsMoveUp");
		onScatterBottomCoins = serializedObject.FindProperty("onScatterBottomCoins");
		onTopCoinsMoveDown = serializedObject.FindProperty("onTopCoinsMoveDown");
		loopEffect = serializedObject.FindProperty("loopEffect");
		loopCount = serializedObject.FindProperty("loopCount");

		rotationLeftAngle = serializedObject.FindProperty("rotationLeftAngle");
		rotationRightAngle = serializedObject.FindProperty("rotationRightAngle");
		rotationDuration = serializedObject.FindProperty("rotationDuration");
		pingPongLoop = serializedObject.FindProperty("pingPongLoop");

		punchInScale = serializedObject.FindProperty("punchInScale");
		punchOutScale = serializedObject.FindProperty("punchOutScale");
		punchDuration = serializedObject.FindProperty("punchDuration");
		punchIntensity = serializedObject.FindProperty("punchIntensity");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(effectType);
		EditorGUILayout.PropertyField(target);
		if (GUILayout.Button("Get Target From Rect"))
		{
			foreach (var obj in targets)
			{
				UIEffectsToolkit toolkit = (UIEffectsToolkit)obj;
				RectTransform rect = toolkit.GetComponent<RectTransform>();
				if (rect != null)
				{
					Undo.RecordObject(toolkit, "Assign RectTransform as Target");
					toolkit.target = rect;
					EditorUtility.SetDirty(toolkit);
				}
			}
			serializedObject.Update();
		}
		EditorGUILayout.PropertyField(playOnAwake);
		EditorGUILayout.PropertyField(delayOnAwake);

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex != UIEffectsToolkit.UIEffectType.None)
			EditorGUILayout.PropertyField(duration);

		var showsEase =
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PopupElastic ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.FallBounce ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.SlideInFromLeft ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.FlipReveal ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PumpSequence;
		if (showsEase)
			EditorGUILayout.PropertyField(easeType);

		var showsRotation =
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.RotateLoop ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.RotateAndPumpLoop;
		if (showsRotation)
			EditorGUILayout.PropertyField(rotationSpeed);

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.FadeInOutLoop)
		{
			EditorGUILayout.PropertyField(targetCanvasGroup);
			EditorGUILayout.PropertyField(minAlpha);
			EditorGUILayout.PropertyField(maxAlpha);
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.HeartBeatLoop ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.RotateAndPumpLoop ||
			(UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PumpSequence)
		{
			EditorGUILayout.PropertyField(pumpScaleMultiplier);
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.FallBounce)
		{
			EditorGUILayout.PropertyField(fallStartFromScreenTop, new GUIContent("Start From Screen Top"));

			if (!fallStartFromScreenTop.boolValue)
			{
				// Show manual offset only if not using screen top calculation
				EditorGUILayout.PropertyField(fallStartYOffset, new GUIContent("Start Y Offset (Px)"));
			}
			EditorGUILayout.PropertyField(fallStartRect, new GUIContent("Start Position Rect"));

			EditorGUILayout.PropertyField(fallDropTime, new GUIContent("Fall Drop Time (s)"));
			EditorGUILayout.PropertyField(fallBounceOffset, new GUIContent("Bounce Offset (Px)"));
			EditorGUILayout.PropertyField(fallBounceUpTime, new GUIContent("Bounce Up Time (s)"));
			EditorGUILayout.PropertyField(fallBounceDownTime, new GUIContent("Bounce Down Time (s)"));
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.Shake)
		{
			EditorGUILayout.PropertyField(shakeDuration, new GUIContent("Shake Duration"));
			EditorGUILayout.PropertyField(shakeStrength, new GUIContent("Shake Strength"));
			EditorGUILayout.PropertyField(shakeVibrato, new GUIContent("Vibrato"));
			EditorGUILayout.PropertyField(shakeRandomness, new GUIContent("Randomness Angle"));
			EditorGUILayout.PropertyField(shakeSnapping, new GUIContent("Snapping"));
			EditorGUILayout.PropertyField(shakeFadeOut, new GUIContent("Fade Out"));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(shakeLoop, new GUIContent("Loop Shake"));
			if (shakeLoop.boolValue)
			{
				EditorGUILayout.PropertyField(shakeLoopInterval, new GUIContent("Loop Interval (seconds)"));
			}
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.LeafBranchSwayEffect)
		{
			EditorGUILayout.PropertyField(leafFallFirstStartRect, new GUIContent("Leaf Start Rect"));
			EditorGUILayout.PropertyField(leafFallEndRect, new GUIContent("Leaf End Rect"));
			EditorGUILayout.PropertyField(leafFallDuration, new GUIContent("Fall Duration (s)"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Wing Effect Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(wingSwayDistance, new GUIContent("Sway Distance (px)"));
			EditorGUILayout.PropertyField(wingSwayDuration, new GUIContent("Sway Duration (s)"));
			EditorGUILayout.PropertyField(wingRotationAngle, new GUIContent("Rotation Angle (deg)"));
			EditorGUILayout.PropertyField(wingRotationDuration, new GUIContent("Rotation Duration (s)"));
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PumpSequence
			|| (UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PopupElasticWithPump
			|| (UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PopupElasticWithHeartBeat)
		{
			// Show pump sequence settings
			EditorGUILayout.PropertyField(pumpLargeScaleMultiplier, new GUIContent("Large Pump Scale ×"));
			EditorGUILayout.PropertyField(pumpLargeScaleTime, new GUIContent("Large Pump Time (s)"));

			EditorGUILayout.PropertyField(pumpSmallScaleMultiplier, new GUIContent("Small Pump Scale ×"));
			EditorGUILayout.PropertyField(pumpSmallScaleTime, new GUIContent("Small Pump Time (s)"));

			EditorGUILayout.PropertyField(pumpMediumScaleMultiplier, new GUIContent("Medium Pump Scale ×"));
			EditorGUILayout.PropertyField(pumpMediumScaleTime, new GUIContent("Medium Pump Time (s)"));

			if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PopupElasticWithHeartBeat)
			{
				EditorGUILayout.PropertyField(pumpScaleMultiplier);
			}
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.LeafFallLoop)
		{
			EditorGUILayout.PropertyField(useSelfAsFirstStart, new GUIContent("Use Self (Target) as First Start"));
			if (!useSelfAsFirstStart.boolValue)
			{
				EditorGUILayout.PropertyField(leafFallFirstStartRect, new GUIContent("First Start Rect"));
			}
			EditorGUILayout.PropertyField(useSelfAsLoopStart, new GUIContent("Use Self (Target) as Loop Start"));
			if (!useSelfAsLoopStart.boolValue)
			{
				EditorGUILayout.PropertyField(leafFallLoopStartRect, new GUIContent("Loop Start Rect"));
			}
			EditorGUILayout.PropertyField(leafFallEndRect, new GUIContent("End Rect"));
			EditorGUILayout.PropertyField(leafFallDuration, new GUIContent("Fall Duration (s)"));
		}


		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.LeafWindEffect)
		{
			EditorGUILayout.PropertyField(windDriftRadius, new GUIContent("Drift Radius (px)"));
			EditorGUILayout.PropertyField(windDriftDuration, new GUIContent("Drift Duration (s)"));
			EditorGUILayout.PropertyField(windRotationStrength, new GUIContent("Rotation Strength (deg)"));
			EditorGUILayout.PropertyField(windRotationDuration, new GUIContent("Rotation Duration (s)"));
			EditorGUILayout.PropertyField(windDriftRandomize, new GUIContent("Randomize Drift Target"));
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.LeafBranchSwayEffect)
		{
			EditorGUILayout.PropertyField(swayDistance, new GUIContent("Sway Distance (px)"));
			EditorGUILayout.PropertyField(swayDuration, new GUIContent("Sway Duration (s)"));
			EditorGUILayout.PropertyField(swayYOffset, new GUIContent("Vertical Offset (px)"));
			EditorGUILayout.PropertyField(swayRandomize, new GUIContent("Randomize Sway Direction"));
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.BouncePop)
		{
			EditorGUILayout.PropertyField(bounceLoop, new GUIContent("Loop Effect"));
			EditorGUILayout.PropertyField(bounceMoveOffset, new GUIContent("Move Up Offset (px)"));
			EditorGUILayout.PropertyField(bounceMoveTime, new GUIContent("Move Up Time (s)"));
			EditorGUILayout.PropertyField(bounceUpscaleFactor, new GUIContent("Upscale × Factor"));
			EditorGUILayout.PropertyField(bounceUpscaleTime, new GUIContent("Upscale Time (s)"));
			EditorGUILayout.PropertyField(bounceDownscaleFactor, new GUIContent("Downscale × Factor"));
			EditorGUILayout.PropertyField(bounceDownscaleTime, new GUIContent("Downscale Time (s)"));
			EditorGUILayout.PropertyField(bounceRecoverTime, new GUIContent("Recover Time (s)"));
		}
		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.SinWave)
		{
			EditorGUILayout.LabelField("Sin Wave Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(sinWaveHorizontal, new GUIContent("Horizontal Wave"));
			EditorGUILayout.PropertyField(sinWaveAmplitude, new GUIContent("Amplitude (px)"));
			EditorGUILayout.PropertyField(sinWavePeriod, new GUIContent("Period (s)"));
			EditorGUILayout.PropertyField(sinWaveLoops, new GUIContent("Loops (-1 = Infinite)"));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(sinWaveUseInitialPos, new GUIContent("Use Initial Position as Center"));
			if (!sinWaveUseInitialPos.boolValue)
			{
				EditorGUILayout.PropertyField(sinWaveCenterRect, new GUIContent("Custom Center Rect"));
			}
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.SinWaveMove)
		{
			EditorGUILayout.LabelField("Sin Wave Move", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(sinWaveStartRect, new GUIContent("Start Rect"));
			EditorGUILayout.PropertyField(sinWaveEndRect, new GUIContent("End Rect"));
			EditorGUILayout.PropertyField(sinWaveDuration, new GUIContent("Duration (s)"));
			EditorGUILayout.PropertyField(sinWaveAmplitude, new GUIContent("Amplitude (px)"));
			EditorGUILayout.PropertyField(sinWaveLoops, new GUIContent("Loops (-1 = Infinite)"));
			EditorGUILayout.PropertyField(sinWavePingPong, new GUIContent("Ping Pong"));
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.BounceSquashStretch)
		{
			EditorGUILayout.LabelField("Bounce Squash Stretch", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(bagSquashScale, new GUIContent("Squash Scale (XYZ)"));
			EditorGUILayout.PropertyField(bagStretchScale, new GUIContent("Stretch Scale (XYZ)"));
			EditorGUILayout.PropertyField(bagBounceHeight, new GUIContent("Bounce Height (px)"));
			EditorGUILayout.PropertyField(bagBounceDuration, new GUIContent("Total Bounce Duration"));
			EditorGUILayout.PropertyField(apexPause, new GUIContent("Apex Pause"));

			EditorGUILayout.PropertyField(bagSettleDuration, new GUIContent("Settle Duration"));
			EditorGUILayout.PropertyField(pauseBetweenLoops, new GUIContent("Loop Pause"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Easing", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(bagLaunchEase);
			EditorGUILayout.PropertyField(bagFallEase);
			EditorGUILayout.PropertyField(bagSettleEase);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Callbacks", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(onTopCoinsMoveUp);
			EditorGUILayout.PropertyField(onScatterBottomCoins);
			EditorGUILayout.PropertyField(onTopCoinsMoveDown);

			EditorGUILayout.PropertyField(loopEffect, new GUIContent("Loop Effect"));

			if (loopEffect.boolValue)
			{
				EditorGUILayout.PropertyField(loopCount, new GUIContent("Loop Count (-1 = Infinite)"));
			}
		}

		if ((UIEffectsToolkit.UIEffectType)effectType.enumValueIndex == UIEffectsToolkit.UIEffectType.PingPongRotation)
		{
			EditorGUILayout.LabelField("Ping Pong Rotation", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(rotationLeftAngle, new GUIContent("Left Angle (deg)"));
			EditorGUILayout.PropertyField(rotationRightAngle, new GUIContent("Right Angle (deg)"));
			EditorGUILayout.PropertyField(rotationDuration, new GUIContent("Cycle Duration (s)"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Punch Scale", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(punchInScale, new GUIContent("Punch In Scale"));
			EditorGUILayout.PropertyField(punchOutScale, new GUIContent("Punch Out Scale"));
			EditorGUILayout.PropertyField(punchDuration, new GUIContent("Punch Duration (s)"));
			EditorGUILayout.PropertyField(punchIntensity, new GUIContent("Punch Strength (0-1)"));

			EditorGUILayout.PropertyField(pingPongLoop, new GUIContent("Infinite Loop"));
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Callbacks", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(onEffectComplete);
		EditorGUILayout.PropertyField(playAnotherEffect);

		serializedObject.ApplyModifiedProperties();
	}
}
