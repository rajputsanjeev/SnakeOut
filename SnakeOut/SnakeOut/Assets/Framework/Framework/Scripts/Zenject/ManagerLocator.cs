using System.Collections.Generic;
using UnityEngine;

public class ManagerLocator
{
	private static Dictionary<System.Type, object> _managers = new Dictionary<System.Type, object>();

	// Register a manager instance for a specific type
	public static void Register<T>(T manager)
	{
		var type = typeof(T);
		if (!_managers.ContainsKey(type))
		{
			_managers[type] = manager;
		}
	}

	// Retrieve the registered manager by type
	public static T Resolve<T>()
	{
		var type = typeof(T);
		if (_managers.ContainsKey(type))
		{
			return (T)_managers[type];
		}
		else
		{
			Debug.LogError($"Manager of type {type} not registered.");
			return default;
		}
	}

	public static void ClearList()
	{
		_managers.Clear();
	}
}
