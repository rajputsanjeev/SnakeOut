#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	[Serializable]
	public class PrivilegeRewardEntry
	{
		public Sprite freeIcon;
		public RewardType freeType;

		public Sprite paidIcon;
		public RewardType paidType;
	}

	public class PrivilegeCycleRandomGenerator : EditorWindow
	{
		private PrivilegeCycleSet targetCycle;

		private List<PrivilegeRewardEntry> rewardEntries = new();
		private List<Sprite> spriteLibrary = new();

		private int selectedEntryIndex = -1;

		private int freeMinQty = 1;
		private int freeMaxQty = 5;

		private int paidMinQty = 1;
		private int paidMaxQty = 5;

		private Vector2 rewardScroll;
		private Vector2 libraryScroll;

		[MenuItem("Tools/Data/Privilege/28-Day Generator (Pro)")]
		public static void Open()
		{
			var w = GetWindow<PrivilegeCycleRandomGenerator>("Privilege Generator");
			w.minSize = new Vector2(600, 540);
		}

		private void OnGUI()
		{
			DrawHeader();
			DrawTarget();

			EditorGUILayout.Space(8);
			DrawSpriteLibrary();

			EditorGUILayout.Space(10);
			DrawRewardEntries();

			EditorGUILayout.Space(10);
			DrawQuantitySettings();

			EditorGUILayout.Space(12);
			DrawGenerateButton();
		}
		// ================= HEADER =================

		private void DrawHeader()
		{
			EditorGUILayout.LabelField("Privilege Cycle Generator", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox(
				"Click a reward entry to select it.\n" +
				"Then click a sprite in Sprite Library to assign it.",
				MessageType.Info);
		}

		private void DrawTarget()
		{
			targetCycle = (PrivilegeCycleSet)EditorGUILayout.ObjectField(
				"Privilege Cycle",
				targetCycle,
				typeof(PrivilegeCycleSet),
				false);
		}

		// ================= SPRITE LIBRARY =================

		private void DrawSpriteLibrary()
		{
			EditorGUILayout.LabelField("Sprite Library", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Sprite"))
				EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", 1001);

			if (GUILayout.Button("Clear"))
				spriteLibrary.Clear();
			EditorGUILayout.EndHorizontal();

			HandleSpritePicker();

			libraryScroll = EditorGUILayout.BeginScrollView(libraryScroll, GUILayout.Height(100));

			EditorGUILayout.BeginHorizontal();
			foreach (var sprite in spriteLibrary)
			{
				if (sprite == null) continue;

				if (GUILayout.Button(
					new GUIContent(AssetPreview.GetAssetPreview(sprite)),
					GUILayout.Width(64),
					GUILayout.Height(64)))
				{
					ShowAssignMenu(sprite);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		private void ShowAssignMenu(Sprite sprite)
		{
			if (selectedEntryIndex < 0 || selectedEntryIndex >= rewardEntries.Count)
			{
				EditorUtility.DisplayDialog(
					"No Entry Selected",
					"Please select a Reward Entry first.",
					"OK");
				return;
			}

			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Assign to FREE"), false, () =>
			{
				rewardEntries[selectedEntryIndex].freeIcon = sprite;
				Repaint();
			});

			menu.AddItem(new GUIContent("Assign to PAID"), false, () =>
			{
				rewardEntries[selectedEntryIndex].paidIcon = sprite;
				Repaint();
			});

			menu.ShowAsContext();
		}

		// ================= REWARD ENTRIES =================

		private void DrawRewardEntries()
		{
			EditorGUILayout.LabelField("Reward Entries", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Entry"))
				rewardEntries.Add(new PrivilegeRewardEntry());
			if (GUILayout.Button("Clear All"))
			{
				rewardEntries.Clear();
				selectedEntryIndex = -1;
			}
			EditorGUILayout.EndHorizontal();

			rewardScroll = EditorGUILayout.BeginScrollView(rewardScroll);

			for (int i = 0; i < rewardEntries.Count; i++)
			{
				DrawRewardCard(i);
			}

			EditorGUILayout.EndScrollView();
		}

		private void DrawRewardCard(int index)
		{
			bool selected = index == selectedEntryIndex;
			GUI.backgroundColor = selected ? new Color(0.8f, 0.9f, 1f) : Color.white;

			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Entry {index + 1}", EditorStyles.boldLabel);

			if (GUILayout.Button("Select", GUILayout.Width(60)))
				selectedEntryIndex = index;

			EditorGUILayout.EndHorizontal();

			DrawRewardRow("FREE", rewardEntries[index].freeIcon, ref rewardEntries[index].freeType);
			DrawRewardRow("PAID", rewardEntries[index].paidIcon, ref rewardEntries[index].paidType);

			if (GUILayout.Button("Remove Entry"))
			{
				rewardEntries.RemoveAt(index);
				if (selectedEntryIndex == index) selectedEntryIndex = -1;
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.Space(6);

			GUI.backgroundColor = Color.white;
		}

		private void DrawRewardRow(string label, Sprite icon, ref RewardType type)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label, GUILayout.Width(40));

			GUILayout.Box(
				icon ? AssetPreview.GetAssetPreview(icon) : null,
				GUILayout.Width(48),
				GUILayout.Height(48));

			type = (RewardType)EditorGUILayout.EnumPopup(type);
			EditorGUILayout.EndHorizontal();
		}

		// ================= QUANTITY =================

		private void DrawQuantitySettings()
		{
			EditorGUILayout.LabelField("Quantity Settings", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.LabelField("FREE Quantity");
			freeMinQty = EditorGUILayout.IntField("Min", freeMinQty);
			freeMaxQty = EditorGUILayout.IntField("Max", freeMaxQty);

			EditorGUILayout.Space(6);

			EditorGUILayout.LabelField("PAID Quantity");
			paidMinQty = EditorGUILayout.IntField("Min", paidMinQty);
			paidMaxQty = EditorGUILayout.IntField("Max", paidMaxQty);

			EditorGUILayout.EndVertical();

			freeMinQty = Mathf.Max(0, freeMinQty);
			freeMaxQty = Mathf.Max(freeMinQty, freeMaxQty);
			paidMinQty = Mathf.Max(0, paidMinQty);
			paidMaxQty = Mathf.Max(paidMinQty, paidMaxQty);
		}

		// ================= GENERATE =================

		private void DrawGenerateButton()
		{
			GUI.enabled = targetCycle != null && rewardEntries.Count > 0;

			if (GUILayout.Button("Generate 28-Day Cycle", GUILayout.Height(40)))
			{
				if (EditorUtility.DisplayDialog(
					"Confirm",
					"This will overwrite existing steps.",
					"Generate",
					"Cancel"))
				{
					Generate();
				}
			}

			GUI.enabled = true;
		}

		private void Generate()
		{
			Undo.RecordObject(targetCycle, "Generate Privilege Cycle");

			if (targetCycle.steps == null || targetCycle.steps.Length != 28)
				targetCycle.steps = new PrivilegeStepData[28];

			for (int i = 0; i < 28; i++)
			{
				if (targetCycle.steps[i] == null)
					targetCycle.steps[i] = new PrivilegeStepData();

				var step = targetCycle.steps[i];
				step.description = $"Auto-generated Day {i + 1}";

				var entry = rewardEntries[UnityEngine.Random.Range(0, rewardEntries.Count)];

				step.freeRewards = new[]
				{
					new RewardItem
					{
						icon = entry.freeIcon,
						type = entry.freeType,
						quantity = UnityEngine.Random.Range(freeMinQty, freeMaxQty + 1)
					}
				};

				step.paidRewards = new[]
				{
					new RewardItem
					{
						icon = entry.paidIcon,
						type = entry.paidType,
						quantity = UnityEngine.Random.Range(paidMinQty, paidMaxQty + 1)
					}
				};
			}

			EditorUtility.SetDirty(targetCycle);
			AssetDatabase.SaveAssets();
			EditorUtility.DisplayDialog("Done", "Privilege Cycle Generated", "OK");
		}

		// ================= OBJECT PICKER =================

		private void HandleSpritePicker()
		{
			if (Event.current.commandName == "ObjectSelectorClosed")
			{
				var sprite = EditorGUIUtility.GetObjectPickerObject() as Sprite;
				if (sprite != null && !spriteLibrary.Contains(sprite))
					spriteLibrary.Add(sprite);

				Repaint();
			}
		}
	}
}
#endif
