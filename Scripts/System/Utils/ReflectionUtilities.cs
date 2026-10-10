using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utils
{
    public static class Reflection
    {
        public static BindingFlags AnyFlags() => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        public static BindingFlags PrivateStaticFlags() => BindingFlags.Static | BindingFlags.NonPublic;
        public static BindingFlags PublicStaticFlags() => BindingFlags.Public | BindingFlags.Static;

        public static Type GetGenericBaseType<T>() => typeof(T).GetGenericBaseType();

        public static List<Type> GetConcreteChildren<T>()
        {
            var baseType = typeof(T);
            return CollectUserTypes(t => !t.IsAbstract && t.IsOfType(baseType));
        }

        public static List<Type> CollectUserTypes(Predicate<Type> predicate)
        {
            var list = new List<Type>();
            IterateUserTypes(type =>
            {
                if (predicate(type) == true)
                {
                    list.Add(type);
                }
            });
            return list;
        }

        public static void IterateUserTypes(Action<Type> action)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsUserAssembly() == true)
                {
                    foreach (var type in assembly.DefinedTypes)
                    {
                        action(type);
                    }
                }
            }
        }
    }
}
