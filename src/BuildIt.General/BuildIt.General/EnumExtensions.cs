using System;
using System.Linq;
using System.Reflection;

namespace BuildIt
{
    /// <summary>
    /// Helper methods for enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves attributes of specific type attached to the enum value.
        /// </summary>
        /// <typeparam name="TAttribute">The custom attribute type to retrieve.</typeparam>
        /// <param name="value">The enum value to look for attributes on.</param>
        /// <returns>The instance of the attribute attached to the value.</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            var typeInfo = type.GetTypeInfo();
            return typeInfo.GetDeclaredField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .FirstOrDefault();
        }
    }
}