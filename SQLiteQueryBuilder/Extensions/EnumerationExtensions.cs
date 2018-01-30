using System;
using System.Linq;
using System.Reflection;

namespace SQLiteQueryBuilder.Extensions
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(enumValue, false);
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue, bool inherit) where TAttribute : Attribute
        {
            var type = enumValue.GetType();
            var typeInfo = type.GetTypeInfo();
            var member = typeInfo.DeclaredMembers.FirstOrDefault(m => m.Name == enumValue.ToString());

            return member?.GetCustomAttribute<TAttribute>(inherit);
        }
    }
}
