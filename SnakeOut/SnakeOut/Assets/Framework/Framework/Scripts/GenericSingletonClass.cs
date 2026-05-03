using System;
using UnityEngine;

namespace Framework
{
	public class GenericSingletonClass<T> : MonoBehaviour where T : Component
	{
		private static T _instance;
		public Action OnInit;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
				}

				return _instance;
			}
		}

		public virtual void Awake() { }

		public virtual void OnDestroy()
		{
			if (_instance != null)
			{
				Destroy(_instance);
			}
		}
	}
}