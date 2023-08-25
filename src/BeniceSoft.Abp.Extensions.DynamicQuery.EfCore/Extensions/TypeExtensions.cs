using System.Reflection;

namespace BeniceSoft.Abp.Extensions.DynamicQuery.EfCore.Extensions;

public static class TypeExtensions
{
    public static object? GetDefaultValue(this Type type)
    {
        return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// 是否是简单类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSimpleType(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // nullable type, check if the nested type is simple.
            return IsSimpleType(typeInfo.GetGenericArguments()[0]);
        }

        return typeInfo.IsPrimitive
               || typeInfo.IsEnum
               || type == typeof(string)
               || type == typeof(decimal);
    }

    /// <summary>
    /// 是否是 IList`T`
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsGenericList(this Type type)
    {
        return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                      type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                                      type.GetGenericTypeDefinition() == typeof(List<>) ||
                                      type.GetGenericTypeDefinition() == typeof(HashSet<>));
    }

    public static bool IsGuid(this Type o)
        => o.UnderlyingSystemType.Name == "Guid" || Nullable.GetUnderlyingType(o)?.Name == "Guid";
}