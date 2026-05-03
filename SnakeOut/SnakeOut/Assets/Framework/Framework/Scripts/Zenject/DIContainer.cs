using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base.Interfaces;
using UnityEngine;

namespace Base.Zenject
{
	public class DIContainer
	{
		private readonly Dictionary<Type, object> _singletonInstances = new Dictionary<Type, object>();
		private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();

		private readonly GameObject _gameObject;

		public DIContainer(GameObject gameObject)
		{
			_gameObject = gameObject;
		}

		public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
		{
			var instance = (TInterface)Activator.CreateInstance(typeof(TImplementation));
			_singletonInstances[typeof(TInterface)] = instance;
		}

		public void RegisterMonoBehaviour<TInterface, TMonoBehaviour>(TMonoBehaviour instance, TInterface instanceInterface) where TInterface : IZenject where TMonoBehaviour : MonoBehaviour
		{
			
		}

		public void RegisterSingletonMonoBehaviour<TInterface, TImplementation>(TImplementation instance) where TImplementation : IZenject, TInterface
		{
			_singletonInstances[typeof(TInterface)] = instance;
		}

		public void RegisterSingletonMonoBehaviour<TInterface, TImplementation>() where TImplementation : MonoBehaviour, TInterface
		{
			var instance = _gameObject.AddComponent<TImplementation>();
			_singletonInstances[typeof(TInterface)] = instance;
		}

		public void Register<TInterface, TImplementation>() where TImplementation : TInterface
		{
			_typeMappings[typeof(TInterface)] = typeof(TImplementation);
		}

		public TInterface Resolve<TInterface>()
		{
			return (TInterface)Resolve(typeof(TInterface));
		}

		public object Resolve(Type type)
		{
			if (_singletonInstances.TryGetValue(type, out var singletonInstance))
			{
				return singletonInstance;
			}

			if (_typeMappings.TryGetValue(type, out var implementationType))
			{
				if (typeof(MonoBehaviour).IsAssignableFrom(implementationType))
				{
					var instance = _gameObject.AddComponent(implementationType);
					return instance;
				}
				else
				{
					var instance = CreateInstance(implementationType);
					return instance;
				}
			}

			throw new Exception($"Type {type} is not registered.");
		}

		// Internal method to create instances and resolve constructor dependencies
		private object CreateInstance(Type implementationType)
		{
			var constructor = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
				.OrderByDescending(c => c.GetParameters().Length)
				.FirstOrDefault();

			if (constructor == null)
			{
				throw new Exception($"No public constructors found for type {implementationType}.");
			}

			var parameters = constructor.GetParameters();
			var parameterInstances = new object[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				parameterInstances[i] = Resolve(parameters[i].ParameterType);
			}

			var newInstance = constructor.Invoke(parameterInstances);

			return newInstance;
		}
	}
}