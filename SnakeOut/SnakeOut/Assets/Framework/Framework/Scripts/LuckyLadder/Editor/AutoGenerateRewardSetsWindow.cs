#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
	[Serializable]
	public class RewardIconTypeEntry
	{
		public Sprite icon;
		public RewardType type;
	}

	public class AutoGenerateRewardSetsWindow : EditorWindow
	{
		private int setsToCreate = 5;
		private string baseName = "AutoSet";
		private int maxItemsPerStep = 3;

		private int minQuantity = 1;
		private int maxQuantity = 100;

		private List<RewardIconTypeEntry> rewardEntries = new();

		[MenuItem("Tools/Data/Lucky Ladder/Auto Generate Sets")]
		public static void Open()
		{
			GetWindow<AutoGenerateRewardSetsWindow>("AutoGen Lucky Sets");
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Auto Generate Lucky Ladder Reward Sets", EditorStyles.boldLabel);

			EditorGUILayout.Space(6);
			setsToCreate = EditorGUILayout.IntField("Sets to create", setsToCreate);
			baseName = EditorGUILayout.TextField("Base name", baseName);
			maxItemsPerStep = EditorGUILayout.IntSlider("Max items per step", maxItemsPerStep, 1, 4);

			EditorGUILayout.Space(8);
			EditorGUILayout.LabelField("Quantity Range", EditorStyles.miniBoldLabel);
			minQuantity = EditorGUILayout.IntField("Min", minQuantity);
			maxQuantity = EditorGUILayout.IntField("Max", maxQuantity);

			minQuantity = Mathf.Max(1, minQuantity);
			maxQuantity = Mathf.Max(minQuantity, maxQuantity);

			EditorGUILayout.Space(10);
			DrawRewardEntries();

			EditorGUILayout.Space(12);
			if (GUILayout.Button("Generate Reward Sets", GUILayout.Height(36)))
			{
				if (EditorUtility.DisplayDialog(
					"Confirm",
					"This will create new RewardSet assets.\nContinue?",
					"Generate",
					"Cancel"))
				{
					Generate();
				}
			}
		}

		// ================= ENTRIES =================

		private void DrawRewardEntries()
		{
			EditorGUILayout.LabelField("Reward Entries (Icon + Type)", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Entry"))
				rewardEntries.Add(new RewardIconTypeEntry());
			if (GUILayout.Button("Clear"))
				rewardEntries.Clear();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(4);

			for (int i = 0; i < rewardEntries.Count; i++)
			{
				EditorGUILayout.BeginVertical("box");

				EditorGUILayout.BeginHorizontal();

				rewardEntries[i].icon = (Sprite)EditorGUILayout.ObjectField(
					rewardEntries[i].icon,
					typeof(Sprite),
					false,
					GUILayout.Width(64),
					GUILayout.Height(64));

				rewardEntries[i].type =
					(RewardType)EditorGUILayout.EnumPopup(rewardEntries[i].type);

				if (GUILayout.Button("X", GUILayout.Width(24)))
				{
					rewardEntries.RemoveAt(i);
					break;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			if (rewardEntries.Count == 0)
			{
				EditorGUILayout.HelpBox(
					"Add at least one Reward Entry (Icon + Type).",
					MessageType.Warning);
			}
		}

		// ================= GENERATION =================

		private void Generate()
		{
			if (rewardEntries.Count == 0)
			{
				EditorUtility.DisplayDialog("No Rewards", "Add reward entries first.", "OK");
				return;
			}

			System.IO.Directory.CreateDirectory("Assets/LuckyLadder/RewardSets");

			for (int s = 0; s < setsToCreate; s++)
			{
				var set = ScriptableObject.CreateInstance<LuckyLadderRewardSet>();
				set.SetName = $"{baseName}_{s + 1}";
				set.steps = new List<RewardStepData>();

				for (int st = 0; st < 6; st++)
				{
					var step = new RewardStepData();
					step.items = new List<RewardItem>();

					int itemCount = UnityEngine.Random.Range(1, maxItemsPerStep + 1);

					for (int i = 0; i < itemCount; i++)
					{
						var entry = rewardEntries[UnityEngine.Random.Range(0, rewardEntries.Count)];

						step.items.Add(new RewardItem
						{
							icon = entry.icon,
							type = entry.type,
							quantity = UnityEngine.Random.Range(minQuantity, maxQuantity + 1)
						});
					}

					set.steps.Add(step);
				}

				string path = $"Assets/LuckyLadder/RewardSets/{set.SetName}.asset";
				AssetDatabase.CreateAsset(set, path);
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log("Lucky Ladder reward sets generated successfully.");
		}
	}
}
#endif
