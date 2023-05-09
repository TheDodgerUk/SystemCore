using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ReflectionExtensions
{
    public static bool IsInUserAssembly(this Type type) => type.Assembly.IsUserAssembly();

    public static bool IsUserAssembly(this Assembly assembly)
    {
        return assembly.FullName.StartsWith("Assembly-CSharp");
    }

    public static bool ImplementsInterface<TInterface>(this Type type) => typeof(TInterface).IsAssignableFrom(type);

    public static bool ImplementsInterface(this Type type, Type interfaceType) => interfaceType.IsAssignableFrom(type);

    public static List<Type> GetChildTypes(this Type type)
    {
        return Utils.Reflection.CollectUserTypes(t => t.IsSubclassOf(type));
    }

    public static Type GetGenericBaseType(this Type type)
    {
        if (type.IsGenericType == false)
        {
            if (type.BaseType != null)
            {
                return GetGenericBaseType(type.BaseType);
            }
            return null;
        }
        return type;
    }

    public static bool IsOfType<T>(this Type type) => type.IsOfType(typeof(T));

    public static bool IsOfType(this Type type, Type targetType)
    {
        if (type == targetType)
        {
            return true;
        }
        if (targetType.IsInterface == true)
        {
            return type.ImplementsInterface(targetType);
        }
        if (type.BaseType != null)
        {
            return type.BaseType.IsOfType(targetType);
        }
        return false;
    }

    public static List<Type> GetBaseTypes(this Type type, Type stopAt = null)
    {
        var types = new List<Type> { type };
        if (type.BaseType != null && type.BaseType != stopAt)
        {
            types.InsertRange(0, type.BaseType.GetBaseTypes(stopAt));
        }
        return types;
    }

    public static T GetAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetCustomAttributes(false).OfType<T>().SingleOrDefault();
    }

    public static T DeepCopy<T>(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException("Object cannot be null");
        return (T)Process(obj);
    }

    static object Process(object obj)
    {
        if (obj == null)
            return null;
        Type type = obj.GetType();
        if (type.IsValueType || type == typeof(string))
        {
            return obj;
        }
        else if (type.IsArray)
        {
            Type elementType = Type.GetType(
                 type.FullName.Replace("[]", string.Empty));
            var array = obj as Array;
            Array copied = Array.CreateInstance(elementType, array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                copied.SetValue(Process(array.GetValue(i)), i);
            }
            return Convert.ChangeType(copied, obj.GetType());
        }
        else if (type.IsClass)
        {
            object toret = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                        BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(obj);
                if (fieldValue == null)
                    continue;
                field.SetValue(toret, Process(fieldValue));
            }
            return toret;
        }
        else
            throw new ArgumentException("Unknown type");
    }
}