using UnityEditor;
using UnityEngine;
using static Framework.SeasonPack;

namespace Framework
{
	[CustomEditor(typeof(SeasonPack))]
	public class SeasonPackEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var mode = serializedObject.FindProperty("displayMode");
			EditorGUILayout.PropertyField(mode);
			PackManager.Mode selectedTheme = (PackManager.Mode)mode.enumValueIndex;

			if (selectedTheme == PackManager.Mode.MonthWise)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("theme"));
			}
			if (selectedTheme == PackManager.Mode.CategoryTag)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("categoryTag"));
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("packId"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("UI Sprites", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundSprite"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("upperSprite"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomSprite"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("badgeSprite"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Prices & Rewards", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("price"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("offPrice"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("priceLabelFormat"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("SmallItemReward"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("CoinReward"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("alwaysShow"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("useSpecificDates"));
			if (serializedObject.FindProperty("useSpecificDates").boolValue)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("startYear"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("startMonthExact"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("startDayExact"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("endYear"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("endMonthExact"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("endDayExact"));
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("useMonthRange"));
				if (serializedObject.FindProperty("useMonthRange").boolValue)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("startMonth"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("endMonth"));
				}
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("saleDaysOverride"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("priority"));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}