using System;
using System.Linq;
using System.Reflection;

namespace BuildIt.MvvmCross
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            var typeInfo = type.GetTypeInfo();
            return typeInfo.GetDeclaredField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}