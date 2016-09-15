using System;
using System.Collections.Generic;
using System.Linq;

namespace yellowx.Framework.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Type[] PrimitiveTypes = new[] { typeof(Enum),
                              typeof (string),
                              typeof (char),
                              typeof (Guid),
                              typeof (bool),
                              typeof (byte),
                              typeof (short),
                              typeof (int),
                              typeof (long),
                              typeof (float),
                              typeof (double),
                              typeof (decimal),

                              typeof (sbyte),
                              typeof (ushort),
                              typeof (uint),
                              typeof (ulong),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan)};

        private static readonly Type[] NullablePrimitiveTypes = PrimitiveTypes.Where(t => t.IsValueType).Select(t => typeof(Nullable<>).MakeGenericType(t)).ToArray();

        /// <summary>
        /// Get types in the same assembly 
        /// </summary>
        /// <param name="type">the root type.</param>
        /// <param name="assignableType">The type that you want to get.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesInSameAssembly(this Type type, Type assignableType)
        {
            var types = type.Assembly.GetExportedTypes().Where(t => assignableType.IsAssignableFrom(t));
            return types;
        }

        public static bool IsPrimitiveType(this Type type)
        {
            if (type == null)
                return false;
            if (PrimitiveTypes.Concat(NullablePrimitiveTypes).Any(x => x.IsAssignableFrom(type)))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }
    }
}
