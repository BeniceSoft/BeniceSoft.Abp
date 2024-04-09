using System;
using System.Collections;
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
        // if data type is simple type; eg: string, int, boolean etc. skip handle
        if (!dataType.IsClass) return data;

        // only handle nested type; eg: custom class, IEnumerable<>, etc.
        // data is IEnumerable<T>
        if (dataType.IsAssignableTo(typeof(IEnumerable)) && dataType.IsGenericType)
        {
            // skip IEnumerable<string>
            if (dataType.GetGenericArguments()[0] == typeof(string)) return data;
            
            foreach (var item in (IEnumerable)data)
            {
                Desensitize(item);
            }
        }
        // data is other type
        else
        {
            var dataProperties = CachedPropertyInfosMap.GetOrAdd(dataType, type => type.GetProperties()
                .ToArray());
            foreach (var property in dataProperties)
            {
                // 只有字符串属性才行
                if (property.GetCustomAttribute<DesensitizeAttribute>() is not null &&
                    property.PropertyType == typeof(string))
                {
                    DesensitizeProperty(data, property);
                }
                else if(property.PropertyType != typeof(string))
                {
                    var propertyValue = property.GetValue(data);
                    if(propertyValue is null) continue;
                    Desensitize(propertyValue);
                }
            }
        }

        return data;
    }

    private static void DesensitizeProperty(object data, PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanWrite) return;

        var desensitizeAttribute = propertyInfo.GetCustomAttribute<DesensitizeAttribute>()!;

        var value = propertyInfo.GetValue(data) as string;
        if (string.IsNullOrWhiteSpace(value)) return;
        propertyInfo.SetValue(data, DesensitizeValue(value, desensitizeAttribute));
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