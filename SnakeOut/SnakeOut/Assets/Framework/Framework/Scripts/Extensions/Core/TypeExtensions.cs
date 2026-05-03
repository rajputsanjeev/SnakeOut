using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class TypeExtensions
{
	public static List<Func<object, bool>> IsKeyTypeMethods = new();
	public static Dictionary<Type, object> defaults = new();

	public static bool Is(this Type current, Type value)
	{
		return current == value || current.IsSubclassOf(value);
	}

	public static bool Is<ValueType>(this Type current, ValueType value)
	{
		return current == typeof(ValueType) || current.IsSubclassOf(typeof(ValueType));
	}

	public static bool Is(this Type current, string name)
	{
		var type = Type.GetType(name);
		if (type.IsNull())
		{
			System.Console.WriteLine("[ObjectExtension] Type -- " + name + " not found.");
			return false;
		}

		return current == type || current.IsSubclassOf(type);
	}

	public static bool IsNot<ValueType>(this Type current, ValueType value)
	{
		return !current.Is(value);
	}

	public static bool IsNot(this Type current, string name)
	{
		return !current.Is(name);
	}

	public static bool IsNot<Type>(this Type current)
	{
		return !current.Is<Type>();
	}

	public static bool IsType(this Type current, object value)
	{
		return current.IsType(value.GetType());
	}

	public static bool IsType(this Type current, Type value)
	{
		return value.IsAssignableFrom(current);
	}

	public static bool IsType<TargetType>(this Type current)
	{
		return typeof(TargetType).IsAssignableFrom(current);
	}

	public static bool HasEmptyConstructor(this Type current)
	{
		return current.GetType().GetConstructor(Type.EmptyTypes) != null;
	}

	public static bool IsKeyType(this Type current)
	{
		foreach (var method in TypeExtensions.IsKeyTypeMethods)
		{
			if (method(current))
			{
				return true;
			}
		}

		return current.IsCollection();
	}

	public static bool IsBool(this Type current)
	{
		return current == typeof(bool);
	}

	public static bool IsInt(this Type current)
	{
		return current == typeof(int);
	}

	public static bool IsFloat(this Type current)
	{
		return current == typeof(float);
	}

	public static bool IsEnum(this Type current)
	{
		return current == typeof(Enum);
	}

	public static bool IsGeneric(this Type current)
	{
		return current.ContainsGenericParameters || current.IsGenericType;
	}

	public static bool IsEnumerable(this Type current)
	{
		return current.IsType(typeof(IEnumerable));
	}

	public static bool IsDelegate(this Type current)
	{
		return current.IsType(typeof(Delegate));
	}

	public static bool IsString(this Type current)
	{
		return current == typeof(string);
	}

	public static bool IsClass(this Type current)
	{
		return current.IsClass && !current.IsCollection() && !current.IsDelegate() && !current.IsString();
	}

	public static bool IsValueTuple(this Type current)
	{
		if (current == typeof(ValueTuple))
		{
			return true;
		}

		if (current.IsGenericType)
		{
			var type = current.GetGenericTypeDefinition();
			if (type == typeof(ValueTuple<>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,,,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,,,,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,,,,,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,,,,,,>))
			{
				return true;
			}

			if (type == typeof(ValueTuple<,,,,,,,>))
			{
				return true;
			}
		}

		return false;
	}

	public static bool IsList(this Type current)
	{
		return current.IsType(typeof(IList)) && current.IsGenericType;
	}

	public static bool IsDictionary(this Type current)
	{
		return current.IsType(typeof(IDictionary)) && current.IsGenericType;
	}

	public static bool IsStatic(this Type current)
	{
		return current.IsAbstract && current.IsSealed;
	}

	public static bool IsSubclass(this Type current, Type value)
	{
		while (value != null && value != typeof(object))
		{
			var core = value.IsGenericType ? value.GetGenericTypeDefinition() : value;
			if (current == core)
			{
				return true;
			}

			value = value.BaseType;
		}

		return false;
	}

	public static bool IsSimple(this Type current)
	{
		return !current.IsComplex();
	}

	public static bool IsComplex(this Type current)
	{
		return current.IsCollection() || current.IsClass() || current.IsTuple();
	}

	public static bool IsTuple(this Type current)
	{
		return current.IsType(typeof(ITuple));
	}

	public static bool IsCollection(this Type current)
	{
		return !current.IsString() && current.IsType(typeof(IEnumerable));
	}

	public static object Instance(this Type current, params object[] terms)
	{
		if (current.IsType<string>())
		{
			return "";
		}

		if (current.IsArray())
		{
			var size = terms.Length > 0 ? (int)terms[1] : 0;
			return Array.CreateInstance(current.GetElementType() ?? throw new InvalidOperationException(), size);
		}

		return current.IsType<string>() ? "" : Activator.CreateInstance(current, terms);
	}

	public static ReturnType Instance<ReturnType>(this Type current, params object[] terms)
	{
		return (ReturnType)current.Instance(terms);
	}

	public static string GetName(this Type current, bool full = true)
	{
		var name = full ? current.FullName : current.Name;
		var generics = current.GetGenericArguments();
		if (generics.Length > 0)
		{
			name = name.Split("[")[0];
			name += "[" + generics.Select(x => x.GetName(full)).Join(",") + "]";
		}

		return name;
	}

	public static object GetDefault(this Type current)
	{
		if (!TypeExtensions.defaults.ContainsKey(current))
		{
			TypeExtensions.defaults[current] = current.IsValueType ? Activator.CreateInstance(current) : null;
		}

		return TypeExtensions.defaults[current];
	}

	public static string GetTypeName(this Type current)
	{
		return current.AssemblyQualifiedName;
	}

	public static Type GetKeyType(this Type current)
	{
		return current.GetGenericArguments()[0];
	}

	public static Type GetValueType(this Type current)
	{
		return current.GetGenericArguments()[1];
	}
}