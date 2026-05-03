using Framework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Framework.WatchAdsPower))]
public class WatchAdsPowerEditor : Editor
{
	bool resetUses = false;
	bool resetCooldown = false;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUILayout.Space(15);
		EditorGUILayout.LabelField("Watch Ads Tools", EditorStyles.boldLabel);

		var script = (Framework.WatchAdsPower)target;

		WatchAdsManager.Load();
		var data = WatchAdsManager.GetPowerUp(script.powerUpName);

		// ---------------------------------------------------------
		// SHOW LIVE JSON VALUES
		// ---------------------------------------------------------
		GUILayout.Space(10);
		EditorGUILayout.LabelField("Saved Data (Live JSON)", EditorStyles.boldLabel);

		if (data == null)
		{
			EditorGUILayout.HelpBox("No saved data found for this powerup.", MessageType.Warning);
		}
		else
		{
			EditorGUILayout.LabelField("📝 PowerUp Name:", data.powerUpName);
			EditorGUILayout.LabelField("📌 Max Uses:", data.maxUses.ToString());
			EditorGUILayout.LabelField("⌛ Cooldown:", data.cooldownEndUtc.ToString());

			EditorGUILayout.LabelField("▶ Current Uses:", data.currentUses.ToString());
			EditorGUILayout.LabelField("⏲ Last Used Time:", data.lastResetUtc.ToString());
		}

		GUILayout.Space(20);

		// ======================================================
		// REGISTER BUTTON
		// ======================================================
		if (GUILayout.Button("Register PowerUp"))
		{
			WatchAdsManager.RegisterPowerUp(new PowerUpData()
			{
				powerUpName = script.powerUpName,
				maxUses = script.maxUses,
				currentUses = 0,
				lastResetUtc = ""
			});

			Debug.Log("PowerUp Registered Successfully!");
		}

		GUILayout.Space(15);

		// ======================================================
		// UPDATE POWERUP SECTION
		// ======================================================
		EditorGUILayout.LabelField("Update Options", EditorStyles.boldLabel);

		resetUses = EditorGUILayout.Toggle("Reset Current Uses", resetUses);
		resetCooldown = EditorGUILayout.Toggle("Reset Cooldown Timer", resetCooldown);

		if (GUILayout.Button("Update Existing PowerUp Data"))
		{
			UpdatePowerUpData(script);
		}

		GUILayout.Space(20);

		// ======================================================
		// DELETE JSON BUTTON
		// ======================================================
		if (GUILayout.Button("Delete All Saved PowerUp JSON"))
		{
			if (EditorUtility.DisplayDialog(
				"Delete JSON?",
				"Are you sure you want to delete all saved power-up data?\nThis cannot be undone.",
				"Delete", "Cancel"))
			{
				DeleteJsonFile();
			}
		}
	}

	// ---------------------------------------------------------
	// UPDATE SPECIFIC POWERUP DATA
	// ---------------------------------------------------------
	private void UpdatePowerUpData(Framework.WatchAdsPower script)
	{
		WatchAdsManager.Load();
		var data = WatchAdsManager.GetPowerUp(script.powerUpName);

		if (data == null)
		{
			Debug.LogError("PowerUp not found in JSON: " + script.powerUpName);
			return;
		}

		// Update properties
		data.maxUses = script.maxUses;

		// Reset current uses if checkbox active
		if (resetUses)
		{
			data.currentUses = 0;
		}

		// Reset cooldown timer (ready to use immediately)
		if (resetCooldown)
		{
			data.lastResetUtc = "";
		}

		// Clamp invalid uses
		if (data.currentUses > data.maxUses)
			data.currentUses = data.maxUses;

		WatchAdsManager.Save();

		Debug.Log($"Updated PowerUp '{script.powerUpName}' Successfully!");
	}

	// ---------------------------------------------------------
	// DELETE JSON
	// ---------------------------------------------------------
	private void DeleteJsonFile()
	{
		string filePath = Application.persistentDataPath + "/powerup_data.json";

		if (System.IO.File.Exists(filePath))
		{
			System.IO.File.Delete(filePath);
			Debug.Log("Deleted file: " + filePath);
		}
		else
		{
			Debug.LogWarning("No JSON found to delete at: " + filePath);
		}

		WatchAdsManager.Data = new PowerUpSaveModel();
		WatchAdsManager.Save();

		AssetDatabase.Refresh();
	}
}