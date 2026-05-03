using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
	[InitializeOnLoad]
	public static class StaticUnloader
	{
		private const string METHOD_NAME = "UnloadStatic";

		private static IEnumerable<Type> registeredElements;

		static StaticUnloader()
		{
			registeredElements =
	AppDomain.CurrentDomain.GetAssemblies()
	.SelectMany(a =>
	{
		try { return a.GetTypes(); }
		catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
	})
	.Where(t => t.IsDefined(typeof(StaticUnloadAttribute), true))
	.ToList();

			EditorApplication.playModeStateChanged += ModeChanged;
		}

		private static void ModeChanged(PlayModeStateChange change)
		{
			if (change == PlayModeStateChange.EnteredEditMode)
			{
				EditorApplication.delayCall += UnloadStaticElements;
			}
		}

		private static void UnloadStaticElements()
		{
			foreach (Type element in registeredElements)
			{
				MethodInfo methodInfo = element.GetMethod(METHOD_NAME, ReflectionUtils.FLAGS_STATIC);
				if (methodInfo != null)
				{
					methodInfo.Invoke(null, null);
				}
				else
				{
					Debug.LogWarning($"The StaticUnloadAttribute is attached to the {element.FullName} class, but {METHOD_NAME} method is not declared!");
				}
			}
		}
	}
}

