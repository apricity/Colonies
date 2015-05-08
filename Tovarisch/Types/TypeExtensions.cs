namespace Wacton.Tovarisch.Types
{
    using System;

    public static class TypeExtensions
    {
        public static bool IsA<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsImplementationOf<T>(this Type type)
        {
            return type.IsClass && type.IsA<T>();
        }
    }
}
