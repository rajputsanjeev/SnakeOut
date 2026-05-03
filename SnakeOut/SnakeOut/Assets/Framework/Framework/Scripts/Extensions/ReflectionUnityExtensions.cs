using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public static class ReflectionUnityExtensions
{
	public static void Destroy(UnityObject target, bool destroyAssets = false)
	{
		if (target == null)
		{
			return;
		}

		if (target is Component)
		{
			var component = target.As<Component>();
			if (component.gameObject.IsNull())
			{
				return;
			}
		}

		if (!Application.isPlaying)
		{
			UnityObject.DestroyImmediate(target, destroyAssets);
		}
		else
		{
			UnityObject.Destroy(target);
		}
	}
}