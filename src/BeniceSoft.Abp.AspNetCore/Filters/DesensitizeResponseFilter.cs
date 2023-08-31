using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using BeniceSoft.Abp.Core.Attributes;
using BeniceSoft.Abp.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.AspNetCore.Filters;

public class DesensitizeResponseFilter : IActionFilter, ITransientDependency
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var actionResult = context.Result;
        if (actionResult is ObjectResult { Value: not null } objectResult)
        {
            objectResult.Value = Desensitize(objectResult.Value);
        }
    }

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> CachedPropertyInfosMap = new();

    private static object Desensitize(object data)
    {
        var dataType = data.GetType();
        var dataProperties = CachedPropertyInfosMap.GetOrAdd(dataType, type => type.GetProperties()
            .Where(x => x.GetCustomAttribute<DesensitizeAttribute>() is not null)
            .ToArray());
        foreach (var property in dataProperties)
        {
            DesensitizeProperty(data, property);
        }

        return data;
    }

    private static void DesensitizeProperty(object data, PropertyInfo propertyInfo)
    {
        var desensitizeAttribute = propertyInfo.GetCustomAttribute<DesensitizeAttribute>()!;

        // 字符串直接脱敏
        if (propertyInfo.PropertyType == typeof(string))
        {
            if (!propertyInfo.CanWrite) return;

            var value = propertyInfo.GetValue(data) as string;
            if (string.IsNullOrWhiteSpace(value)) return;
            propertyInfo.SetValue(data, DesensitizeValue(value, desensitizeAttribute));
        }
        else
        {
            if (propertyInfo.PropertyType.IsClass)
            {
                
            }
            // TODO 特殊类型，要么是类，要么是集合
        }
    }

    private static string DesensitizeValue(string value, DesensitizeAttribute desensitizeAttribute)
    {
        var (startIndex, length) = desensitizeAttribute.Type switch
        {
            DesensitizeType.PhoneNumber => (3, 4),
            _ => (Math.Max(desensitizeAttribute.StartIndex, 0), Math.Max(desensitizeAttribute.Length, 4))
        };
        return value.Mask(startIndex, length, maskCharacter: desensitizeAttribute.Mask);
    }
}