using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BeniceSoft.Abp.Core.Extensions;

public static class EnumExtensions
{
    /// <summary>获取描述</summary>
    /// <param name="en"></param>
    /// <param name="defaultDescription"></param>
    /// <returns></returns>
    public static string GetDescription<T>(this T en, string? defaultDescription = null) where T : Enum
    {
        var enumType = en.GetType();
        if (enumType.GetCustomAttribute<FlagsAttribute>() is not null)
        {
            var descriptions = new List<string>();
            foreach (T value in Enum.GetValues(enumType).Cast<T>())
            {
                if (en.HasFlag(value))
                {
                    descriptions.Add(GetEnumDescription(value));
                }
            }

            return string.Join(",", descriptions);
        }

        return GetEnumDescription(en, defaultDescription);
    }

    private static string GetEnumDescription(Enum @enum, string? defaultDescription = null)
    {
        MemberInfo[] member = @enum.GetType().GetMember(@enum.ToString());
        if (member.Length != 0)
        {
            object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (customAttributes.Length != 0)
            {
                return ((DescriptionAttribute)customAttributes[0]).Description;
            }
        }

        return defaultDescription ?? @enum.ToString();
    }

    public static IDictionary<TEnum, T> ToDictionary<TEnum, T>(Func<Enum, T> func) where TEnum : struct
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum)
        {
            throw new InvalidOperationException($"类型{enumType.FullName}不是枚举类型");
        }

        return Enum.GetValues(enumType).OfType<Enum>().ToDictionary(x => x.To<TEnum>(), func);
    }

    public static int ToInt(this Enum aim) => Convert.ToInt32(aim);

    public static T ToEnum<T>(this int aim)
        where T : struct, Enum
    {
        var result = (T)Enum.ToObject(typeof(T), aim);
        return result;
    }

    public static T ToEnum<T>(this string aim, bool ignoreCase = true, T defaultValue = default)
        where T : struct, Enum
    {
        if (Enum.TryParse<T>(aim, ignoreCase, out var result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }
}