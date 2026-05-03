using System;
using System.Collections;
using System.Collections.Generic;

public static class ObjectExtensions
{
	public static List<Func<object, object, object, object>> SetByKeyMethods = new();
	public static List<Func<object, object, object>> GetByKeyMethods = new();

#region Checks

	public static bool Exists<Type>(this Type current)
	{
		return !current.IsNull();
	}

	public static bool IsDefault<Type>(this Type current)
	{
		return current.Equals(default);
	}

	public static bool IsDefaultValue(this object current)
	{
		return current.Equals(current.GetDefault());
	}

	public static bool IsEmpty(this object current)
	{
		var isEmptyString = current is string s && s.IsEmpty();
		var isEmptyCollection = current is IList { Count: 0 };
		return current.IsNull() || isEmptyCollection || isEmptyString;
	}

	public static bool IsNumber(this object current)
	{
		var isByte = current is sbyte or byte;
		var isInteger = current is short or ushort or int or uint or long or ulong;
		var isDecimal = current is float or double or decimal;
		return isInteger || isDecimal || isByte;
	}

	public static bool IsNull<Type>(this Type current)
	{
		return current == null || current.Equals(null);
	}

	public static bool IsSet<Type>(this Type current)
	{
		return current != null;
	}

	public static bool IsStatic(this object current)
	{
		return current.AsType().IsStatic();
	}

	public static bool IsEnum(this object current)
	{
		return current.AsType().IsEnum;
	}

	public static bool IsArray(this object current)
	{
		return current.AsType().IsArray;
	}

	public static bool IsKeyType(this object current)
	{
		return current.AsType().IsKeyType();
	}

	public static bool IsGeneric(this object current)
	{
		return current.AsType().IsGeneric();
	}

	public static bool IsList(this object current)
	{
		return current.AsType().IsList();
	}

	public static bool IsDictionary(this object current)
	{
		return current.AsType().IsDictionary();
	}

	public static bool IsString(this object current)
	{
		return current.AsType().IsString();
	}

	public static bool IsInt(this object current)
	{
		return current.AsType().IsInt();
	}

	public static bool IsFloat(this object current)
	{
		return current.AsType().IsFloat();
	}

	public static bool IsAny<A, B>(this object current)
	{
		return current.Is<A>() || current.Is<B>();
	}

	public static bool IsAny<A, B, C>(this object current)
	{
		return current.Is<A>() || current.Is<B>() || current.Is<C>();
	}

	public static bool IsAny<A, B, C, D>(this object current)
	{
		return current.Is<A>() || current.Is<B>() || current.Is<C>() || current.Is<D>();
	}

	public static bool IsAny<A, B, C, D, E>(this object current)
	{
		return current.Is<A>() || current.Is<D>() || current.Is<C>() || current.Is<D>() || current.Is<E>();
	}

	public static bool IsSimple(this object current)
	{
		return current.AsType().IsSimple();
	}

	public static bool IsComplex(this object current)
	{
		return current.AsType().IsComplex();
	}

	public static bool IsTuple(this object current)
	{
		return current.AsType().IsTuple();
	}

	public static bool IsCollection(this object current)
	{
		return current.AsType().IsCollection();
	}

	public static bool Is<Type>(this object current)
	{
		if (current.IsNull())
		{
			return false;
		}

		var type = current.AsType();
		return type == typeof(Type) || type.IsSubclassOf(typeof(Type));
	}

	public static bool Is<MainType>(this MainType current, Type value)
	{
		return current.AsType().Is(value);
	}

	public static bool IsClass(this object current)
	{
		return current.AsType().IsClass();
	}

	public static bool IsValueType(this object current)
	{
		return current != null && current.GetType().IsValueType;
	}

	public static bool IsValueTuple(this object current)
	{
		return current != null && current.GetType().IsValueTuple();
	}

	public static object ValueOr(this object current, object other)
	{
		return current.IsNull() ? other : current;
	}

	public static string GetTypeName(this object current)
	{
		return current.AsType().GetTypeName();
	}
#endregion

#region Other
	public static Type AsType(this object current)
	{
		return current is Type ? (Type)current : current.GetType();
	}

	public static Type Default<Type>(this Type current)
	{
		return default;
	}

	public static System.Type Instance<Type>(this Type current)
	{
		System.Type type = current.Exists() ? current.GetType() : typeof(Type);
		return (System.Type)type.Instance();
	}

	public static object GetDefault(this object current)
	{
		return current?.GetType().GetDefault();
	}

	public static object GetByKey<Key>(this object current, Key key)
	{
		if (!key.IsNull())
		{
			foreach (var method in ObjectExtensions.GetByKeyMethods)
			{
				current = method(current, key);
				if (current != null)
				{
					return current;
				}
			}

			switch (current)
			{
				case IDictionary dictionary:
				{
					var data = dictionary;
					return data[key];
				}
				case IList list:
				{
					var data = list;
					var index = System.Convert.ToInt32(key);
					return data[index];
				}
			}
		}

		return null;
	}

	public static object SetByKey<Key, Value>(this object current, Key key, Value value)
	{
		if (!key.IsNull())
		{
			foreach (var method in ObjectExtensions.SetByKeyMethods)
			{
				current = method(current, key, value);
				if (current != null)
				{
					return current;
				}
			}

			switch (current)
			{
				case IDictionary dictionary:
				{
					var data = dictionary;
					data[key] = value;
					return data;
				}
				case IList list:
				{
					var data = list;
					var index = Convert.ToInt32(key);
					data[index] = value;
					return data;
				}
			}
		}

		return current;
	}
#endregion
}