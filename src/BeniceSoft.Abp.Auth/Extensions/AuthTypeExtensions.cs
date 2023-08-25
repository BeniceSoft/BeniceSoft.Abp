using System.Reflection;

namespace BeniceSoft.Abp.Auth.Extensions;

public static class AuthTypeExtensions
{
    /// <summary>
    /// 是否是简单类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CurrentTypeIsSimpleType(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // nullable type, check if the nested type is simple.
            return CurrentTypeIsSimpleType(typeInfo.GetGenericArguments()[0]);
        }

        return typeInfo.IsPrimitive
               || typeInfo.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(Guid);
    }
}