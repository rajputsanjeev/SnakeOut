using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for MonoBehaviour objects
/// </summary>
public static class MonoBehaviourExtensions
{
#if UNITY_EDITOR
	/// <summary>
	/// Start this behaviour running in edit mode
	/// This sets runInEditMode to true, which, if the behaviour is enabled will call OnDisable and then OnEnable
	/// </summary>
	/// <param name="behaviour">The behaviour</param>
	public static void StartRunInEditMode(this MonoBehaviour behaviour)
	{
		behaviour.runInEditMode = true;
	}

	/// <summary>
	/// Stop this behaviour running in edit mode
	/// If the behaviour is enabled, we first disable it so that OnDisable is called. Then we set runInEditMode to false. Then, if the behaviour was enabled, we re-enable it
	/// </summary>
	/// <param name="behaviour">The behaviour</param>
	public static void StopRunInEditMode(this MonoBehaviour behaviour)
	{
		var wasEnabled = behaviour.enabled;
		if (wasEnabled)
			behaviour.enabled = false;

		behaviour.runInEditMode = false;

		if (wasEnabled)
			behaviour.enabled = true;
	}
#endif
	
	public static bool IsEnabled(this MonoBehaviour current)
	{
		return !current.IsNull() && current.enabled && current.gameObject.activeInHierarchy;
	}

	/// <summary>
	/// Destroy all children of a transform except the ones whose names are in the exception list.
	/// </summary>
	public static void DestroyChildrenExcept(this Transform parent, List<string> exceptionNames)
	{
		// If null or empty, destroy all children
		if (exceptionNames == null)
			exceptionNames = new List<string>();

		// Loop backwards to avoid index issues
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Transform child = parent.GetChild(i);

			// If child name is in exception list → skip it
			if (exceptionNames.Contains(child.name))
				continue;

			// Destroy depending on playmode
			if (Application.isPlaying)
				Object.Destroy(child.gameObject);
			else
				Object.DestroyImmediate(child.gameObject);
		}
	}

	/// <summary>
	/// Destroy all children of a transform.
	/// </summary>
	public static void DestroyChild(this Transform parent)
	{
		// Loop backwards to avoid index issues
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Transform child = parent.GetChild(i);

			// Destroy depending on playmode
			if (Application.isPlaying)
				Object.Destroy(child.gameObject);
			else
				Object.DestroyImmediate(child.gameObject);
		}
	}
}