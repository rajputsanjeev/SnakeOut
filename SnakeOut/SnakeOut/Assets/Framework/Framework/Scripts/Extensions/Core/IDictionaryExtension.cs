using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class IDictionaryExtension
{
	#region Dictionary
	public static Dictionary<Key, Value> Copy<Key, Value>(this Dictionary<Key, Value> current)
	{
		return new Dictionary<Key, Value>(current);
	}

	public static Dictionary<Key, Value> Remove<Key, Value>(this Dictionary<Key, Value> current,
		Func<KeyValuePair<Key, Value>, bool> method)
	{
		foreach (var item in current.Copy())
		{
			if (method(item))
			{
				current.Remove(item.Key);
			}
		}

		return current;
	}

	public static Dictionary<Key, Value> RemoveValue<Key, Value>(this Dictionary<Key, Value> current, Value value)
	{
		foreach (var item in current.Copy())
		{
			if (item.Value.Equals(value))
			{
				current.Remove(item.Key);
			}
		}

		return current;
	}

	public static Dictionary<Key, Value> RemoveDefault<Key, Value>(this Dictionary<Key, Value> current)
	{
		foreach (var item in current.Copy())
		{
			if (item.Value.Equals(default))
			{
				current.Remove(item.Key);
			}
		}

		return current;
	}

	public static Dictionary<Key, Value> DefaultValues<Key, Value>(this Dictionary<Key, Value> current)
	{
		foreach (var item in current.Copy())
		{
			current[item.Key] = default;
		}

		return current;
	}

	public static Key GetKey<Key, Value>(this Dictionary<Key, Value> current, Value value)
	{
		return current.FirstOrDefault(x => x.Value.Equals(value)).Key;
	}

	public static Dictionary<Key, Value> Merge<Key, Value>(this Dictionary<Key, Value> current,
		Dictionary<Key, Value> other)
	{
		foreach (var item in other)
		{
			current[item.Key] = item.Value;
		}

		return current;
	}

	public static Dictionary<Key, Value> Difference<Key, Value>(this Dictionary<Key, Value> current,
		Dictionary<Key, Value> other)
	{
		var output = new Dictionary<Key, Value>();
		foreach (var item in other)
		{
			var key = item.Key;
			if (current.TryGetValue(key, out var value))
			{
				var nullMatch = value.IsNull() && other[key].IsNull();
				var referenceMatch = !nullMatch && !other[key].GetType().IsValueType;
				var valueMatch = !nullMatch && other[key].Equals(current[key]);
				var match = nullMatch || referenceMatch || valueMatch;
				/*if(current[key] is IEnumerable){
					match = current[key].As<IEnumerable>().SequenceEqual(other[key]);
				}*/
				if (match)
				{
					continue;
				}
			}

			output[item.Key] = item.Value;
		}

		return output;
	}

	public static Dictionary<string, Value> GetContains<Value>(this Dictionary<string, Value> current, string value)
	{
		var similar = new Dictionary<string, Value>();
		foreach (string key in current.Keys)
		{
			if (key.Contains(value))
			{
				similar.Add(key, current[key]);
			}
		}

		return similar;
	}
#endregion

#region IDictionary

	public static bool Has<Key, Value>(this IDictionary<Key, Value> current, Key value)
	{
		return current.ContainsKey(value);
	}

	public static Value Get<Key, Value>(this IDictionary<Key, Value> current, Key key, Value value = default)
		where Value : new()
	{
		if (!current.TryGetValue(key, out var output))
		{
			return value;
		}

		return output;
	}

	public static IDictionary<Key, Value> SetValues<Key, Value>(this IDictionary<Key, Value> current,
		IList<Value> values) where Value : new()
	{
		var index = 0;
		foreach (var key in current.Keys.ToList())
		{
			current[key] = values[index];
			++index;
		}

		return current;
	}

	public static Value AddNew<Key, Value>(this IDictionary<Key, Value> current, Key key) where Value : new()
	{
		if (!current.TryGetValue(key, out var output))
		{
			current[key] = output = new Value();
		}

		return output;
	}

	public static Value AddNew<Key, Value>(this IDictionary<Key, Value> current, Key key, Value value)
		where Value : new()
	{
		if (!current.TryGetValue(key, out var output))
		{
			current[key] = output = value;
		}

		return output;
	}

	public static Value AddNewSequence<Key, Value>(this IDictionary<IList<Key>, Value> current, IList<Key> key)
		where Value : new()
	{
		if (!current.Keys.ToArray().Exists(x => x.SequenceEqual(key)))
		{
			current[key] = new Value();
		}

		return current[key];
	}

	public static bool ContainsKey(this IDictionary current, string value, bool ignoreCase)
	{
		value = value.ToLower();
		foreach (string key in current.Keys)
		{
			if (key.ToLower() == value)
			{
				return true;
			}
		}

		return false;
	}

	public static Dictionary<string, Value> AddPrefix<Value>(this Dictionary<string, Value> current, string prefix)
	{
		var output = new Dictionary<string, Value>();
		foreach (var item in current)
		{
			output[prefix + item.Key] = item.Value;
		}

		return output;
	}

	public static Value TryGet<Key, Value>(this IDictionary<Key, Value> current, Key key)
	{
		current.TryGetValue(key, out var output);
		return output;
	}

	public static float FirstOrNaN<Key>(this IDictionary<Key, float> current)
	{
		foreach (var item in current)
		{
			return item.Value;
		}

		return float.NaN;
	}

	public static Value FirstValue<Key, Value>(this IDictionary<Key, Value> current,
		Func<KeyValuePair<Key, Value>, bool> method)
	{
		var match = current.FirstOrDefault(method);
		return match.IsDefault() ? default : match.Value;
	}

	public static Value FirstValue<Key, Value>(this IDictionary<Key, Value> current)
	{
		return current.Count > 0 ? current.First().Value : default;
	}

	public static bool Exists<Key, Value>(this IDictionary<Key, Value> current,
		Func<KeyValuePair<Key, Value>, bool> method)
	{
		foreach (var item in current)
		{
			if (method(item))
			{
				return true;
			}
		}

		return false;
	}

	public static Dictionary<TargetKey, TargetValue> Remap<Key, Value, TargetKey, TargetValue>(
		this Dictionary<Key, Value> current, Func<KeyValuePair<Key, Value>, TargetKey> keyMethod,
		Func<KeyValuePair<Key, Value>, TargetValue> valueMethod)
	{
		var result = new Dictionary<TargetKey, TargetValue>();
		foreach (var item in current)
		{
			var key = keyMethod(item);
			var value = valueMethod(item);
			result[key] = value;
		}

		return result;
	}

	public static Type GetKeyType(this IDictionary current)
	{
		return current.AsType().GetKeyType();
	}

	public static Type GetValueType(this IDictionary current)
	{
		return current.AsType().GetValueType();
	}
	
	#endregion
}