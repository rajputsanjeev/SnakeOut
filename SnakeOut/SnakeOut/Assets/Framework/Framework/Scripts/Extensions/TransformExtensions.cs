using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for UnityEngine.Transform.
/// </summary>
public static class TransformExtensions
{
	/// <summary>
	/// The SetGlobalScale method is used to set a transform scale based on a global scale instead of a local scale.
	/// </summary>
	/// <param name="transform">The reference to the transform to scale.</param>
	/// <param name="globalScale">A Vector3 of a global scale to apply to the given transform.</param>
	public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
	{
		transform.localScale = Vector3.one;
		var lossyScale = transform.lossyScale;
		transform.localScale = new Vector3(globalScale.x / lossyScale.x, 
			globalScale.y / lossyScale.y, globalScale.z / lossyScale.z);
	}

	/// <summary>
	/// Makes the given game objects children of the transform.
	/// </summary>
	/// <param name="transform">Parent transform.</param>
	/// <param name="children">Game objects to make children.</param>
	public static void AddChildren(this Transform transform, GameObject[] children)
	{
		Array.ForEach(children, child => child.transform.parent = transform);
	}

	/// <summary>
	/// Makes the game objects of given components children of the transform.
	/// </summary>
	/// <param name="transform">Parent transform.</param>
	/// <param name="children">Components of game objects to make children.</param>
	public static void AddChildren(this Transform transform, Component[] children)
	{
		Array.ForEach(children, child => child.transform.parent = transform);
	}

	/// <summary>
	/// Sets the position of a transform's children to zero.
	/// </summary>
	/// <param name="transform">Parent transform.</param>
	/// <param name="recursive">Also reset ancestor positions?</param>
	public static void ResetChildPositions(this Transform transform, bool recursive = false)
	{
		foreach (Transform child in transform)
		{
			child.position = Vector3.zero;

			if (recursive)
			{
				child.ResetChildPositions(true);
			}
		}
	}

	/// <summary>
	/// Sets the layer of the transform's children.
	/// </summary>
	/// <param name="transform">Parent transform.</param>
	/// <param name="layerName">Name of layer.</param>
	/// <param name="recursive">Also set ancestor layers?</param>
	public static void SetChildLayers(this Transform transform, string layerName, bool recursive = false)
	{
		var layer = LayerMask.NameToLayer(layerName);
		SetChildLayersHelper(transform, layer, recursive);
	}

	static void SetChildLayersHelper(Transform transform, int layer, bool recursive)
	{
		foreach (Transform child in transform)
		{
			child.gameObject.layer = layer;

			if (recursive)
			{
				SetChildLayersHelper(child, layer, true);
			}
		}
	}

	/// <summary>
	/// Sets the x component of the transform's position.
	/// </summary>
	/// <param name="x">Value of x.</param>
	public static void SetX( this Transform transform, float x, bool localPosition = false )
	{
		if (localPosition)
			transform.localPosition = new Vector3( x, transform.localPosition.y, transform.localPosition.z );
		else
			transform.position = new Vector3( x, transform.position.y, transform.position.z );
	}

	/// <summary>
	/// Sets the y component of the transform's position.
	/// </summary>
	/// <param name="y">Value of y.</param>
	public static void SetY( this Transform transform, float y, bool localPosition = false )
	{
		if (localPosition)
			transform.localPosition = new Vector3( transform.localPosition.x, y, transform.localPosition.z );
		else
			transform.position = new Vector3( transform.position.x, y, transform.position.z );
	}

	/// <summary>
	/// Sets the z component of the transform's position.
	/// </summary>
	/// <param name="transform">Transform</param>
	/// <param name="z">Value of z.</param>
	/// <param name="localPosition">Whether to use local position or global</param>
	public static void SetZ( this Transform transform, float z, bool localPosition = false )
	{
		if (localPosition)
			transform.localPosition = new Vector3( transform.localPosition.x, transform.localPosition.y, z );
		else
			transform.position = new Vector3( transform.position.x, transform.position.y, z );
	}
    
	//Breadth-first search
	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		var queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (c.name == aName)
				return c;
			foreach(Transform t in c)
				queue.Enqueue(t);
		}
		return null;
	}
}