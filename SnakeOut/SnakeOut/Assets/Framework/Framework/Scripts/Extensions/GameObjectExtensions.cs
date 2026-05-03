using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameObjectExtensions
{
	public static List<Func<GameObject, string>> GetPathMethods = new();
	public static List<Func<GameObject, bool>> IsPrefabMethods = new();
	public static List<Func<GameObject, bool>> IsPrefabFileMethods = new();

	public static GameObject[] GetByName(this GameObject current, string name, bool includeInactive = true)
	{
		if (current == null)
		{
			return null;
		}

		var all = current.GetComponentsInChildren<Transform>(includeInactive);
		var matches = new List<GameObject>();
		foreach (var transform in all)
		{
			if (transform.name == name)
			{
				matches.Add(transform.gameObject);
			}
		}

		return matches.ToArray();
	}

	public static Mesh GetMesh(this GameObject current)
	{
		return current == null ? null : current.transform.GetMesh();
	}

	public static Mesh[] GetMeshes(this GameObject current)
	{
		return current == null ? Array.Empty<Mesh>() : current.transform.GetMeshes();
	}

	#region Layers&Tags
	public static void ReplaceLayer(this GameObject current, string search, string replace)
	{
		var layer = LayerMask.NameToLayer(replace);
		foreach (var item in current.GetByLayer(search))
		{
			item.layer = layer;
		}
	}

	public static void ReplaceTag(this GameObject current, string search, string replace)
	{
		foreach (var item in current.GetByTag(search))
		{
			item.tag = replace;
		}
	}

	public static GameObject[] GetByLayer(this GameObject current, string search)
	{
		var layer = LayerMask.NameToLayer(search);
		var results = new List<GameObject>();
		var children = current.GetComponentsInChildren<Transform>(true);
		foreach (var child in children)
		{
			if (child.gameObject.layer == layer)
			{
				results.Add(child.gameObject);
			}
		}

		return results.ToArray();
	}

	public static GameObject[] GetByTag(this GameObject current, string search)
	{
		var results = new List<GameObject>();
		var children = current.GetComponentsInChildren<Transform>(true);
		foreach (var child in children)
		{
			if (child.gameObject.CompareTag(search))
			{
				results.Add(child.gameObject);
			}
		}

		return results.ToArray();
	}

	public static void SetAllTags(this GameObject current, string name)
	{
		var children = current.GetComponentsInChildren<Transform>(true);
		foreach (var child in children)
		{
			child.gameObject.tag = name;
		}
	}

	public static void SetAllLayers(this GameObject current, string name)
	{
		var layer = LayerMask.NameToLayer(name);
		var children = current.GetComponentsInChildren<Transform>(true);
		foreach (var child in children)
		{
			child.gameObject.layer = layer;
		}
	}

	public static void SetLayer(this GameObject current, string name)
	{
		var layer = LayerMask.NameToLayer(name);
		current.layer = layer;
	}

	#endregion

	#region Collisions

	public static void ToggleAllCollisions(this GameObject current, bool state)
	{
		current.ToggleComponents(state, true, typeof(Collider));
	}

	public static void ToggleAllTriggers(this GameObject current, bool state)
	{
		var colliders = current.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			collider.isTrigger = state;
		}
	}

	public static void ToggleIgnoreCollisions(this GameObject current, GameObject target, bool state)
	{
		var colliders = current.GetComponentsInChildren<Collider>();
		var targetColliders = target.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			if (!collider.enabled)
			{
				continue;
			}

			foreach (var targetCollider in targetColliders)
			{
				if (collider == targetCollider)
				{
					continue;
				}

				if (!targetCollider.enabled)
				{
					continue;
				}

				Physics.IgnoreCollision(collider, targetCollider, state);
			}
		}
	}

	public static void IgnoreAllCollisions(this GameObject current, GameObject target)
	{
		current.ToggleIgnoreCollisions(target, true);
	}

	public static void UnignoreAllCollisions(this GameObject current, GameObject target)
	{
		current.ToggleIgnoreCollisions(target, false);
	}
	#endregion

	#region Components
	public static GameObject Destroy<Type>(this GameObject current) where Type : Component
	{
		return current.RemoveComponent<Type>();
	}

	public static GameObject Remove<Type>(this GameObject current) where Type : Component
	{
		return current.RemoveComponent<Type>();
	}

	public static GameObject RemoveComponent<Type>(this GameObject current) where Type : Component
	{
		var target = current.GetComponent<Type>();
		target?.GetComponent<Type>();

		return current;
	}

	public static Type Add<Type>(this GameObject current) where Type : Component
	{
		return current.AddComponent<Type>();
	}

	public static Type GetChild<Type>(this GameObject current)
	{
		return current.GetComponent<Type>();
	}

	public static Component[] GetComponentsByInterface<T>(this GameObject current)
	{
		var results = new List<Component>();
		var items = current.GetComponentsInChildren<Component>(true);
		foreach (var item in items)
		{
			if (item.GetType().IsAssignableFrom(typeof(T)))
			{
				results.Add(item);
			}
		}

		return results.ToArray();
	}

	public static bool Has<T>(this GameObject current, bool includeInactive = false) where T : Component
	{
		return current.HasComponent<T>(includeInactive);
	}

	public static bool HasComponent<T>(this GameObject current, bool includeInactive = false) where T : Component
	{
		if (current == null)
		{
			return false;
		}

		return current.GetComponent<T>(includeInactive);
	}

	public static T GetComponent<T>(this GameObject current, bool includeInactive = false) where T : Component
	{
		if (current == null)
		{
			return null;
		}

		var results = current.GetComponentsInChildren<T>(includeInactive);
		foreach (var item in results)
		{
			if (item.transform == current.transform)
			{
				return item;
			}
		}

		return null;
	}

	public static T[] GetComponents<T>(this GameObject current, bool includeInactive = false) where T : Component
	{
		if (current == false)
		{
			return null;
		}

		var results = new List<T>();
		var search = current.GetComponentsInChildren<T>(includeInactive);
		foreach (var item in search)
		{
			if (item.transform == current.transform)
			{
				results.Add(item);
			}
		}

		return results.ToArray();
	}

	public static T GetComponentInParent<T>(this GameObject current, bool includeInactive = false) where T : Component
	{
		if (current == null)
		{
			return null;
		}

		var results = current.GetComponentsInParent<T>(includeInactive);
		if (results.Length > 0)
		{
			return results[0];
		}

		return null;
	}

	public static T GetComponentInChildren<T>(this GameObject current, bool includeInactive = false) where T : Component
	{
		var results = current.GetComponentsInChildren<T>(includeInactive);
		if (results.Length > 0)
		{
			return results[0];
		}

		return null;
	}

	public static void EnableComponents(this GameObject current, params Type[] types)
	{
		current.ToggleComponents(true, false, types);
	}

	public static void DisableComponents(this GameObject current, params Type[] types)
	{
		current.ToggleComponents(false, false, types);
	}

	public static void EnableAllComponents(this GameObject current, params Type[] types)
	{
		current.ToggleComponents(true, true, types);
	}

	public static void DisableAllComponents(this GameObject current, params Type[] types)
	{
		current.ToggleComponents(false, true, types);
	}

	public static void ToggleComponents(this GameObject current, bool state, bool all = true, params Type[] types)
	{
		var renderer = typeof(Renderer);
		var collider = typeof(Collider);
		var behaviour = typeof(MonoBehaviour);
		var animation = typeof(Animation);
		foreach (var type in types)
		{
			var components = all ? current.GetComponentsInChildren(type, true) : current.GetComponents(type);
			foreach (var item in components)
			{
				var itemType = item.GetType();
				bool Matches(Type x) => itemType.IsAssignableFrom(x);
				if (SubClass(renderer) || Matches(renderer))
				{
					((Renderer)item).enabled = state;
				}
				else if (SubClass(behaviour) || Matches(behaviour))
				{
					((MonoBehaviour)item).enabled = state;
				}
				else if (SubClass(collider) || Matches(collider))
				{
					((Collider)item).enabled = state;
				}
				else if (SubClass(animation) || Matches(animation))
				{
					((Animation)item).enabled = state;
				}

				bool SubClass(Type x) => itemType.IsSubclassOf(x);
			}
		}
	}

	public static void ToggleAllVisible(this GameObject current, bool state)
	{
		current.ToggleComponents(state, true, typeof(Renderer));
	}
	#endregion

	#region Utility

	public static void Remove(this GameObject current)
	{
		current.Destroy();
	}

	public static void Destroy(this GameObject current)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(current);
		}
		else
		{
			Object.DestroyImmediate(current);
		}
	}

	public static void MoveTo(this GameObject current, Vector3 location, bool useX = true, bool useY = true,
		bool useZ = true)
	{
		var position = current.transform.position;
		if (useX)
		{
			position.x = location.x;
		}

		if (useY)
		{
			position.y = location.y;
		}

		if (useZ)
		{
			position.z = location.z;
		}

		current.transform.position = position;
	}

	public static string GetPath(this GameObject current)
	{
		if (current == null || current.transform == null)
		{
			return string.Empty;
		}

		var path = current.transform.name;
		foreach (var method in GameObjectExtensions.GetPathMethods)
		{
			path = method(current);
		}

		var parent = current.transform.parent;
		while (parent != null)
		{
			path = parent.name + "/" + path;
			parent = parent.parent;
		}

		return "/" + path + "/";
	}

	public static GameObject GetParent(this GameObject current)
	{
		if (current.transform.parent != null)
		{
			return current.transform.parent.gameObject;
		}

		return null;
	}

	public static bool IsPrefab(this GameObject current)
	{
		if (current == null)
		{
			return false;
		}

		foreach (var method in IsPrefabMethods)
		{
			if (method(current))
			{
				return true;
			}
		}

		return false;
	}

	public static bool InPrefabFile(this Component current)
	{
		if (current == null)
		{
			return false;
		}

		return current.gameObject.IsPrefabFile();
	}

	public static bool IsPrefabFile(this GameObject current)
	{
		if (current == null)
		{
			return false;
		}

		if (current.hideFlags is HideFlags.NotEditable or HideFlags.HideAndDontSave)
		{
			return true;
		}

		foreach (var method in GameObjectExtensions.IsPrefabFileMethods)
		{
			if (method(current))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if this GameObject and its parents are active, up to maxDepth.
	/// maxDepth = 0  → only check this object
	/// maxDepth = 1  → check this + direct parent
	/// maxDepth = -1 → unlimited (full chain)
	/// </summary>
	public static bool IsActiveWithParents(this GameObject obj, int maxDepth = -1)
	{
		if (obj == null) return false;

		Transform current = obj.transform;
		int depth = 0;

		while (current != null)
		{
			if (!current.gameObject.activeSelf)
				return false;

			if (maxDepth >= 0 && depth >= maxDepth)
				break;

			current = current.parent;
			depth++;
		}

		return true;
	}

	/// <summary>
	/// MonoBehaviour shortcut
	/// </summary>
	public static bool IsActiveWithParents(this MonoBehaviour mb, int maxDepth = -1)
	{
		if (mb == null) return false;
		return mb.gameObject.IsActiveWithParents(maxDepth);
	}
	#endregion
}