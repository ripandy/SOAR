using System;
using System.Linq;

namespace Soar
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc) 
        /// or complex (i.e. custom class with public properties and methods).
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || 
                   OtherPrimitives.Contains(type);
        }

        private static readonly Type[] OtherPrimitives =
        {
            typeof(String),
            typeof(Decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };
    }
}