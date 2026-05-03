using System;
using System.Collections.Generic;

public static class EventBus
{
	private static readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

	public static void Subscribe<T>(Action<T> callback)
	{
		if (!_subscribers.ContainsKey(typeof(T)))
		{
			_subscribers[typeof(T)] = new List<Delegate>();
		}
		_subscribers[typeof(T)].Add(callback);
	}

	public static void Unsubscribe<T>(Action<T> callback)
	{
		if (_subscribers.ContainsKey(typeof(T)))
		{
			_subscribers[typeof(T)].Remove(callback);
		}
	}

	public static void Publish<T>(T message)
	{
		if (_subscribers.ContainsKey(typeof(T)))
		{
			foreach (var callback in _subscribers[typeof(T)])
			{
				((Action<T>)callback)?.Invoke(message);
			}
		}
	}
}