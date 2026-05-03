using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ComponentExtension
{
	public static Mesh GetMesh(this Component current)
	{
		var filter = current.GetComponentInChildren<MeshFilter>();
		var skinned = current.GetComponentInChildren<SkinnedMeshRenderer>();
		if (filter)
		{
			return filter.sharedMesh;
		}

		if (skinned)
		{
			return skinned.sharedMesh;
		}

		return null;
	}

	public static Mesh[] GetMeshes(this Component current)
	{
		var filters = current.GetComponentsInChildren<MeshFilter>();
		var skinned = current.GetComponentsInChildren<SkinnedMeshRenderer>();
		var meshes = new List<Mesh>();
		meshes.AddRange(filters.Select(x => x.sharedMesh));
		meshes.AddRange(skinned.Select(x => x.sharedMesh));
		return meshes.ToArray();
	}

	public static GameObject GetParent(this Component current)
	{
		return current == null ? null : current.gameObject.GetParent();
	}

	public static string GetPath(this Component current, bool includeSelf = true)
	{
		if (current == null || current.gameObject == null)
		{
			return "Null";
		}

		var path = current.gameObject.GetPath();
		return path;
	}

	public static bool IsEnabled(this Component current)
	{
		var enabled = current == null && current.gameObject.activeInHierarchy;
		//if(current is MonoBehaviour){enabled = enabled && current.As<MonoBehaviour>().enabled;}
		return enabled;
	}

	public static GameObject Remove(this Component current)
	{
		return Destroy(current);
	}

	public static GameObject Destroy(this Component current)
	{
		var source = current.gameObject;
		if (Application.isPlaying)
		{
			Object.Destroy(current);
		}
		else
		{
			Object.DestroyImmediate(current);
		}

		return source;
	}

	public static Component[] GetComponentsByInterface<T>(this Component current) where T : Component
	{
		if (current.IsNull())
		{
			return Array.Empty<Component>();
		}

		return current.gameObject.GetComponentsByInterface<T>();
	}

	public static T GetComponent<T>(this Component current, bool includeInactive) where T : Component
	{
		if (current.IsNull())
		{
			return null;
		}

		return current.gameObject.GetComponent<T>(includeInactive);
	}

	public static T[] GetComponents<T>(this Component current, bool includeInactive) where T : Component
	{
		if (current.IsNull())
		{
			return Array.Empty<T>();
		}

		return current.gameObject.GetComponents<T>(includeInactive);
	}

	public static T GetComponentInParent<T>(this Component current, bool includeInactive) where T : Component
	{
		if (current.IsNull())
		{
			return null;
		}

		return current.gameObject.GetComponentInParent<T>(includeInactive);
	}

	public static T GetComponentInChildren<T>(this Component current, bool includeInactive) where T : Component
	{
		if (current.IsNull())
		{
			return null;
		}

		return current.gameObject.GetComponentInChildren<T>(includeInactive);
	}
}